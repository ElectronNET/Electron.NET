using System.Collections.Generic;
using System.Collections.ObjectModel;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Create native application menus and context menus.
    /// </summary>
    public interface IMenu
    {
        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        IReadOnlyCollection<MenuItem> MenuItems { get; }

        /// <summary>
        /// Gets the context menu items.
        /// </summary>
        /// <value>
        /// The context menu items.
        /// </value>
        IReadOnlyDictionary<int, ReadOnlyCollection<MenuItem>> ContextMenuItems { get; }

        /// <summary>
        /// Sets the application menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        void SetApplicationMenu(MenuItem[] menuItems);

        /// <summary>
        /// Sets the context menu.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        /// <param name="menuItems">The menu items.</param>
        void SetContextMenu(BrowserWindow browserWindow, MenuItem[] menuItems);

        /// <summary>
        /// Contexts the menu popup.
        /// </summary>
        /// <param name="browserWindow">The browser window.</param>
        void ContextMenuPopup(BrowserWindow browserWindow);
    }
}