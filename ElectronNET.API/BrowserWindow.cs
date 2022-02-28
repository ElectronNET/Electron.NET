using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using ElectronNET.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ElectronNET.API
{
    /// <summary>
    /// Create and control browser windows.
    /// </summary>
    public class BrowserWindow
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; private set; }

        /// <summary>
        /// Emitted when the web page has been rendered (while not being shown) and 
        /// window can be displayed without a visual flash.
        /// </summary>
        public event Action OnReadyToShow
        {
            add
            {
                if (_readyToShow == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-ready-to-show", Id);
                }
                _readyToShow += value;
            }
            remove
            {
                _readyToShow -= value;
            }
        }

        public void TriggerOnReadyToShow()
        {
            _readyToShow();
        }

        private event Action _readyToShow;

        /// <summary>
        /// Emitted when the document changed its title.
        /// </summary>
        public event Action<string> OnPageTitleUpdated
        {
            add
            {
                if (_pageTitleUpdated == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-page-title-updated", Id);
                }
                _pageTitleUpdated += value;
            }
            remove
            {
                _pageTitleUpdated -= value;
            }
        }

        public void TriggerOnPageTitleUpdated(string title)
        {
            _pageTitleUpdated(title);
        }

        private event Action<string> _pageTitleUpdated;

        /// <summary>
        /// Emitted when the window is going to be closed.
        /// </summary>
        public event Action OnClose
        {
            add
            {
                if (_close == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-close", Id);
                }
                _close += value;
            }
            remove
            {
                _close -= value;
            }
        }

        public void TriggerOnClose()
        {
            _close();
        }

        private event Action _close;

        /// <summary>
        /// Emitted when the window is closed. 
        /// After you have received this event you should remove the 
        /// reference to the window and avoid using it any more.
        /// </summary>
        public event Action OnClosed
        {
            add
            {
                if (_closed == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-closed", Id);
                }
                _closed += value;
            }
            remove
            {
                _closed -= value;
            }
        }

        public void TriggerOnClosed()
        {
            // Trigger only if we've a listener
            if (_closed != null)
            {
                _closed();
            }
        }

        private event Action _closed;

        /// <summary>
        /// Emitted when window session is going to end due to force shutdown or machine restart or session log off.
        /// </summary>
        public event Action OnSessionEnd
        {
            add
            {
                if (_sessionEnd == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-session-end", Id);
                }
                _sessionEnd += value;
            }
            remove
            {
                _sessionEnd -= value;
            }
        }

        public void TriggerOnSessionEnd()
        {
            _sessionEnd();
        }

        private event Action _sessionEnd;

        /// <summary>
        /// Emitted when the web page becomes unresponsive.
        /// </summary>
        public event Action OnUnresponsive
        {
            add
            {
                if (_unresponsive == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-unresponsive", Id);
                }
                _unresponsive += value;
            }
            remove
            {
                _unresponsive -= value;
            }
        }

        public void TriggerOnUnresponsive()
        {
            _unresponsive();
        }

        private event Action _unresponsive;

        /// <summary>
        /// Emitted when the unresponsive web page becomes responsive again.
        /// </summary>
        public event Action OnResponsive
        {
            add
            {
                if (_responsive == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-responsive", Id);
                }
                _responsive += value;
            }
            remove
            {
                _responsive -= value;
            }
        }

        public void TriggerOnResponsive()
        {
            _responsive();
        }

        private event Action _responsive;

        /// <summary>
        /// Emitted when the window loses focus.
        /// </summary>
        public event Action OnBlur
        {
            add
            {
                if (_blur == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-blur", Id);
                }
                _blur += value;
            }
            remove
            {
                _blur -= value;
            }
        }

        public void TriggerOnBlur()
        {
            _blur();
        }

        public event Action _blur;

        /// <summary>
        /// Emitted when the window gains focus.
        /// </summary>
        public event Action OnFocus
        {
            add
            {
                if (_focus == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-focus", Id);
                }
                _focus += value;
            }
            remove
            {
                _focus -= value;
            }
        }

        public void TriggerOnFocus()
        {
            _focus();
        }

        private event Action _focus;

        /// <summary>
        /// Emitted when the window is shown.
        /// </summary>
        public event Action OnShow
        {
            add
            {
                if (_show == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-show", Id);
                }
                _show += value;
            }
            remove
            {
                _show -= value;
            }
        }

        public void TriggerOnShow()
        {
            _show();
        }

        private event Action _show;

        /// <summary>
        /// Emitted when the window is hidden.
        /// </summary>
        public event Action OnHide
        {
            add
            {
                if (_hide == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-hide", Id);
                }
                _hide += value;
            }
            remove
            {
                _hide -= value;
            }
        }

        public void TriggerOnHide()
        {
            _hide();
        }

        private event Action _hide;

        /// <summary>
        /// Emitted when window is maximized.
        /// </summary>
        public event Action OnMaximize
        {
            add
            {
                if (_maximize == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-maximize", Id);
                }
                _maximize += value;
            }
            remove
            {
                _maximize -= value;
            }
        }

        public void TriggerOnMaximize()
        {
            _maximize();
        }

        private event Action _maximize;

        /// <summary>
        /// Emitted when the window exits from a maximized state.
        /// </summary>
        public event Action OnUnmaximize
        {
            add
            {
                if (_unmaximize == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-unmaximize", Id);
                }
                _unmaximize += value;
            }
            remove
            {
                _unmaximize -= value;
            }
        }

        public void TriggerOnUnmaximize()
        {
            _unmaximize();
        }

        private event Action _unmaximize;

        /// <summary>
        /// Emitted when the window is minimized.
        /// </summary>
        public event Action OnMinimize
        {
            add
            {
                if (_minimize == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-minimize", Id);
                }
                _minimize += value;
            }
            remove
            {
                _minimize -= value;
            }
        }

        public void TriggerOnMinimize()
        {
            _minimize();
        }

        private event Action _minimize;

        /// <summary>
        /// Emitted when the window is restored from a minimized state.
        /// </summary>
        public event Action OnRestore
        {
            add
            {
                if (_restore == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-restore", Id);
                }
                _restore += value;
            }
            remove
            {
                _restore -= value;
            }
        }

        public void TriggerOnRestore()
        {
            _restore();
        }

        private event Action _restore;

        /// <summary>
        /// Emitted when the window is being resized.
        /// </summary>
        public event Action OnResize
        {
            add
            {
                if (_resize == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-resize", Id);
                }
                _resize += value;
            }
            remove
            {
                _resize -= value;
            }
        }

        public void TriggerOnResize()
        {
            _resize();
        }

        private event Action _resize;

        /// <summary>
        /// Emitted when the window is being moved to a new position.
        /// 
        /// Note: On macOS this event is just an alias of moved.
        /// </summary>
        public event Action OnMove
        {
            add
            {
                if (_move == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-move", Id);
                }
                _move += value;
            }
            remove
            {
                _move -= value;
            }
        }

        public void TriggerOnMove()
        {
            _move();
        }

        private event Action _move;

        /// <summary>
        /// macOS: Emitted once when the window is moved to a new position.
        /// </summary>
        public event Action OnMoved
        {
            add
            {
                if (_moved == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-moved", Id);
                }
                _moved += value;
            }
            remove
            {
                _moved -= value;
            }
        }

        public void TriggerOnMoved()
        {
            _moved();
        }

        private event Action _moved;

        /// <summary>
        /// Emitted when the window enters a full-screen state.
        /// </summary>
        public event Action OnEnterFullScreen
        {
            add
            {
                if (_enterFullScreen == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-enter-full-screen", Id);
                }
                _enterFullScreen += value;
            }
            remove
            {
                _enterFullScreen -= value;
            }
        }

        public void TriggerOnEnterFullScreen()
        {
            _enterFullScreen();
        }

        private event Action _enterFullScreen;

        /// <summary>
        /// Emitted when the window leaves a full-screen state.
        /// </summary>
        public event Action OnLeaveFullScreen
        {
            add
            {
                if (_leaveFullScreen == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-leave-full-screen", Id);
                }
                _leaveFullScreen += value;
            }
            remove
            {
                _leaveFullScreen -= value;
            }
        }

        public void TriggerOnLeaveFullScreen()
        {
            _leaveFullScreen();
        }

        private event Action _leaveFullScreen;

        /// <summary>
        /// Emitted when the window enters a full-screen state triggered by HTML API.
        /// </summary>
        public event Action OnEnterHtmlFullScreen
        {
            add
            {
                if (_enterHtmlFullScreen == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-enter-html-full-screen", Id);
                }
                _enterHtmlFullScreen += value;
            }
            remove
            {
                _enterHtmlFullScreen -= value;
            }
        }

        public void TriggerOnEnterHtmlFullScreen()
        {
            _enterHtmlFullScreen();
        }

        private event Action _enterHtmlFullScreen;

        /// <summary>
        /// Emitted when the window leaves a full-screen state triggered by HTML API.
        /// </summary>
        public event Action OnLeaveHtmlFullScreen
        {
            add
            {
                if (_leaveHtmlFullScreen == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-leave-html-full-screen", Id);
                }
                _leaveHtmlFullScreen += value;
            }
            remove
            {
                _leaveHtmlFullScreen -= value;
            }
        }

        public void TriggerOnLeaveHtmlFullScreen()
        {
            _leaveHtmlFullScreen();
        }

        private event Action _leaveHtmlFullScreen;

        /// <summary>
        /// Emitted when an App Command is invoked. These are typically related to 
        /// keyboard media keys or browser commands, as well as the “Back” button 
        /// built into some mice on Windows.
        /// 
        /// Commands are lowercased, underscores are replaced with hyphens, 
        /// and the APPCOMMAND_ prefix is stripped off.e.g.APPCOMMAND_BROWSER_BACKWARD 
        /// is emitted as browser-backward.
        /// </summary>
        public event Action<string> OnAppCommand
        {
            add
            {
                if (_appCommand == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-app-command", Id);
                }
                _appCommand += value;
            }
            remove
            {
                _appCommand -= value;
            }
        }

        public void TriggerOnAppCommand(string command)
        {
            _appCommand(command);
        }

        private event Action<string> _appCommand;

        /// <summary>
        /// Emitted when scroll wheel event phase has begun.
        /// </summary>
        public event Action OnScrollTouchBegin
        {
            add
            {
                if (_scrollTouchBegin == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-scroll-touch-begin", Id);
                }
                _scrollTouchBegin += value;
            }
            remove
            {
                _scrollTouchBegin -= value;
            }
        }

        public void TriggerOnScrollTouchBegin()
        {
            _scrollTouchBegin();
        }

        private event Action _scrollTouchBegin;

        /// <summary>
        /// Emitted when scroll wheel event phase has ended.
        /// </summary>
        public event Action OnScrollTouchEnd
        {
            add
            {
                if (_scrollTouchEnd == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-scroll-touch-end", Id);
                }
                _scrollTouchEnd += value;
            }
            remove
            {
                _scrollTouchEnd -= value;
            }
        }

        public void TriggerOnScrollTouchEnd()
        {
            _scrollTouchEnd();
        }

        private event Action _scrollTouchEnd;

        /// <summary>
        /// Emitted when scroll wheel event phase filed upon reaching the edge of element.
        /// </summary>
        public event Action OnScrollTouchEdge
        {
            add
            {
                if (_scrollTouchEdge == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-scroll-touch-edge", Id);
                }
                _scrollTouchEdge += value;
            }
            remove
            {
                _scrollTouchEdge -= value;
            }
        }

        public void TriggerOnScrollTouchEdge()
        {
            _scrollTouchEdge();
        }

        private event Action _scrollTouchEdge;

        /// <summary>
        /// Emitted on 3-finger swipe. Possible directions are up, right, down, left.
        /// </summary>
        public event Action<string> OnSwipe
        {
            add
            {
                if (_swipe == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-swipe", Id);
                }
                _swipe += value;
            }
            remove
            {
                _swipe -= value;
            }
        }

        public void TriggerOnSwipe(string direction)
        {
            _swipe(direction);
        }

        private event Action<string> _swipe;

        /// <summary>
        /// Emitted when the window opens a sheet.
        /// </summary>
        public event Action OnSheetBegin
        {
            add
            {
                if (_sheetBegin == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-sheet-begin", Id);
                }
                _sheetBegin += value;
            }
            remove
            {
                _sheetBegin -= value;
            }
        }

        public void TriggerOnSheetBegin()
        {
            _sheetBegin();
        }

        private event Action _sheetBegin;

        /// <summary>
        /// Emitted when the window has closed a sheet.
        /// </summary>
        public event Action OnSheetEnd
        {
            add
            {
                if (_sheetEnd == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-sheet-end", Id);
                }
                _sheetEnd += value;
            }
            remove
            {
                _sheetEnd -= value;
            }
        }

        public void TriggerOnSheetEnd()
        {
            _sheetEnd();
        }

        private event Action _sheetEnd;

        /// <summary>
        /// Emitted when the native new tab button is clicked.
        /// </summary>
        public event Action OnNewWindowForTab
        {
            add
            {
                if (_newWindowForTab == null)
                {
                    Electron.SignalrElectron.Clients.All.SendAsync("register-browserWindow-new-window-for-tab", Id);
                }
                _newWindowForTab += value;
            }
            remove
            {
                _newWindowForTab -= value;
            }
        }

        public void TriggerOnNewWindowForTab()
        {
            _newWindowForTab();
        }

        private event Action _newWindowForTab;

        internal BrowserWindow(int id) {
            Id = id;
            WebContents = new WebContents(id);
        }

        /// <summary>
        /// Force closing the window, the unload and beforeunload event won’t be 
        /// emitted for the web page, and close event will also not be emitted 
        /// for this window, but it guarantees the closed event will be emitted.
        /// </summary>
        public async void Destroy()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowDestroy", Id);
        }

        /// <summary>
        /// Try to close the window. This has the same effect as a user manually 
        /// clicking the close button of the window. The web page may cancel the close though. 
        /// </summary>
        public async void Close()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowClose", Id);
        }

        /// <summary>
        /// Focuses on the window.
        /// </summary>
        public async void Focus()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowFocus", Id);
        }

        /// <summary>
        /// Removes focus from the window.
        /// </summary>
        public async void Blur()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowBlur", Id);
        }

        /// <summary>
        /// Whether the window is focused.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFocusedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsFocused", Id);
        }

        /// <summary>
        /// Whether the window is destroyed.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDestroyedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsDestroyed", Id);
        }

        /// <summary>
        /// Shows and gives focus to the window.
        /// </summary>
        public async void Show()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowShow", Id);
        }

        /// <summary>
        /// Shows the window but doesn’t focus on it.
        /// </summary>
        public async void ShowInactive()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowShowInactive", Id);
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public async void Hide()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowHide", Id);
        }

        /// <summary>
        /// Whether the window is visible to the user.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsVisibleAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsVisible", Id);
        }

        /// <summary>
        /// Whether current window is a modal window.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsModalAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsModal", Id);
        }

        /// <summary>
        /// Maximizes the window. This will also show (but not focus) the window if it isn’t being displayed already.
        /// </summary>
        public async void Maximize()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowMaximize", Id);
        }

        /// <summary>
        /// Unmaximizes the window.
        /// </summary>
        public async void Unmaximize()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowUnmaximize", Id);
        }

        /// <summary>
        /// Whether the window is maximized.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsMaximizedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMaximized", Id);
        }

        /// <summary>
        /// Minimizes the window. On some platforms the minimized window will be shown in the Dock.
        /// </summary>
        public async void Minimize()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowMinimize", Id);
        }

        /// <summary>
        /// Restores the window from minimized state to its previous state.
        /// </summary>
        public async void Restore()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowRestore", Id);
        }

        /// <summary>
        /// Whether the window is minimized.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsMinimizedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMinimized", Id);
        }

        /// <summary>
        /// Sets whether the window should be in fullscreen mode.
        /// </summary>
        public async void SetFullScreen(bool flag)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetFullScreen", Id, flag);
        }

        /// <summary>
        /// Whether the window is in fullscreen mode.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFullScreenAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsFullScreen", Id);
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
        public async void SetAspectRatio(int aspectRatio, Size extraSize)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAspectRatio", Id, aspectRatio, JObject.FromObject(extraSize, _jsonSerializer));
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        public async void PreviewFile(string path)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowPreviewFile", Id, path);
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        /// <param name="displayname">The name of the file to display on the Quick Look modal view. This is 
        /// purely visual and does not affect the content type of the file. Defaults to path.</param>
        public async void PreviewFile(string path, string displayname)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowPreviewFile", Id, path, displayname);
        }

        /// <summary>
        /// Closes the currently open Quick Look panel.
        /// </summary>
        public async void CloseFilePreview()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowCloseFilePreview", Id);
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        public async void SetBounds(Rectangle bounds)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public async void SetBounds(Rectangle bounds, bool animate)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        /// <summary>
        /// Gets the bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<Rectangle> GetBoundsAsync()
        {
            var result = await SignalrSerializeHelper.GetSignalrResultJObject("browserWindowGetBounds", Id);
            return result.ToObject<Rectangle>();
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public async void SetContentBounds(Rectangle bounds)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public async void SetContentBounds(Rectangle bounds, bool animate)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        /// <summary>
        /// Gets the content bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<Rectangle> GetContentBoundsAsync()
        {
            var result = await SignalrSerializeHelper.GetSignalrResultJObject("browserWindowGetContentBounds", Id);
            return result.ToObject<Rectangle>();
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public async void SetSize(int width, int height)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public async void SetSize(int width, int height, bool animate)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s width and height.
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetSizeAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetSize", Id)).ToObject<int[]>();
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public async void SetContentSize(int width, int height)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetContentSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public async void SetContentSize(int width, int height, bool animate)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetContentSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s client area’s width and height.
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetContentSizeAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetContentSize", Id)).ToObject<int[]>();
        }

        /// <summary>
        /// Sets the minimum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public async void SetMinimumSize(int width, int height)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMinimumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s minimum width and height.
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetMinimumSizeAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetMinimumSize", Id)).ToObject<int[]>();
        }

        /// <summary>
        /// Sets the maximum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public async void SetMaximumSize(int width, int height)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMaximumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s maximum width and height.
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetMaximumSizeAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetMaximumSize", Id)).ToObject<int[]>();
        }

        /// <summary>
        /// Sets whether the window can be manually resized by user.
        /// </summary>
        /// <param name="resizable"></param>
        public async void SetResizable(bool resizable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetResizable", Id, resizable);
        }

        /// <summary>
        /// Whether the window can be manually resized by user.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsResizableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsResizable", Id);
        }

        /// <summary>
        /// Sets whether the window can be moved by user. On Linux does nothing.
        /// </summary>
        /// <param name="movable"></param>
        public async void SetMovable(bool movable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMovable", Id, movable);
        }

        /// <summary>
        /// Whether the window can be moved by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public async Task<bool> IsMovableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMovable", Id);
        }

        /// <summary>
        /// Sets whether the window can be manually minimized by user. On Linux does nothing.
        /// </summary>
        /// <param name="minimizable"></param>
        public async void SetMinimizable(bool minimizable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMinimizable", Id, minimizable);
        }

        /// <summary>
        /// Whether the window can be manually minimized by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public async Task<bool> IsMinimizableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMinimizable", Id);
        }

        /// <summary>
        /// Sets whether the window can be manually maximized by user. On Linux does nothing.
        /// </summary>
        /// <param name="maximizable"></param>
        public async void SetMaximizable(bool maximizable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMaximizable", Id, maximizable);
        }

        /// <summary>
        /// Whether the window can be manually maximized by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public async Task<bool> IsMaximizableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMaximizable", Id);
        }

        /// <summary>
        /// Sets whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <param name="fullscreenable"></param>
        public async void SetFullScreenable(bool fullscreenable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetFullScreenable", Id, fullscreenable);
        }

        /// <summary>
        /// Whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFullScreenableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsFullScreenable", Id);
        }

        /// <summary>
        /// Sets whether the window can be manually closed by user. On Linux does nothing.
        /// </summary>
        /// <param name="closable"></param>
        public async void SetClosable(bool closable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetClosable", Id, closable);
        }

        /// <summary>
        /// Whether the window can be manually closed by user.
        /// 
        /// On Linux always returns true.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        public async Task<bool> IsClosableAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsClosable", Id);
        }

        /// <summary>
        /// Sets whether the window should show always on top of other windows. 
        /// After setting this, the window is still a normal window, not a toolbox 
        /// window which can not be focused on.
        /// </summary>
        /// <param name="flag"></param>
        public async void SetAlwaysOnTop(bool flag)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAlwaysOnTop", Id, flag);
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
        public async void SetAlwaysOnTop(bool flag, OnTopLevel level)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription());
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
        public async void SetAlwaysOnTop(bool flag, OnTopLevel level, int relativeLevel)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription(), relativeLevel);
        }

        /// <summary>
        /// Whether the window is always on top of other windows.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAlwaysOnTopAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsAlwaysOnTop", Id);
        }

        /// <summary>
        /// Moves window to the center of the screen.
        /// </summary>
        public async void Center()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowCenter", Id);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(int x, int y)
        {
            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (isWindows10())
            {
                x = x - 7;
            }

            Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetPosition", Id, x, y);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate"></param>
        public void SetPosition(int x, int y, bool animate)
        {
            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (isWindows10())
            {
                x = x - 7;
            }

            Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetPosition", Id, x, y, animate);
        }

        private bool isWindows10()
        {
            return RuntimeInformation.OSDescription.Contains("Windows 10");
        }

        /// <summary>
        /// Contains the window’s current position.
        /// </summary>
        /// <returns></returns>
        public async Task<int[]> GetPositionAsync()
        {
            return (await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetPosition", Id)).ToObject<int[]>();
        }

        /// <summary>
        /// Changes the title of native window to title.
        /// </summary>
        /// <param name="title"></param>
        public async void SetTitle(string title)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetTitle", Id, title);
        }

        /// <summary>
        /// The title of the native window.
        /// 
        /// Note: The title of web page can be different from the title of the native window.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTitleAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("browserWindowGetTitle", Id);
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        public async void SetSheetOffset(float offsetY)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetSheetOffset", Id, offsetY);
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        /// <param name="offsetX"></param>
        public async void SetSheetOffset(float offsetY, float offsetX)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetSheetOffset", Id, offsetY, offsetX);
        }

        /// <summary>
        /// Starts or stops flashing the window to attract user’s attention.
        /// </summary>
        /// <param name="flag"></param>
        public async void FlashFrame(bool flag)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowFlashFrame", Id, flag);
        }

        /// <summary>
        /// Makes the window not show in the taskbar.
        /// </summary>
        /// <param name="skip"></param>
        public async void SetSkipTaskbar(bool skip)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetSkipTaskbar", Id, skip);
        }

        /// <summary>
        /// Enters or leaves the kiosk mode.
        /// </summary>
        /// <param name="flag"></param>
        public async void SetKiosk(bool flag)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetKiosk", Id, flag);
        }

        /// <summary>
        /// Whether the window is in kiosk mode.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsKioskAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsKiosk", Id);
        }

        /// <summary>
        /// Returns the native type of the handle is HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.
        /// </summary>
        /// <returns>string of the native handle obtained, HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.</returns>
        public async Task<string> GetNativeWindowHandle()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("browserWindowGetNativeWindowHandle", Id);
        }

        /// <summary>
        /// Sets the pathname of the file the window represents, 
        /// and the icon of the file will show in window’s title bar.
        /// </summary>
        /// <param name="filename"></param>
        public async void SetRepresentedFilename(string filename)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetRepresentedFilename", Id, filename);
        }

        /// <summary>
        /// The pathname of the file the window represents.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRepresentedFilenameAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultString("browserWindowGetRepresentedFilename", Id);
        }

        /// <summary>
        /// Specifies whether the window’s document has been edited, 
        /// and the icon in title bar will become gray when set to true.
        /// </summary>
        /// <param name="edited"></param>
        public async void SetDocumentEdited(bool edited)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetDocumentEdited", Id, edited);
        }

        /// <summary>
        /// Whether the window’s document has been edited.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsDocumentEditedAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsDocumentEdited", Id);
        }

        /// <summary>
        /// Focuses the on web view.
        /// </summary>
        public async void FocusOnWebView()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowFocusOnWebView", Id);
        }

        /// <summary>
        /// Blurs the web view.
        /// </summary>
        public async void BlurWebView()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowBlurWebView", Id);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        public async void LoadURL(string url)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowLoadURL", Id, url);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public async void LoadURL(string url, LoadURLOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowLoadURL", Id, url, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.reload.
        /// </summary>
        public async void Reload()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowReload", Id);
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        // ToDo: Check this -> Possibly wrong

        /// <summary>
        /// Sets the menu as the window’s menu bar, 
        /// setting it to null will remove the menu bar.
        /// </summary>
        /// <param name="menuItems"></param>
        /*public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("browserWindowSetMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("windowMenuItemClicked");
            BridgeConnector.Socket.On("windowMenuItemClicked", (id) => {
                MenuItem menuItem = _items.GetMenuItem(id.ToString());
                menuItem?.Click();
            });
        }*/

        public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMenu", Id, JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);
        }

        public void TriggerOnMenuItemClicked(string id)
        {
            MenuItem menuItem = _items.GetMenuItem(id.ToString());
            menuItem?.Click();
        }

        /// <summary>
        /// Remove the window's menu bar.
        /// </summary>
        public async void RemoveMenu()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowRemoveMenu", Id);
        }

        /// <summary>
        /// Sets progress value in progress bar. Valid range is [0, 1.0]. Remove progress
        /// bar when progress smaler as 0; Change to indeterminate mode when progress bigger as 1. On Linux
        /// platform, only supports Unity desktop environment, you need to specify the
        /// .desktop file name to desktopName field in package.json.By default, it will
        /// assume app.getName().desktop.On Windows, a mode can be passed.Accepted values
        /// are none, normal, indeterminate, error, and paused. If you call setProgressBar
        /// without a mode set (but with a value within the valid range), normal will be
        /// assumed.
        /// </summary>
        /// <param name="progress"></param>
        public async void SetProgressBar(double progress)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetProgressBar", Id, progress);
        }

        /// <summary>
        /// Sets progress value in progress bar. Valid range is [0, 1.0]. Remove progress
        /// bar when progress smaler as 0; Change to indeterminate mode when progress bigger as 1. On Linux
        /// platform, only supports Unity desktop environment, you need to specify the
        /// .desktop file name to desktopName field in package.json.By default, it will
        /// assume app.getName().desktop.On Windows, a mode can be passed.Accepted values
        /// are none, normal, indeterminate, error, and paused. If you call setProgressBar
        /// without a mode set (but with a value within the valid range), normal will be
        /// assumed.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="progressBarOptions"></param>
        public async void SetProgressBar(double progress, ProgressBarOptions progressBarOptions)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetProgressBar", Id, progress, JObject.FromObject(progressBarOptions, _jsonSerializer));
        }

        /// <summary>
        /// Sets whether the window should have a shadow. On Windows and Linux does nothing.
        /// </summary>
        /// <param name="hasShadow"></param>
        public async void SetHasShadow(bool hasShadow)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetHasShadow", Id, hasShadow);
        }

        /// <summary>
        /// Whether the window has a shadow.
        /// 
        /// On Windows and Linux always returns true.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasShadowAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowHasShadow", Id);
        }

        /// <summary>
        /// Gets the thumbar buttons.
        /// </summary>
        /// <value>
        /// The thumbar buttons.
        /// </value>
        public IReadOnlyCollection<ThumbarButton> ThumbarButtons { get { return _thumbarButtons.AsReadOnly(); } }
        private List<ThumbarButton> _thumbarButtons = new List<ThumbarButton>();

        /// <summary>
        /// Add a thumbnail toolbar with a specified set of buttons to the thumbnail 
        /// image of a window in a taskbar button layout. Returns a Boolean object 
        /// indicates whether the thumbnail has been added successfully.
        /// 
        /// The number of buttons in thumbnail toolbar should be no greater than 7 due 
        /// to the limited room.Once you setup the thumbnail toolbar, the toolbar cannot
        /// be removed due to the platform’s limitation.But you can call the API with an
        /// empty array to clean the buttons.
        /// </summary>
        /// <param name="thumbarButtons"></param>
        /// <returns>Whether the buttons were added successfully.</returns>
        public async Task<bool> SetThumbarButtonsAsync(ThumbarButton[] thumbarButtons)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            thumbarButtons.AddThumbarButtonsId();
            bool result = await SignalrSerializeHelper.GetSignalrResultBool("browserWindowSetThumbarButtons", Id, JArray.FromObject(thumbarButtons, _jsonSerializer));
            taskCompletionSource.SetResult(result);
            _thumbarButtons.Clear();
            _thumbarButtons.AddRange(thumbarButtons);

            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the region of the window to show as the thumbnail image displayed when hovering over
        /// the window in the taskbar. You can reset the thumbnail to be the entire window by specifying
        /// an empty region: {x: 0, y: 0, width: 0, height: 0}.
        /// </summary>
        /// <param name="rectangle"></param>
        public async void SetThumbnailClip(Rectangle rectangle)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetThumbnailClip", Id, rectangle);
        }

        /// <summary>
        /// Sets the toolTip that is displayed when hovering over the window thumbnail in the taskbar.
        /// </summary>
        /// <param name="tooltip"></param>
        public async void SetThumbnailToolTip(string tooltip)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetThumbnailToolTip", Id, tooltip);
        }

        /// <summary>
        /// Sets the properties for the window’s taskbar button.
        /// 
        /// Note: relaunchCommand and relaunchDisplayName must always be set together. 
        /// If one of those properties is not set, then neither will be used.
        /// </summary>
        /// <param name="options"></param>
        public async void SetAppDetails(AppDetailsOptions options)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAppDetails", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.showDefinitionForSelection().
        /// </summary>
        public async void ShowDefinitionForSelection()
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowShowDefinitionForSelection", Id);
        }

        /// <summary>
        /// Sets whether the window menu bar should hide itself automatically. 
        /// Once set the menu bar will only show when users press the single Alt key.
        /// 
        /// If the menu bar is already visible, calling setAutoHideMenuBar(true) won’t hide it immediately.
        /// </summary>
        /// <param name="hide"></param>
        public async void SetAutoHideMenuBar(bool hide)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAutoHideMenuBar", Id, hide);
        }

        /// <summary>
        /// Whether menu bar automatically hides itself.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsMenuBarAutoHideAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMenuBarAutoHide", Id);
        }

        /// <summary>
        /// Sets whether the menu bar should be visible. If the menu bar is auto-hide,
        /// users can still bring up the menu bar by pressing the single Alt key.
        /// </summary>
        /// <param name="visible"></param>
        public async void SetMenuBarVisibility(bool visible)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetMenuBarVisibility", Id, visible);
        }

        /// <summary>
        /// Whether the menu bar is visible.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsMenuBarVisibleAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsMenuBarVisible", Id);
        }

        /// <summary>
        /// Sets whether the window should be visible on all workspaces.
        /// 
        /// Note: This API does nothing on Windows.
        /// </summary>
        /// <param name="visible"></param>
        public async void SetVisibleOnAllWorkspaces(bool visible)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetVisibleOnAllWorkspaces", Id, visible);
        }

        /// <summary>
        /// Whether the window is visible on all workspaces.
        /// 
        /// Note: This API always returns false on Windows.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsVisibleOnAllWorkspacesAsync()
        {
            return await SignalrSerializeHelper.GetSignalrResultBool("browserWindowIsVisibleOnAllWorkspaces", Id);
        }

        /// <summary>
        /// Makes the window ignore all mouse events.
        /// 
        /// All mouse events happened in this window will be passed to the window 
        /// below this window, but if this window has focus, it will still receive keyboard events.
        /// </summary>
        /// <param name="ignore"></param>
        public async void SetIgnoreMouseEvents(bool ignore)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetIgnoreMouseEvents", Id, ignore);
        }

        /// <summary>
        /// Prevents the window contents from being captured by other apps.
        /// 
        /// On macOS it sets the NSWindow’s sharingType to NSWindowSharingNone. 
        /// On Windows it calls SetWindowDisplayAffinity with WDA_MONITOR.
        /// </summary>
        /// <param name="enable"></param>
        public async void SetContentProtection(bool enable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetContentProtection", Id, enable);
        }

        /// <summary>
        /// Changes whether the window can be focused.
        /// </summary>
        /// <param name="focusable"></param>
        public async void SetFocusable(bool focusable)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetFocusable", Id, focusable);
        }

        /// <summary>
        /// Sets parent as current window’s parent window, 
        /// passing null will turn current window into a top-level window.
        /// </summary>
        /// <param name="parent"></param>
        public async void SetParentWindow(BrowserWindow parent)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetParentWindow", Id, JObject.FromObject(parent, _jsonSerializer));
        }

        /// <summary>
        /// The parent window.
        /// </summary>
        /// <returns></returns>
        public async Task<BrowserWindow> GetParentWindowAsync()
        {
            string parentId = await SignalrSerializeHelper.GetSignalrResultString("browserWindowGetParentWindow", Id);
            var browserWindowId = int.Parse(parentId.ToString());
            var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == browserWindowId);
            return browserWindow;
        }

        /// <summary>
        /// All child windows.
        /// </summary>
        /// <returns></returns>
        public async Task<List<BrowserWindow>> GetChildWindowsAsync()
        {
            JArray childWindows = await SignalrSerializeHelper.GetSignalrResultJArray("browserWindowGetChildWindows", Id);
            var browserWindowIds = (childWindows).ToObject<int[]>();
            var browserWindows = new List<BrowserWindow>();

            browserWindowIds.ToList().ForEach(id =>
            {
                var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == id);
                browserWindows.Add(browserWindow);
            });

            return browserWindows;
        }

        /// <summary>
        /// Controls whether to hide cursor when typing.
        /// </summary>
        /// <param name="autoHide"></param>
        public async void SetAutoHideCursor(bool autoHide)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetAutoHideCursor", Id, autoHide);
        }

        /// <summary>
        /// Adds a vibrancy effect to the browser window. 
        /// Passing null or an empty string will remove the vibrancy effect on the window.
        /// </summary>
        /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
        /// menu, popover, sidebar, medium-light or ultra-dark. 
        /// See the macOS documentation for more details.</param>
        public async void SetVibrancy(Vibrancy type)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindowSetVibrancy", Id, type.GetDescription());
        }

        /// <summary>
        /// Render and control web pages.
        /// </summary>
        public WebContents WebContents { get; internal set; }

        /// <summary>
        /// A BrowserView can be used to embed additional web content into a BrowserWindow. 
        /// It is like a child window, except that it is positioned relative to its owning window. 
        /// It is meant to be an alternative to the webview tag.
        /// </summary>
        /// <param name="browserView"></param>
        public async void SetBrowserView(BrowserView browserView)
        {
            await Electron.SignalrElectron.Clients.All.SendAsync("browserWindow-setBrowserView", Id, browserView.Id);
        }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
