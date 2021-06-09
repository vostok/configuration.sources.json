using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vostok.Configuration.Sources.Json.Helpers
{
    internal static class JsonHelper
    {
        private static readonly JsonLoadSettings LoadSettings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Ignore,
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        private static readonly JsonMergeSettings MergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Ignore
        };

        public static JToken Parse(string content)
        {
            using (var reader = new JsonTextReader(new StringReader(content))
            {
                DateParseHandling = DateParseHandling.None,
            })
                return JToken.Load(reader, LoadSettings);
        }

        public static JToken Merge(JToken a, JToken b)
        {
            if (a.Type == JTokenType.Object && b.Type == JTokenType.Object)
            {
                var result = new JObject();
                result.Merge(a, MergeSettings);
                result.Merge(b, MergeSettings);
                return result;
            }

            return b;
        }
    }
}