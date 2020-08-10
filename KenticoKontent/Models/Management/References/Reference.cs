using System;

using Newtonsoft.Json;

namespace KenticoKontent.Models.Management.References
{
    [JsonConverter(typeof(ReferenceConverter))]
    public abstract class Reference
    {
        protected enum ReferenceType
        {
            Id,
            ExternalId,
            Codename
        }

        private readonly ReferenceType type;

        public string Value { get; }

        protected Reference(ReferenceType type, string value)
        {
            this.type = type;
            Value = value;
        }

        public static implicit operator string(Reference reference)
        {
            return reference.type switch
            {
                ReferenceType.Id => reference.Value,
                ReferenceType.ExternalId => $"external-id/{reference.Value}",
                ReferenceType.Codename => $"codename/{reference.Value}",
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToString()
        {
            return this;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Reference other)
            {
                return Value == other.Value;
            }

            return base.Equals(obj);
        }
    }
}