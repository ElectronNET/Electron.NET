using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum Vibrancy
    {
        /// <summary>
        /// The appearance based
        /// </summary>
        [JsonProperty("appearance-based")]
        appearanceBased,

        /// <summary>
        /// The light
        /// </summary>
        light,

        /// <summary>
        /// The dark
        /// </summary>
        dark,

        /// <summary>
        /// The titlebar
        /// </summary>
        titlebar,

        /// <summary>
        /// The selection
        /// </summary>
        selection,

        /// <summary>
        /// The menu
        /// </summary>
        menu,

        /// <summary>
        /// The popover
        /// </summary>
        popover,

        /// <summary>
        /// The sidebar
        /// </summary>
        sidebar,

        /// <summary>
        /// The medium light
        /// </summary>
        [JsonProperty("medium-light")]
        mediumLight,

        /// <summary>
        /// The ultra dark
        /// </summary>
        [JsonProperty("ultra-dark")]
        ultraDark
    }
}