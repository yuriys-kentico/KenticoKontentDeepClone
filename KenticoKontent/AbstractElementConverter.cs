using System;

using KenticoKontent.Models.Management.Elements;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KenticoKontent
{
    internal class AbstractElementConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings SubclassResolverSettings = new JsonSerializerSettings { ContractResolver = new SubclassResolver<AbstractElement>() };

        public override bool CanConvert(Type objectType) => objectType == typeof(AbstractElement);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            static T deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, SubclassResolverSettings)!;

            var rawObject = JObject.Load(reader);

            if (rawObject.ContainsKey(nameof(RichTextElement.Components).ToLower()))
            {
                return deserialize<RichTextElement>(rawObject.ToString());
            }

            if (rawObject.ContainsKey(nameof(CustomElement.Searchable_Value).ToLower()))
            {
                return deserialize<CustomElement>(rawObject.ToString());
            }

            if (rawObject.ContainsKey(nameof(UrlSlugElement.Mode).ToLower()))
            {
                return deserialize<UrlSlugElement>(rawObject.ToString());
            }

            switch (rawObject["value"]?.Type)
            {
                case JTokenType.String:
                    return deserialize<TextElement>(rawObject.ToString());

                case JTokenType.Float:
                    return deserialize<NumberElement>(rawObject.ToString());

                case JTokenType.Date:
                    return deserialize<DateTimeElement>(rawObject.ToString());

                case JTokenType.Array:
                    return deserialize<AbstractReferenceListElement>(rawObject.ToString());
            }

            throw new NotImplementedException("Item element type could not be determined.");
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}