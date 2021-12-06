using System;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Retrieve information about screen size, displays, cursor position, etc.
    /// </summary>
    public interface IScreen
    {
        /// <summary>
        /// Emitted when an new Display has been added.
        /// </summary>
        event Action<Display> OnDisplayAdded;

        /// <summary>
        /// Emitted when oldDisplay has been removed.
        /// </summary>
        event Action<Display> OnDisplayRemoved;

        /// <summary>
        /// Emitted when one or more metrics change in a display. 
        /// The changedMetrics is an array of strings that describe the changes. 
        /// Possible changes are bounds, workArea, scaleFactor and rotation.
        /// </summary>
        event Action<Display, string[]> OnDisplayMetricsChanged;

        /// <summary>
        /// The current absolute position of the mouse pointer.
        /// </summary>
        /// <returns></returns>
        Task<Point> GetCursorScreenPointAsync();

        /// <summary>
        /// macOS: The height of the menu bar in pixels.
        /// </summary>
        /// <returns>The height of the menu bar in pixels.</returns>
        Task<int> GetMenuBarHeightAsync();

        /// <summary>
        /// The primary display.
        /// </summary>
        /// <returns></returns>
        Task<Display> GetPrimaryDisplayAsync();

        /// <summary>
        /// An array of displays that are currently available.
        /// </summary>
        /// <returns>An array of displays that are currently available.</returns>
        Task<Display[]> GetAllDisplaysAsync();

        /// <summary>
        /// The display nearest the specified point.
        /// </summary>
        /// <returns>The display nearest the specified point.</returns>
        Task<Display> GetDisplayNearestPointAsync(Point point);

        /// <summary>
        /// The display that most closely intersects the provided bounds.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns>The display that most closely intersects the provided bounds.</returns>
        Task<Display> GetDisplayMatchingAsync(Rectangle rectangle);
    }
}