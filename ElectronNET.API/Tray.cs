using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Microsoft.AspNetCore.SignalR;
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-click", GetHashCode());
                }
                _click += value;
            }
            remove
            {
                _click -= value;
            }
        }

        public void TriggerOnClick(TrayClickEventArgs trayClickEventArgs, Rectangle rectangle)
        {
            _click(trayClickEventArgs, rectangle);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-right-click", GetHashCode());
                }
                _rightClick += value;
            }
            remove
            {
                _rightClick -= value;
            }
        }

        public void TriggerOnRightClick(TrayClickEventArgs trayClickEventArgs, Rectangle rectangle)
        {
            _rightClick(trayClickEventArgs, rectangle);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-double-click", GetHashCode());
                }
                _doubleClick += value;
            }
            remove
            {
                _doubleClick -= value;
            }
        }

        public void TriggerOnDoubleClick(TrayClickEventArgs trayClickEventArgs, Rectangle rectangle)
        {
            _doubleClick(trayClickEventArgs, rectangle);
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-balloon-show", GetHashCode());
                }
                _balloonShow += value;
            }
            remove
            {
                _balloonShow -= value;
            }
        }

        public void TriggerOnBalloonShow()
        {
            _balloonShow();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-balloon-click", GetHashCode());
                }
                _balloonClick += value;
            }
            remove
            {
                _balloonClick -= value;
            }
        }

        public void TriggerOnBalloonClick()
        {
            _balloonClick();
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
                    Electron.SignalrElectron.Clients.All.SendAsync("register-tray-balloon-closed", GetHashCode());
                }
                _balloonClosed += value;
            }
            remove
            {
                _balloonClosed -= value;
            }
        }

        public void TriggerOnBalloonClosed()
        {
            _balloonClosed();
        }

        private event Action _balloonClosed;

        // TODO: Implement macOS Events

        private static Tray _tray;
        private static object _syncRoot = new object();

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
        public async Task Show(string image, MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            await Electron.SignalrElectron.Clients.All.SendAsync("create-tray", image, JArray.FromObject(menuItems, _jsonSerializer));

            _items.Clear();
            _items.AddRange(menuItems);
        }

        /// <summary>
        /// Shows the Traybar (empty).
        /// </summary>
        /// <param name="image">The image.</param>
        public async Task Show(string image)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("create-tray", image);
        }

        /// <summary>
        /// Destroys the tray icon immediately.
        /// </summary>
        public async void Destroy()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-destroy");
            _items.Clear();
        }

        /// <summary>
        /// Sets the image associated with this tray icon.
        /// </summary>
        /// <param name="image"></param>
        public async void SetImage(string image)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-setImage", image);
        }

        /// <summary>
        /// Sets the image associated with this tray icon when pressed on macOS.
        /// </summary>
        /// <param name="image"></param>
        public async void SetPressedImage(string image)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-setPressedImage", image);
        }

        /// <summary>
        /// Sets the hover text for this tray icon.
        /// </summary>
        /// <param name="toolTip"></param>
        public async void SetToolTip(string toolTip)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-setToolTip", toolTip);
        }

        /// <summary>
        /// macOS: Sets the title displayed aside of the tray icon in the status bar.
        /// </summary>
        /// <param name="title"></param>
        public async void SetTitle(string title)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-setTitle", title);
        }

        /// <summary>
        /// Windows: Displays a tray balloon.
        /// </summary>
        /// <param name="options"></param>
        public async void DisplayBalloon(DisplayBalloonOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("tray-displayBalloon", JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Whether the tray icon is destroyed.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDestroyedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("tray-isDestroyed");
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private const string ModuleName = "tray";
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action fn)
            => Events.Instance.On(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void On(string eventName, Action<object> fn)
            => Events.Instance.On(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action fn)
            => Events.Instance.Once(ModuleName, eventName, fn);
        /// <summary>
        /// Subscribe to an unmapped event on the <see cref="Tray"/> module once.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="fn">The handler</param>
        public void Once(string eventName, Action<object> fn)
            => Events.Instance.Once(ModuleName, eventName, fn);
    }
}
