using System;
using System.Text.Json.Serialization;
using System.Runtime.Versioning;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Up-to-date with Electron API 39.2</remarks>
    public class MenuItem
    {
        /// <summary>
        /// Will be called with click(menuItem, browserWindow, event) when the menu item is
        /// clicked.
        /// </summary>
        [JsonIgnore]
        public Action Click { get; set; }

        /// <summary>
        /// Gets or sets the action (role) of the menu item. When specified, the click property will be ignored.
        /// </summary>
        public MenuRole Role { get; set; }

        /// <summary>
        /// Gets or sets the menu item type. Can be normal, separator, submenu, checkbox, radio, header (macOS 14+), or palette (macOS 14+).
        /// </summary>
        public MenuType Type { get; set; }


        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; set; }


        /// <summary>
        /// Gets or sets the sublabel.
        /// </summary>
        /// <value>
        /// The sublabel.
        /// </value>
        [SupportedOSPlatform("macos")]
        public string Sublabel { get; set; }

        /// <summary>
        /// Hover text for this menu item (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        public string ToolTip { get; set; }


        /// <summary>
        /// Gets or sets the accelerator.
        /// </summary>
        /// <value>
        /// The accelerator.
        /// </value>
        public string Accelerator { get; set; }


        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled. If false, the menu item will be greyed out and unclickable.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the item is visible. If false, the menu item will be entirely hidden.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the accelerator should work when the item is hidden. Default is true (macOS).
        /// When false, prevents the accelerator from triggering the item if the item is not visible.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public bool? AcceleratorWorksWhenHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the accelerator should be registered with the system or only displayed (Linux/Windows). Defaults to true.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public bool? RegisterAccelerator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is checked. Should only be specified for checkbox or radio items.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Should be specified for submenu type menu items. If submenu is specified, the
        /// type: 'submenu' can be omitted.If the value is not a Menu then it will be
        /// automatically converted to one using Menu.buildFromTemplate.
        /// </summary>
        public MenuItem[] Submenu { get; set; }

        /// <summary>
        /// The item to share when the role is shareMenu (macOS).
        /// </summary>
        [SupportedOSPlatform("macos")]
        public SharingItem SharingItem { get; set; }

        /// <summary>
        /// Gets or sets a unique id within a single menu. If defined then it can be used as a reference for placement.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// This field allows fine-grained definition of the specific location within a given menu.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets a list of item ids. Inserts this item before the item(s) with the specified id(s).
        /// If the referenced item doesn't exist the item will be inserted at the end of the menu.
        /// Also implies that this item should be placed in the same group as the referenced item(s).
        /// </summary>
        public string[] Before { get; set; }

        /// <summary>
        /// Gets or sets a list of item ids. Inserts this item after the item(s) with the specified id(s).
        /// If the referenced item doesn't exist the item will be inserted at the end of the menu.
        /// </summary>
        public string[] After { get; set; }

        /// <summary>
        /// Gets or sets a list of item ids. Places this item's containing group before the containing group
        /// of the item(s) with the specified id(s).
        /// </summary>
        public string[] BeforeGroupContaining { get; set; }

        /// <summary>
        /// Gets or sets a list of item ids. Places this item's containing group after the containing group
        /// of the item(s) with the specified id(s).
        /// </summary>
        public string[] AfterGroupContaining { get; set; }
    }
}