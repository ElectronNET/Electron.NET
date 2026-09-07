using System.Collections.Generic;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Options for loading a local file into a web page.
    /// </summary>
    public class LoadFileOptions
    {
        /// <summary>
        /// Passed to url.format().
        /// </summary>
        public Dictionary<string, string> Query { get; set; }

        /// <summary>
        /// Passed to url.format().
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Passed to url.format().
        /// </summary>
        public string Hash { get; set; }
    }
}
