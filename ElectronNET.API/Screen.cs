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
                    BridgeConnector.On<Display>("screen-display-added-event" + GetHashCode(), (display) =>
                    {
                        _onDisplayAdded(display);
                    });

                    BridgeConnector.Emit("register-screen-display-added", GetHashCode());
                }
                _onDisplayAdded += value;
            }
            remove
            {
                _onDisplayAdded -= value;

                if (_onDisplayAdded == null)
                    BridgeConnector.Off("screen-display-added-event" + GetHashCode());
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
                    BridgeConnector.On<Display>("screen-display-removed-event" + GetHashCode(), (display) =>
                    {
                        _onDisplayRemoved(display);
                    });

                    BridgeConnector.Emit("register-screen-display-removed", GetHashCode());
                }
                _onDisplayRemoved += value;
            }
            remove
            {
                _onDisplayRemoved -= value;

                if (_onDisplayRemoved == null)
                    BridgeConnector.Off("screen-display-removed-event" + GetHashCode());
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
                    BridgeConnector.On<DisplayChanged>("screen-display-metrics-changed-event" + GetHashCode(), (args) =>
                    {
                        _onDisplayMetricsChanged(args.display, args.metrics);
                    });

                    BridgeConnector.Emit("register-screen-display-metrics-changed", GetHashCode());
                }
                _onDisplayMetricsChanged += value;
            }
            remove
            {
                _onDisplayMetricsChanged -= value;

                if (_onDisplayMetricsChanged == null)
                    BridgeConnector.Off("screen-display-metrics-changed-event" + GetHashCode());
            }
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
        public Task<Point> GetCursorScreenPointAsync() 
        {
            var taskCompletionSource = new TaskCompletionSource<Point>();

            BridgeConnector.On<Point>("screen-getCursorScreenPointCompleted", (point) =>
            {
                BridgeConnector.Off("screen-getCursorScreenPointCompleted");

                taskCompletionSource.SetResult(point);
            });

            BridgeConnector.Emit("screen-getCursorScreenPoint");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        public Task<int> GetMenuBarHeightAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();

            BridgeConnector.On<int>("screen-getMenuBarHeightCompleted", (height) =>
            {
                BridgeConnector.Off("screen-getMenuBarHeightCompleted");

                taskCompletionSource.SetResult(height);
            });

            BridgeConnector.Emit("screen-getMenuBarHeight");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        public Task<Display> GetPrimaryDisplayAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Display>();

            BridgeConnector.On<Display>("screen-getPrimaryDisplayCompleted", (display) =>
            {
                BridgeConnector.Off("screen-getPrimaryDisplayCompleted");

                taskCompletionSource.SetResult(display);
            });

            BridgeConnector.Emit("screen-getPrimaryDisplay");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        public Task<Display[]> GetAllDisplaysAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Display[]>();

            BridgeConnector.On<Display[]>("screen-getAllDisplaysCompleted", (displays) =>
            {
                BridgeConnector.Off("screen-getAllDisplaysCompleted");

                taskCompletionSource.SetResult(displays);
            });

            BridgeConnector.Emit("screen-getAllDisplays");

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        public Task<Display> GetDisplayNearestPointAsync(Point point)
        {
            var taskCompletionSource = new TaskCompletionSource<Display>();

            BridgeConnector.On<Display>("screen-getDisplayNearestPointCompleted", (display) =>
            {
                BridgeConnector.Off("screen-getDisplayNearestPointCompleted");

                taskCompletionSource.SetResult(display);
            });

            BridgeConnector.Emit("screen-getDisplayNearestPoint", JObject.FromObject(point, _jsonSerializer));

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

            BridgeConnector.On<Display>("screen-getDisplayMatching", (display) =>
            {
                BridgeConnector.Off("screen-getDisplayMatching");

                taskCompletionSource.SetResult(display);
            });

            BridgeConnector.Emit("screen-getDisplayMatching", JObject.FromObject(rectangle, _jsonSerializer));

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