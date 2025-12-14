using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for BrowserWindow.setProgressBar(progress, options).
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class ProgressBarOptions
    {
        /// <summary>
        /// Mode for the progress bar on Windows. Can be 'none' | 'normal' | 'indeterminate' | 'error' | 'paused'.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public ProgressBarMode Mode { get; set; } = ProgressBarMode.normal;
    }
}