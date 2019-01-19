using JetBrains.Annotations;
using Vostok.Configuration.Sources.Manual;

namespace Vostok.Configuration.Sources.Json
{
    /// <summary>
    /// A source that works by parsing in-memory JSON strings.
    /// </summary>
    [PublicAPI]
    public class JsonStringSource : ManualFeedSource<string>
    {
        public JsonStringSource(string json)
            : base(JsonConfigurationParser.Parse)
        {
            Push(json);
        }
    }
}