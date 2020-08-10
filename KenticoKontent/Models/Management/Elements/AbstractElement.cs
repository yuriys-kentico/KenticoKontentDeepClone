using KenticoKontent.Models.Management.References;

using Newtonsoft.Json;

namespace KenticoKontent.Models.Management.Elements
{
    [JsonConverter(typeof(AbstractElementConverter))]
    public abstract class AbstractElement
    {
        public Reference? Element { get; set; }
    }
}