using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using ElectronNET.API.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace ElectronNET.API
{
    /// <summary>
    /// Create native application menus and context menus.
    /// </summary>
    public sealed class Menu
    {
        private static Menu _menu;
        private static object _syncRoot = new object();

        internal Menu()
        {
        }

        internal static Menu Instance
        {
            get
            {
                if (_menu == null)
                {
                    lock (_syncRoot)
                    {
                        if (_menu == null)
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
        public IReadOnlyCollection<MenuItem> MenuItems
        {
            get
            {
                return _menuItems.AsReadOnly();
            }
        }

        private List<MenuItem> _menuItems = new List<MenuItem>();

        /// <summary>
        /// Sets the application menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public void SetApplicationMenu(MenuItem[] menuItems)
        {
            _menuItems.Clear();

            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            BridgeConnector.Socket.Emit("menu-setApplicationMenu", new[] { menuItems });
            _menuItems.AddRange(menuItems);

            BridgeConnector.Socket.Off("menuItemClicked");
            BridgeConnector.Socket.On<string>("menuItemClicked", (id) =>
            {
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

        private Dictionary<int, List<MenuItem>> _contextMenuItems = new Dictionary<int, List<MenuItem>>();

        /// <summary>
        /// Sets the context menu.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        /// <param name="menuItems">The menu items.</param>
        public void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            BridgeConnector.Socket.Emit("menu-setContextMenu", browserWindow.Id, menuItems);

            if (!_contextMenuItems.ContainsKey(browserWindow.Id))
            {
                _contextMenuItems.Add(browserWindow.Id, menuItems.ToList());
                var x = _contextMenuItems.ToDictionary(kv => kv.Key, kv => kv.Value.AsReadOnly());
                ContextMenuItems = new ReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>>(x);
            }

            BridgeConnector.Socket.Off("contextMenuItemClicked");
            BridgeConnector.Socket.On<JsonElement>("contextMenuItemClicked", (results) =>
            {
                var arr = results.EnumerateArray();
                var e = arr.GetEnumerator();
                e.MoveNext();
                var id = e.Current.GetString();
                e.MoveNext();
                var browserWindowId = e.Current.GetInt32();

                MenuItem menuItem = _contextMenuItems[browserWindowId].GetMenuItem(id);
                menuItem.Click?.Invoke();
            });
        }

        /// <summary>
        /// Contexts the menu popup.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        public void ContextMenuPopup(BrowserWindow browserWindow)
        {
            BridgeConnector.Socket.Emit("menu-contextMenuPopup", browserWindow.Id);
        }
    }
}