using System;
using System.ComponentModel;

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
        public bool Activate { get; set; } = true;

        /// <summary>
        /// The working directory.
        /// </summary>
        public string WorkingDirectory { get; set; }
    }
}