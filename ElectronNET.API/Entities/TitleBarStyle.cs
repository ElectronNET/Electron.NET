using Newtonsoft.Json;

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
        [JsonProperty("default")]
        defaultStyle,

        /// <summary>
        /// The hidden
        /// </summary>
        hidden,

        /// <summary>
        /// The hidden inset
        /// </summary>
        hiddenInset,

        /// <summary>
        /// The custom buttons on hover
        /// </summary>
        customButtonsOnHover
    }

    public class TitleBarOverlayConfig
    {
        public string color { get; set; }
        public string symbolColor { get; set; }
        public int height { get; set; }
    }
}