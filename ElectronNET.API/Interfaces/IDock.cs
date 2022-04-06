using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API.Entities;

namespace ElectronNET.API.Interfaces
{
    /// <summary>
    /// Control your app in the macOS dock.
    /// </summary>
    public interface IDock
    {
        /// <summary>
        /// When <see cref="DockBounceType.Critical"/> is passed, the dock icon will bounce until either the application becomes
        /// active or the request is canceled. When <see cref="DockBounceType.Informational"/> is passed, the dock icon will bounce
        /// for one second. However, the request remains active until either the application becomes active or the request is canceled.
        /// <para/>
        /// Note: This method can only be used while the app is not focused; when the app is focused it will return -1.
        /// </summary>
        /// <param name="type">Can be critical or informational. The default is informational.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Return an ID representing the request.</returns>
        Task<int> BounceAsync(DockBounceType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel the bounce of id.
        /// </summary>
        /// <param name="id">Id of the request.</param>
        void CancelBounce(int id);

        /// <summary>
        /// Bounces the Downloads stack if the filePath is inside the Downloads folder.
        /// </summary>
        /// <param name="filePath"></param>
        void DownloadFinished(string filePath);

        /// <summary>
        /// Sets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="text"></param>
        void SetBadge(string text);

        /// <summary>
        /// Gets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The badge string of the dock.</returns>
        Task<string> GetBadgeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Hides the dock icon.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows the dock icon.
        /// </summary>
        void Show();

        /// <summary>
        /// Whether the dock icon is visible. The app.dock.show() call is asynchronous
        /// so this method might not return true immediately after that call.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the dock icon is visible.</returns>
        Task<bool> IsVisibleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the dock menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        IReadOnlyCollection<MenuItem> MenuItems { get; }

        /// <summary>
        /// Sets the application's dock menu.
        /// </summary>
        void SetMenu(MenuItem[] menuItems);

        /// <summary>
        /// TODO: Menu (macOS) still to be implemented
        /// Gets the application's dock menu.
        /// </summary>
        Task<Menu> GetMenu(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the image associated with this dock icon.
        /// </summary>
        /// <param name="image"></param>
        void SetIcon(string image);
    }
}