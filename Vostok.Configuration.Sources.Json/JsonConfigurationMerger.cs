using System;
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

            if (!(a is ValueNode aValue) || aValue.Name?.EndsWith(JsonExtension) != true)
                return false;

            if (!(b is ValueNode bValue) || bValue.Name?.EndsWith(JsonExtension) != true)
                return false;

            if (aValue.Name != bValue.Name)
                return false;

            try
            {
                var aParsed = JsonHelper.Parse(aValue.Value);
                var bParsed = JsonHelper.Parse(bValue.Value);

                var merged = JsonHelper.Merge(aParsed, bParsed);

                var json = merged.ToString();

                result = new ValueNode(aValue.Name, json);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}