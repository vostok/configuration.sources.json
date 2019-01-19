using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Json
{
    [PublicAPI]
    public static class JsonConfigurationParser
    {
        public static ISettingsNode Parse(string configuration)
        {
            return string.IsNullOrWhiteSpace(configuration) ? null : ParseObject(JObject.Parse(configuration));
        }
        
        private static ISettingsNode ParseObject(JObject jObject, string tokenKey = null)
        {
            var childNodes = new List<ISettingsNode>(jObject.Count);

            foreach (var token in jObject)
                switch (token.Value.Type)
                {
                    case JTokenType.Null:
                        childNodes.Add(new ValueNode(token.Key, null));
                        break;
                    case JTokenType.Object:
                        childNodes.Add(ParseObject((JObject) token.Value, token.Key));
                        break;
                    case JTokenType.Array:
                        childNodes.Add(ParseArray((JArray) token.Value, token.Key));
                        break;
                    default:
                        childNodes.Add(new ValueNode(token.Key, token.Value.ToString()));
                        break;
                }

            return new ObjectNode(tokenKey, childNodes);
        }

        private static ISettingsNode ParseArray(JArray array, string tokenKey)
        {
            if (array.Count <= 0)
                return new ArrayNode(tokenKey);

            var childNodes = new List<ISettingsNode>(array.Count);
            var index = 0;

            foreach (var item in array)
            {
                ISettingsNode node;

                switch (item.Type)
                {
                    case JTokenType.Null:
                        node = new ValueNode(null);
                        break;
                    case JTokenType.Object:
                        node = ParseObject((JObject) item, index.ToString());
                        break;
                    case JTokenType.Array:
                        node = ParseArray((JArray) item, index.ToString());
                        break;
                    default:
                        node = new ValueNode(item.ToString());
                        break;
                }

                index++;
                childNodes.Add(node);
            }

            return new ArrayNode(tokenKey, childNodes);
        }
    }
}