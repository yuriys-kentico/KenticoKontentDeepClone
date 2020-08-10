using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using KenticoKontent;
using KenticoKontent.Models;
using KenticoKontent.Models.Management;
using KenticoKontent.Models.Management.References;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.Webhooks
{
    public partial class KontentClone : BaseFunction
    {
        private readonly IKontentRepository kontentRepository;

        public KontentClone(
            ILogger<KontentClone> logger,
            IKontentRepository kontentRepository
            ) : base(logger)
        {
            this.kontentRepository = kontentRepository;
        }

        [FunctionName(nameof(KontentClone))]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                "post",
                Route = Routes.KontentClone
            )] string body,
            string itemCodename,
            string languageCodename
            )
        {
            try
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                var oldItemReference = new CodenameReference(itemCodename);
                var newItemReference = new ExternalIdReference(kontentRepository.GetExternalId());
                var languageReference = new CodenameReference(languageCodename);
                var newItemVariants = new Dictionary<Reference, ItemVariant>();

                await kontentRepository.PrepareDeepClone(new PrepareItemParameters
                {
                    ItemVariant = await kontentRepository.GetItemVariant(new GetItemVariantParameters
                    {
                        OldItemReference = oldItemReference,
                        NewItemReference = newItemReference,
                        LanguageReference = languageReference,
                        NewItemVariants = newItemVariants
                    }),
                    LanguageReference = languageReference,
                    NewItemVariants = newItemVariants
                });

                var newItems = new List<ContentItem>();

                foreach (var (newItem, newVariant) in newItemVariants.Values)
                {
                    newItems.Add(await kontentRepository.UpsertContentItem(newItem));

                    await kontentRepository.UpsertLanguageVariant(new UpsertLanguageVariantParameters
                    {
                        LanguageReference = languageReference,
                        Variant = newVariant
                    });
                }

                stopwatch.Stop();

                return LogOkObject(new
                {
                    TotalApiCalls = kontentRepository.ApiCalls,
                    TotalMilliseconds = stopwatch.ElapsedMilliseconds,
                    NewItems = newItems
                });
            }
            catch (Exception ex)
            {
                return LogException(ex);
            }
        }
    }
}