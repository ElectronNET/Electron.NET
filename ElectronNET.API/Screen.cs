using ElectronNET.API.Entities;
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
                    BridgeConnector.Socket.On("screen-display-added-event" + GetHashCode(), (display) =>
                    {
                        _onDisplayAdded(((JObject)display).ToObject<Display>());
                    });

                    BridgeConnector.Socket.Emit("register-screen-display-added", GetHashCode());
                }
                _onDisplayAdded += value;
            }
            remove
            {
                _onDisplayAdded -= value;

                if (_onDisplayAdded == null)
                    BridgeConnector.Socket.Off("screen-display-added-event" + GetHashCode());
            }
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
                    BridgeConnector.Socket.On("screen-display-removed-event" + GetHashCode(), (display) =>
                    {
                        _onDisplayRemoved(((JObject)display).ToObject<Display>());
                    });

                    BridgeConnector.Socket.Emit("register-screen-display-removed", GetHashCode());
                }
                _onDisplayRemoved += value;
            }
            remove
            {
                _onDisplayRemoved -= value;

                if (_onDisplayRemoved == null)
                    BridgeConnector.Socket.Off("screen-display-removed-event" + GetHashCode());
            }
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
                    BridgeConnector.Socket.On("screen-display-metrics-changed-event" + GetHashCode(), (args) =>
                    {
                        var display = ((JArray)args).First.ToObject<Display>();
                        var metrics = ((JArray)args).Last.ToObject<string[]>();

                        _onDisplayMetricsChanged(display, metrics);
                    });

                    BridgeConnector.Socket.Emit("register-screen-display-metrics-changed", GetHashCode());
                }
                _onDisplayMetricsChanged += value;
            }
            remove
            {
                _onDisplayMetricsChanged -= value;

                if (_onDisplayMetricsChanged == null)
                    BridgeConnector.Socket.Off("screen-display-metrics-changed-event" + GetHashCode());
            }
        }

        private event Action<Display, string[]> _onDisplayMetricsChanged;

        private static Screen _screen;
        private static object _syncRoot = new Object();

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
        public Task<Point> GetCursorScreenPointAsync() 
        {
            var taskCompletionSource = new TaskCompletionSource<Point>();

            BridgeConnector.Socket.On("screen-getCursorScreenPointCompleted", (point) =>
            {
                BridgeConnector.Socket.Off("screen-getCursorScreenPointCompleted");

                taskCompletionSource.SetResult(((JObject)point).ToObject<Point>());
            });

            BridgeConnector.Socket.Emit("screen-getCursorScreenPoint");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        public Task<int> GetMenuBarHeightAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();

            BridgeConnector.Socket.On("screen-getMenuBarHeightCompleted", (height) =>
            {
                BridgeConnector.Socket.Off("screen-getMenuBarHeightCompleted");

                taskCompletionSource.SetResult(int.Parse(height.ToString()));
            });

            BridgeConnector.Socket.Emit("screen-getMenuBarHeight");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        public Task<Display> GetPrimaryDisplayAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Display>();

            BridgeConnector.Socket.On("screen-getPrimaryDisplayCompleted", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getPrimaryDisplayCompleted");

                taskCompletionSource.SetResult(((JObject)display).ToObject<Display>());
            });

            BridgeConnector.Socket.Emit("screen-getPrimaryDisplay");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        public Task<Display[]> GetAllDisplaysAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Display[]>();

            BridgeConnector.Socket.On("screen-getAllDisplaysCompleted", (displays) =>
            {
                BridgeConnector.Socket.Off("screen-getAllDisplaysCompleted");

                taskCompletionSource.SetResult(((JArray)displays).ToObject<Display[]>());
            });

            BridgeConnector.Socket.Emit("screen-getAllDisplays");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        public Task<Display> GetDisplayNearestPointAsync(Point point)
        {
            var taskCompletionSource = new TaskCompletionSource<Display>();

            BridgeConnector.Socket.On("screen-getDisplayNearestPointCompleted", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getDisplayNearestPointCompleted");

                taskCompletionSource.SetResult(((JObject)display).ToObject<Display>());
            });

            BridgeConnector.Socket.Emit("screen-getDisplayNearestPoint", JObject.FromObject(point, _jsonSerializer));

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The display that most closely intersects the provided bounds.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>The display that most closely intersects the provided bounds.</returns>
        public Task<Display> GetDisplayMatchingAsync(Rectangle rectangle)
        {
            var taskCompletionSource = new TaskCompletionSource<Display>();

            BridgeConnector.Socket.On("screen-getDisplayMatching", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getDisplayMatching");

                taskCompletionSource.SetResult(((JObject)display).ToObject<Display>());
            });

            BridgeConnector.Socket.Emit("screen-getDisplayMatching", JObject.FromObject(rectangle, _jsonSerializer));

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