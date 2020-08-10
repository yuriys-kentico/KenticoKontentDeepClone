using System.Threading.Tasks;

using Functions.Webhooks;

using KenticoKontent.Models;
using KenticoKontent.Models.Management;
using KenticoKontent.Models.Management.References;

namespace KenticoKontent
{
    public interface IKontentRepository
    {
        int ApiCalls { get; }

        Task PrepareDeepClone(PrepareItemParameters prepareItemParameters);

        Task<ItemVariant> GetItemVariant(GetItemVariantParameters getItemVariantParameters);

        Task<ContentItem> RetrieveContentItem(Reference itemReference);

        Task<LanguageVariant?> RetrieveLanguageVariant(RetrieveLanguageVariantParameters retrieveLanguageVariantParameters);

        Task<ContentItem> UpsertContentItem(ContentItem contentItem);

        Task UpsertLanguageVariant(UpsertLanguageVariantParameters upsertLanguageVariantParameters);

        Task<ContentType> RetrieveContentType(Reference typeReference);

        Task<ContentType> RetrieveContentTypeSnippet(Reference typeReference);

        string GetExternalId();
    }
}