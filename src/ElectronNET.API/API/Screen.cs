using ElectronNET.API.Entities;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ElectronNET.API.Serialization;

namespace ElectronNET.API
{
    /// <summary>
    /// Retrieve information about screen size, displays, cursor position, etc.
    /// </summary>
    public sealed class Screen: ApiBase
    {
        protected override SocketTaskEventNameTypes SocketTaskEventNameType => SocketTaskEventNameTypes.DashesLowerFirst;
        protected override SocketTaskMessageNameTypes SocketTaskMessageNameType => SocketTaskMessageNameTypes.DashesLowerFirst;
        protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashedLower;

        /// <summary>
        /// Emitted when an new Display has been added.
        /// </summary>
        public event Action<Display> OnDisplayAdded
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

        /// <summary>
        /// Emitted when oldDisplay has been removed.
        /// </summary>
        public event Action<Display> OnDisplayRemoved
        {
            add => AddEvent(value, GetHashCode());
            remove => RemoveEvent(value, GetHashCode());
        }

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
                    BridgeConnector.Socket.On<JsonElement>("screen-display-metrics-changed" + GetHashCode(), (args) =>
                    {
                        var arr = args.EnumerateArray().ToArray();
                        var display = arr[0].Deserialize(ElectronJsonContext.Default.Display);
                        var metrics = arr[1].Deserialize<string[]>(ElectronJson.Options);

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
                    BridgeConnector.Socket.Off("screen-display-metrics-changed" + GetHashCode());
            }
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
        public Task<Point> GetCursorScreenPointAsync() => GetPropertyAsync<Point>();

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        public Task<Rectangle> GetMenuBarWorkAreaAsync() => GetPropertyAsync<Rectangle>();

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        public Task<Display> GetPrimaryDisplayAsync() => GetPropertyAsync<Display>();

        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        public Task<Display[]> GetAllDisplaysAsync() => GetPropertyAsync<Display[]>();

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        public Task<Display> GetDisplayNearestPointAsync(Point point) => GetPropertyAsync<Display>(point);

        /// <summary>
        /// The display that most closely intersects the provided bounds.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>The display that most closely intersects the provided bounds.</returns>
        public Task<Display> GetDisplayMatchingAsync(Rectangle rectangle) => GetPropertyAsync<Display>(rectangle);
    }
}
