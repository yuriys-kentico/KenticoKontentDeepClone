using System;
using System.Collections.Generic;

using KenticoKontent.Models;
using KenticoKontent.Models.Management.References;

namespace Functions.Webhooks
{
    public class PrepareItemParameters
    {
        public ItemVariant? ItemVariant { get; set; }

        public Reference? LanguageReference { get; set; }

        public IDictionary<Reference, ItemVariant>? NewItemVariants { get; set; }

        public void Deconstruct(
            out ItemVariant itemVariant,
            out Reference languageReference,
            out IDictionary<Reference, ItemVariant> newItemVariants
        )
        {
            itemVariant = ItemVariant ?? throw new ArgumentNullException(nameof(ItemVariant));
            languageReference = LanguageReference ?? throw new ArgumentNullException(nameof(LanguageReference));
            newItemVariants = NewItemVariants ?? throw new ArgumentNullException(nameof(NewItemVariants));
        }
    }
}