namespace ElectronNET.API
{
    /// <summary>
    /// 
    /// </summary>
    public class ShortcutDetails
    {
        /// <summary>
        /// The Application User Model ID. Default is empty.
        /// </summary>
        public string AppUserModelId { get; set; }

        /// <summary>
        /// The arguments to be applied to target when launching from this shortcut. Default is empty.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// The working directory. Default is empty.
        /// </summary>
        public string Cwd { get; set; }

        /// <summary>
        /// The description of the shortcut. Default is empty.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The path to the icon, can be a DLL or EXE. icon and iconIndex have to be set
        /// together.Default is empty, which uses the target's icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The resource ID of icon when icon is a DLL or EXE. Default is 0.
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// The target to launch from this shortcut.
        /// </summary>
        public string Target { get; set; }
    }
}