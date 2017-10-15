namespace ElectronNET.API.Entities
{
    public class JumpListItem
    {
        /// <summary>
        /// The command line arguments when program is executed. Should only be set if type is task.
        /// </summary>
        public string Args { get; set; } = string.Empty;

        /// <summary>
        /// Description of the task (displayed in a tooltip). Should only be set if type is task.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The index of the icon in the resource file. If a resource file contains multiple
        /// icons this value can be used to specify the zero-based index of the icon that
        /// should be displayed for this task.If a resource file contains only one icon,
        /// this property should be set to zero.
        /// </summary>
        public int IconIndex { get; set; } = 0;

        /// <summary>
        /// The absolute path to an icon to be displayed in a Jump List, which can be an
        /// arbitrary resource file that contains an icon(e.g. .ico, .exe, .dll). You can
        /// usually specify process.execPath to show the program icon.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>
        /// Path of the file to open, should only be set if type is file.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Path of the program to execute, usually you should specify process.execPath
        /// which opens the current program.Should only be set if type is task.
        /// </summary>
        public string Program { get; set; } = string.Empty;

        /// <summary>
        /// The text to be displayed for the item in the Jump List. Should only be set if type is task.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// One of the following: "task" | "separator" | "file"
        /// </summary>
        public string Type {get; set; } = string.Empty;
    }
}
