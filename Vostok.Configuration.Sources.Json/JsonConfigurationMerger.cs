using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Json.Helpers;

namespace Vostok.Configuration.Sources.Json
{
    [PublicAPI]
    public static class JsonConfigurationMerger
    {
        private const string JsonExtension = ".json";

        public static bool TryMerge(ISettingsNode a, ISettingsNode b, out ISettingsNode result)
        {
            result = null;

            if (a.Name?.EndsWith(JsonExtension) != true)
                return false;
            if (b.Name?.EndsWith(JsonExtension) != true)
                return false;

            if (a.Name != b.Name)
                return false;

            if (!TryGetValue(a, out var aValue))
                return false;
            if (!TryGetValue(b, out var bValue))
                return false;

            try
            {
                var aParsed = JsonHelper.Parse(aValue);
                var bParsed = JsonHelper.Parse(bValue);

                var merged = JsonHelper.Merge(aParsed, bParsed);

                var mergedJson = JsonHelper.Write(merged);

                result = a is ObjectNode && b is ObjectNode
                    ? (ISettingsNode)new ObjectNode(a.Name, new []{new ValueNode(string.Empty, mergedJson)})
                    : new ValueNode(a.Name, mergedJson);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryGetValue(ISettingsNode node, out string result)
        {
            if (node is ValueNode valueNode)
            {
                result = valueNode.Value;
                return true;
            }

            if (node is ObjectNode objectNode && objectNode.Children.Count() == 1)
                return TryGetValue(objectNode.Children.Single(), out result);

            result = null;
            return false;
        }
    }
}