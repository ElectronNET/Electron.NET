using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Mode for BrowserWindow/BaseWindow setProgressBar on Windows.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("Windows")]
    public enum ProgressBarMode
    {
        /// <summary>
        /// The none
        /// </summary>
        none,

        /// <summary>
        /// The normal
        /// </summary>
        normal,

        /// <summary>
        /// The indeterminate
        /// </summary>
        indeterminate,

        /// <summary>
        /// The error
        /// </summary>
        error,

        /// <summary>
        /// The paused
        /// </summary>
        paused
    }
}