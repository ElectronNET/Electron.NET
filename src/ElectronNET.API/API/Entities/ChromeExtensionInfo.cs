namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Provide metadata about the current loaded Chrome extension
    /// </summary>
    /// <yremarks>Project-specific: no matching Electron structure found in MCP docs (electronjs).</yremarks>
    public class ChromeExtensionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChromeExtensionInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the Chrome extension.</param>
        /// <param name="version">The version of the Chrome extension.</param>
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