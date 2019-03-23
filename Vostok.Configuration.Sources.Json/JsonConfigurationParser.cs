using System.Collections.Generic;
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
            LineInfoHandling = LineInfoHandling.Ignore
        };

        public static ISettingsNode Parse(string content)
            => Parse(content, null);

        public static ISettingsNode Parse(string content, string rootName)
            => string.IsNullOrWhiteSpace(content) ? null : ParseRootToken(JToken.Parse(content, Settings), rootName);

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

            foreach (var namedToken in childTokens)
                childNodes.Add(ParseToken(namedToken.Value, namedToken.Key));

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
                var node = ParseToken(item, index.ToString());

                index++;
                childNodes.Add(node);
            }

            return new ArrayNode(tokenKey, childNodes);
        }

        private static ISettingsNode ParseToken(JToken token, string name)
        {
            switch (token.Type)
            {
                case JTokenType.Null:
                    return new ValueNode(name, null);
                case JTokenType.Object:
                    return ParseRootToken((JObject)token, name);
                case JTokenType.Array:
                    return ParseArray((JArray)token, name);
                default:
                    return new ValueNode(name, token.ToString());
            }
        }
    }
}