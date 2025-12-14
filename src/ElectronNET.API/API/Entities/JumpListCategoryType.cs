using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Jump list category kinds for app.setJumpList (Windows).
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("Windows")]
    public enum JumpListCategoryType
    {
        /// <summary>
        /// The tasks
        /// </summary>
        tasks,

        /// <summary>
        /// The frequent
        /// </summary>
        frequent,

        /// <summary>
        /// The recent
        /// </summary>
        recent,

        /// <summary>
        /// The custom
        /// </summary>
        custom
    }
}