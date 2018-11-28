using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.File;

namespace Vostok.Configuration.Sources.Json
{
    /// <inheritdoc />
    /// <summary>
    /// Json converter to <see cref="ISettingsNode"/> tree from file
    /// </summary>
    public class JsonFileSource : SingletonConfigurationSource
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a <see cref="JsonFileSource"/> instance using given parameter <paramref name="filePath"/>
        /// </summary>
        /// <param name="filePath">File name with settings</param>
        /// <param name="settings">File parsing settings</param>
        public JsonFileSource([NotNull] string filePath, FileSourceSettings settings = null)
            : base((filePath, settings), () => new BaseFileSource(filePath, settings, ParseSettings))
        {
        }

        private static ISettingsNode ParseSettings(string str) => new JsonConfigurationConverter().Convert(str);
    }
}