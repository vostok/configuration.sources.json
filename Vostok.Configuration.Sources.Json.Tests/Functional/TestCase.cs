using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.Json.Tests.Functional
{
    internal static class TestCase
    {
        public static readonly string Json = @"{ ""key"": [1, 2, 3] }";
        public static readonly ISettingsNode SettingsNode = new ObjectNode(new []
        {
            new ArrayNode("key", new []
            {
                new ValueNode("0", "1"),
                new ValueNode("1", "2"),
                new ValueNode("2", "3")
            })
        });
    }
}