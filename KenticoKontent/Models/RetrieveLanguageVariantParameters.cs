using System;

using KenticoKontent.Models.Management.References;

namespace KenticoKontent.Models
{
    public class RetrieveLanguageVariantParameters
    {
        public Reference? ItemReference { get; set; }

        public Reference? TypeReference { get; set; }

        public Reference? LanguageReference { get; set; }

        public void Deconstruct(
            out Reference itemReference,
            out Reference typeReference,
            out Reference languageReference
            )
        {
            itemReference = ItemReference ?? throw new ArgumentNullException(nameof(ItemReference));
            typeReference = TypeReference ?? throw new ArgumentNullException(nameof(TypeReference));
            languageReference = LanguageReference ?? throw new ArgumentNullException(nameof(LanguageReference));
        }
    }
}