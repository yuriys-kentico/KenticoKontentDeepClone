using System;

using KenticoKontent.Models.Management;
using KenticoKontent.Models.Management.References;

namespace KenticoKontent.Models
{
    public class UpsertLanguageVariantParameters
    {
        public Reference? LanguageReference { get; set; }

        public LanguageVariant? Variant { get; set; }

        public void Deconstruct(
            out Reference? languageReference,
            out LanguageVariant variant
            )
        {
            languageReference = LanguageReference;
            variant = Variant ?? throw new ArgumentNullException(nameof(Variant));
        }
    }
}