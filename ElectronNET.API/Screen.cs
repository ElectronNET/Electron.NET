using ElectronNET.API.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Retrieve information about screen size, displays, cursor position, etc.
    /// </summary>
    public sealed class Screen
    {
        /// <summary>
        /// Emitted when an new Display has been added.
        /// </summary>
        public event Action<Display> OnDisplayAdded
        {
            add
            {
                if (_onDisplayAdded == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-screen-display-added", GetHashCode());
                }
                _onDisplayAdded += value;
            }
            remove
            {
                _onDisplayAdded -= value;
            }
        }

        public void TriggerOnDisplayAdded(Display display)
        {
            _onDisplayAdded(display);
        }

        private event Action<Display> _onDisplayAdded;

        /// <summary>
        /// Emitted when oldDisplay has been removed.
        /// </summary>
        public event Action<Display> OnDisplayRemoved
        {
            add
            {
                if (_onDisplayRemoved == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-screen-display-removed", GetHashCode());
                }
                _onDisplayRemoved += value;
            }
            remove
            {
                _onDisplayRemoved -= value;
            }
        }

        public void TriggerOnDisplayRemoved(Display display)
        {
            _onDisplayAdded(display);
        }

        private event Action<Display> _onDisplayRemoved;

        /// <summary>
        /// Emitted when one or more metrics change in a display. 
        /// The changedMetrics is an array of strings that describe the changes. 
        /// Possible changes are bounds, workArea, scaleFactor and rotation.
        /// </summary>
        public event Action<Display, string[]> OnDisplayMetricsChanged
        {
            add
            {
                if (_onDisplayMetricsChanged == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-screen-display-metrics-changed", GetHashCode());

                }
                _onDisplayMetricsChanged += value;
            }
            remove
            {
                _onDisplayMetricsChanged -= value;
            }
        }

        public void TriggerOnDisplayMetricsChanged(Display display, string[] metrics)
        {
            _onDisplayMetricsChanged(display, metrics);
        }

        private event Action<Display, string[]> _onDisplayMetricsChanged;

        private static Screen _screen;
        private static object _syncRoot = new object();

        internal Screen() { }

        internal static Screen Instance
        {
            get
            {
                if (_screen == null)
                {
                    lock (_syncRoot)
                    {
                        if (_screen == null)
                        {
                            _screen = new Screen();
                        }
                    }
                }

                return _screen;
            }
        }

        /// <summary>
        /// The current absolute position of the mouse pointer.
        /// </summary>
        /// <returns></returns>
        public async Task<Point> GetCursorScreenPointAsync() 
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("screen-getCursorScreenPoint");
            return signalrResult.ToObject<Point>();
        }

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        public async Task<int> GetMenuBarHeightAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultInt("screen-getMenuBarHeight");
        }

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        public async Task<Display> GetPrimaryDisplayAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("screen-getPrimaryDisplay");
            return signalrResult.ToObject<Display>();
        }

        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        public async Task<Display[]> GetAllDisplaysAsync()
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJArray("screen-getAllDisplays");
            return signalrResult.ToObject<Display[]>();
        }

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        public async Task<Display> GetDisplayNearestPointAsync(Point point)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("screen-getDisplayNearestPoint", JObject.FromObject(point, _jsonSerializer));
            return signalrResult.ToObject<Display>();
        }

        /// <summary>
        /// The display that most closely intersects the provided bounds.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>The display that most closely intersects the provided bounds.</returns>
        public async Task<Display> GetDisplayMatchingAsync(Rectangle rectangle)
        {
            var signalrResult = await SignalrSerializeHelper.GetSignalrResultJObject("screen-getDisplayMatching", JObject.FromObject(rectangle, _jsonSerializer));
            return signalrResult.ToObject<Display>();
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}