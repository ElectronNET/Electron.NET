using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Add icons and context menus to the system's notification area.
    /// </summary>
    public interface ITray
    {
        /// <summary>
        /// Emitted when the tray icon is clicked.
        /// </summary>
        event Action<TrayClickEventArgs, Rectangle> OnClick;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is right clicked.
        /// </summary>
        event Action<TrayClickEventArgs, Rectangle> OnRightClick;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is double clicked.
        /// </summary>
        event Action<TrayClickEventArgs, Rectangle> OnDoubleClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon shows.
        /// </summary>
        event Action OnBalloonShow;

        /// <summary>
        /// Windows: Emitted when the tray balloon is clicked.
        /// </summary>
        event Action OnBalloonClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
        /// </summary>
        event Action OnBalloonClosed;

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        IReadOnlyCollection<MenuItem> MenuItems { get; }

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItem">The menu item.</param>
        void Show(string image, MenuItem menuItem);

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItems">The menu items.</param>
        void Show(string image, MenuItem[] menuItems);

        /// <summary>
        /// Shows the Traybar (empty).
        /// </summary>
        /// <param name="image">The image.</param>
        void Show(string image);

        /// <summary>
        /// Destroys the tray icon immediately.
        /// </summary>
        void Destroy();

        /// <summary>
        /// Sets the image associated with this tray icon.
        /// </summary>
        /// <param name="image"></param>
        void SetImage(string image);

        /// <summary>
        /// Sets the image associated with this tray icon when pressed on macOS.
        /// </summary>
        /// <param name="image"></param>
        void SetPressedImage(string image);

        /// <summary>
        /// Sets the hover text for this tray icon.
        /// </summary>
        /// <param name="toolTip"></param>
        void SetToolTip(string toolTip);

        /// <summary>
        /// macOS: Sets the title displayed aside of the tray icon in the status bar.
        /// </summary>
        /// <param name="title"></param>
        void SetTitle(string title);

        /// <summary>
        /// Windows: Displays a tray balloon.
        /// </summary>
        /// <param name="options"></param>
        void DisplayBalloon(DisplayBalloonOptions options);

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsDestroyedAsync();

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        void On(string eventName, Action fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        void On(string eventName, Action<object> fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        void Once(string eventName, Action fn);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        void Once(string eventName, Action<object> fn);
    }
}