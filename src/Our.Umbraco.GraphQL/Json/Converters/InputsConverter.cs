using System;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json;

namespace Our.Umbraco.GraphQL.Json.Converters
{
    internal class InputsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Inputs);

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var dict = serializer.Deserialize<Dictionary<string, object>>(reader);
            return new Inputs(dict ?? new Dictionary<string, object>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotSupportedException();
    }
}
