using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenDevToolsOptions
    {
        /// <summary>
        /// Opens the devtools with specified dock state, can be right, bottom, undocked,
        /// detach.Defaults to last used dock state.In undocked mode it's possible to dock
        /// back.In detach mode it's not.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DevToolsMode Mode { get; set; }
    }
}