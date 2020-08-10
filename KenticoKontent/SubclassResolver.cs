using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KenticoKontent
{
    public class SubclassResolver<T> : DefaultContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            if (typeof(T).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;

            return base.ResolveContractConverter(objectType);
        }
    }
}