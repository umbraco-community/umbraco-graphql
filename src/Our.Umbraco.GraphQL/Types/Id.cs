using System;
using System.ComponentModel;

namespace Our.Umbraco.GraphQL.Types
{
    public struct Id
    {
        public Id(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                throw new ArgumentException($"{nameof(value)} cannot be empty.", nameof(value));

            Value = value;
        }

        public string Value { get; }

        public T As<T>()
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
                return (T)converter.ConvertFrom(Value);

            return (T)Convert.ChangeType(Value, typeof(T));
        }

        public bool Equals(Id other) => Value == other.Value;

        public override bool Equals(object obj) => obj is Id other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static bool operator ==(Id left, Id right) => left.Equals(right);

        public static bool operator !=(Id left, Id right) => left.Equals(right) == false;

        public static implicit operator Id(string value) => new Id(value);

        public static implicit operator string(Id id) => id.Value;

        public static implicit operator Id?(string value) => string.IsNullOrEmpty(value) ? null : (Id?)new Id(value);

        public static implicit operator string(Id? id) => id?.Value;
    }
}
