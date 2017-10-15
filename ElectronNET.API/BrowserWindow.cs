using ElectronNET.API.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public class BrowserWindow
    {
        public int Id { get; private set; }

        internal BrowserWindow(int id) {
            Id = id;
        }

        /// <summary>
        /// Force closing the window, the unload and beforeunload event won’t be 
        /// emitted for the web page, and close event will also not be emitted 
        /// for this window, but it guarantees the closed event will be emitted.
        /// </summary>
        public void Destroy()
        {
            BridgeConnector.Socket.Emit("browserWindow-destroy", Id);
        }

        /// <summary>
        /// Try to close the window. This has the same effect as a user manually 
        /// clicking the close button of the window. The web page may cancel the close though. 
        /// </summary>
        public void Close()
        {
            BridgeConnector.Socket.Emit("browserWindow-close", Id);
        }

        /// <summary>
        /// Focuses on the window.
        /// </summary>
        public void Focus()
        {
            BridgeConnector.Socket.Emit("browserWindow-focus", Id);
        }

        /// <summary>
        /// Removes focus from the window.
        /// </summary>
        public void Blur()
        {
            BridgeConnector.Socket.Emit("browserWindow-blur", Id);
        }

        /// <summary>
        /// Whether the window is focused.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFocusedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isFocused-completed", (isFocused) => {
                BridgeConnector.Socket.Off("browserWindow-isFocused-completed");

                taskCompletionSource.SetResult((bool)isFocused);
            });

            BridgeConnector.Socket.Emit("browserWindow-isFocused", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Whether the window is destroyed.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsDestroyedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isDestroyed-completed", (isDestroyed) => {
                BridgeConnector.Socket.Off("browserWindow-isDestroyed-completed");

                taskCompletionSource.SetResult((bool)isDestroyed);
            });

            BridgeConnector.Socket.Emit("browserWindow-isDestroyed", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Shows and gives focus to the window.
        /// </summary>
        public void Show()
        {
            BridgeConnector.Socket.Emit("browserWindow-show", Id);
        }

        /// <summary>
        /// Shows the window but doesn’t focus on it.
        /// </summary>
        public void ShowInactive()
        {
            BridgeConnector.Socket.Emit("browserWindow-showInactive", Id);
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public void Hide()
        {
            BridgeConnector.Socket.Emit("browserWindow-hide", Id);
        }

        /// <summary>
        /// Whether the window is visible to the user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsVisibleAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isVisible-completed", (isVisible) => {
                BridgeConnector.Socket.Off("browserWindow-isVisible-completed");

                taskCompletionSource.SetResult((bool)isVisible);
            });

            BridgeConnector.Socket.Emit("browserWindow-isVisible", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Whether current window is a modal window.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsModalAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isModal-completed", (isModal) => {
                BridgeConnector.Socket.Off("browserWindow-isModal-completed");

                taskCompletionSource.SetResult((bool)isModal);
            });

            BridgeConnector.Socket.Emit("browserWindow-isModal", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Maximizes the window. This will also show (but not focus) the window if it isn’t being displayed already.
        /// </summary>
        public void Maximize()
        {
            BridgeConnector.Socket.Emit("browserWindow-maximize", Id);
        }

        /// <summary>
        /// Unmaximizes the window.
        /// </summary>
        public void Unmaximize()
        {
            BridgeConnector.Socket.Emit("browserWindow-unmaximize", Id);
        }

        /// <summary>
        /// Whether the window is maximized.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMaximizedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMaximized-completed", (isMaximized) => {
                BridgeConnector.Socket.Off("browserWindow-isMaximized-completed");

                taskCompletionSource.SetResult((bool)isMaximized);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMaximized", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Minimizes the window. On some platforms the minimized window will be shown in the Dock.
        /// </summary>
        public void Minimize()
        {
            BridgeConnector.Socket.Emit("browserWindow-minimize", Id);
        }

        /// <summary>
        /// Restores the window from minimized state to its previous state.
        /// </summary>
        public void Restore()
        {
            BridgeConnector.Socket.Emit("browserWindow-restore", Id);
        }

        /// <summary>
        /// Whether the window is minimized.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMinimizedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMinimized-completed", (isMinimized) => {
                BridgeConnector.Socket.Off("browserWindow-isMinimized-completed");

                taskCompletionSource.SetResult((bool)isMinimized);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMinimized", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window should be in fullscreen mode.
        /// </summary>
        public void SetFullScreen(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindow-setFullScreen", Id, flag);
        }

        /// <summary>
        /// Whether the window is in fullscreen mode.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFullScreenAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isFullScreen-completed", (isFullScreen) => {
                BridgeConnector.Socket.Off("browserWindow-isFullScreen-completed");

                taskCompletionSource.SetResult((bool)isFullScreen);
            });

            BridgeConnector.Socket.Emit("browserWindow-isFullScreen", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// This will make a window maintain an aspect ratio. The extra size allows a developer to have space, 
        /// specified in pixels, not included within the aspect ratio calculations. This API already takes into
        /// account the difference between a window’s size and its content size.
        ///
        /// Consider a normal window with an HD video player and associated controls.Perhaps there are 15 pixels
        /// of controls on the left edge, 25 pixels of controls on the right edge and 50 pixels of controls below
        /// the player. In order to maintain a 16:9 aspect ratio (standard aspect ratio for HD @1920x1080) within
        /// the player itself we would call this function with arguments of 16/9 and[40, 50]. The second argument
        /// doesn’t care where the extra width and height are within the content view–only that they exist. Just 
        /// sum any extra width and height areas you have within the overall content view.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio to maintain for some portion of the content view.</param>
        /// <param name="extraSize">The extra size not to be included while maintaining the aspect ratio.</param>
        public void SetAspectRatio(int aspectRatio, Size extraSize)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAspectRatio", Id, aspectRatio, JObject.FromObject(extraSize, _jsonSerializer));
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        public void PreviewFile(string path)
        {
            BridgeConnector.Socket.Emit("browserWindow-previewFile", Id, path);
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        /// <param name="displayname">The name of the file to display on the Quick Look modal view. This is 
        /// purely visual and does not affect the content type of the file. Defaults to path.</param>
        public void PreviewFile(string path, string displayname)
        {
            BridgeConnector.Socket.Emit("browserWindow-previewFile", Id, path, displayname);
        }

        /// <summary>
        /// Closes the currently open Quick Look panel.
        /// </summary>
        public void CloseFilePreview()
        {
            BridgeConnector.Socket.Emit("browserWindow-closeFilePreview", Id);
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        public void SetBounds(Rectangle bounds)
        {
            BridgeConnector.Socket.Emit("browserWindow-setBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public void SetBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindow-setBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        public Task<Rectangle> GetBoundsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Rectangle>();

            BridgeConnector.Socket.On("browserWindow-getBounds-completed", (getBounds) => {
                BridgeConnector.Socket.Off("browserWindow-getBounds-completed");

                taskCompletionSource.SetResult(((JObject)getBounds).ToObject<Rectangle>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getBounds", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public void SetContentBounds(Rectangle bounds)
        {
            BridgeConnector.Socket.Emit("browserWindow-setContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public void SetContentBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindow-setContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        public Task<Rectangle> GetContentBoundsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Rectangle>();

            BridgeConnector.Socket.On("browserWindow-getContentBounds-completed", (getContentBounds) => {
                BridgeConnector.Socket.Off("browserWindow-getContentBounds-completed");

                taskCompletionSource.SetResult(((JObject)getContentBounds).ToObject<Rectangle>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getContentBounds", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindow-setSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetSize(int width, int height, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindow-setSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetSizeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int[]>();

            BridgeConnector.Socket.On("browserWindow-getSize-completed", (size) => {
                BridgeConnector.Socket.Off("browserWindow-getSize-completed");

                taskCompletionSource.SetResult(((JArray)size).ToObject<int[]>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetContentSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindow-setContentSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetContentSize(int width, int height, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindow-setContentSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s client area’s width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetContentSizeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int[]>();

            BridgeConnector.Socket.On("browserWindow-getContentSize-completed", (size) => {
                BridgeConnector.Socket.Off("browserWindow-getContentSize-completed");

                taskCompletionSource.SetResult(((JArray)size).ToObject<int[]>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getContentSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the minimum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMinimumSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMinimumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s minimum width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetMinimumSizeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int[]>();

            BridgeConnector.Socket.On("browserWindow-getMinimumSize-completed", (size) => {
                BridgeConnector.Socket.Off("browserWindow-getMinimumSize-completed");

                taskCompletionSource.SetResult(((JArray)size).ToObject<int[]>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getMinimumSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the maximum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMaximumSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMaximumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s maximum width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetMaximumSizeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int[]>();

            BridgeConnector.Socket.On("browserWindow-getMaximumSize-completed", (size) => {
                BridgeConnector.Socket.Off("browserWindow-getMaximumSize-completed");

                taskCompletionSource.SetResult(((JArray)size).ToObject<int[]>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getMaximumSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually resized by user.
        /// </summary>
        /// <param name="resizable"></param>
        public void SetResizable(bool resizable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setResizable", Id, resizable);
        }

        /// <summary>
        /// Whether the window can be manually resized by user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsResizableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isResizable-completed", (resizable) => {
                BridgeConnector.Socket.Off("browserWindow-isResizable-completed");

                taskCompletionSource.SetResult((bool)resizable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isResizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be moved by user. On Linux does nothing.
        /// </summary>
        /// <param name="movable"></param>
        public void SetMovable(bool movable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMovable", Id, movable);
        }

        /// <summary>
        /// Whether the window can be moved by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public Task<bool> IsMovableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMovable-completed", (movable) => {
                BridgeConnector.Socket.Off("browserWindow-isMovable-completed");

                taskCompletionSource.SetResult((bool)movable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMovable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually minimized by user. On Linux does nothing.
        /// </summary>
        /// <param name="minimizable"></param>
        public void SetMinimizable(bool minimizable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMinimizable", Id, minimizable);
        }

        /// <summary>
        /// Whether the window can be manually minimized by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public Task<bool> IsMinimizableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMinimizable-completed", (minimizable) => {
                BridgeConnector.Socket.Off("browserWindow-isMinimizable-completed");

                taskCompletionSource.SetResult((bool)minimizable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMinimizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually maximized by user. On Linux does nothing.
        /// </summary>
        /// <param name="maximizable"></param>
        public void SetMaximizable(bool maximizable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMaximizable", Id, maximizable);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
