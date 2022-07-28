using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using ElectronNET.API.Extensions;
using System.Linq;
using System.Collections.ObjectModel;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Create native application menus and context menus.
    /// </summary>
    public sealed class Menu : IMenu
    {
        private static Menu _menu;
        private static readonly object _syncRoot = new();

        internal Menu() { }

        internal static Menu Instance
        {
            get
            {
                if (_menu == null)
                {
                    lock (_syncRoot)
                    {
                        if(_menu == null)
                        {
                            _menu = new Menu();
                        }
                    }
                }

                return _menu;
            }
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _menuItems.AsReadOnly(); } }
        private readonly List<MenuItem> _menuItems = new();

        /// <summary>
        /// Sets the application menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public void SetApplicationMenu(MenuItem[] menuItems)
        {
            _menuItems.Clear();

            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            BridgeConnector.Emit("menu-setApplicationMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _menuItems.AddRange(menuItems);

            BridgeConnector.Off("menuItemClicked");
            BridgeConnector.On<string>("menuItemClicked", (id) => {
                MenuItem menuItem = _menuItems.GetMenuItem(id);
                menuItem.Click?.Invoke();
            });
        }

        /// <summary>
        /// Gets the context menu items.
        /// </summary>
        /// <value>
        /// The context menu items.
        /// </value>
        public IReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>> ContextMenuItems { get; internal set; }
        private readonly Dictionary<int, List<MenuItem>> _contextMenuItems = new();

        /// <summary>
        /// Sets the context menu.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        /// <param name="menuItems">The menu items.</param>
        public void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            BridgeConnector.Emit("menu-setContextMenu", browserWindow.Id, JArray.FromObject(menuItems, _jsonSerializer));

            if (!_contextMenuItems.ContainsKey(browserWindow.Id))
            {
                _contextMenuItems.Add(browserWindow.Id, menuItems.ToList());
                var x = _contextMenuItems.ToDictionary(kv => kv.Key, kv => kv.Value.AsReadOnly());
                ContextMenuItems = new ReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>>(x);
            }

            BridgeConnector.Off("contextMenuItemClicked");
            BridgeConnector.On<MenuResponse>("contextMenuItemClicked", (results) =>
            {
                MenuItem menuItem = _contextMenuItems[results.windowId].GetMenuItem(results.id);
                menuItem.Click?.Invoke();
            });
        }

        /// <summary>
        /// Contexts the menu popup.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        public void ContextMenuPopup(BrowserWindow browserWindow)
        {
            BridgeConnector.Emit("menu-contextMenuPopup", browserWindow.Id);
        }

        private readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
