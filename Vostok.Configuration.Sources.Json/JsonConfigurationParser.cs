using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Json.Helpers;

namespace Vostok.Configuration.Sources.Json
{
    [PublicAPI]
    public static class JsonConfigurationParser
    {
        public static ISettingsNode Parse(string content)
            => Parse(content, null);

        public static ISettingsNode Parse(string content, string rootName)
            => Parse(content, rootName, null);

        public static ISettingsNode Parse(string content, string rootName, Func<string, string> internDelegate)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var token = JsonHelper.Parse(content);

            if (token.Type == JTokenType.Null)
                return null;

            if (token is JValue jValue)
                return new ValueNode(rootName, Intern(jValue.Value.ToString(), internDelegate));

            return ParseRootToken(token, internDelegate, rootName);
        }

        private static ISettingsNode ParseRootToken(JToken token, Func<string, string> internDelegate, string tokenKey = null)
        {
            if (token is JObject jObject)
                return new ObjectNode(Intern(tokenKey, internDelegate), ParseChildNodes(jObject, internDelegate));

            if (token is JArray jArray)
                return new ArrayNode(Intern(tokenKey, internDelegate), ParseChildNodes(jArray.Select((t, index) => new KeyValuePair<string, JToken>(index.ToString(), t)), internDelegate));

            throw new JsonException($"Parsed root token was of unexpected type '{token?.GetType()}'.");
        }

        private static List<ISettingsNode> ParseChildNodes(IEnumerable<KeyValuePair<string, JToken>> childTokens, Func<string, string> internDelegate)
        {
            var childNodes = new List<ISettingsNode>();

            foreach (var token in childTokens)
                switch (token.Value.Type)
                {
                    case JTokenType.Null:
                        childNodes.Add(new ValueNode(Intern(token.Key, internDelegate), null));
                        break;
                    case JTokenType.Object:
                        childNodes.Add(ParseRootToken((JObject)token.Value, internDelegate, token.Key));
                        break;
                    case JTokenType.Array:
                        childNodes.Add(ParseArray((JArray)token.Value, token.Key, internDelegate));
                        break;
                    default:
                        childNodes.Add(new ValueNode(Intern(token.Key, internDelegate), Intern(token.Value.ToString(), internDelegate)));
                        break;
                }

            return childNodes;
        }

        private static ISettingsNode ParseArray(JArray array, string tokenKey, Func<string, string> internDelegate)
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
                        node = ParseRootToken((JObject)item, internDelegate, index.ToString());
                        break;
                    case JTokenType.Array:
                        node = ParseArray((JArray)item, index.ToString(), internDelegate);
                        break;
                    default:
                        node = new ValueNode(Intern(item.ToString(), internDelegate));
                        break;
                }

                index++;
                childNodes.Add(node);
            }

            return new ArrayNode(tokenKey, childNodes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Intern(string value, Func<string,string> interningFunc)
        {
            return interningFunc == null || value == null 
                    ? value 
                    : interningFunc(value);
        }
    }
}