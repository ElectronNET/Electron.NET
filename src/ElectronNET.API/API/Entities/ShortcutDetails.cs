namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Structure of a shortcut.
    /// </summary>
    public class ShortcutDetails
    {
        /// <summary>
        /// The Application User Model ID. Default is <see cref="string.Empty"/>.
        /// </summary>
        public string AppUserModelId { get; set; }

        /// <summary>
        /// The arguments to be applied to <see cref="Target"/> when launching from this shortcut. Default is <see cref="string.Empty"/>.
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// The working directory. Default is <see cref="string.Empty"/>.
        /// </summary>
        public string Cwd { get; set; }

        /// <summary>
        /// The description of the shortcut. Default is <see cref="string.Empty"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The path to the icon, can be a DLL or EXE. <see cref="Icon"/> and <see cref="IconIndex"/> have to be set
        /// together. Default is <see cref="string.Empty"/>, which uses the target's icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The resource ID of icon when <see cref="Icon"/> is a DLL or EXE. Default is 0.
        /// </summary>
        public int IconIndex { get; set; }

        /// <summary>
        /// The target to launch from this shortcut.
        /// </summary>
        public string Target { get; set; }
    }
}