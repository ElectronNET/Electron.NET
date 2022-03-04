using System;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Controls the behavior of OpenExternal.
    /// </summary>
    public class OpenExternalOptions
    {
        /// <summary>
        /// <see langword="true"/> to bring the opened application to the foreground. The default is <see langword="true"/>.
        /// </summary>
        [DefaultValue(true)]
        [SupportedOSPlatform("macos")]
        public bool Activate { get; set; } = true;

        /// <summary>
        /// The working directory.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public string WorkingDirectory { get; set; }
    }
}