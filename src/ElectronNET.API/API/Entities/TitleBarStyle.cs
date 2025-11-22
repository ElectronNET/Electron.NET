using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum TitleBarStyle
    {
        /// <summary>
        /// The default style
        /// </summary>
        [JsonPropertyName("default")]
        defaultStyle,

        /// <summary>
        /// The hidden
        /// </summary>
        hidden,

        /// <summary>
        /// The hidden inset
        /// </summary>
        [SupportedOSPlatform("macos")]
        hiddenInset,

        /// <summary>
        /// The custom buttons on hover
        /// </summary>
        [SupportedOSPlatform("macos")]
        customButtonsOnHover
    }
}