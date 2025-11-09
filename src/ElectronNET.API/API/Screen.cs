using ElectronNET.API.Entities;
using ElectronNET.API.Serialization;
using ElectronNET.Common;
using System;
using System.Text.Json;
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
            add => ApiEventManager.AddEvent("screen-display-added", GetHashCode(), _onDisplayAdded, value, (args) => args.Deserialize(ElectronJsonContext.Default.Display));
            remove => ApiEventManager.RemoveEvent("screen-display-added", GetHashCode(), _onDisplayAdded, value);
        }

        private event Action<Display> _onDisplayAdded;

        /// <summary>
        /// Emitted when oldDisplay has been removed.
        /// </summary>
        public event Action<Display> OnDisplayRemoved
        {
            add => ApiEventManager.AddEvent("screen-display-removed", GetHashCode(), _onDisplayRemoved, value, (args) => args.Deserialize(ElectronJsonContext.Default.Display));
            remove => ApiEventManager.RemoveEvent("screen-display-removed", GetHashCode(), _onDisplayRemoved, value);
        }

        private event Action<Display> _onDisplayRemoved;

        /// <summary>
        /// Emitted when one or more metrics change in a display. 
        /// The changedMetrics is an array of strings that describe the changes. 
        /// Possible changes are bounds, workArea, scaleFactor and rotation.
        /// </summary>
        public event Action<Display, string[]> OnDisplayMetricsChanged
        {
            add => ApiEventManager.AddScreenEvent("screen-display-metrics-changed", GetHashCode(), _onDisplayMetricsChanged, value);
            remove => ApiEventManager.RemoveScreenEvent("screen-display-metrics-changed", GetHashCode(), _onDisplayMetricsChanged, value);
        }

        private event Action<Display, string[]> _onDisplayMetricsChanged;

        private static Screen _screen;
        private static object _syncRoot = new object();

        internal Screen()
        {
        }

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

            BridgeConnector.Socket.On<JsonElement>("screen-getCursorScreenPointCompleted", (point) =>
            {
                BridgeConnector.Socket.Off("screen-getCursorScreenPointCompleted");

                taskCompletionSource.SetResult(point.Deserialize<Point>(ElectronJson.Options));
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

            BridgeConnector.Socket.On<JsonElement>("screen-getMenuBarHeightCompleted", (height) =>
            {
                BridgeConnector.Socket.Off("screen-getMenuBarHeightCompleted");

                taskCompletionSource.SetResult(height.GetInt32());
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

            BridgeConnector.Socket.On<JsonElement>("screen-getPrimaryDisplayCompleted", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getPrimaryDisplayCompleted");

                taskCompletionSource.SetResult(display.Deserialize(ElectronJsonContext.Default.Display));
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

            BridgeConnector.Socket.On<JsonElement>("screen-getAllDisplaysCompleted", (displays) =>
            {
                BridgeConnector.Socket.Off("screen-getAllDisplaysCompleted");

                taskCompletionSource.SetResult(displays.Deserialize<Display[]>(ElectronJson.Options));
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

            BridgeConnector.Socket.On<JsonElement>("screen-getDisplayNearestPointCompleted", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getDisplayNearestPointCompleted");

                taskCompletionSource.SetResult(display.Deserialize(ElectronJsonContext.Default.Display));
            });

            BridgeConnector.Socket.Emit("screen-getDisplayNearestPoint", point);

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

            BridgeConnector.Socket.On<JsonElement>("screen-getDisplayMatching", (display) =>
            {
                BridgeConnector.Socket.Off("screen-getDisplayMatching");

                taskCompletionSource.SetResult(display.Deserialize(ElectronJsonContext.Default.Display));
            });

            BridgeConnector.Socket.Emit("screen-getDisplayMatching", rectangle);

            return taskCompletionSource.Task;
        }


    }
}
