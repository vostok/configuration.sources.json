using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Json
{
    [PublicAPI]
    public static class JsonConfigurationParser
    {
        private static readonly JsonLoadSettings Settings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Ignore,
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        public static ISettingsNode Parse(string content)
            => Parse(content, null);

        public static ISettingsNode Parse(string content, string rootName)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var token = JToken.Parse(content, Settings);
            if (token.Type == JTokenType.Null)
                return null;

            if (token is JValue jValue)
                return new ValueNode(rootName, jValue.Value.ToString());

            return ParseRootToken(token, rootName);
        }

        private static ISettingsNode ParseRootToken(JToken token, string tokenKey = null)
        {
            if (token is JObject jObject)
                return new ObjectNode(tokenKey, ParseChildNodes(jObject));

            if (token is JArray jArray)
                return new ArrayNode(tokenKey, ParseChildNodes(jArray.Select((t, index) => new KeyValuePair<string, JToken>(index.ToString(), t))));

            throw new JsonException($"Parsed root token was of unexpected type '{token?.GetType()}'.");
        }

        private static List<ISettingsNode> ParseChildNodes(IEnumerable<KeyValuePair<string, JToken>> childTokens)
        {
            var childNodes = new List<ISettingsNode>();

            foreach (var token in childTokens)
                switch (token.Value.Type)
                {
                    case JTokenType.Null:
                        childNodes.Add(new ValueNode(token.Key, null));
                        break;
                    case JTokenType.Object:
                        childNodes.Add(ParseRootToken((JObject)token.Value, token.Key));
                        break;
                    case JTokenType.Array:
                        childNodes.Add(ParseArray((JArray)token.Value, token.Key));
                        break;
                    default:
                        childNodes.Add(new ValueNode(token.Key, Convert.ToString(token.Value, CultureInfo.InvariantCulture)));
                        break;
                }

            return childNodes;
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
                        node = ParseRootToken((JObject)item, index.ToString());
                        break;
                    case JTokenType.Array:
                        node = ParseArray((JArray)item, index.ToString());
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