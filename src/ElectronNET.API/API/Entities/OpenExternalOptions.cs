using System.ComponentModel;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Controls the behavior of OpenExternal.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class OpenExternalOptions
    {
        /// <summary>
        /// Gets or sets whether to bring the opened application to the foreground. The default is <see langword="true"/>.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [DefaultValue(true)]
        public bool Activate { get; set; } = true;

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating a user-initiated launch that enables tracking of frequently used programs and other behaviors. The default is <see langword="false"/>.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [DefaultValue(false)]
        public bool LogUsage { get; set; } = false;
    }
}