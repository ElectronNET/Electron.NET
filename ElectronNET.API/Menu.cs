using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using ElectronNET.API.Extensions;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace ElectronNET.API
{
    public sealed class Menu
    {
        private static Menu _menu;

        internal Menu() { }

        internal static Menu Instance
        {
            get
            {
                if (_menu == null)
                {
                    _menu = new Menu();
                }

                return _menu;
            }
        }

        public IReadOnlyCollection<MenuItem> MenuItems { get { return _menuItems.AsReadOnly(); } }
        private List<MenuItem> _menuItems = new List<MenuItem>();

        public void SetApplicationMenu(MenuItem[] menuItems)
        {
            _menuItems.Clear();

            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("menu-setApplicationMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _menuItems.AddRange(menuItems);

            BridgeConnector.Socket.Off("menuItemClicked");
            BridgeConnector.Socket.On("menuItemClicked", (id) => {
                MenuItem menuItem = _menuItems.GetMenuItem(id.ToString());
                menuItem.Click?.Invoke();
            });
        }

        public IReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>> ContextMenuItems { get; internal set; }
        private Dictionary<int, List<MenuItem>> _contextMenuItems = new Dictionary<int, List<MenuItem>>();

        public void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems)
        {
            if (!_contextMenuItems.ContainsKey(browserWindow.Id))
            {
                menuItems.AddMenuItemsId();
                BridgeConnector.Socket.Emit("menu-setContextMenu", browserWindow.Id, JArray.FromObject(menuItems, _jsonSerializer));
                _contextMenuItems.Add(browserWindow.Id, menuItems.ToList());

                var x = _contextMenuItems.ToDictionary(kv => kv.Key, kv => kv.Value.AsReadOnly());
                ContextMenuItems = new ReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>>(x);

                BridgeConnector.Socket.Off("contextMenuItemClicked");
                BridgeConnector.Socket.On("contextMenuItemClicked", (results) =>
                {
                    var id = ((JArray)results).First.ToString();
                    var browserWindowId = (int)((JArray)results).Last;

                    MenuItem menuItem = _contextMenuItems[browserWindowId].GetMenuItem(id);
                    menuItem.Click?.Invoke();
                });
            }
        }

        public void ContextMenuPopup(BrowserWindow browserWindow)
        {
            BridgeConnector.Socket.Emit("menu-contextMenuPopup", browserWindow.Id);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
