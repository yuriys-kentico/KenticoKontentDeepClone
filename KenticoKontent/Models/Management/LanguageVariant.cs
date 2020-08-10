using System.Collections.Generic;

using KenticoKontent.Models.Management.Elements;
using KenticoKontent.Models.Management.References;

using Newtonsoft.Json;

namespace KenticoKontent.Models.Management
{
    public class LanguageVariant
    {
        [JsonProperty("item")]
        public Reference? ItemReference { get; set; }

        public IList<AbstractElement>? Elements { get; set; }
    }
}