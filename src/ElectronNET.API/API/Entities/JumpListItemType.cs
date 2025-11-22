using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Jump list item kinds for app.setJumpList (Windows).
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("Windows")]
    public enum JumpListItemType
    {
        /// <summary>
        /// The task
        /// </summary>
        task,

        /// <summary>
        /// The separator
        /// </summary>
        separator,

        /// <summary>
        /// The file
        /// </summary>
        file
    }
}