using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// About panel options.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class AboutPanelOptions
    {
        /// <summary>
        /// The app's name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The app's version.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Copyright information.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The app's build version number (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Version { get; set; }

        /// <summary>
        /// Credit information (macOS, Windows).
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public string Credits { get; set; }

        /// <summary>
        /// List of app authors (Linux).
        /// </summary>
        [SupportedOSPlatform("linux")]
        public string[] Authors { get; set; }

        /// <summary>
        /// The app's website (Linux).
        /// </summary>
        [SupportedOSPlatform("linux")]
        public string Website { get; set; }

        /// <summary>
        /// Path to the app's icon in a JPEG or PNG file format. On Linux, will be shown as 64x64 pixels while retaining aspect ratio. On Windows, a 48x48 PNG will result in the best visual quality.
        /// </summary>
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("windows")]
        public string IconPath { get; set; }
    }
}