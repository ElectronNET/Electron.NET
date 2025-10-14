using System.ComponentModel;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Defines the ThemeSourceMode enumeration.
    /// </summary>
    public enum ThemeSourceMode
    {
        /// <summary>
        /// Operating system default.
        /// </summary>
        [Description("system")]
        System,

        /// <summary>
        /// Light theme.
        /// </summary>
        [Description("light")]
        Light,

        /// <summary>
        /// Dark theme.
        /// </summary>
        [Description("dark")]
        Dark
    }
}