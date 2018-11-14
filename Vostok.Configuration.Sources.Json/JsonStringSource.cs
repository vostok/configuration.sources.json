using System;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.Constant;

namespace Vostok.Configuration.Sources.Json
{
    /// <inheritdoc />
    /// <summary>
    /// Json converter to <see cref="ISettingsNode"/> tree from string
    /// </summary>
    public class JsonStringSource : BaseConstantSource
    {
        /// <summary>
        /// <para>Creates a <see cref="JsonStringSource"/> instance using given string in <paramref name="json"/> parameter</para>
        /// <para>Parsing is here.</para>
        /// </summary>
        /// <param name="json">Json data in string</param>
        /// <exception cref="Exception">Json has wrong format</exception>
        public JsonStringSource(string json)
            : base(() => new JsonConfigurationConverter().Convert(json))
        {
        }
    }
}