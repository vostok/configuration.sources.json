using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Configuration.Sources.Json.Tests
{
    [TestFixture]
    public class JsonConfigurationParser_Tests
    {
        [TestCase(null, TestName = "when string is null")]
        [TestCase(" ", TestName = "when string is whitespace")]
        public void Should_return_null(string json)
        {
            JsonConfigurationParser.Parse(json).Should().BeNull();
        }
        
        [TestCase("")]
        [TestCase("string")]
        public void Should_parse_string_value(string value)
        {
            var result = JsonConfigurationParser.Parse($"{{ 'StringValue': '{value}' }}");
            result["StringValue"].Value.Should().Be(value);
        }

        [Test]
        public void Should_parse_integer_value()
        {
            const string value = "{ 'IntValue': 123 }";

            var result = JsonConfigurationParser.Parse(value);
            result["IntValue"].Value.Should().Be("123");
        }

        [Test]
        public void Should_parse_Double_value()
        {
            const string value = "{ 'DoubleValue': 123.321 }";

            var result = JsonConfigurationParser.Parse(value);
            result["DoubleValue"].Value.Should().Be(123.321d.ToString(CultureInfo.CurrentCulture));
        }

        [Test]
        public void Should_parse_Boolean_value()
        {
            const string value = "{ 'BooleanValue': true }";

            var result = JsonConfigurationParser.Parse(value);
            result["BooleanValue"].Value.Should().Be("True");
        }

        [Test]
        public void Should_parse_Null_value()
        {
            const string value = "{ 'NullValue': null }";

            var result = JsonConfigurationParser.Parse(value);
            result["NullValue"].Value.Should().Be(null);
        }

        [Test]
        public void Should_parse_Array_value()
        {
            const string value = "{ 'IntArray': [1, 2, 3] }";

            var result = JsonConfigurationParser.Parse(value);
            result["IntArray"].Children.Select(c => c.Value).Should().Equal("1", "2", "3");
        }

        [Test]
        public void Should_parse_Object_value()
        {
            const string value = "{ 'Object': { 'StringValue': 'str' } }";

            var result = JsonConfigurationParser.Parse(value);
            result["Object"]["StringValue"].Value.Should().Be("str");
        }
        
        [Test]
        public void Should_parse_empty_Object_value()
        {
            const string value = "{ 'Object': { } }";

            var result = JsonConfigurationParser.Parse(value);
            result["Object"].Should().Be(new ObjectNode("Object"));
        }
        
        [Test]
        public void Should_parse_ArrayOfObjects_value()
        {
            const string value = "{ 'Array': [{ 'StringValue': 'str' }, { 'IntValue': 123 }] }";
            
            var result = JsonConfigurationParser.Parse(value);
            result["Array"].Children.Count().Should().Be(2);
            result["Array"].Children.First()["StringValue"].Value.Should().Be("str");
            result["Array"].Children.Last()["IntValue"].Value.Should().Be("123");
        }

        [Test]
        public void Should_parse_ArrayOfNulls_value()
        {
            const string value = "{ 'Array': [null, null] }";

            var result = JsonConfigurationParser.Parse(value);
            result["Array"].Children.Select(c => c.Value).Should().Equal(null, null);
        }

        [Test]
        public void Should_parse_ArrayOfArrays_value()
        {
            const string value = "{ 'Array': [['s', 't'], ['r']] }";

            var result = JsonConfigurationParser.Parse(value);
            result["Array"].Children.Count().Should().Be(2);
            result["Array"].Children.First().Children.Select(c => c.Value).Should().Equal("s", "t");
            result["Array"].Children.Last().Children.Select(c => c.Value).Should().Equal("r");
        }

        [Test]
        public void Should_parse_value_with_empty_root_object()
        {
            const string value = "{ }";

            var result = JsonConfigurationParser.Parse(value);
            result.Should().Be(new ObjectNode(new ISettingsNode[]{}));
        }
        
        [Test]
        public void Should_ignore_key_case()
        {
            var settings = JsonConfigurationParser.Parse("{ 'StringValue': 'string' }");
            settings["STRINGVALUE"].Value.Should().Be("string");
        }

        [Test]
        public void Should_throw_on_wrong_json_format()
        {
            const string value = "wrong format";
            new Action(() => { JsonConfigurationParser.Parse(value); }).Should().Throw<Exception>();
        }

        [Test]
        public void Should_throw_when_value_is_array()
        {
            const string value = "[1, 2]";
            new Action(() => { JsonConfigurationParser.Parse(value); }).Should().Throw<Exception>();
        }
    }
}