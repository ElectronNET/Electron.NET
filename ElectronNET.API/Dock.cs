using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
            return await SignalrSerializeHelper.GetSignalrResultInt("dock-bounce", type.GetDescription());
        }

        /// <summary>
        /// Cancel the bounce of id.
        /// </summary>
        /// <param name="id">Id of the request.</param>
        public async void CancelBounce(int id)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-cancelBounce", id);
        }

        /// <summary>
        /// Bounces the Downloads stack if the filePath is inside the Downloads folder.
        /// </summary>
        /// <param name="filePath"></param>
        public async void DownloadFinished(string filePath)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-downloadFinished", filePath);
        }

        /// <summary>
        /// Sets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="text"></param>
        public async void SetBadge(string text)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-setBadge", text);
        }

        /// <summary>
        /// Gets the string to be displayed in the dock’s badging area.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The badge string of the dock.</returns>
        public async Task<string> GetBadgeAsync(CancellationToken cancellationToken = default)
        {
            return await SignalrSerializeHelper.GetSignalrResultString("dock-getBadge");
        }

        /// <summary>
        /// Hides the dock icon.
        /// </summary>
        public async void Hide()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-hide");
        }

        /// <summary>
        /// Shows the dock icon.
        /// </summary>
        public async void Show()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-show");
        }

        /// <summary>
        /// Whether the dock icon is visible. The app.dock.show() call is asynchronous
        /// so this method might not return true immediately after that call.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Whether the dock icon is visible.</returns>
        public async Task<bool> IsVisibleAsync(CancellationToken cancellationToken = default)
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("dock-isVisible");
        }

        /// <summary>
        /// Gets the dock menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        /// <summary>
        /// Sets the application's dock menu.
        /// </summary>
        public async void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-setMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);          
        }

        /// <summary>
        /// TODO: Menu (macOS) still to be implemented
        /// Gets the application's dock menu.
        /// </summary>
        public async Task<Menu> GetMenu(CancellationToken cancellationToken = default)
        {
            var result = await SignalrSerializeHelper.GetSignalrResultJObject("dock-getMenu");
            return result.ToObject<Menu>();
        }

        /// <summary>
        /// Sets the image associated with this dock icon.
        /// </summary>
        /// <param name="image"></param>
        public async void SetIcon(string image)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("dock-setIcon", image);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}