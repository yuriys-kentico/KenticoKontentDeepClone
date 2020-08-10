using Newtonsoft.Json;

namespace KenticoKontent.Models.Management.Elements
{
    public class CustomElement : AbstractElement
    {
        public string? Value { get; set; }

        [JsonProperty("searchable_value")]
        public string? Searchable_Value { get; set; }
    }
}