using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectronNET.API.Extensions
{
    internal static class MenuItemExtensions
    {
        public static MenuItem[] AddMenuItemsId(this MenuItem[] menuItems)
        {
            for (int index = 0; index < menuItems.Length; index++)
            {
                var menuItem = menuItems[index];
                if (menuItem?.Submenu?.Length > 0)
                {
                    AddMenuItemsId(menuItem.Submenu);
                }

                if (string.IsNullOrEmpty(menuItem.Id) && menuItem.Click != null)
                {
                    menuItem.Id = Guid.NewGuid().ToString();
                }
            }

            return menuItems;
        }

        public static MenuItem GetMenuItem(this List<MenuItem> menuItems, string id)
        {
            MenuItem result = new MenuItem();

            foreach (var item in menuItems)
            {
                if (item.Id == id)
                {
                    result = item;
                }
                else if (item?.Submenu?.Length > 0)
                {
                    var menuItem = GetMenuItem(item.Submenu.ToList(), id);
                    if (menuItem.Id == id)
                    {
                        result = menuItem;
                    }
                }
            }

            return result;
        }
    }
}
