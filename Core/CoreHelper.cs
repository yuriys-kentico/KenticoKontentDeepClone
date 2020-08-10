using Newtonsoft.Json;

namespace Core
{
    public static class CoreHelper
    {
        public static TValue Deserialize<TValue>(string json)
        {
            return JsonConvert.DeserializeObject<TValue>(json);
        }

        public static string Serialize<TValue>(TValue value) where TValue : notnull
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}