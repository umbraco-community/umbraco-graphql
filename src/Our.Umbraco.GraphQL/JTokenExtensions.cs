using Newtonsoft.Json.Linq;

namespace Our.Umbraco.GraphQL
{
    internal static class JTokenExtensions
    {
        public static object GetValueOrDefault(this JToken token, string key, object defaultValue = null)
        {
            var childToken = token.Value<JToken>(key);
            if (childToken == null)
            {
                return defaultValue;
            }
            return childToken.HasValues ? childToken : defaultValue;
        }
    }
}
