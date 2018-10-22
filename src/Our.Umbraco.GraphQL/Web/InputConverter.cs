using GraphQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Web
{
    internal class InputConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Inputs);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
                return null;

            var dict = serializer.Deserialize<Dictionary<string, object>>(reader);

            return new Inputs(dict ?? new Dictionary<string, object>());
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
