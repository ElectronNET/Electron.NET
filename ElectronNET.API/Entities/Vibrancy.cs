using System.Runtime.Serialization;
using System;

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
        [Obsolete("Removed in macOS Catalina (10.15).")]
        appearanceBased,

        /// <summary>
        /// The light
        /// </summary>
        [Obsolete("Removed in macOS Catalina (10.15).")]
        light,

        /// <summary>
        /// The dark
        /// </summary>
        [Obsolete("Removed in macOS Catalina (10.15).")]
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
        [Obsolete("Removed in macOS Catalina (10.15).")]
        mediumLight,

        /// <summary>
        /// The ultra dark
        /// </summary>
        [EnumMember(Value = "ultra-dark")]
        [Obsolete("Removed in macOS Catalina (10.15).")]
        ultraDark,
        
        header, 
        
        sheet, 
        
        window, 
        
        hud, 
        
        [EnumMember(Value = "fullscreen-ui")]
        fullscreenUI, 
        
        tooltip, 
        
        content, 
        
        [EnumMember(Value = "under-window")]
        underWindow,
        
        [EnumMember(Value = "under-page")]
        underPage
        
        
        
    }
}
