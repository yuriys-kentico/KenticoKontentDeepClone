using System;

using KenticoKontent.Models.Management.References;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KenticoKontent
{
    internal class ReferenceConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings SubclassResolverSettings = new JsonSerializerSettings { ContractResolver = new SubclassResolver<Reference>() };

        public override bool CanConvert(Type objectType) => objectType == typeof(Reference);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            static T deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, SubclassResolverSettings)!;

            var rawObject = JObject.Load(reader);

            if (rawObject.ContainsKey("id"))
            {
                return deserialize<IdReference>(rawObject.ToString());
            }

            if (rawObject.ContainsKey("codename"))
            {
                return deserialize<CodenameReference>(rawObject.ToString());
            }

            if (rawObject.ContainsKey("external_id"))
            {
                return deserialize<ExternalIdReference>(rawObject.ToString());
            }

            throw new NotImplementedException("Reference format not supported.");
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (value)
            {
                case IdReference idReference:
                    JToken.FromObject(new { id = idReference.Value }).WriteTo(writer);
                    break;

                case CodenameReference codenameReference:
                    JToken.FromObject(new { codename = codenameReference.Value }).WriteTo(writer);
                    break;

                case ExternalIdReference externalIdReference:
                    JToken.FromObject(new { external_id = externalIdReference.Value }).WriteTo(writer);
                    break;
            }
        }
    }
}