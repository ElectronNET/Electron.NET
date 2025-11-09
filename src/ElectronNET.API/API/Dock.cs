using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Control your app in the macOS dock.
    /// </summary>
    public sealed class Dock
    {
        private static Dock _dock;
        private static object _syncRoot = new object();

        internal Dock()
        {
        }

        internal static Dock Instance
        {
            get
            {
                if (_dock == null)
                {
                    lock (_syncRoot)
                    {
                        if (_dock == null)
                        {
                            _dock = new Dock();
                        }
                    }
                }

                return _dock;
            }
        }

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
        public async Task<int> BounceAsync(DockBounceType type, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<int>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On<JsonElement>("dock-bounce-completed", (id) =>
                {
                    BridgeConnector.Socket.Off("dock-bounce-completed");
                    taskCompletionSource.SetResult(id.GetInt32());
                });

                BridgeConnector.Socket.Emit("dock-bounce", type.GetDescription());

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Cancel the bounce of id.
        /// </summary>
        /// <param name="id">Id of the request.</param>
        public void CancelBounce(int id)
        {
            BridgeConnector.Socket.Emit("dock-cancelBounce", id);
        }

        /// <summary>
        /// Bounces the Downloads stack if the filePath is inside the Downloads folder.
        /// </summary>
        /// <param name="filePath"></param>
        public void DownloadFinished(string filePath)
        {
            BridgeConnector.Socket.Emit("dock-downloadFinished", filePath);
        }

        /// <summary>
        /// Sets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="text"></param>
        public void SetBadge(string text)
        {
            BridgeConnector.Socket.Emit("dock-setBadge", text);
        }

        /// <summary>
        /// Gets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The badge string of the dock.</returns>
        public async Task<string> GetBadgeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<string>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On<JsonElement>("dock-getBadge-completed", (text) =>
                {
                    BridgeConnector.Socket.Off("dock-getBadge-completed");
                    taskCompletionSource.SetResult(text.GetString());
                });

                BridgeConnector.Socket.Emit("dock-getBadge");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Hides the dock icon.
        /// </summary>
        public void Hide()
        {
            BridgeConnector.Socket.Emit("dock-hide");
        }

        /// <summary>
        /// Shows the dock icon.
        /// </summary>
        public void Show()
        {
            BridgeConnector.Socket.Emit("dock-show");
        }

        /// <summary>
        /// Whether the dock icon is visible. The app.dock.show() call is asynchronous
        /// so this method might not return true immediately after that call.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the dock icon is visible.</returns>
        public async Task<bool> IsVisibleAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On<JsonElement>("dock-isVisible-completed", (isVisible) =>
                {
                    BridgeConnector.Socket.Off("dock-isVisible-completed");
                    taskCompletionSource.SetResult(isVisible.GetBoolean());
                });

                BridgeConnector.Socket.Emit("dock-isVisible");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the dock menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems
        {
            get
            {
                return _items.AsReadOnly();
            }
        }

        private List<MenuItem> _items = new List<MenuItem>();

        /// <summary>
        /// Sets the application's dock menu.
        /// </summary>
        public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("dock-setMenu", menuItems);
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("dockMenuItemClicked");
            BridgeConnector.Socket.On<JsonElement>("dockMenuItemClicked", (id) =>
            {
                MenuItem menuItem = _items.GetMenuItem(id.GetString());
                menuItem?.Click();
            });
        }

        /// <summary>
        /// TODO: Menu (macOS) still to be implemented
        /// Gets the application's dock menu.
        /// </summary>
        public async Task<Menu> GetMenu(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskCompletionSource = new TaskCompletionSource<Menu>();
            using (cancellationToken.Register(() => taskCompletionSource.TrySetCanceled()))
            {
                BridgeConnector.Socket.On<JsonElement>("dock-getMenu-completed", (menu) =>
                {
                    BridgeConnector.Socket.Off("dock-getMenu-completed");
                    taskCompletionSource.SetResult(JsonSerializer.Deserialize<Menu>(menu, Serialization.ElectronJson.Options));
                });

                BridgeConnector.Socket.Emit("dock-getMenu");

                return await taskCompletionSource.Task
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets the image associated with this dock icon.
        /// </summary>
        /// <param name="image"></param>
        public void SetIcon(string image)
        {
            BridgeConnector.Socket.Emit("dock-setIcon", image);
        }


    }
}
