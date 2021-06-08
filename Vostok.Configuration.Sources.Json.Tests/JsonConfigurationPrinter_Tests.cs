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
        
        [Test]
        public void Should_not_corrupt_date()
        {
            var json = @"{
    ""A"": ""2020-11-16T00:00:00.000+06:00"",
    ""B"": [ ""2020-11-16T00:00:00.000+06:00"" ]
}
";

            var parsed = JsonConfigurationParser.Parse(json);

            var printed = JsonConfigurationPrinter.Print(parsed);
            printed.Should().Contain("2020-11-16T00:00:00.000+06:00");            
            
            var parsed2 = JsonConfigurationParser.Parse(printed);

            parsed2.Should().BeEquivalentTo(parsed);
        }
    }
}