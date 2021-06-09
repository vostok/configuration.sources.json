using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vostok.Configuration.Sources.Json.Helpers
{
    internal static class JsonParser
    {
        private static readonly JsonLoadSettings LoadSettings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Ignore,
            LineInfoHandling = LineInfoHandling.Ignore,
        };

        public static JToken Parse(string content)
        {
            using (var reader = new JsonTextReader(new StringReader(content))
            {
                DateParseHandling = DateParseHandling.None,
            })
                return JToken.Load(reader, LoadSettings);
        }
    }
}