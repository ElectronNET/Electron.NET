using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using ElectronNET.API.Extensions;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using Microsoft.AspNetCore.SignalR;

namespace ElectronNET.API
{
    /// <summary>
    /// Create native application menus and context menus.
    /// </summary>
    public sealed class Menu
    {
        private static Menu _menu;
        private static object _syncRoot = new object();

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
        private List<MenuItem> _menuItems = new List<MenuItem>();

        /// <summary>
        /// Sets the application menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public async void SetApplicationMenu(MenuItem[] menuItems)
        {
            _menuItems.Clear();

            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            await Electron.SignalrElectron.Clients.All.SendAsync("menu-setApplicationMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _menuItems.AddRange(menuItems);
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
        public async void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            menuItems.AddSubmenuTypes();

            Electron.SignalrElectron.Clients.All.SendAsync("menu-setContextMenu", browserWindow.Id, JArray.FromObject(menuItems, _jsonSerializer));

            if (!_contextMenuItems.ContainsKey(browserWindow.Id))
            {
                _contextMenuItems.Add(browserWindow.Id, menuItems.ToList());
                var x = _contextMenuItems.ToDictionary(kv => kv.Key, kv => kv.Value.AsReadOnly());
                ContextMenuItems = new ReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>>(x);
            }
        }

        /// <summary>
        /// Contexts the menu popup.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        public async void ContextMenuPopup(BrowserWindow browserWindow)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("menu-contextMenuPopup", browserWindow.Id);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
