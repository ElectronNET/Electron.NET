using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginItemSettingsOptions
    {
        /// <summary>
        /// The executable path to compare against. Defaults to process.execPath.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string Path { get; set; }

        /// <summary>
        /// The command-line arguments to compare against. Defaults to an empty array.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string[] Args { get; set; }

        /// <summary>
        /// The type of service to query on macOS 13+. Defaults to 'mainAppService'. Only available on macOS 13 and up.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string Type { get; set; }

        /// <summary>
        /// The name of the service. Required if type is non-default. Only available on macOS 13 and up.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string ServiceName { get; set; }
    }
}