using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Core;

using Functions.Webhooks;

using KenticoKontent.Models;
using KenticoKontent.Models.Management;
using KenticoKontent.Models.Management.Elements;
using KenticoKontent.Models.Management.References;
using KenticoKontent.Models.Management.Types;

using Newtonsoft.Json.Serialization;

namespace KenticoKontent
{
    public class KontentRepository : IKontentRepository
    {
        private const int buffer = 1;
        private const int secondsMax = 10 - buffer;
        private static readonly Queue<DateTime> secondsQueue = new Queue<DateTime>(secondsMax);
        private const int minutesMax = 400 - buffer;
        private static readonly Queue<DateTime> minutesQueue = new Queue<DateTime>(minutesMax);
        private const int hoursMax = 15000 - buffer;
        private static readonly Queue<DateTime> hoursQueue = new Queue<DateTime>(hoursMax);

        private readonly HttpClient httpClient;
        private readonly Settings settings;

        public int ApiCalls { get; private set; }

        public IDictionary<string, object> CacheDictionary { get; } = new ConcurrentDictionary<string, object>();

        public KontentRepository(
            HttpClient httpClient,
            Settings settings
            )
        {
            this.httpClient = httpClient;
            this.settings = settings;

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", settings.ManagementApiKey);
        }

        public async Task PrepareDeepClone(PrepareItemParameters prepareItemParameters)
        {
            var ((item, variant), languageReference, newItemVariants) = prepareItemParameters;

            var descendantReferences = new HashSet<ItemVariant>();

            foreach (var element in variant.Elements!)
            {
                switch (element)
                {
                    case RichTextElement richTextElement:

                        void ReplaceReferences(ref string richTextValue, string pattern, Func<string, Reference> getReference)
                        {
                            var matches = Regex.Matches(richTextValue, pattern);

                            foreach (Match? match in matches)
                            {
                                if (match?.Success == true)
                                {
                                    var matchValue = match.Groups[1];
                                    var oldReference = getReference(matchValue.Value);

                                    if (newItemVariants!.TryGetValue(oldReference, out var itemVariant) && itemVariant.Variant?.ItemReference != null)
                                    {
                                        richTextValue = richTextValue.Replace(matchValue.Value, itemVariant.Variant.ItemReference.Value);
                                    }
                                    else
                                    {
                                        var newReference = new ExternalIdReference(GetExternalId());

                                        descendantReferences!.Add(GetItemVariant(new GetItemVariantParameters
                                        {
                                            OldItemReference = oldReference,
                                            NewItemReference = newReference,
                                            LanguageReference = languageReference,
                                            NewItemVariants = newItemVariants
                                        }).Result);

                                        richTextValue = richTextValue.Replace(matchValue.Value, newReference.Value);
                                    }
                                }
                            }
                        }

                        if (richTextElement.Value != null)
                        {
                            if (richTextElement.Components != null)
                            {
                                foreach (var component in richTextElement.Components)
                                {
                                    var newGuid = Guid.NewGuid();

                                    richTextElement.Value = richTextElement.Value.Replace(component.Id.ToString(), newGuid.ToString());

                                    component.Id = newGuid;
                                }
                            }

                            var newRichTextValue = richTextElement.Value;

                            ReplaceReferences(ref newRichTextValue, "(?<=data-type=\"item\" *)data-id=\"(.*?)\"", value => new IdReference(value));
                            ReplaceReferences(ref newRichTextValue, "(?<=data-type=\"item\" *)data-external-id=\"(.*?)\"", value => new ExternalIdReference(value));

                            newRichTextValue = Regex.Replace(newRichTextValue, "(?<=data-type=\"item\" *)data-id", "data-external-id");

                            ReplaceReferences(ref newRichTextValue, "data-item-id=\"(.*?)\"", value => new IdReference(value));
                            ReplaceReferences(ref newRichTextValue, "data-item-external-id=\"(.*?)\"", value => new ExternalIdReference(value));

                            newRichTextValue = newRichTextValue.Replace("data-item-id", "data-item-external-id");

                            richTextElement.Value = newRichTextValue;
                        }

                        break;

                    case LinkedItemsElement linkedItemsElement:

                        Reference ReplaceReference(Reference reference)
                        {
                            if (newItemVariants.TryGetValue(reference, out var itemVariant) && itemVariant.Variant?.ItemReference != null)
                            {
                                return itemVariant.Variant.ItemReference;
                            }

                            var newReference = new ExternalIdReference(GetExternalId());

                            descendantReferences.Add(GetItemVariant(new GetItemVariantParameters
                            {
                                OldItemReference = reference,
                                NewItemReference = newReference,
                                LanguageReference = languageReference,
                                NewItemVariants = newItemVariants
                            }).Result);

                            return newReference;
                        }

                        if (linkedItemsElement.Value != null)
                        {
                            linkedItemsElement.Value = linkedItemsElement.Value.Select(ReplaceReference).ToList();
                        }

                        break;
                }
            }

            foreach (var itemVariant in descendantReferences)
            {
                await PrepareDeepClone(new PrepareItemParameters
                {
                    ItemVariant = itemVariant,
                    LanguageReference = languageReference,
                    NewItemVariants = newItemVariants
                });
            }
        }

        public async Task<ItemVariant> GetItemVariant(GetItemVariantParameters getItemVariantParameters)
        {
            var (oldItemReference, newItemReference, languageReference, newItemVariants) = getItemVariantParameters;

            var oldItem = await RetrieveContentItem(oldItemReference);

            oldItem.Codename = null;
            oldItem.ExternalId = newItemReference.Value;

            var variant = await RetrieveLanguageVariant(new RetrieveLanguageVariantParameters
            {
                ItemReference = oldItemReference,
                TypeReference = oldItem.TypeReference,
                LanguageReference = languageReference
            });

            if (variant == null)
            {
                throw new NotImplementedException("Variant could not be retrieved.");
            }

            variant.ItemReference = newItemReference;

            var itemVariant = new ItemVariant { Item = oldItem, Variant = variant };

            newItemVariants.TryAdd(new IdReference(oldItem.Id!), itemVariant);

            return itemVariant;
        }

        public string GetExternalId() => Guid.NewGuid().ToString();

        public async Task<ContentItem> RetrieveContentItem(Reference itemReference)
        {
            return await Cache(
                () => WithRetry(() => Get($"items/{itemReference}")),
                itemReference,
                async response =>
            {
                await ThrowIfNotSuccessStatusCode(response);

                return await response.Content.ReadAsAsync<ContentItem>();
            });
        }

        public async Task<LanguageVariant?> RetrieveLanguageVariant(RetrieveLanguageVariantParameters retrieveLanguageVariantParameters)
        {
            var (itemReference, typeReference, languageReference) = retrieveLanguageVariantParameters;

            var variant = await Cache(
                () => WithRetry(() => Get($"items/{itemReference}/variants/{languageReference}")),
                itemReference + languageReference,
                async response =>
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                await ThrowIfNotSuccessStatusCode(response);

                return await response.Content.ReadAsAsync<LanguageVariant>();
            });

            if (variant == null)
            {
                return null;
            }

            var itemTypeElements = new HashSet<ElementType>();

            var itemType = await RetrieveContentType(typeReference);

            if (itemType.Elements == null)
            {
                throw new NotImplementedException("Item type does not have elements.");
            }

            itemTypeElements.UnionWith(itemType.Elements);

            foreach (var typeElement in itemType.Elements)
            {
                if (typeElement.Snippet != null)
                {
                    var itemTypeSnippet = await RetrieveContentTypeSnippet(typeElement.Snippet);

                    if (itemTypeSnippet.Elements == null)
                    {
                        throw new NotImplementedException("Item type snippet does not have elements.");
                    }

                    itemTypeElements.UnionWith(itemTypeSnippet.Elements);
                }
            }

            variant.Elements = variant.Elements.Select(element =>
            {
                if (element is AbstractReferenceListElement listElement)
                {
                    var elementType = itemTypeElements.First(typeElement => typeElement.Id == element.Element?.Value);

                    switch (elementType.Type)
                    {
                        case "asset":
                            return new AssetElement(listElement);

                        case "multiple_choice":
                            return new MultipleChoiceElement(listElement);

                        case "taxonomy":
                            return new TaxonomyElement(listElement);

                        case "modular_content":
                            return new LinkedItemsElement(listElement);
                    }
                }

                return element;
            }).ToList();

            return variant;
        }

        public async Task<ContentType> RetrieveContentType(Reference typeReference)
        {
            return await Cache(
                () => WithRetry(() => Get($"types/{typeReference}")),
                typeReference,
                async response =>
            {
                await ThrowIfNotSuccessStatusCode(response);

                return await response.Content.ReadAsAsync<ContentType>();
            });
        }

        public async Task<ContentType> RetrieveContentTypeSnippet(Reference typeReference)
        {
            return await Cache(
                () => WithRetry(() => Get($"snippets/{typeReference}")),
                typeReference,
                async response =>
            {
                await ThrowIfNotSuccessStatusCode(response);

                return await response.Content.ReadAsAsync<ContentType>();
            });
        }

        public async Task<ContentItem> UpsertContentItem(ContentItem contentItem)
        {
            var response = await WithRetry(() => Put($"items/{new ExternalIdReference(contentItem.ExternalId!)}", contentItem));

            await ThrowIfNotSuccessStatusCode(response);

            return await response.Content.ReadAsAsync<ContentItem>();
        }

        public async Task UpsertLanguageVariant(UpsertLanguageVariantParameters upsertLanguageVariantParameters)
        {
            var (language, variant) = upsertLanguageVariantParameters;

            var response = await WithRetry(() => Put($"items/{variant.ItemReference}/variants/{language}", variant));

            await ThrowIfNotSuccessStatusCode(response);
        }

        private async Task<HttpResponseMessage> Get(string relativePath) => await httpClient.GetAsync(GetEndpoint(relativePath));

        private async Task<HttpResponseMessage> Put(string relativePath, object? value = default)
        {
            var response = await httpClient.PutAsync(GetEndpoint(relativePath), value, new JsonMediaTypeFormatter()
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            return response;
        }

        private async Task<HttpResponseMessage> WithRetry(Func<Task<HttpResponseMessage>> doRequest)
        {
            static async Task CheckQueue(Queue<DateTime> queue, TimeSpan span, int max)
            {
                queue.Enqueue(DateTime.Now.Add(span));

                if (queue.Count == max)
                {
                    var now = DateTime.Now;
                    var next = queue.Dequeue();
                    var flushed = false;

                    while (now > next)
                    {
                        next = queue.Dequeue();
                        flushed = true;
                    }

                    if (!flushed)
                    {
                        await Task.Delay((int)(next - now).TotalMilliseconds);
                    }
                }
            }

            await CheckQueue(hoursQueue, TimeSpan.FromHours(1), hoursMax);
            await CheckQueue(minutesQueue, TimeSpan.FromMinutes(1), minutesMax);
            await CheckQueue(secondsQueue, TimeSpan.FromSeconds(1), secondsMax);

            var response = await doRequest();

            if (response.StatusCode == HttpStatusCode.TooManyRequests && response.Headers.RetryAfter.Delta != null)
            {
                await Task.Delay(response.Headers.RetryAfter.Delta.Value);

                return await WithRetry(doRequest);
            }

            ApiCalls++;

            return response;
        }

        private async Task<TOut> Cache<TIn, TOut>(Func<Task<TIn>> doRequest, string key, Func<TIn, Task<TOut>> getItem) where TOut : class?
        {
            if (CacheDictionary.TryGetValue(key, out var item) && item is TOut tItem)
            {
                return tItem;
            }

            var newItem = await getItem(await doRequest());

            if (newItem != null)
            {
                CacheDictionary.Add(key, newItem);
            }

            return newItem;
        }

        private string GetEndpoint(string? endpoint = default) => $@"https://manage.kontent.ai/v2/projects/{settings.ProjectId}/{endpoint}";

        private static async Task ThrowIfNotSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsAsync<APIErrorResponse>();
                throw errorContent.GetException();
            }
        }
    }
}