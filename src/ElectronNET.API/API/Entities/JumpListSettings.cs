using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Settings returned by app.getJumpListSettings() on Windows.
    /// Matches Electron's JumpListSettings object.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("windows")]
    public class JumpListSettings
    {
        /// <summary>
        /// The minimum number of items that will be shown in the Jump List.
        /// </summary>
        public int MinItems { get; set; } = 0;

        /// <summary>
        /// Array of JumpListItem objects that correspond to items that the user has explicitly removed from custom categories
        /// in the Jump List. These items must not be re-added to the Jump List in the next call to app.setJumpList(categories);
        /// Windows will not display any custom category that contains any of the removed items.
        /// </summary>
        public JumpListItem[] RemovedItems { get; set; } = new JumpListItem[0];
    }
}