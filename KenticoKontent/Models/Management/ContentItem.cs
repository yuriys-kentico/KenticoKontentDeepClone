using KenticoKontent.Models.Management.References;

using Newtonsoft.Json;

namespace KenticoKontent.Models.Management
{
    public class ContentItem
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Codename { get; set; }

        [JsonProperty("type")]
        public Reference? TypeReference { get; set; }

        [JsonProperty("external_id")]
        public string? ExternalId { get; set; }
    }
}