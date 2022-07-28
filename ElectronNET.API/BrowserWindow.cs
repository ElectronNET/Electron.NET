using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

//TODO: Add setTrafficLightPosition and getTrafficLightPosition: https://www.electronjs.org/docs/api/browser-window#winsettrafficlightpositionposition-macos

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
                    BridgeConnector.On("browserWindow-ready-to-show" + Id, () =>
                    {
                        _readyToShow();
                    });

                    BridgeConnector.Emit("register-browserWindow-ready-to-show", Id);
                }
                _readyToShow += value;
            }
            remove
            {
                _readyToShow -= value;

                if (_readyToShow == null)
                    BridgeConnector.Off("browserWindow-ready-to-show" + Id);
            }
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
                    BridgeConnector.On<string>("browserWindow-page-title-updated" + Id, (title) =>
                    {
                        _pageTitleUpdated(title);
                    });

                    BridgeConnector.Emit("register-browserWindow-page-title-updated", Id);
                }
                _pageTitleUpdated += value;
            }
            remove
            {
                _pageTitleUpdated -= value;

                if (_pageTitleUpdated == null)
                    BridgeConnector.Off("browserWindow-page-title-updated" + Id);
            }
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
                    BridgeConnector.On("browserWindow-close" + Id, () =>
                    {
                        _close();
                    });

                    BridgeConnector.Emit("register-browserWindow-close", Id);
                }
                _close += value;
            }
            remove
            {
                _close -= value;

                if (_close == null)
                    BridgeConnector.Off("browserWindow-close" + Id);
            }
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
                    BridgeConnector.On("browserWindow-closed" + Id, () =>
                    {
                        _closed();
                    });

                    BridgeConnector.Emit("register-browserWindow-closed", Id);
                }
                _closed += value;
            }
            remove
            {
                _closed -= value;

                if (_closed == null)
                    BridgeConnector.Off("browserWindow-closed" + Id);
            }
        }

        private event Action _closed;

        /// <summary>
        /// Emitted when window session is going to end due to force shutdown or machine restart or session log off.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public event Action OnSessionEnd
        {
            add
            {
                if (_sessionEnd == null)
                {
                    BridgeConnector.On("browserWindow-session-end" + Id, () =>
                    {
                        _sessionEnd();
                    });

                    BridgeConnector.Emit("register-browserWindow-session-end", Id);
                }
                _sessionEnd += value;
            }
            remove
            {
                _sessionEnd -= value;

                if (_sessionEnd == null)
                    BridgeConnector.Off("browserWindow-session-end" + Id);
            }
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
                    BridgeConnector.On("browserWindow-unresponsive" + Id, () =>
                    {
                        _unresponsive();
                    });

                    BridgeConnector.Emit("register-browserWindow-unresponsive", Id);
                }
                _unresponsive += value;
            }
            remove
            {
                _unresponsive -= value;

                if (_unresponsive == null)
                    BridgeConnector.Off("browserWindow-unresponsive" + Id);
            }
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
                    BridgeConnector.On("browserWindow-responsive" + Id, () =>
                    {
                        _responsive();
                    });

                    BridgeConnector.Emit("register-browserWindow-responsive", Id);
                }
                _responsive += value;
            }
            remove
            {
                _responsive -= value;

                if (_responsive == null)
                    BridgeConnector.Off("browserWindow-responsive" + Id);
            }
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
                    BridgeConnector.On("browserWindow-blur" + Id, () =>
                    {
                        _blur();
                    });

                    BridgeConnector.Emit("register-browserWindow-blur", Id);
                }
                _blur += value;
            }
            remove
            {
                _blur -= value;

                if (_blur == null)
                    BridgeConnector.Off("browserWindow-blur" + Id);
            }
        }

        private event Action _blur;

        /// <summary>
        /// Emitted when the window gains focus.
        /// </summary>
        public event Action OnFocus
        {
            add
            {
                if (_focus == null)
                {
                    BridgeConnector.On("browserWindow-focus" + Id, () =>
                    {
                        _focus();
                    });

                    BridgeConnector.Emit("register-browserWindow-focus", Id);
                }
                _focus += value;
            }
            remove
            {
                _focus -= value;

                if (_focus == null)
                    BridgeConnector.Off("browserWindow-focus" + Id);
            }
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
                    BridgeConnector.On("browserWindow-show" + Id, () =>
                    {
                        _show();
                    });

                    BridgeConnector.Emit("register-browserWindow-show", Id);
                }
                _show += value;
            }
            remove
            {
                _show -= value;

                if (_show == null)
                    BridgeConnector.Off("browserWindow-show" + Id);
            }
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
                    BridgeConnector.On("browserWindow-hide" + Id, () =>
                    {
                        _hide();
                    });

                    BridgeConnector.Emit("register-browserWindow-hide", Id);
                }
                _hide += value;
            }
            remove
            {
                _hide -= value;

                if (_hide == null)
                    BridgeConnector.Off("browserWindow-hide" + Id);
            }
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
                    BridgeConnector.On("browserWindow-maximize" + Id, () =>
                    {
                        _maximize();
                    });

                    BridgeConnector.Emit("register-browserWindow-maximize", Id);
                }
                _maximize += value;
            }
            remove
            {
                _maximize -= value;

                if (_maximize == null)
                    BridgeConnector.Off("browserWindow-maximize" + Id);
            }
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
                    BridgeConnector.On("browserWindow-unmaximize" + Id, () =>
                    {
                        _unmaximize();
                    });

                    BridgeConnector.Emit("register-browserWindow-unmaximize", Id);
                }
                _unmaximize += value;
            }
            remove
            {
                _unmaximize -= value;

                if (_unmaximize == null)
                    BridgeConnector.Off("browserWindow-unmaximize" + Id);
            }
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
                    BridgeConnector.On("browserWindow-minimize" + Id, () =>
                    {
                        _minimize();
                    });

                    BridgeConnector.Emit("register-browserWindow-minimize", Id);
                }
                _minimize += value;
            }
            remove
            {
                _minimize -= value;

                if (_minimize == null)
                    BridgeConnector.Off("browserWindow-minimize" + Id);
            }
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
                    BridgeConnector.On("browserWindow-restore" + Id, () =>
                    {
                        _restore();
                    });

                    BridgeConnector.Emit("register-browserWindow-restore", Id);
                }
                _restore += value;
            }
            remove
            {
                _restore -= value;

                if (_restore == null)
                    BridgeConnector.Off("browserWindow-restore" + Id);
            }
        }

        private event Action _restore;

        /// <summary>
        /// Emitted when the window is being resized.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public event Action OnResize
        {
            add
            {
                if (_resize == null)
                {
                    BridgeConnector.On("browserWindow-resize" + Id, () =>
                    {
                        _resize();
                    });

                    BridgeConnector.Emit("register-browserWindow-resize", Id);
                }
                _resize += value;
            }
            remove
            {
                _resize -= value;

                if (_resize == null)
                    BridgeConnector.Off("browserWindow-resize" + Id);
            }
        }

        private event Action _resize;

        /// <summary>
        /// Emitted when the window is being moved to a new position.
        /// 
        /// Note: On macOS this event is just an alias of moved.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public event Action OnMove
        {
            add
            {
                if (_move == null)
                {
                    BridgeConnector.On("browserWindow-move" + Id, () =>
                    {
                        _move();
                    });

                    BridgeConnector.Emit("register-browserWindow-move", Id);
                }
                _move += value;
            }
            remove
            {
                _move -= value;

                if (_move == null)
                    BridgeConnector.Off("browserWindow-move" + Id);
            }
        }

        private event Action _move;

        /// <summary>
        /// Emitted once when the window is moved to a new position.
        /// </summary>
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public event Action OnMoved
        {
            add
            {
                if (_moved == null)
                {
                    BridgeConnector.On("browserWindow-moved" + Id, () =>
                    {
                        _moved();
                    });

                    BridgeConnector.Emit("register-browserWindow-moved", Id);
                }
                _moved += value;
            }
            remove
            {
                _moved -= value;

                if (_moved == null)
                    BridgeConnector.Off("browserWindow-moved" + Id);
            }
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
                    BridgeConnector.On("browserWindow-enter-full-screen" + Id, () =>
                    {
                        _enterFullScreen();
                    });

                    BridgeConnector.Emit("register-browserWindow-enter-full-screen", Id);
                }
                _enterFullScreen += value;
            }
            remove
            {
                _enterFullScreen -= value;

                if (_enterFullScreen == null)
                    BridgeConnector.Off("browserWindow-enter-full-screen" + Id);
            }
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
                    BridgeConnector.On("browserWindow-leave-full-screen" + Id, () =>
                    {
                        _leaveFullScreen();
                    });

                    BridgeConnector.Emit("register-browserWindow-leave-full-screen", Id);
                }
                _leaveFullScreen += value;
            }
            remove
            {
                _leaveFullScreen -= value;

                if (_leaveFullScreen == null)
                    BridgeConnector.Off("browserWindow-leave-full-screen" + Id);
            }
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
                    BridgeConnector.On("browserWindow-enter-html-full-screen" + Id, () =>
                    {
                        _enterHtmlFullScreen();
                    });

                    BridgeConnector.Emit("register-browserWindow-enter-html-full-screen", Id);
                }
                _enterHtmlFullScreen += value;
            }
            remove
            {
                _enterHtmlFullScreen -= value;

                if (_enterHtmlFullScreen == null)
                    BridgeConnector.Off("browserWindow-enter-html-full-screen" + Id);
            }
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
                    BridgeConnector.On("browserWindow-leave-html-full-screen" + Id, () =>
                    {
                        _leaveHtmlFullScreen();
                    });

                    BridgeConnector.Emit("register-browserWindow-leave-html-full-screen", Id);
                }
                _leaveHtmlFullScreen += value;
            }
            remove
            {
                _leaveHtmlFullScreen -= value;

                if (_leaveHtmlFullScreen == null)
                    BridgeConnector.Off("browserWindow-leave-html-full-screen" + Id);
            }
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
        [SupportedOSPlatform("macos")]
        [SupportedOSPlatform("windows")]
        public event Action<string> OnAppCommand
        {
            add
            {
                if (_appCommand == null)
                {
                    BridgeConnector.On<string>("browserWindow-app-command" + Id, (command) =>
                    {
                        _appCommand(command);
                    });

                    BridgeConnector.Emit("register-browserWindow-app-command", Id);
                }
                _appCommand += value;
            }
            remove
            {
                _appCommand -= value;

                if (_appCommand == null)
                    BridgeConnector.Off("browserWindow-app-command" + Id);
            }
        }

        private event Action<string> _appCommand;

        /// <summary>
        /// Emitted when scroll wheel event phase has begun.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnScrollTouchBegin
        {
            add
            {
                if (_scrollTouchBegin == null)
                {
                    BridgeConnector.On("browserWindow-scroll-touch-begin" + Id, () =>
                    {
                        _scrollTouchBegin();
                    });

                    BridgeConnector.Emit("register-browserWindow-scroll-touch-begin", Id);
                }
                _scrollTouchBegin += value;
            }
            remove
            {
                _scrollTouchBegin -= value;

                if (_scrollTouchBegin == null)
                    BridgeConnector.Off("browserWindow-scroll-touch-begin" + Id);
            }
        }

        private event Action _scrollTouchBegin;

        /// <summary>
        /// Emitted when scroll wheel event phase has ended.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnScrollTouchEnd
        {
            add
            {
                if (_scrollTouchEnd == null)
                {
                    BridgeConnector.On("browserWindow-scroll-touch-end" + Id, () =>
                    {
                        _scrollTouchEnd();
                    });

                    BridgeConnector.Emit("register-browserWindow-scroll-touch-end", Id);
                }
                _scrollTouchEnd += value;
            }
            remove
            {
                _scrollTouchEnd -= value;

                if (_scrollTouchEnd == null)
                    BridgeConnector.Off("browserWindow-scroll-touch-end" + Id);
            }
        }

        private event Action _scrollTouchEnd;

        /// <summary>
        /// Emitted when scroll wheel event phase filed upon reaching the edge of element.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnScrollTouchEdge
        {
            add
            {
                if (_scrollTouchEdge == null)
                {
                    BridgeConnector.On("browserWindow-scroll-touch-edge" + Id, () =>
                    {
                        _scrollTouchEdge();
                    });

                    BridgeConnector.Emit("register-browserWindow-scroll-touch-edge", Id);
                }
                _scrollTouchEdge += value;
            }
            remove
            {
                _scrollTouchEdge -= value;

                if (_scrollTouchEdge == null)
                    BridgeConnector.Off("browserWindow-scroll-touch-edge" + Id);
            }
        }

        private event Action _scrollTouchEdge;

        /// <summary>
        /// Emitted on 3-finger swipe. Possible directions are up, right, down, left.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action<string> OnSwipe
        {
            add
            {
                if (_swipe == null)
                {
                    BridgeConnector.On<string>("browserWindow-swipe" + Id, (direction) =>
                    {
                        _swipe(direction);
                    });

                    BridgeConnector.Emit("register-browserWindow-swipe", Id);
                }
                _swipe += value;
            }
            remove
            {
                _swipe -= value;

                if (_swipe == null)
                    BridgeConnector.Off("browserWindow-swipe" + Id);
            }
        }

        private event Action<string> _swipe;

        /// <summary>
        /// Emitted when the window opens a sheet.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnSheetBegin
        {
            add
            {
                if (_sheetBegin == null)
                {
                    BridgeConnector.On("browserWindow-sheet-begin" + Id, () =>
                    {
                        _sheetBegin();
                    });

                    BridgeConnector.Emit("register-browserWindow-sheet-begin", Id);
                }
                _sheetBegin += value;
            }
            remove
            {
                _sheetBegin -= value;

                if (_sheetBegin == null)
                    BridgeConnector.Off("browserWindow-sheet-begin" + Id);
            }
        }

        private event Action _sheetBegin;

        /// <summary>
        /// Emitted when the window has closed a sheet.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnSheetEnd
        {
            add
            {
                if (_sheetEnd == null)
                {
                    BridgeConnector.On("browserWindow-sheet-end" + Id, () =>
                    {
                        _sheetEnd();
                    });

                    BridgeConnector.Emit("register-browserWindow-sheet-end", Id);
                }
                _sheetEnd += value;
            }
            remove
            {
                _sheetEnd -= value;

                if (_sheetEnd == null)
                    BridgeConnector.Off("browserWindow-sheet-end" + Id);
            }
        }

        private event Action _sheetEnd;

        /// <summary>
        /// Emitted when the native new tab button is clicked.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public event Action OnNewWindowForTab
        {
            add
            {
                if (_newWindowForTab == null)
                {
                    BridgeConnector.On("browserWindow-new-window-for-tab" + Id, () =>
                    {
                        _newWindowForTab();
                    });

                    BridgeConnector.Emit("register-browserWindow-new-window-for-tab", Id);
                }
                _newWindowForTab += value;
            }
            remove
            {
                _newWindowForTab -= value;

                if (_newWindowForTab == null)
                    BridgeConnector.Off("browserWindow-new-window-for-tab" + Id);
            }
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
        public void Destroy()
        {
            BridgeConnector.Emit("browserWindowDestroy", Id);
        }

        /// <summary>
        /// Try to close the window. This has the same effect as a user manually 
        /// clicking the close button of the window. The web page may cancel the close though. 
        /// </summary>
        public void Close()
        {
            BridgeConnector.Emit("browserWindowClose", Id);
        }

        /// <summary>
        /// Focuses on the window.
        /// </summary>
        public void Focus()
        {
            BridgeConnector.Emit("browserWindowFocus", Id);
        }

        /// <summary>
        /// Removes focus from the window.
        /// </summary>
        public void Blur()
        {
            BridgeConnector.Emit("browserWindowBlur", Id);
        }

        /// <summary>
        /// Whether the window is focused.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFocusedAsync() => BridgeConnector.OnResult<bool>("browserWindowIsFocused", "browserWindow-isFocused-completed" + Id, Id);

        /// <summary>
        /// Whether the window is destroyed.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsDestroyedAsync() => BridgeConnector.OnResult<bool>("browserWindowIsDestroyed", "browserWindow-isDestroyed-completed" + Id, Id);

        /// <summary>
        /// Shows and gives focus to the window.
        /// </summary>
        public void Show()
        {
            BridgeConnector.Emit("browserWindowShow", Id);
        }

        /// <summary>
        /// Shows the window but doesn’t focus on it.
        /// </summary>
        public void ShowInactive()
        {
            BridgeConnector.Emit("browserWindowShowInactive", Id);
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public void Hide()
        {
            BridgeConnector.Emit("browserWindowHide", Id);
        }

        /// <summary>
        /// Whether the window is visible to the user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsVisibleAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsVisible", "browserWindow-isVisible-completed" + Id, Id);
        }

        /// <summary>
        /// Whether current window is a modal window.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsModalAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsModal", "browserWindow-isModal-completed" + Id, Id);
        }

        /// <summary>
        /// Maximizes the window. This will also show (but not focus) the window if it isn’t being displayed already.
        /// </summary>
        public void Maximize()
        {
            BridgeConnector.Emit("browserWindowMaximize", Id);
        }

        /// <summary>
        /// Unmaximizes the window.
        /// </summary>
        public void Unmaximize()
        {
            BridgeConnector.Emit("browserWindowUnmaximize", Id);
        }

        /// <summary>
        /// Whether the window is maximized.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMaximizedAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMaximized", "browserWindow-isMaximized-completed" + Id, Id);
        }

        /// <summary>
        /// Minimizes the window. On some platforms the minimized window will be shown in the Dock.
        /// </summary>
        public void Minimize()
        {
            BridgeConnector.Emit("browserWindowMinimize", Id);
        }

        /// <summary>
        /// Restores the window from minimized state to its previous state.
        /// </summary>
        public void Restore()
        {
            BridgeConnector.Emit("browserWindowRestore", Id);
        }

        /// <summary>
        /// Whether the window is minimized.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMinimizedAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMinimized", "browserWindow-isMinimized-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window should be in fullscreen mode.
        /// </summary>
        public void SetFullScreen(bool flag)
        {
            BridgeConnector.Emit("browserWindowSetFullScreen", Id, flag);
        }
        
        /// <summary>
        /// Sets whether the background color of the window
        /// </summary>
        public void SetBackgroundColor(string color)
        {
            BridgeConnector.Emit("browserWindowSetBackgroundColor", Id, color);
        }

        /// <summary>
        /// Whether the window is in fullscreen mode.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFullScreenAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsFullScreen", "browserWindow-isFullScreen-completed" + Id, Id);
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
        public void SetAspectRatio(int aspectRatio)
        {
            BridgeConnector.Emit("browserWindowSetAspectRatio", Id, aspectRatio, new Size() { Height = 0, Width = 0 });
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
        [SupportedOSPlatform("macos")]
        public void SetAspectRatio(int aspectRatio, Size extraSize)
        {
            BridgeConnector.Emit("browserWindowSetAspectRatio", Id, aspectRatio, extraSize);
        }




        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        [SupportedOSPlatform("macos")]
        public void PreviewFile(string path)
        {
            BridgeConnector.Emit("browserWindowPreviewFile", Id, path);
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        /// <param name="displayname">The name of the file to display on the Quick Look modal view. This is 
        /// purely visual and does not affect the content type of the file. Defaults to path.</param>
        [SupportedOSPlatform("macos")]
        public void PreviewFile(string path, string displayname)
        {
            BridgeConnector.Emit("browserWindowPreviewFile", Id, path, displayname);
        }

        /// <summary>
        /// Closes the currently open Quick Look panel.
        /// </summary>
        [SupportedOSPlatform("macos")]
        public void CloseFilePreview()
        {
            BridgeConnector.Emit("browserWindowCloseFilePreview", Id);
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        public void SetBounds(Rectangle bounds)
        {
            BridgeConnector.Emit("browserWindowSetBounds", Id, bounds);
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        [SupportedOSPlatform("macos")]
        public void SetBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Emit("browserWindowSetBounds", Id, bounds, animate);
        }

        /// <summary>
        /// Gets the bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<Rectangle> GetBoundsAsync()
        {
            return BridgeConnector.OnResult<Rectangle>("browserWindowGetBounds", "browserWindow-getBounds-completed" + Id, Id);
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public void SetContentBounds(Rectangle bounds)
        {
            BridgeConnector.Emit("browserWindowSetContentBounds", Id, bounds);
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        [SupportedOSPlatform("macos")]
        public void SetContentBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Emit("browserWindowSetContentBounds", Id, bounds, animate);
        }

        /// <summary>
        /// Gets the content bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<Rectangle> GetContentBoundsAsync()
        {
            return BridgeConnector.OnResult<Rectangle>("browserWindowGetContentBounds", "browserWindow-getContentBounds-completed" + Id, Id);
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height)
        {
            BridgeConnector.Emit("browserWindowSetSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        [SupportedOSPlatform("macos")]
        public void SetSize(int width, int height, bool animate)
        {
            BridgeConnector.Emit("browserWindowSetSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetSizeAsync()
        {
            return BridgeConnector.OnResult<int[]>("browserWindowGetSize", "browserWindow-getSize-completed" + Id, Id);
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetContentSize(int width, int height)
        {
            BridgeConnector.Emit("browserWindowSetContentSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        [SupportedOSPlatform("macos")]
        public void SetContentSize(int width, int height, bool animate)
        {
            BridgeConnector.Emit("browserWindowSetContentSize", Id, width, height, animate);
        }

        /// <summary>
        /// Contains the window’s client area’s width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetContentSizeAsync()
        {
            return BridgeConnector.OnResult<int[]>("browserWindowGetContentSize", "browserWindow-getContentSize-completed" + Id, Id);
        }

        /// <summary>
        /// Sets the minimum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMinimumSize(int width, int height)
        {
            BridgeConnector.Emit("browserWindowSetMinimumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s minimum width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetMinimumSizeAsync()
        {
            return BridgeConnector.OnResult<int[]>("browserWindowGetMinimumSize", "browserWindow-getMinimumSize-completed" + Id, Id);
        }

        /// <summary>
        /// Sets the maximum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMaximumSize(int width, int height)
        {
            BridgeConnector.Emit("browserWindowSetMaximumSize", Id, width, height);
        }

        /// <summary>
        /// Contains the window’s maximum width and height.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetMaximumSizeAsync()
        {
            return BridgeConnector.OnResult<int[]>("browserWindowGetMaximumSize", "browserWindow-getMaximumSize-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window can be manually resized by user.
        /// </summary>
        /// <param name="resizable"></param>
        public void SetResizable(bool resizable)
        {
            BridgeConnector.Emit("browserWindowSetResizable", Id, resizable);
        }

        /// <summary>
        /// Whether the window can be manually resized by user.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsResizableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsResizable", "browserWindow-isResizable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window can be moved by user. On Linux does nothing.
        /// </summary>
        /// <param name="movable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetMovable(bool movable)
        {
            BridgeConnector.Emit("browserWindowSetMovable", Id, movable);
        }

        /// <summary>
        /// Whether the window can be moved by user.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public Task<bool> IsMovableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMovable", "browserWindow-isMovable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window can be manually minimized by user. On Linux does nothing.
        /// </summary>
        /// <param name="minimizable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetMinimizable(bool minimizable)
        {
            BridgeConnector.Emit("browserWindowSetMinimizable", Id, minimizable);
        }

        /// <summary>
        /// Whether the window can be manually minimized by user.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public Task<bool> IsMinimizableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMinimizable", "browserWindow-isMinimizable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window can be manually maximized by user. On Linux does nothing.
        /// </summary>
        /// <param name="maximizable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetMaximizable(bool maximizable)
        {
            BridgeConnector.Emit("browserWindowSetMaximizable", Id, maximizable);
        }

        /// <summary>
        /// Whether the window can be manually maximized by user.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public Task<bool> IsMaximizableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMaximizable", "browserWindow-isMaximizable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <param name="fullscreenable"></param>
        public void SetFullScreenable(bool fullscreenable)
        {
            BridgeConnector.Emit("browserWindowSetFullScreenable", Id, fullscreenable);
        }

        /// <summary>
        /// Whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsFullScreenableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsFullScreenable", "browserWindow-isFullScreenable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window can be manually closed by user. On Linux does nothing.
        /// </summary>
        /// <param name="closable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetClosable(bool closable)
        {
            BridgeConnector.Emit("browserWindowSetClosable", Id, closable);
        }

        /// <summary>
        /// Whether the window can be manually closed by user.
        /// </summary>
        /// <returns>On Linux always returns true.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public Task<bool> IsClosableAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsClosable", "browserWindow-isClosable-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window should show always on top of other windows. 
        /// After setting this, the window is still a normal window, not a toolbox 
        /// window which can not be focused on.
        /// </summary>
        /// <param name="flag"></param>
        public void SetAlwaysOnTop(bool flag)
        {
            BridgeConnector.Emit("browserWindowSetAlwaysOnTop", Id, flag);
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
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetAlwaysOnTop(bool flag, OnTopLevel level)
        {
            BridgeConnector.Emit("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription());
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
        [SupportedOSPlatform("macos")]
        public void SetAlwaysOnTop(bool flag, OnTopLevel level, int relativeLevel)
        {
            BridgeConnector.Emit("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription(), relativeLevel);
        }

        /// <summary>
        /// Whether the window is always on top of other windows.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsAlwaysOnTopAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsAlwaysOnTop", "browserWindow-isAlwaysOnTop-completed" + Id, Id);
        }

        /// <summary>
        /// Moves window to the center of the screen.
        /// </summary>
        public void Center()
        {
            BridgeConnector.Emit("browserWindowCenter", Id);
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
                x -= 7;
            }

            BridgeConnector.Emit("browserWindowSetPosition", Id, x, y);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate"></param>
        [SupportedOSPlatform("macos")]
        public void SetPosition(int x, int y, bool animate)
        {
            // Workaround Windows 10 / Electron Bug
            // https://github.com/electron/electron/issues/4045
            if (isWindows10())
            {
                x -= 7;
            }

            BridgeConnector.Emit("browserWindowSetPosition", Id, x, y, animate);
        }

        private bool isWindows10()
        {
            return OperatingSystem.IsWindowsVersionAtLeast(10);
        }

        /// <summary>
        /// Contains the window’s current position.
        /// </summary>
        /// <returns></returns>
        public Task<int[]> GetPositionAsync()
        {
            return BridgeConnector.OnResult<int[]>("browserWindowGetPosition", "browserWindow-getPosition-completed" + Id, Id);
        }

        /// <summary>
        /// Changes the title of native window to title.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            BridgeConnector.Emit("browserWindowSetTitle", Id, title);
        }

        /// <summary>
        /// The title of the native window.
        /// 
        /// Note: The title of web page can be different from the title of the native window.
        /// </summary>
        /// <returns></returns>
        public Task<string> GetTitleAsync()
        {
            return BridgeConnector.OnResult<string>("browserWindowGetTitle", "browserWindow-getTitle-completed" + Id, Id);
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        [SupportedOSPlatform("macos")]
        public void SetSheetOffset(float offsetY)
        {
            BridgeConnector.Emit("browserWindowSetSheetOffset", Id, offsetY);
        }

        /// <summary>
        /// Changes the attachment point for sheets on macOS. 
        /// By default, sheets are attached just below the window frame, 
        /// but you may want to display them beneath a HTML-rendered toolbar.
        /// </summary>
        /// <param name="offsetY"></param>
        /// <param name="offsetX"></param>
        [SupportedOSPlatform("macos")]
        public void SetSheetOffset(float offsetY, float offsetX)
        {
            BridgeConnector.Emit("browserWindowSetSheetOffset", Id, offsetY, offsetX);
        }

        /// <summary>
        /// Starts or stops flashing the window to attract user’s attention.
        /// </summary>
        /// <param name="flag"></param>
        public void FlashFrame(bool flag)
        {
            BridgeConnector.Emit("browserWindowFlashFrame", Id, flag);
        }

        /// <summary>
        /// Makes the window not show in the taskbar.
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipTaskbar(bool skip)
        {
            BridgeConnector.Emit("browserWindowSetSkipTaskbar", Id, skip);
        }

        /// <summary>
        /// Enters or leaves the kiosk mode.
        /// </summary>
        /// <param name="flag"></param>
        public void SetKiosk(bool flag)
        {
            BridgeConnector.Emit("browserWindowSetKiosk", Id, flag);
        }

        /// <summary>
        /// Whether the window is in kiosk mode.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsKioskAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsKiosk", "browserWindow-isKiosk-completed" + Id, Id);
        }

        /// <summary>
        /// Returns the native type of the handle is HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.
        /// </summary>
        /// <returns>string of the native handle obtained, HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.</returns>
        public Task<string> GetNativeWindowHandle()
        {
            return BridgeConnector.OnResult<string>("browserWindowGetNativeWindowHandle", "browserWindow-getNativeWindowHandle-completed" + Id, Id);
        }

        /// <summary>
        /// Sets the pathname of the file the window represents, 
        /// and the icon of the file will show in window’s title bar.
        /// </summary>
        /// <param name="filename"></param>
        [SupportedOSPlatform("macos")]
        public void SetRepresentedFilename(string filename)
        {
            BridgeConnector.Emit("browserWindowSetRepresentedFilename", Id, filename);
        }

        /// <summary>
        /// The pathname of the file the window represents.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("macos")]
        public Task<string> GetRepresentedFilenameAsync()
        {
            return BridgeConnector.OnResult<string>("browserWindowGetRepresentedFilename", "browserWindow-getRepresentedFilename-completed" + Id, Id);
        }

        /// <summary>
        /// Specifies whether the window’s document has been edited, 
        /// and the icon in title bar will become gray when set to true.
        /// </summary>
        /// <param name="edited"></param>
        [SupportedOSPlatform("macos")]
        public void SetDocumentEdited(bool edited)
        {
            BridgeConnector.Emit("browserWindowSetDocumentEdited", Id, edited);
        }

        /// <summary>
        /// Whether the window’s document has been edited.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("macos")]
        public Task<bool> IsDocumentEditedAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsDocumentEdited", "browserWindow-isDocumentEdited-completed" + Id, Id);
        }

        /// <summary>
        /// Focuses the on web view.
        /// </summary>
        public void FocusOnWebView()
        {
            BridgeConnector.Emit("browserWindowFocusOnWebView", Id);
        }

        /// <summary>
        /// Blurs the web view.
        /// </summary>
        public void BlurWebView()
        {
            BridgeConnector.Emit("browserWindowBlurWebView", Id);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        public void LoadURL(string url)
        {
            BridgeConnector.Emit("browserWindowLoadURL", Id, url);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public void LoadURL(string url, LoadURLOptions options)
        {
            BridgeConnector.Emit("browserWindowLoadURL", Id, url, options);
        }

        /// <summary>
        /// Same as webContents.reload.
        /// </summary>
        public void Reload()
        {
            BridgeConnector.Emit("browserWindowReload", Id);
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private readonly List<MenuItem> _items = new();

        /// <summary>
        /// Sets the menu as the window’s menu bar, 
        /// setting it to null will remove the menu bar.
        /// </summary>
        /// <param name="menuItems"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Emit("browserWindowSetMenu", Id, JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Off("windowMenuItemClicked");
            BridgeConnector.On<string>("windowMenuItemClicked", (id) => 
            {
                MenuItem menuItem = _items.GetMenuItem(id);
                menuItem?.Click();
            });
        }

        /// <summary>
        /// Remove the window's menu bar.
        /// </summary>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public void RemoveMenu()
        {
            BridgeConnector.Emit("browserWindowRemoveMenu", Id);
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
        public void SetProgressBar(double progress)
        {
            BridgeConnector.Emit("browserWindowSetProgressBar", Id, progress);
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
        [SupportedOSPlatform("windows")]
        public void SetProgressBar(double progress, ProgressBarOptions progressBarOptions)
        {
            BridgeConnector.Emit("browserWindowSetProgressBar", Id, progress, progressBarOptions);
        }

        /// <summary>
        /// Sets whether the window should have a shadow. On Windows and Linux does nothing.
        /// </summary>
        /// <param name="hasShadow"></param>

        [SupportedOSPlatform("macos")]
        public void SetHasShadow(bool hasShadow)
        {
            BridgeConnector.Emit("browserWindowSetHasShadow", Id, hasShadow);
        }

        /// <summary>
        /// Whether the window has a shadow.
        /// 
        /// On Windows and Linux always returns true.
        /// </summary>
        /// <returns></returns>
        public Task<bool> HasShadowAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowHasShadow", "browserWindow-hasShadow-completed" + Id, Id);
        }

        /// <summary>
        /// Gets the thumbar buttons.
        /// </summary>
        /// <value>
        /// The thumbar buttons.
        /// </value>
        public IReadOnlyCollection<ThumbarButton> ThumbarButtons { get { return _thumbarButtons.AsReadOnly(); } }

        private readonly List<ThumbarButton> _thumbarButtons = new();

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
        [SupportedOSPlatform("windows")]
        public Task<bool> SetThumbarButtonsAsync(ThumbarButton[] thumbarButtons)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            BridgeConnector.On<bool>("browserWindowSetThumbarButtons-completed" + Id, (success) => 
            {
                BridgeConnector.Off("browserWindowSetThumbarButtons-completed" + Id);

                taskCompletionSource.SetResult(success);
            });

            thumbarButtons.AddThumbarButtonsId();
            BridgeConnector.Emit("browserWindowSetThumbarButtons", Id, JArray.FromObject(thumbarButtons, _jsonSerializer));
            _thumbarButtons.Clear();
            _thumbarButtons.AddRange(thumbarButtons);

            BridgeConnector.Off("thumbarButtonClicked");
            BridgeConnector.On<string>("thumbarButtonClicked", (id) => 
            {
                ThumbarButton thumbarButton = _thumbarButtons.GetThumbarButton(id);
                thumbarButton?.Click();
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the region of the window to show as the thumbnail image displayed when hovering over
        /// the window in the taskbar. You can reset the thumbnail to be the entire window by specifying
        /// an empty region: {x: 0, y: 0, width: 0, height: 0}.
        /// </summary>
        /// <param name="rectangle"></param>
        [SupportedOSPlatform("windows")]
        public void SetThumbnailClip(Rectangle rectangle)
        {
            BridgeConnector.Emit("browserWindowSetThumbnailClip", Id, rectangle);
        }

        /// <summary>
        /// Sets the toolTip that is displayed when hovering over the window thumbnail in the taskbar.
        /// </summary>
        /// <param name="tooltip"></param>
        [SupportedOSPlatform("windows")]
        public void SetThumbnailToolTip(string tooltip)
        {
            BridgeConnector.Emit("browserWindowSetThumbnailToolTip", Id, tooltip);
        }

        /// <summary>
        /// Sets the properties for the window’s taskbar button.
        /// 
        /// Note: relaunchCommand and relaunchDisplayName must always be set together. 
        /// If one of those properties is not set, then neither will be used.
        /// </summary>
        /// <param name="options"></param>
        [SupportedOSPlatform("windows")]
        public void SetAppDetails(AppDetailsOptions options)
        {
            BridgeConnector.Emit("browserWindowSetAppDetails", Id, options);
        }

        /// <summary>
        ///On a Window with Window Controls Overlay already enabled, this method updates
        /// the style of the title bar overlay. It should not be called unless you enabled WCO
        /// when creating the window.
        /// </summary>
        /// <param name="options"></param>
        [SupportedOSPlatform("win")]
        [SupportedOSPlatform("macos")]
        public void SetTitleBarOverlay(TitleBarOverlayConfig options)
        {
            BridgeConnector.Emit("browserWindowSetTitleBarOverlay", Id, options);
        }

        /// <summary>
        /// Same as webContents.showDefinitionForSelection().
        /// </summary>
        [SupportedOSPlatform("macos")]
        public void ShowDefinitionForSelection()
        {
            BridgeConnector.Emit("browserWindowShowDefinitionForSelection", Id);
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
            BridgeConnector.Emit("browserWindowSetAutoHideMenuBar", Id, hide);
        }

        /// <summary>
        /// Whether menu bar automatically hides itself.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMenuBarAutoHideAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMenuBarAutoHide", "browserWindow-isMenuBarAutoHide-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the menu bar should be visible. If the menu bar is auto-hide,
        /// users can still bring up the menu bar by pressing the single Alt key.
        /// </summary>
        /// <param name="visible"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public void SetMenuBarVisibility(bool visible)
        {
            BridgeConnector.Emit("browserWindowSetMenuBarVisibility", Id, visible);
        }

        /// <summary>
        /// Whether the menu bar is visible.
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public Task<bool> IsMenuBarVisibleAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsMenuBarVisible", "browserWindow-isMenuBarVisible-completed" + Id, Id);
        }

        /// <summary>
        /// Sets whether the window should be visible on all workspaces.
        /// 
        /// Note: This API does nothing on Windows.
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisibleOnAllWorkspaces(bool visible)
        {
            BridgeConnector.Emit("browserWindowSetVisibleOnAllWorkspaces", Id, visible);
        }

        /// <summary>
        /// Whether the window is visible on all workspaces.
        /// 
        /// Note: This API always returns false on Windows.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsVisibleOnAllWorkspacesAsync()
        {
            return BridgeConnector.OnResult<bool>("browserWindowIsVisibleOnAllWorkspaces", "browserWindow-isVisibleOnAllWorkspaces-completed" + Id, Id);
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
            BridgeConnector.Emit("browserWindowSetIgnoreMouseEvents", Id, ignore);
        }

        /// <summary>
        /// Prevents the window contents from being captured by other apps.
        /// 
        /// On macOS it sets the NSWindow’s sharingType to NSWindowSharingNone. 
        /// On Windows it calls SetWindowDisplayAffinity with WDA_MONITOR.
        /// </summary>
        /// <param name="enable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetContentProtection(bool enable)
        {
            BridgeConnector.Emit("browserWindowSetContentProtection", Id, enable);
        }

        /// <summary>
        /// Changes whether the window can be focused.
        /// </summary>
        /// <param name="focusable"></param>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("macos")]
        public void SetFocusable(bool focusable)
        {
            BridgeConnector.Emit("browserWindowSetFocusable", Id, focusable);
        }

        /// <summary>
        /// Sets parent as current window’s parent window, 
        /// passing null will turn current window into a top-level window.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParentWindow(BrowserWindow parent)
        {
            BridgeConnector.Emit("browserWindowSetParentWindow", Id, parent);
        }

        /// <summary>
        /// The parent window.
        /// </summary>
        /// <returns></returns>
        public async Task<BrowserWindow> GetParentWindowAsync()
        {
            var windowID = await BridgeConnector.OnResult<int>("browserWindowGetParentWindow", "browserWindow-getParentWindow-completed" + Id, Id);
            return Electron.WindowManager.BrowserWindows.Where(w => w.Id == windowID).Single();
        }

        /// <summary>
        /// All child windows.
        /// </summary>
        /// <returns></returns>
        public async Task<List<BrowserWindow>> GetChildWindowsAsync()
        {
            var windowIDs = new HashSet<int>(await BridgeConnector.OnResult<int[]>("browserWindowGetChildWindows", "browserWindow-getChildWindows-completed" + Id, Id));
            return Electron.WindowManager.BrowserWindows.Where(w => windowIDs.Contains(w.Id)).ToList();
        }

        /// <summary>
        /// Controls whether to hide cursor when typing.
        /// </summary>
        /// <param name="autoHide"></param>
        [SupportedOSPlatform("macos")]
        public void SetAutoHideCursor(bool autoHide)
        {
            BridgeConnector.Emit("browserWindowSetAutoHideCursor", Id, autoHide);
        }

        /// <summary>
        /// Adds a vibrancy effect to the browser window. 
        /// Passing null or an empty string will remove the vibrancy effect on the window.
        /// </summary>
        /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
        /// menu, popover, sidebar, medium-light or ultra-dark. 
        /// See the macOS documentation for more details.</param>
        [SupportedOSPlatform("macos")]
        public void SetVibrancy(Vibrancy type)
        {
            BridgeConnector.Emit("browserWindowSetVibrancy", Id, type.GetDescription());
        }

        /// <summary>
        /// Adds a vibrancy effect to the browser window. 
        /// Passing null or an empty string will remove the vibrancy effect on the window.
        /// </summary>
        /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
        /// menu, popover, sidebar, medium-light or ultra-dark. 
        /// See the macOS documentation for more details.</param>
        [SupportedOSPlatform("macos")]
        public void ExcludeFromShownWindowsMenu()
        {
            BridgeConnector.Emit("browserWindowSetExcludedFromShownWindowsMenu", Id);
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
        public void SetBrowserView(BrowserView browserView)
        {
            BridgeConnector.Emit("browserWindow-setBrowserView", Id, browserView.Id);
        }

        private static readonly JsonSerializer _jsonSerializer = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
