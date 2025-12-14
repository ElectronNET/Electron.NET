using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Jump List item used in app.setJumpList(categories) on Windows.
    /// Matches Electron's JumpListItem structure.
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    [SupportedOSPlatform("windows")]
    public class JumpListItem
    {
        /// <summary>
        /// Gets or sets the command line arguments when <c>program</c> is executed. Should only be set if <c>type</c> is <c>task</c>.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Gets or sets the description of the task (displayed in a tooltip). Should only be set if <c>type</c> is <c>task</c>. Maximum length 260 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the index of the icon in the resource file. If a resource file contains multiple
        /// icons this value can be used to specify the zero-based index of the icon that
        /// should be displayed for this task. If a resource file contains only one icon,
        /// this property should be set to zero.
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to an icon to be displayed in a Jump List, which can be an
        /// arbitrary resource file that contains an icon(e.g. .ico, .exe, .dll). You can
        /// usually specify process.execPath to show the program icon.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the file to open; should only be set if <c>type</c> is <c>file</c>.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the path of the program to execute, usually specify process.execPath
        /// which opens the current program. Should only be set if <c>type</c> is <c>task</c>.
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// Gets or sets the text to be displayed for the item in the Jump List. Should only be set if <c>type</c> is <c>task</c>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the item type. One of: <c>task</c> | <c>separator</c> | <c>file</c>.
        /// </summary>
        public JumpListItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the working directory. Default is empty.
        /// </summary>
        public string WorkingDirectory { get; set; }
    }
}