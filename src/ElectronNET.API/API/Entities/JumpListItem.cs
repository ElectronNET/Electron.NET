using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class JumpListItem
    {
        /// <summary>
        /// The command line arguments when program is executed. Should only be set if type is task.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// Description of the task (displayed in a tooltip). Should only be set if type is task.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The index of the icon in the resource file. If a resource file contains multiple
        /// icons this value can be used to specify the zero-based index of the icon that
        /// should be displayed for this task.If a resource file contains only one icon,
        /// this property should be set to zero.
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// The absolute path to an icon to be displayed in a Jump List, which can be an
        /// arbitrary resource file that contains an icon(e.g. .ico, .exe, .dll). You can
        /// usually specify process.execPath to show the program icon.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Path of the file to open, should only be set if type is file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Path of the program to execute, usually you should specify process.execPath
        /// which opens the current program.Should only be set if type is task.
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// The text to be displayed for the item in the Jump List. Should only be set if type is task.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// One of the following: "task" | "separator" | "file"
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public JumpListItemType Type { get; set; }
    }
}
