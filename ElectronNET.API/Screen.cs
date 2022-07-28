using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using ElectronNET.API.Interfaces;

namespace ElectronNET.API
{
    /// <summary>
    /// Retrieve information about screen size, displays, cursor position, etc.
    /// </summary>
    public sealed class Screen : IScreen
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
        private static readonly object _syncRoot = new();

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
        public Task<Point> GetCursorScreenPointAsync() => BridgeConnector.OnResult<Point>("screen-getCursorScreenPoint", "screen-getCursorScreenPointCompleted");

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        public Task<int> GetMenuBarHeightAsync() => BridgeConnector.OnResult<int>("screen-getMenuBarHeight", "screen-getMenuBarHeightCompleted");

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        public Task<Display> GetPrimaryDisplayAsync() => BridgeConnector.OnResult<Display>("screen-getPrimaryDisplay", "screen-getPrimaryDisplayCompleted");


        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        public Task<Display[]> GetAllDisplaysAsync() => BridgeConnector.OnResult<Display[]>("screen-getAllDisplays", "screen-getAllDisplaysCompleted");

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        public Task<Display> GetDisplayNearestPointAsync(Point point) => BridgeConnector.OnResult<Display>("screen-getDisplayNearestPoint", "screen-getDisplayNearestPointCompleted", point);

        /// <summary>
        /// The display that most closely intersects the provided bounds.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>The display that most closely intersects the provided bounds.</returns>
        public Task<Display> GetDisplayMatchingAsync(Rectangle rectangle) => BridgeConnector.OnResult<Display>("screen-getDisplayMatching", "screen-getDisplayMatchingCompleted", rectangle);
    }
}