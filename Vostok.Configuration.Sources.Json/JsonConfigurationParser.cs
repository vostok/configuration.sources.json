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
            return string.IsNullOrWhiteSpace(configuration) ? null : ParseJson(JObject.Parse(configuration));
        }
        
        private static ISettingsNode ParseJson(JObject jObject, string tokenKey = null)
        {
            var list = new List<ISettingsNode>();
            foreach (var token in jObject)
                switch (token.Value.Type)
                {
                    case JTokenType.Null:
                        list.Add(new ValueNode(token.Key, null));
                        break;
                    case JTokenType.Object:
                        list.Add(ParseJson((JObject) token.Value, token.Key));
                        break;
                    case JTokenType.Array:
                        list.Add(ParseJson((JArray) token.Value, token.Key));
                        break;
                    default:
                        list.Add(new ValueNode(token.Key, token.Value.ToString()));
                        break;
                }

            return new ObjectNode(tokenKey, list);
        }

        private static ISettingsNode ParseJson(JArray jArray, string tokenKey)
        {
            if (jArray.Count <= 0)
                return new ArrayNode(tokenKey);

            var list = new List<ISettingsNode>(jArray.Count);
            var i = 0;
            foreach (var item in jArray)
            {
                ISettingsNode obj;
                switch (item.Type)
                {
                    case JTokenType.Null:
                        obj = new ValueNode(null);
                        break;
                    case JTokenType.Object:
                        obj = ParseJson((JObject) item, i.ToString());
                        break;
                    case JTokenType.Array:
                        obj = ParseJson((JArray) item, i.ToString());
                        break;
                    default:
                        obj = new ValueNode(item.ToString());
                        break;
                }

                i++;
                list.Add(obj);
            }

            return new ArrayNode(tokenKey, list);
        }
    }
}