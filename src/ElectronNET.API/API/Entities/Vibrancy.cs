using System.Runtime.Serialization;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum Vibrancy
    {
        /// <summary>
        /// The appearance based
        /// </summary>
        [EnumMember(Value = "appearance-based")]
        appearanceBased,

        /// <summary>
        /// The light
        /// </summary>
        light,

        /// <summary>
        /// The dark
        /// </summary>
        dark,

        /// <summary>
        /// The titlebar
        /// </summary>
        titlebar,

        /// <summary>
        /// The selection
        /// </summary>
        selection,

        /// <summary>
        /// The menu
        /// </summary>
        menu,

        /// <summary>
        /// The popover
        /// </summary>
        popover,

        /// <summary>
        /// The sidebar
        /// </summary>
        sidebar,

        /// <summary>
        /// The medium light
        /// </summary>
        [EnumMember(Value = "medium-light")]
        mediumLight,

        /// <summary>
        /// The ultra dark
        /// </summary>
        [EnumMember(Value = "ultra-dark")]
        ultraDark
    }
}