using JetBrains.Annotations;
using Vostok.Configuration.Sources.Constant;

namespace Vostok.Configuration.Sources.Json
{
    /// <summary>
    /// A source that works by parsing in-memory JSON strings.
    /// </summary>
    [PublicAPI]
    public class JsonStringSource : LazyConstantSource
    {
        public JsonStringSource(string json)
            : base(() => JsonConfigurationParser.Parse(json))
        {
        }
    }
}