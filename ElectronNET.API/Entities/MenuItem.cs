using Newtonsoft.Json;
using System;

namespace ElectronNET.API.Entities
{
    public class MenuItem
    {
        /// <summary>
        /// Will be called with click(menuItem, browserWindow, event) when the menu item is 
        /// clicked.
        /// </summary>
        [JsonIgnore]
        public Action Click { get; set; }

        /// <summary>
        /// Define the action of the menu item, when specified the click property will be
        /// ignored.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Can be normal, separator, submenu, checkbox or radio.
        /// </summary>
        public string Type { get; set; }


        public string Label { get; set; }


        public string Sublabel { get; set; }


        public string Accelerator { get; set; }


        public string Icon { get; set; }

        /// <summary>
        /// If false, the menu item will be greyed out and unclickable.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// If false, the menu item will be entirely hidden.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Should only be specified for checkbox or radio type menu items.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Should be specified for submenu type menu items. If submenu is specified, the
        /// type: 'submenu' can be omitted.If the value is not a Menu then it will be
        /// automatically converted to one using Menu.buildFromTemplate.
        /// </summary>
        public MenuItem[] Submenu { get; set; }

        /// <summary>
        /// Unique within a single menu. If defined then it can be used as a reference to
        /// this item by the position attribute.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// This field allows fine-grained definition of the specific location within a
        /// given menu.
        /// </summary>
        public string Position { get; set; }
    }
}
