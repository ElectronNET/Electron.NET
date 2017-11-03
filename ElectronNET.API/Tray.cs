using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            add
            {
                if (_click == null)
                {
                    BridgeConnector.Socket.On("tray-click-event" + GetHashCode(), (result) =>
                    {
                        var args = ((JArray)result).ToObject<object[]>();
                        var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
                        var bounds = ((JObject)args[1]).ToObject<Rectangle>();
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
                    BridgeConnector.Socket.Off("tray-click-event" + GetHashCode());
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
                    BridgeConnector.Socket.On("tray-right-click-event" + GetHashCode(), (result) =>
                    {
                        var args = ((JArray)result).ToObject<object[]>();
                        var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
                        var bounds = ((JObject)args[1]).ToObject<Rectangle>();
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
                    BridgeConnector.Socket.Off("tray-right-click-event" + GetHashCode());
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
                    BridgeConnector.Socket.On("tray-double-click-event" + GetHashCode(), (result) =>
                    {
                        var args = ((JArray)result).ToObject<object[]>();
                        var trayClickEventArgs = ((JObject)args[0]).ToObject<TrayClickEventArgs>();
                        var bounds = ((JObject)args[1]).ToObject<Rectangle>();
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
                    BridgeConnector.Socket.Off("tray-double-click-event" + GetHashCode());
            }
        }

        private event Action<TrayClickEventArgs, Rectangle> _doubleClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon shows.
        /// </summary>
        public event Action OnBalloonShow
        {
            add
            {
                if (_balloonShow == null)
                {
                    BridgeConnector.Socket.On("tray-balloon-show-event" + GetHashCode(), () =>
                    {
                        _balloonShow();
                    });

                    BridgeConnector.Socket.Emit("register-tray-balloon-show", GetHashCode());
                }
                _balloonShow += value;
            }
            remove
            {
                _balloonShow -= value;

                if (_balloonShow == null)
                    BridgeConnector.Socket.Off("tray-balloon-show-event" + GetHashCode());
            }
        }

        private event Action _balloonShow;

        /// <summary>
        /// Windows: Emitted when the tray balloon is clicked.
        /// </summary>
        public event Action OnBalloonClick
        {
            add
            {
                if (_balloonClick == null)
                {
                    BridgeConnector.Socket.On("tray-balloon-click-event" + GetHashCode(), () =>
                    {
                        _balloonClick();
                    });

                    BridgeConnector.Socket.Emit("register-tray-balloon-click", GetHashCode());
                }
                _balloonClick += value;
            }
            remove
            {
                _balloonClick -= value;

                if (_balloonClick == null)
                    BridgeConnector.Socket.Off("tray-balloon-click-event" + GetHashCode());
            }
        }

        private event Action _balloonClick;

        /// <summary>
        /// Windows: Emitted when the tray balloon is closed 
        /// because of timeout or user manually closes it.
        /// </summary>
        public event Action OnBalloonClosed
        {
            add
            {
                if (_balloonClosed == null)
                {
                    BridgeConnector.Socket.On("tray-balloon-closed-event" + GetHashCode(), () =>
                    {
                        _balloonClosed();
                    });

                    BridgeConnector.Socket.Emit("register-tray-balloon-closed", GetHashCode());
                }
                _balloonClosed += value;
            }
            remove
            {
                _balloonClosed -= value;

                if (_balloonClosed == null)
                    BridgeConnector.Socket.Off("tray-balloon-closed-event" + GetHashCode());
            }
        }

        private event Action _balloonClosed;

        // TODO: Implement macOS Events

        private static Tray _tray;
        private static object _syncRoot = new Object();

        internal Tray() { }

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
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItem">The menu item.</param>
        public void Show(string image, MenuItem menuItem)
        {
            Show(image, new MenuItem[] { menuItem });
        }

        /// <summary>
        /// Shows the Traybar.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="menuItems">The menu items.</param>
        public void Show(string image, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("create-tray", image, JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("trayMenuItemClicked");
            BridgeConnector.Socket.On("trayMenuItemClicked", (id) =>
            {
                MenuItem menuItem = _items.GetMenuItem(id.ToString());
                menuItem?.Click();
            });
        }

        /// <summary>
        /// Destroys the tray icon immediately.
        /// </summary>
        public void Destroy()
        {
            BridgeConnector.Socket.Emit("tray-destroy");
            _items.Clear();
        }

        /// <summary>
        /// Sets the image associated with this tray icon.
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(string image)
        {
            BridgeConnector.Socket.Emit("tray-setImage", image);
        }

        /// <summary>
        /// Sets the image associated with this tray icon when pressed on macOS.
        /// </summary>
        /// <param name="image"></param>
        public void SetPressedImage(string image)
        {
            BridgeConnector.Socket.Emit("tray-setPressedImage", image);
        }

        /// <summary>
        /// Sets the hover text for this tray icon.
        /// </summary>
        /// <param name="toolTip"></param>
        public void SetToolTip(string toolTip)
        {
            BridgeConnector.Socket.Emit("tray-setToolTip", toolTip);
        }

        /// <summary>
        /// macOS: Sets the title displayed aside of the tray icon in the status bar.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            BridgeConnector.Socket.Emit("tray-setTitle", title);
        }

        /// <summary>
        /// macOS: Sets when the tray’s icon background becomes highlighted (in blue).
        ///
        /// Note: You can use highlightMode with a BrowserWindow by toggling between 
        /// 'never' and 'always' modes when the window visibility changes.
        /// </summary>
        /// <param name="highlightMode"></param>
        public void SetHighlightMode(HighlightMode highlightMode)
        {
            BridgeConnector.Socket.Emit("tray-setHighlightMode", highlightMode.ToString());
        }

        /// <summary>
        /// Windows: Displays a tray balloon.
        /// </summary>
        /// <param name="options"></param>
        public void DisplayBalloon(DisplayBalloonOptions options)
        {
            BridgeConnector.Socket.Emit("tray-displayBalloon", JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsDestroyedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("tray-isDestroyedCompleted", (isDestroyed) =>
            {
                BridgeConnector.Socket.Off("tray-isDestroyedCompleted");

                taskCompletionSource.SetResult((bool)isDestroyed);
            });

            BridgeConnector.Socket.Emit("tray-isDestroyed");

            return taskCompletionSource.Task;
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
