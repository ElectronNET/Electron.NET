using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ElectronNET.API.Serialization;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API
{
    /// <summary>
    /// Add icons and context menus to the system's notification area.
    /// </summary>
    public sealed class Tray : ApiBase
    {
        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashedLower;

        /// <summary>
        /// Emitted when the tray icon is clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnClick
        {
            add
            {
                if (_click == null)
                {
                    BridgeConnector.Socket.On<JsonElement>("tray-click" + GetHashCode(), (result) =>
                    {
                        var array = result.EnumerateArray().ToArray();
                        var trayClickEventArgs = array[0].Deserialize(ElectronJsonContext.Default.TrayClickEventArgs);
                        var bounds = array[1].Deserialize(ElectronJsonContext.Default.Rectangle);
                        _click(trayClickEventArgs, bounds);
                    });

                    BridgeConnector.Socket.Emit("register-tray-click", GetHashCode());
                }

                _click += value;
            }
            remove
            {
                _click -= value;

                if (_click == null)
                {
                    BridgeConnector.Socket.Off("tray-click" + GetHashCode());
                }
            }
        }

        private event Action<TrayClickEventArgs, Rectangle> _click;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is right clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnRightClick
        {
            add
            {
                if (_rightClick == null)
                {
                    BridgeConnector.Socket.On<JsonElement>("tray-right-click" + GetHashCode(), (result) =>
                    {
                        var array = result.EnumerateArray().ToArray();
                        var trayClickEventArgs = array[0].Deserialize(ElectronJsonContext.Default.TrayClickEventArgs);
                        var bounds = array[1].Deserialize(ElectronJsonContext.Default.Rectangle);
                        _rightClick(trayClickEventArgs, bounds);
                    });

                    BridgeConnector.Socket.Emit("register-tray-right-click", GetHashCode());
                }

                _rightClick += value;
            }
            remove
            {
                _rightClick -= value;

                if (_rightClick == null)
                {
                    BridgeConnector.Socket.Off("tray-right-click" + GetHashCode());
                }
            }
        }

        private event Action<TrayClickEventArgs, Rectangle> _rightClick;

        /// <summary>
        /// macOS, Windows: Emitted when the tray icon is double clicked.
        /// </summary>
        public event Action<TrayClickEventArgs, Rectangle> OnDoubleClick
        {
            add
            {
                if (_doubleClick == null)
                {
                    BridgeConnector.Socket.On<JsonElement>("tray-double-click" + GetHashCode(), (result) =>
                    {
                        var array = result.EnumerateArray().ToArray();
                        var trayClickEventArgs = array[0].Deserialize(ElectronJsonContext.Default.TrayClickEventArgs);
                        var bounds = array[1].Deserialize(ElectronJsonContext.Default.Rectangle);
                        _doubleClick(trayClickEventArgs, bounds);
                    });

                    BridgeConnector.Socket.Emit("register-tray-double-click", GetHashCode());
                }

                _doubleClick += value;
            }
            remove
            {
                _doubleClick -= value;

                if (_doubleClick == null)
                {
                    BridgeConnector.Socket.Off("tray-double-click" + GetHashCode());
                }
            }
        }

        private event Action<TrayClickEventArgs, Rectangle> _doubleClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon shows.
        /// </summary>
        public event Action OnBalloonShow
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Windows: Emitted when the tray balloon is clicked.
        /// </summary>
        public event Action OnBalloonClick
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Windows: Emitted when the tray balloon is closed
        /// because of timeout or user manually closes it.
        /// </summary>
        public event Action OnBalloonClosed
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

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
            await BridgeConnector.Socket.Emit("create-tray", image, menuItems).ConfigureAwait(false);
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
            await BridgeConnector.Socket.Emit("tray-displayBalloon", options).ConfigureAwait(false);
        }

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDestroyedAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.Once<bool>("tray-isDestroyedCompleted", tcs.SetResult);
            await BridgeConnector.Socket.Emit("tray-isDestroyed").ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }


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