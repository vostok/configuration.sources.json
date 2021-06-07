﻿using System.Linq;
using Newtonsoft.Json.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Json
{
    internal static class JsonConfigurationPrinter
    {
        public static string Print(ISettingsNode node)
        {
            var token = Build(node);
            return token.ToString();
        }

        private static JToken Build(ISettingsNode node)
        {
            switch (node)
            {
                case ObjectNode objectNode:
                    return new JObject(
                        objectNode.Children.Select(c => new JProperty(c.Name,
                            c is ValueNode ? c.Value : Build(c))));
                case ArrayNode arrayNode:
                    return new JArray(
                        arrayNode.Children.Select(Build));
                case ValueNode valueNode:
                    return new JValue(valueNode.Value);
                default:
                    return null;
            }
        }
    }
}