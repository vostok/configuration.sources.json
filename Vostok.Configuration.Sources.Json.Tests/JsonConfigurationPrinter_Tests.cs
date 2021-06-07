using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Configuration.Sources.Json.Tests
{
    [TestFixture]
    internal class JsonConfigurationPrinter_Tests
    {
        [Test]
        public void Should_print()
        {
            var json = @"{
    ""B"": 2,
    ""C"": {
        ""D"": ""4"",
        ""E"": [""5"", ""6"", ""7""],
        ""F"": []
    },
    ""F"": null,
    ""K"": [ true, false ],
    ""Q"": ""string \""X\n"",
    ""P"": [ {""A"": 3}, {""A"": 4 } ]
}
";

            var parsed = JsonConfigurationParser.Parse(json);

            var printed = JsonConfigurationPrinter.Print(parsed);

            var parsed2 = JsonConfigurationParser.Parse(printed);

            parsed2.Should().BeEquivalentTo(parsed);
        }
    }
}