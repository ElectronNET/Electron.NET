using ElectronNET.API.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;

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

        public IReadOnlyCollection<MenuItem> Items { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        public void SetApplicationMenu(MenuItem[] menuItems)
        {
            AddMenuItemsId(menuItems);
            BridgeConnector.Socket.Emit("menu-setApplicationMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Socket.On("menuItemClicked", (id) => {
                MenuItem menuItem = GetMenuItem(_items, id.ToString());
                menuItem?.Click();
            });
        }

        private void AddMenuItemsId(MenuItem[] menuItems)
        {
            for (int index = 0; index < menuItems.Length; index++)
            {
                var menuItem = menuItems[index];
                if(menuItem?.Submenu?.Length > 0)
                {
                    AddMenuItemsId(menuItem.Submenu);
                }

                if(string.IsNullOrEmpty(menuItem.Role))
                {
                    menuItem.Id = Guid.NewGuid().ToString();
                }
            }
        }

        private MenuItem GetMenuItem(List<MenuItem> menuItems, string id)
        {
            MenuItem result = new MenuItem();

            foreach (var item in menuItems)
            {
                if(item.Id == id)
                {
                    result = item;
                }
                else if(item?.Submenu?.Length > 0)
                {
                    var menuItem = GetMenuItem(item.Submenu.ToList(), id);
                    if(menuItem.Id == id)
                    {
                        result = menuItem;
                    }
                }
            }

            return result;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
