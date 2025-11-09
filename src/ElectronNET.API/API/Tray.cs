using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronNET.Common;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Add icons and context menus to the system's notification area.
    /// </summary>
    public sealed class Tray
    {
        /// <summary>
        /// Emitted when the tray icon is clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnClick
        {
            add => ApiEventManager.AddTrayEvent("tray-click", GetHashCode(), _click, value);
            remove => ApiEventManager.RemoveTrayEvent("tray-click", GetHashCode(), _click, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _click;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is right clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnRightClick
        {
            add => ApiEventManager.AddTrayEvent("tray-right-click", GetHashCode(), _rightClick, value);
            remove => ApiEventManager.RemoveTrayEvent("tray-right-click", GetHashCode(), _rightClick, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _rightClick;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is double clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnDoubleClick
        {
            add => ApiEventManager.AddTrayEvent("tray-double-click", GetHashCode(), _doubleClick, value);
            remove => ApiEventManager.RemoveTrayEvent("tray-double-click", GetHashCode(), _doubleClick, value);
        }

        private event Action<TrayClickEventArgs, Rectangle> _doubleClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon shows.
        /// </summary>
        public event Action OnBalloonShow
        {
            add => ApiEventManager.AddEvent("tray-balloon-show", GetHashCode(), _balloonShow, value);
            remove => ApiEventManager.RemoveEvent("tray-balloon-show", GetHashCode(), _balloonShow, value);
        }

        private event Action _balloonShow;

        /// <summary>
        /// Windows: Emitted when the tray balloon is clicked.
        /// </summary>
        public event Action OnBalloonClick
        {
            add => ApiEventManager.AddEvent("tray-balloon-click", GetHashCode(), _balloonClick, value);
            remove => ApiEventManager.RemoveEvent("tray-balloon-click", GetHashCode(), _balloonClick, value);
        }

        private event Action _balloonClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
        /// </summary>
        public event Action OnBalloonClosed
        {
            add => ApiEventManager.AddEvent("tray-balloon-closed", GetHashCode(), _balloonClosed, value);
            remove => ApiEventManager.RemoveEvent("tray-balloon-closed", GetHashCode(), _balloonClosed, value);
        }

        private event Action _balloonClosed;

        // TODO: Implement macOS Events

        private static Tray _tray;
        private static readonly object _syncRoot = new();

        internal Tray()
        {
        }

        internal static Tray Instance
        {
            get
            {
                if (_tray == null)
                {
                    lock (_syncRoot)
                    {
                        if (_tray == null)
                        {
                            _tray = new Tray();
                        }
                    }
                }

                return _tray;
            }
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems => _items.AsReadOnly();

        private readonly List<MenuItem> _items = new();

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItem">The menu item.</param>
        public async Task Show(string image, MenuItem menuItem)
        {
            await this.Show(image, new MenuItem[] { menuItem }).ConfigureAwait(false);
        }

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItems">The menu items.</param>
        public async Task Show(string image, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            await BridgeConnector.Socket.Emit("create-tray", image, JArray.FromObject(menuItems, this._jsonSerializer)).ConfigureAwait(false);
            _items.Clear();
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("trayMenuItemClicked");
            BridgeConnector.Socket.On<string>("trayMenuItemClicked", (id) =>
            {
                MenuItem menuItem = _items.GetMenuItem(id);
                menuItem?.Click();
            });
        }

        /// <summary>
        /// Shows the Traybar (empty).
        /// </summary>
        /// <param name="image">The image.</param>
        public async Task Show(string image)
        {
            await BridgeConnector.Socket.Emit("create-tray", image).ConfigureAwait(false);
        }

        /// <summary>
        /// Destroys the tray icon immediately.
        /// </summary>
        public async Task Destroy()
        {
            await BridgeConnector.Socket.Emit("tray-destroy").ConfigureAwait(false);
            _items.Clear();
        }

        /// <summary>
        /// Sets the image associated with this tray icon.
        /// </summary>
        /// <param name="image"></param>
        public async Task SetImage(string image)
        {
            await BridgeConnector.Socket.Emit("tray-setImage", image).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the image associated with this tray icon when pressed on macOS.
        /// </summary>
        /// <param name="image"></param>
        public async Task SetPressedImage(string image)
        {
            await BridgeConnector.Socket.Emit("tray-setPressedImage", image).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the hover text for this tray icon.
        /// </summary>
        /// <param name="toolTip"></param>
        public async Task SetToolTip(string toolTip)
        {
            await BridgeConnector.Socket.Emit("tray-setToolTip", toolTip).ConfigureAwait(false);
        }

        /// <summary>
        /// macOS: Sets the title displayed aside of the tray icon in the status bar.
        /// </summary>
        /// <param name="title"></param>
        public async Task SetTitle(string title)
        {
            await BridgeConnector.Socket.Emit("tray-setTitle", title).ConfigureAwait(false);
        }

        /// <summary>
        /// Windows: Displays a tray balloon.
        /// </summary>
        /// <param name="options"></param>
        public async Task DisplayBalloon(DisplayBalloonOptions options)
        {
            await BridgeConnector.Socket.Emit("tray-displayBalloon", JObject.FromObject(options, this._jsonSerializer)).ConfigureAwait(false);
        }

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDestroyedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On<bool>("tray-isDestroyedCompleted", (isDestroyed) =>
            {
                BridgeConnector.Socket.Off("tray-isDestroyedCompleted");

                taskCompletionSource.SetResult(isDestroyed);
            });

            await BridgeConnector.Socket.Emit("tray-isDestroyed").ConfigureAwait(false);

            return await taskCompletionSource.Task.ConfigureAwait(false);
        }

        private readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private const string ModuleName = "tray";

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="action">The handler</param>
        public void On(string eventName, Action action)
            => Events.Instance.On(ModuleName, eventName, action);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="action">The handler</param>
        public async Task On<T>(string eventName, Action<T> action)
            => await Events.Instance.On(ModuleName, eventName, action).ConfigureAwait(false);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="action">The handler</param>
        public void Once(string eventName, Action action)
            => Events.Instance.Once(ModuleName, eventName, action);

        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="action">The handler</param>
        public async Task Once<T>(string eventName, Action<T> action)
            => await Events.Instance.Once(ModuleName, eventName, action).ConfigureAwait(false);
    }
}