using System.Text.Json.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenDevToolsOptions
    {
        /// <summary>
        /// Opens the DevTools with specified dock state. Can be left, right, bottom, undocked, or detach.
        /// Defaults to the last used dock state. In undocked mode it's possible to dock back; in detach mode it's not.
        /// </summary>
        public DevToolsMode Mode { get; set; }

        /// <summary>
        /// Whether to bring the opened DevTools window to the foreground. Default is true.
        /// </summary>
        public bool Activate { get; set; } = true;

        /// <summary>
        /// A title for the DevTools window (only visible in undocked or detach mode).
        /// </summary>
        public string Title { get; set; }
    }
}