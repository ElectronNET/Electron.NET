using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    public class BrowserWindow
    {
        public int Id { get; private set; }

        public event Action ReadyToShow
        {
            add
            {
                if (_readyToShow == null)
                {
                    BridgeConnector.Socket.On("browserWindow-ready-to-show", () =>
                    {
                        _readyToShow();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-ready-to-show", Id);
                }
                _readyToShow += value;
            }
            remove
            {
                _readyToShow -= value;
            }
        }

        private event Action _readyToShow;

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

        /// <summary>
        /// Whether the window can be manually maximized by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public Task<bool> IsMaximizableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMaximizable-completed", (maximizable) => {
                BridgeConnector.Socket.Off("browserWindow-isMaximizable-completed");

                taskCompletionSource.SetResult((bool)maximizable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMaximizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <param name="fullscreenable"></param>
        public void SetFullScreenable(bool fullscreenable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setFullScreenable", Id, fullscreenable);
        }

        /// <summary>
        /// Whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFullScreenableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isFullScreenable-completed", (fullscreenable) => {
                BridgeConnector.Socket.Off("browserWindow-isFullScreenable-completed");

                taskCompletionSource.SetResult((bool)fullscreenable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isFullScreenable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually closed by user. On Linux does nothing.
        /// </summary>
        /// <param name="closable"></param>
        public void SetClosable(bool closable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setClosable", Id, closable);
        }

        /// <summary>
        /// Whether the window can be manually closed by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public Task<bool> IsClosableAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isClosable-completed", (closable) => {
                BridgeConnector.Socket.Off("browserWindow-isClosable-completed");

                taskCompletionSource.SetResult((bool)closable);
            });

            BridgeConnector.Socket.Emit("browserWindow-isClosable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window should show always on top of other windows. 
        /// After setting this, the window is still a normal window, not a toolbox 
        /// window which can not be focused on.
        /// </summary>
        /// <param name="flag"></param>
        public void SetAlwaysOnTop(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAlwaysOnTop", Id, flag);
        }

        /// <summary>
        /// Sets whether the window should show always on top of other windows. 
        /// After setting this, the window is still a normal window, not a toolbox 
        /// window which can not be focused on.
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="level">Values include normal, floating, torn-off-menu, modal-panel, main-menu, 
        /// status, pop-up-menu and screen-saver. The default is floating. 
        /// See the macOS docs</param>
        public void SetAlwaysOnTop(bool flag, string level)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAlwaysOnTop", Id, flag, level);
        }

        /// <summary>
        /// Sets whether the window should show always on top of other windows. 
        /// After setting this, the window is still a normal window, not a toolbox 
        /// window which can not be focused on.
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="level">Values include normal, floating, torn-off-menu, modal-panel, main-menu, 
        /// status, pop-up-menu and screen-saver. The default is floating. 
        /// See the macOS docs</param>
        /// <param name="relativeLevel">The number of layers higher to set this window relative to the given level. 
        /// The default is 0. Note that Apple discourages setting levels higher than 1 above screen-saver.</param>
        public void SetAlwaysOnTop(bool flag, string level, int relativeLevel)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAlwaysOnTop", Id, flag, level, relativeLevel);
        }

        /// <summary>
        /// Whether the window is always on top of other windows.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsAlwaysOnTopAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isAlwaysOnTop-completed", (isAlwaysOnTop) => {
                BridgeConnector.Socket.Off("browserWindow-isAlwaysOnTop-completed");

                taskCompletionSource.SetResult((bool)isAlwaysOnTop);
            });

            BridgeConnector.Socket.Emit("browserWindow-isAlwaysOnTop", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Moves window to the center of the screen.
        /// </summary>
        public void Center()
        {
            BridgeConnector.Socket.Emit("browserWindow-center", Id);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(int x, int y)
        {
            BridgeConnector.Socket.Emit("browserWindow-setPosition", Id, x, y);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate"></param>
        public void SetPosition(int x, int y, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindow-setPosition", Id, x, y, animate);
        }

        /// <summary>
        /// Contains the window’s current position.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetPositionAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<int[]>();

            BridgeConnector.Socket.On("browserWindow-getPosition-completed", (position) => {
                BridgeConnector.Socket.Off("browserWindow-getPosition-completed");

                taskCompletionSource.SetResult(((JArray)position).ToObject<int[]>());
            });

            BridgeConnector.Socket.Emit("browserWindow-getPosition", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Changes the title of native window to title.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            BridgeConnector.Socket.Emit("browserWindow-setTitle", Id, title);
        }

        /// <summary>
        /// The title of the native window.
        /// 
        /// Note: The title of web page can be different from the title of the native window.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetTitleAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("browserWindow-getTitle-completed", (title) => {
                BridgeConnector.Socket.Off("browserWindow-getTitle-completed");

                taskCompletionSource.SetResult(title.ToString());
            });

            BridgeConnector.Socket.Emit("browserWindow-getTitle", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        public void SetSheetOffset(float offsetY)
        {
            BridgeConnector.Socket.Emit("browserWindow-setSheetOffset", Id, offsetY);
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        /// <param name="offsetX"></param>
        public void SetSheetOffset(float offsetY, float offsetX)
        {
            BridgeConnector.Socket.Emit("browserWindow-setSheetOffset", Id, offsetY, offsetX);
        }

        /// <summary>
        /// Starts or stops flashing the window to attract user’s attention.
        /// </summary>
        /// <param name="flag"></param>
        public void FlashFrame(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindow-flashFrame", Id, flag);
        }

        /// <summary>
        /// Makes the window not show in the taskbar.
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipTaskbar(bool skip)
        {
            BridgeConnector.Socket.Emit("browserWindow-setSkipTaskbar", Id, skip);
        }

        /// <summary>
        /// Enters or leaves the kiosk mode.
        /// </summary>
        /// <param name="flag"></param>
        public void SetKiosk(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindow-setKiosk", Id, flag);
        }

        /// <summary>
        /// Whether the window is in kiosk mode.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsKioskAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isKiosk-completed", (isKiosk) => {
                BridgeConnector.Socket.Off("browserWindow-isKiosk-completed");

                taskCompletionSource.SetResult((bool)isKiosk);
            });

            BridgeConnector.Socket.Emit("browserWindow-isKiosk", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the pathname of the file the window represents, 
        /// and the icon of the file will show in window’s title bar.
        /// </summary>
        /// <param name="filename"></param>
        public void SetRepresentedFilename(string filename)
        {
            BridgeConnector.Socket.Emit("browserWindow-setRepresentedFilename", Id, filename);
        }

        /// <summary>
        /// The pathname of the file the window represents.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetRepresentedFilenameAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            BridgeConnector.Socket.On("browserWindow-getRepresentedFilename-completed", (pathname) => {
                BridgeConnector.Socket.Off("browserWindow-getRepresentedFilename-completed");

                taskCompletionSource.SetResult(pathname.ToString());
            });

            BridgeConnector.Socket.Emit("browserWindow-getRepresentedFilename", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Specifies whether the window’s document has been edited, 
        /// and the icon in title bar will become gray when set to true.
        /// </summary>
        /// <param name="edited"></param>
        public void SetDocumentEdited(bool edited)
        {
            BridgeConnector.Socket.Emit("browserWindow-setDocumentEdited", Id, edited);
        }

        /// <summary>
        /// Whether the window’s document has been edited.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsDocumentEditedAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isDocumentEdited-completed", (edited) => {
                BridgeConnector.Socket.Off("browserWindow-isDocumentEdited-completed");

                taskCompletionSource.SetResult((bool)edited);
            });

            BridgeConnector.Socket.Emit("browserWindow-isDocumentEdited", Id);

            return taskCompletionSource.Task;
        }

        public void FocusOnWebView()
        {
            BridgeConnector.Socket.Emit("browserWindow-focusOnWebView", Id);
        }

        public void BlurWebView()
        {
            BridgeConnector.Socket.Emit("browserWindow-blurWebView", Id);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        public void LoadURL(string url)
        {
            BridgeConnector.Socket.Emit("browserWindow-loadURL", Id, url);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public void LoadURL(string url, LoadURLOptions options)
        {
            BridgeConnector.Socket.Emit("browserWindow-loadURL", Id, url, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.reload.
        /// </summary>
        public void Reload()
        {
            BridgeConnector.Socket.Emit("browserWindow-reload", Id);
        }

        public IReadOnlyCollection<MenuItem> Items { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        /// <summary>
        /// Sets the menu as the window’s menu bar, 
        /// setting it to null will remove the menu bar.
        /// </summary>
        /// <param name="menuItems"></param>
        public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("browserWindow-setMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("windowMenuItemClicked");
            BridgeConnector.Socket.On("windowMenuItemClicked", (id) => {
                MenuItem menuItem = _items.GetMenuItem(id.ToString());
                menuItem?.Click();
            });
        }

        /// <summary>
        /// Sets progress value in progress bar. Valid range is [0, 1.0]. Remove progress
        /// bar when progress < 0; Change to indeterminate mode when progress > 1. On Linux
        /// platform, only supports Unity desktop environment, you need to specify the
        /// .desktop file name to desktopName field in package.json.By default, it will
        /// assume app.getName().desktop.On Windows, a mode can be passed.Accepted values
        /// are none, normal, indeterminate, error, and paused. If you call setProgressBar
        /// without a mode set (but with a value within the valid range), normal will be
        /// assumed.
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgressBar(int progress)
        {
            BridgeConnector.Socket.Emit("browserWindow-setProgressBar", Id, progress);
        }

        /// <summary>
        /// Sets progress value in progress bar. Valid range is [0, 1.0]. Remove progress
        /// bar when progress < 0; Change to indeterminate mode when progress > 1. On Linux
        /// platform, only supports Unity desktop environment, you need to specify the
        /// .desktop file name to desktopName field in package.json.By default, it will
        /// assume app.getName().desktop.On Windows, a mode can be passed.Accepted values
        /// are none, normal, indeterminate, error, and paused. If you call setProgressBar
        /// without a mode set (but with a value within the valid range), normal will be
        /// assumed.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="progressBarOptions"></param>
        public void SetProgressBar(int progress, ProgressBarOptions progressBarOptions)
        {
            BridgeConnector.Socket.Emit("browserWindow-setProgressBar", Id, progress, JObject.FromObject(progressBarOptions, _jsonSerializer));
        }

        /// <summary>
        /// Sets whether the window should have a shadow. On Windows and Linux does nothing.
        /// </summary>
        /// <param name="hasShadow"></param>
        public void SetHasShadow(bool hasShadow)
        {
            BridgeConnector.Socket.Emit("browserWindow-setHasShadow", Id, hasShadow);
        }

        /// <summary>
        /// Whether the window has a shadow.
        /// 
        /// On Windows and Linux always returns true.
        /// </summary>
        /// <returns></returns>
        public Task<bool> HasShadowAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-hasShadow-completed", (hasShadow) => {
                BridgeConnector.Socket.Off("browserWindow-hasShadow-completed");

                taskCompletionSource.SetResult((bool)hasShadow);
            });

            BridgeConnector.Socket.Emit("browserWindow-hasShadow", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the properties for the window’s taskbar button.
        /// 
        /// Note: relaunchCommand and relaunchDisplayName must always be set together. 
        /// If one of those properties is not set, then neither will be used.
        /// </summary>
        /// <param name="options"></param>
        public void SetAppDetails(AppDetailsOptions options)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAppDetails", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.showDefinitionForSelection().
        /// </summary>
        public void ShowDefinitionForSelection()
        {
            BridgeConnector.Socket.Emit("browserWindow-showDefinitionForSelection", Id);
        }

        /// <summary>
        /// Sets whether the window menu bar should hide itself automatically. 
        /// Once set the menu bar will only show when users press the single Alt key.
        /// 
        /// If the menu bar is already visible, calling setAutoHideMenuBar(true) won’t hide it immediately.
        /// </summary>
        /// <param name="hide"></param>
        public void SetAutoHideMenuBar(bool hide)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAutoHideMenuBar", Id, hide);
        }

        /// <summary>
        /// Whether menu bar automatically hides itself.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMenuBarAutoHideAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMenuBarAutoHide-completed", (isMenuBarAutoHide) => {
                BridgeConnector.Socket.Off("browserWindow-isMenuBarAutoHide-completed");

                taskCompletionSource.SetResult((bool)isMenuBarAutoHide);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMenuBarAutoHide", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the menu bar should be visible. If the menu bar is auto-hide,
        /// users can still bring up the menu bar by pressing the single Alt key.
        /// </summary>
        /// <param name="visible"></param>
        public void SetMenuBarVisibility(bool visible)
        {
            BridgeConnector.Socket.Emit("browserWindow-setMenuBarVisibility", Id, visible);
        }

        /// <summary>
        /// Whether the menu bar is visible.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMenuBarVisibleAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isMenuBarVisible-completed", (isMenuBarVisible) => {
                BridgeConnector.Socket.Off("browserWindow-isMenuBarVisible-completed");

                taskCompletionSource.SetResult((bool)isMenuBarVisible);
            });

            BridgeConnector.Socket.Emit("browserWindow-isMenuBarVisible", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window should be visible on all workspaces.
        /// 
        /// Note: This API does nothing on Windows.
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisibleOnAllWorkspaces(bool visible)
        {
            BridgeConnector.Socket.Emit("browserWindow-setVisibleOnAllWorkspaces", Id, visible);
        }

        /// <summary>
        /// Whether the window is visible on all workspaces.
        /// 
        /// Note: This API always returns false on Windows.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsVisibleOnAllWorkspacesAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindow-isVisibleOnAllWorkspaces-completed", (isVisibleOnAllWorkspaces) => {
                BridgeConnector.Socket.Off("browserWindow-isVisibleOnAllWorkspaces-completed");

                taskCompletionSource.SetResult((bool)isVisibleOnAllWorkspaces);
            });

            BridgeConnector.Socket.Emit("browserWindow-isVisibleOnAllWorkspaces", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Makes the window ignore all mouse events.
        /// 
        /// All mouse events happened in this window will be passed to the window 
        /// below this window, but if this window has focus, it will still receive keyboard events.
        /// </summary>
        /// <param name="ignore"></param>
        public void SetIgnoreMouseEvents(bool ignore)
        {
            BridgeConnector.Socket.Emit("browserWindow-setIgnoreMouseEvents", Id, ignore);
        }

        /// <summary>
        /// Prevents the window contents from being captured by other apps.
        /// 
        /// On macOS it sets the NSWindow’s sharingType to NSWindowSharingNone. 
        /// On Windows it calls SetWindowDisplayAffinity with WDA_MONITOR.
        /// </summary>
        /// <param name="ignore"></param>
        public void SetContentProtection(bool enable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setContentProtection", Id, enable);
        }

        /// <summary>
        /// Changes whether the window can be focused.
        /// </summary>
        /// <param name="focusable"></param>
        public void SetFocusable(bool focusable)
        {
            BridgeConnector.Socket.Emit("browserWindow-setFocusable", Id, focusable);
        }

        /// <summary>
        /// Sets parent as current window’s parent window, 
        /// passing null will turn current window into a top-level window.
        /// </summary>
        /// <param name="browserWindow"></param>
        public void SetParentWindow(BrowserWindow parent)
        {
            BridgeConnector.Socket.Emit("browserWindow-setParentWindow", Id, JObject.FromObject(parent, _jsonSerializer));
        }

        /// <summary>
        /// The parent window.
        /// </summary>
        /// <returns></returns>
        public Task<BrowserWindow> GetParentWindowAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<BrowserWindow>();

            BridgeConnector.Socket.On("browserWindow-getParentWindow-completed", (id) => {
                BridgeConnector.Socket.Off("browserWindow-getParentWindow-completed");
                var browserWindowId = int.Parse(id.ToString());
                var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == browserWindowId);

                taskCompletionSource.SetResult(browserWindow);
            });

            BridgeConnector.Socket.Emit("browserWindow-getParentWindow", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// All child windows.
        /// </summary>
        /// <returns></returns>
        public Task<List<BrowserWindow>> GetChildWindowsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<List<BrowserWindow>>();

            BridgeConnector.Socket.On("browserWindow-getChildWindows-completed", (ids) => {
                BridgeConnector.Socket.Off("browserWindow-getChildWindows-completed");
                var browserWindowIds = ((JArray)ids).ToObject<int[]>();
                var browserWindows = new List<BrowserWindow>();

                browserWindowIds.ToList().ForEach(id =>
                {
                    var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == id);
                    browserWindows.Add(browserWindow);
                });


                taskCompletionSource.SetResult(browserWindows);
            });

            BridgeConnector.Socket.Emit("browserWindow-getChildWindows", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Controls whether to hide cursor when typing.
        /// </summary>
        /// <param name="autoHide"></param>
        public void SetAutoHideCursor(bool autoHide)
        {
            BridgeConnector.Socket.Emit("browserWindow-setAutoHideCursor", Id, autoHide);
        }

        /// <summary>
        /// Adds a vibrancy effect to the browser window. 
        /// Passing null or an empty string will remove the vibrancy effect on the window.
        /// </summary>
        /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
        /// menu, popover, sidebar, medium-light or ultra-dark. 
        /// See the macOS documentation for more details.</param>
        public void SetVibrancy(string type)
        {
            BridgeConnector.Socket.Emit("browserWindow-setVibrancy", Id, type);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
