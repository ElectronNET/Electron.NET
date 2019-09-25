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
        private string _name;
        private string _version;


        internal ChromeExtensionInfo(string name, string version)
        {
            _name = name;
            _version = version;
        }

        /// <summary>
        /// Name of the Chrome extension
        /// </summary>
        public string Name
        {
            get => _name;
        }

        /// <summary>
        /// Version of the Chrome extension
        /// </summary>
        public string Version
        {
            get => _version;
        }
    }
}
