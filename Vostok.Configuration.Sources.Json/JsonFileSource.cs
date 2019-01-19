using JetBrains.Annotations;
using Vostok.Configuration.Sources.File;

namespace Vostok.Configuration.Sources.Json
{
    /// <summary>
    /// A source that parses settings from local files in JSON format.
    /// </summary>
    [PublicAPI]
    public class JsonFileSource : FileSource
    {
        public JsonFileSource([NotNull] string filePath)
            : this(new FileSourceSettings(filePath))
        {
        }
        
        public JsonFileSource([NotNull] FileSourceSettings settings)
            : base(settings, JsonConfigurationParser.Parse)
        {
        }
    }
}