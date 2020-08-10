using System;

using KenticoKontent.Models.Management;

namespace KenticoKontent.Models
{
    public class ItemVariant
    {
        public ContentItem? Item { get; set; }

        public LanguageVariant? Variant { get; set; }

        public void Deconstruct(
            out ContentItem item,
            out LanguageVariant variant
            )
        {
            item = Item ?? throw new ArgumentNullException(nameof(Item));
            variant = Variant ?? throw new ArgumentNullException(nameof(Variant));
        }
    }
}