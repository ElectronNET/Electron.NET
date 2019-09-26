using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Provide metadata about the current loaded Chrome extension
    /// </summary>
    public class ChromeExtensionInfo
    {
        public ChromeExtensionInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Name of the Chrome extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the Chrome extension
        /// </summary>
        public string Version { get; set; }
    }
}
