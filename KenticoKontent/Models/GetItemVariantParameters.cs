using System;
using System.Collections.Generic;

using KenticoKontent.Models;
using KenticoKontent.Models.Management.References;

namespace Functions.Webhooks
{
    public class GetItemVariantParameters
    {
        public Reference? OldItemReference { get; set; }

        public Reference? NewItemReference { get; set; }

        public Reference? LanguageReference { get; set; }

        public IDictionary<Reference, ItemVariant>? NewItemVariants { get; set; }

        public void Deconstruct(
            out Reference oldItemReference,
            out Reference newItemReference,
            out Reference languageReference,
            out IDictionary<Reference, ItemVariant> newItemVariants
        )
        {
            oldItemReference = OldItemReference ?? throw new ArgumentNullException(nameof(OldItemReference));
            newItemReference = NewItemReference ?? throw new ArgumentNullException(nameof(NewItemReference));
            languageReference = LanguageReference ?? throw new ArgumentNullException(nameof(LanguageReference));
            newItemVariants = NewItemVariants ?? throw new ArgumentNullException(nameof(NewItemVariants));
        }
    }
}