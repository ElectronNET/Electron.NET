using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public enum FileIconSize
    {
        /// <summary>
        /// small - 16x16 (per app.getFileIcon size mapping).
        /// </summary>
        small,

        /// <summary>
        /// normal - 32x32 (per app.getFileIcon size mapping).
        /// </summary>
        normal,

        /// <summary>
        /// large - 48x48 on Linux, 32x32 on Windows, unsupported on macOS (per app.getFileIcon size mapping).
        /// </summary>
        [SupportedOSPlatform("Linux")]
        [SupportedOSPlatform("Windows")]
        large
    }
}