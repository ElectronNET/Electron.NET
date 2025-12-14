using System.Collections.Generic;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// PrinterInfo structure as returned by webContents.getPrintersAsync(). Fields backed by MCP: name, displayName, description, options.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class PrinterInfo
    {
        /// <summary>
        /// Gets or sets the name of the printer as understood by the OS.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the printer as shown in Print Preview.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a longer description of the printer's type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status code reported by the OS. Semantics are platform-specific.
        /// Not MCP-backed: this field is not listed in Electron's PrinterInfo structure.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this printer is the system default.
        /// Not MCP-backed: this field is not listed in Electron's PrinterInfo structure.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the platform-specific printer information as an object (keys/values vary by OS).
        /// </summary>
        public Dictionary<string, string> Options { get; set; }
    }
}