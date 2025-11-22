using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// SharingItem for MenuItem role 'shareMenu' (macOS).
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("macos")]
    public class SharingItem
    {
        /// <summary>
        /// An array of text to share.
        /// </summary>
        public string[] Texts { get; set; }

        /// <summary>
        /// An array of files to share.
        /// </summary>
        public string[] FilePaths { get; set; }

        /// <summary>
        /// An array of URLs to share.
        /// </summary>
        public string[] Urls { get; set; }
    }
}