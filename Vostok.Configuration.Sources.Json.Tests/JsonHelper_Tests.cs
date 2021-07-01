using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Sources.Json.Helpers;

namespace Vostok.Configuration.Sources.Json.Tests
{
    [TestFixture]
    internal class JsonHelper_Tests
    {
        [Test]
        public void Should_merge_objects()
        {
            var json1 = @"{
  ""A"": 1,
  ""B"": 2
}";
            var json2 = @"{
  ""B"": 3,
  ""C"": ""4""
}";
            var json3 = @"{
  ""A"": 1,
  ""B"": 3,
  ""C"": ""4""
}";
            
            Check(json1, json2, json3);
        }
        
        [Test]
        public void Should_replace_arrays()
        {
            var json1 = @"{
  ""A"": 1,
  ""B"": [
    2,
    3 
  ]
}";
            var json2 = @"{
  ""B"": [ 
    4 
  ]
}";
            var json3 = @"{
  ""A"": 1,
  ""B"": [
    4
  ]
}";
            
            Check(json1, json2, json3);
        }
        
        [Test]
        public void Should_erase_with_null()
        {
            var json1 = @"{
  ""A"": { ""AA"": 1 },
  ""B"": [
    2,
    3 
  ]
}";
            var json2 = @"{
  ""A"": null,
  ""B"": null
}";
            var json3 = @"{
  ""A"": null,
  ""B"": null
}";
            
            Check(json1, json2, json3);
        }
        
        [Test]
        public void Should_erase_whole_object_with_null()
        {
            var json1 = @"{
  ""A"": { ""AA"": 1 },
  ""B"": [
    2,
    3 
  ]
}";
            var json2 = @"null";
            var json3 = @"null";
            
            Check(json1, json2, json3);
        }
        
        [Test]
        public void Should_erase_whole_object_with_value()
        {
            var json1 = @"{
  ""A"": { ""AA"": 1 },
  ""B"": [
    2,
    3 
  ]
}";
            var json2 = @"2";
            var json3 = @"2";
            
            Check(json1, json2, json3);
        }
        
        [Test]
        public void Should_erase_whole_object_with_string_number_value()
        {
            var json1 = @"{
  ""A"": { ""AA"": 1 },
  ""B"": [
    2,
    3 
  ]
}";
            var json2 = @"""2""";
            var json3 = @"""2""";
            
            Check(json1, json2, json3);
        }

        private void Check(string json1, string json2, string json3)
        {
            var merged = Merge(json1, json2);

            merged = Normalize(merged);
            json3 = Normalize(json3);
            
            merged.Should().Be(json3);
        }

        private static string Merge(string json1, string json2) =>
            JsonHelper.Write(JsonHelper.Merge(JsonHelper.Parse(json1), JsonHelper.Parse(json2)));

        private static string Normalize(string s) =>
            s.Replace("\r\n", "\n");
    }
}