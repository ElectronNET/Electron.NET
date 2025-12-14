using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Jump List category definition used with app.setJumpList(categories).
    /// Matches Electron's JumpListCategory structure.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("windows")]
    public class JumpListCategory
    {
        /// <summary>
        /// Gets or sets the name; must be set if <c>type</c> is <c>custom</c>, otherwise it should be omitted.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the array of <see cref="JumpListItem"/> objects if <c>type</c> is <c>tasks</c> or <c>custom</c>; otherwise it should be omitted.
        /// </summary>
        public JumpListItem[] Items { get; set; }

        /// <summary>
        /// Gets or sets the category type. One of: <c>tasks</c> | <c>frequent</c> | <c>recent</c> | <c>custom</c>.
        /// </summary>
        public JumpListCategoryType Type { get; set; }
    }
}