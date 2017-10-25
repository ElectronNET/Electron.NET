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
                    BridgeConnector.Socket.On("browserWindow-ready-to-show" + Id, () =>
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

                if (_readyToShow == null)
                    BridgeConnector.Socket.Off("browserWindow-ready-to-show" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-page-title-updated" + Id, (title) =>
                    {
                        _pageTitleUpdated(title.ToString());
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-page-title-updated", Id);
                }
                _pageTitleUpdated += value;
            }
            remove
            {
                _pageTitleUpdated -= value;

                if (_pageTitleUpdated == null)
                    BridgeConnector.Socket.Off("browserWindow-page-title-updated" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-close" + Id, () =>
                    {
                        _close();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-close", Id);
                }
                _close += value;
            }
            remove
            {
                _close -= value;

                if (_close == null)
                    BridgeConnector.Socket.Off("browserWindow-close" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-closed" + Id, () =>
                    {
                        _closed();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-closed", Id);
                }
                _closed += value;
            }
            remove
            {
                _closed -= value;

                if (_closed == null)
                    BridgeConnector.Socket.Off("browserWindow-closed" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-session-end" + Id, () =>
                    {
                        _sessionEnd();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-session-end", Id);
                }
                _sessionEnd += value;
            }
            remove
            {
                _sessionEnd -= value;

                if (_sessionEnd == null)
                    BridgeConnector.Socket.Off("browserWindow-session-end" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-unresponsive" + Id, () =>
                    {
                        _unresponsive();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-unresponsive", Id);
                }
                _unresponsive += value;
            }
            remove
            {
                _unresponsive -= value;

                if (_unresponsive == null)
                    BridgeConnector.Socket.Off("browserWindow-unresponsive" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-responsive" + Id, () =>
                    {
                        _responsive();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-responsive", Id);
                }
                _responsive += value;
            }
            remove
            {
                _responsive -= value;

                if (_responsive == null)
                    BridgeConnector.Socket.Off("browserWindow-responsive" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-blur" + Id, () =>
                    {
                        _blur();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-blur", Id);
                }
                _blur += value;
            }
            remove
            {
                _blur -= value;

                if (_blur == null)
                    BridgeConnector.Socket.Off("browserWindow-blur" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-focus" + Id, () =>
                    {
                        _focus();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-focus", Id);
                }
                _focus += value;
            }
            remove
            {
                _focus -= value;

                if (_focus == null)
                    BridgeConnector.Socket.Off("browserWindow-focus" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-show" + Id, () =>
                    {
                        _show();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-show", Id);
                }
                _show += value;
            }
            remove
            {
                _show -= value;

                if (_show == null)
                    BridgeConnector.Socket.Off("browserWindow-show" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-hide" + Id, () =>
                    {
                        _hide();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-hide", Id);
                }
                _hide += value;
            }
            remove
            {
                _hide -= value;

                if (_hide == null)
                    BridgeConnector.Socket.Off("browserWindow-hide" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-maximize" + Id, () =>
                    {
                        _maximize();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-maximize", Id);
                }
                _maximize += value;
            }
            remove
            {
                _maximize -= value;

                if (_maximize == null)
                    BridgeConnector.Socket.Off("browserWindow-maximize" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-unmaximize" + Id, () =>
                    {
                        _unmaximize();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-unmaximize", Id);
                }
                _unmaximize += value;
            }
            remove
            {
                _unmaximize -= value;

                if (_unmaximize == null)
                    BridgeConnector.Socket.Off("browserWindow-unmaximize" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-minimize" + Id, () =>
                    {
                        _minimize();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-minimize", Id);
                }
                _minimize += value;
            }
            remove
            {
                _minimize -= value;

                if (_minimize == null)
                    BridgeConnector.Socket.Off("browserWindow-minimize" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-restore" + Id, () =>
                    {
                        _restore();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-restore", Id);
                }
                _restore += value;
            }
            remove
            {
                _restore -= value;

                if (_restore == null)
                    BridgeConnector.Socket.Off("browserWindow-restore" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-resize" + Id, () =>
                    {
                        _resize();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-resize", Id);
                }
                _resize += value;
            }
            remove
            {
                _resize -= value;

                if (_resize == null)
                    BridgeConnector.Socket.Off("browserWindow-resize" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-move" + Id, () =>
                    {
                        _move();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-move", Id);
                }
                _move += value;
            }
            remove
            {
                _move -= value;

                if (_move == null)
                    BridgeConnector.Socket.Off("browserWindow-move" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-moved" + Id, () =>
                    {
                        _moved();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-moved", Id);
                }
                _moved += value;
            }
            remove
            {
                _moved -= value;

                if (_moved == null)
                    BridgeConnector.Socket.Off("browserWindow-moved" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-enter-full-screen" + Id, () =>
                    {
                        _enterFullScreen();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-enter-full-screen", Id);
                }
                _enterFullScreen += value;
            }
            remove
            {
                _enterFullScreen -= value;

                if (_enterFullScreen == null)
                    BridgeConnector.Socket.Off("browserWindow-enter-full-screen" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-leave-full-screen" + Id, () =>
                    {
                        _leaveFullScreen();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-leave-full-screen", Id);
                }
                _leaveFullScreen += value;
            }
            remove
            {
                _leaveFullScreen -= value;

                if (_leaveFullScreen == null)
                    BridgeConnector.Socket.Off("browserWindow-leave-full-screen" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-enter-html-full-screen" + Id, () =>
                    {
                        _enterHtmlFullScreen();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-enter-html-full-screen", Id);
                }
                _enterHtmlFullScreen += value;
            }
            remove
            {
                _enterHtmlFullScreen -= value;

                if (_enterHtmlFullScreen == null)
                    BridgeConnector.Socket.Off("browserWindow-enter-html-full-screen" + Id);
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
                    BridgeConnector.Socket.On("browserWindow-leave-html-full-screen" + Id, () =>
                    {
                        _leaveHtmlFullScreen();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-leave-html-full-screen", Id);
                }
                _leaveHtmlFullScreen += value;
            }
            remove
            {
                _leaveHtmlFullScreen -= value;

                if (_leaveHtmlFullScreen == null)
                    BridgeConnector.Socket.Off("browserWindow-leave-html-full-screen" + Id);
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
        public event Action<string> OnAppCommand
        {
            add
            {
                if (_appCommand == null)
                {
                    BridgeConnector.Socket.On("browserWindow-app-command" + Id, (command) =>
                    {
                        _appCommand(command.ToString());
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-app-command", Id);
                }
                _appCommand += value;
            }
            remove
            {
                _appCommand -= value;

                if (_appCommand == null)
                    BridgeConnector.Socket.Off("browserWindow-app-command" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-scroll-touch-begin" + Id, () =>
                    {
                        _scrollTouchBegin();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-scroll-touch-begin", Id);
                }
                _scrollTouchBegin += value;
            }
            remove
            {
                _scrollTouchBegin -= value;

                if (_scrollTouchBegin == null)
                    BridgeConnector.Socket.Off("browserWindow-scroll-touch-begin" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-scroll-touch-end" + Id, () =>
                    {
                        _scrollTouchEnd();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-scroll-touch-end", Id);
                }
                _scrollTouchEnd += value;
            }
            remove
            {
                _scrollTouchEnd -= value;

                if (_scrollTouchEnd == null)
                    BridgeConnector.Socket.Off("browserWindow-scroll-touch-end" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-scroll-touch-edge" + Id, () =>
                    {
                        _scrollTouchEdge();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-scroll-touch-edge", Id);
                }
                _scrollTouchEdge += value;
            }
            remove
            {
                _scrollTouchEdge -= value;

                if (_scrollTouchEdge == null)
                    BridgeConnector.Socket.Off("browserWindow-scroll-touch-edge" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-swipe" + Id, (direction) =>
                    {
                        _swipe(direction.ToString());
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-swipe", Id);
                }
                _swipe += value;
            }
            remove
            {
                _swipe -= value;

                if (_swipe == null)
                    BridgeConnector.Socket.Off("browserWindow-swipe" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-sheet-begin" + Id, () =>
                    {
                        _sheetBegin();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-sheet-begin", Id);
                }
                _sheetBegin += value;
            }
            remove
            {
                _sheetBegin -= value;

                if (_sheetBegin == null)
                    BridgeConnector.Socket.Off("browserWindow-sheet-begin" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-sheet-end" + Id, () =>
                    {
                        _sheetEnd();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-sheet-end", Id);
                }
                _sheetEnd += value;
            }
            remove
            {
                _sheetEnd -= value;

                if (_sheetEnd == null)
                    BridgeConnector.Socket.Off("browserWindow-sheet-end" + Id);
            }
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
                    BridgeConnector.Socket.On("browserWindow-new-window-for-tab" + Id, () =>
                    {
                        _newWindowForTab();
                    });

                    BridgeConnector.Socket.Emit("register-browserWindow-new-window-for-tab", Id);
                }
                _newWindowForTab += value;
            }
            remove
            {
                _newWindowForTab -= value;

                if (_newWindowForTab == null)
                    BridgeConnector.Socket.Off("browserWindow-new-window-for-tab" + Id);
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
            BridgeConnector.Socket.Emit("browserWindowDestroy", Id);
        }

        /// <summary>
        /// Try to close the window. This has the same effect as a user manually 
        /// clicking the close button of the window. The web page may cancel the close though. 
        /// </summary>
        public void Close()
        {
            BridgeConnector.Socket.Emit("browserWindowClose", Id);
        }

        /// <summary>
        /// Focuses on the window.
        /// </summary>
        public void Focus()
        {
            BridgeConnector.Socket.Emit("browserWindowFocus", Id);
        }

        /// <summary>
        /// Removes focus from the window.
        /// </summary>
        public void Blur()
        {
            BridgeConnector.Socket.Emit("browserWindowBlur", Id);
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

            BridgeConnector.Socket.Emit("browserWindowIsFocused", Id);

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

            BridgeConnector.Socket.Emit("browserWindowIsDestroyed", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Shows and gives focus to the window.
        /// </summary>
        public void Show()
        {
            BridgeConnector.Socket.Emit("browserWindowShow", Id);
        }

        /// <summary>
        /// Shows the window but doesn’t focus on it.
        /// </summary>
        public void ShowInactive()
        {
            BridgeConnector.Socket.Emit("browserWindowShowInactive", Id);
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public void Hide()
        {
            BridgeConnector.Socket.Emit("browserWindowHide", Id);
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

            BridgeConnector.Socket.Emit("browserWindowIsVisible", Id);

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

            BridgeConnector.Socket.Emit("browserWindowIsModal", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Maximizes the window. This will also show (but not focus) the window if it isn’t being displayed already.
        /// </summary>
        public void Maximize()
        {
            BridgeConnector.Socket.Emit("browserWindowMaximize", Id);
        }

        /// <summary>
        /// Unmaximizes the window.
        /// </summary>
        public void Unmaximize()
        {
            BridgeConnector.Socket.Emit("browserWindowUnmaximize", Id);
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

            BridgeConnector.Socket.Emit("browserWindowIsMaximized", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Minimizes the window. On some platforms the minimized window will be shown in the Dock.
        /// </summary>
        public void Minimize()
        {
            BridgeConnector.Socket.Emit("browserWindowMinimize", Id);
        }

        /// <summary>
        /// Restores the window from minimized state to its previous state.
        /// </summary>
        public void Restore()
        {
            BridgeConnector.Socket.Emit("browserWindowRestore", Id);
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

            BridgeConnector.Socket.Emit("browserWindowIsMinimized", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window should be in fullscreen mode.
        /// </summary>
        public void SetFullScreen(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindowSetFullScreen", Id, flag);
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

            BridgeConnector.Socket.Emit("browserWindowIsFullScreen", Id);

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
            BridgeConnector.Socket.Emit("browserWindowSetAspectRatio", Id, aspectRatio, JObject.FromObject(extraSize, _jsonSerializer));
        }

        /// <summary>
        /// Uses Quick Look to preview a file at a given path.
        /// </summary>
        /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
        /// Quick Look uses the file name and file extension on the path to determine the content type of the 
        /// file to open.</param>
        public void PreviewFile(string path)
        {
            BridgeConnector.Socket.Emit("browserWindowPreviewFile", Id, path);
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
            BridgeConnector.Socket.Emit("browserWindowPreviewFile", Id, path, displayname);
        }

        /// <summary>
        /// Closes the currently open Quick Look panel.
        /// </summary>
        public void CloseFilePreview()
        {
            BridgeConnector.Socket.Emit("browserWindowCloseFilePreview", Id);
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        public void SetBounds(Rectangle bounds)
        {
            BridgeConnector.Socket.Emit("browserWindowSetBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window to the supplied bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public void SetBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindowSetBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        /// <summary>
        /// Gets the bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<Rectangle> GetBoundsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Rectangle>();

            BridgeConnector.Socket.On("browserWindow-getBounds-completed", (getBounds) => {
                BridgeConnector.Socket.Off("browserWindow-getBounds-completed");

                taskCompletionSource.SetResult(((JObject)getBounds).ToObject<Rectangle>());
            });

            BridgeConnector.Socket.Emit("browserWindowGetBounds", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        public void SetContentBounds(Rectangle bounds)
        {
            BridgeConnector.Socket.Emit("browserWindowSetContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer));
        }

        /// <summary>
        /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="animate"></param>
        public void SetContentBounds(Rectangle bounds, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindowSetContentBounds", Id, JObject.FromObject(bounds, _jsonSerializer), animate);
        }

        /// <summary>
        /// Gets the content bounds asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<Rectangle> GetContentBoundsAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Rectangle>();

            BridgeConnector.Socket.On("browserWindow-getContentBounds-completed", (getContentBounds) => {
                BridgeConnector.Socket.Off("browserWindow-getContentBounds-completed");

                taskCompletionSource.SetResult(((JObject)getContentBounds).ToObject<Rectangle>());
            });

            BridgeConnector.Socket.Emit("browserWindowGetContentBounds", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindowSetSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetSize(int width, int height, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindowSetSize", Id, width, height, animate);
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

            BridgeConnector.Socket.Emit("browserWindowGetSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetContentSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindowSetContentSize", Id, width, height);
        }

        /// <summary>
        /// Resizes the window’s client area (e.g. the web page) to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="animate"></param>
        public void SetContentSize(int width, int height, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindowSetContentSize", Id, width, height, animate);
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

            BridgeConnector.Socket.Emit("browserWindowGetContentSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the minimum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMinimumSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMinimumSize", Id, width, height);
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

            BridgeConnector.Socket.Emit("browserWindowGetMinimumSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the maximum size of window to width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMaximumSize(int width, int height)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMaximumSize", Id, width, height);
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

            BridgeConnector.Socket.Emit("browserWindowGetMaximumSize", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually resized by user.
        /// </summary>
        /// <param name="resizable"></param>
        public void SetResizable(bool resizable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetResizable", Id, resizable);
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

            BridgeConnector.Socket.Emit("browserWindowIsResizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be moved by user. On Linux does nothing.
        /// </summary>
        /// <param name="movable"></param>
        public void SetMovable(bool movable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMovable", Id, movable);
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

            BridgeConnector.Socket.Emit("browserWindowIsMovable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually minimized by user. On Linux does nothing.
        /// </summary>
        /// <param name="minimizable"></param>
        public void SetMinimizable(bool minimizable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMinimizable", Id, minimizable);
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

            BridgeConnector.Socket.Emit("browserWindowIsMinimizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually maximized by user. On Linux does nothing.
        /// </summary>
        /// <param name="maximizable"></param>
        public void SetMaximizable(bool maximizable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMaximizable", Id, maximizable);
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

            BridgeConnector.Socket.Emit("browserWindowIsMaximizable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
        /// </summary>
        /// <param name="fullscreenable"></param>
        public void SetFullScreenable(bool fullscreenable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetFullScreenable", Id, fullscreenable);
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

            BridgeConnector.Socket.Emit("browserWindowIsFullScreenable", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the window can be manually closed by user. On Linux does nothing.
        /// </summary>
        /// <param name="closable"></param>
        public void SetClosable(bool closable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetClosable", Id, closable);
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

            BridgeConnector.Socket.Emit("browserWindowIsClosable", Id);

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
            BridgeConnector.Socket.Emit("browserWindowSetAlwaysOnTop", Id, flag);
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
        public void SetAlwaysOnTop(bool flag, OnTopLevel level)
        {
            BridgeConnector.Socket.Emit("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription());
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
        public void SetAlwaysOnTop(bool flag, OnTopLevel level, int relativeLevel)
        {
            BridgeConnector.Socket.Emit("browserWindowSetAlwaysOnTop", Id, flag, level.GetDescription(), relativeLevel);
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

            BridgeConnector.Socket.Emit("browserWindowIsAlwaysOnTop", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Moves window to the center of the screen.
        /// </summary>
        public void Center()
        {
            BridgeConnector.Socket.Emit("browserWindowCenter", Id);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(int x, int y)
        {
            BridgeConnector.Socket.Emit("browserWindowSetPosition", Id, x, y);
        }

        /// <summary>
        /// Moves window to x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="animate"></param>
        public void SetPosition(int x, int y, bool animate)
        {
            BridgeConnector.Socket.Emit("browserWindowSetPosition", Id, x, y, animate);
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

            BridgeConnector.Socket.Emit("browserWindowGetPosition", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Changes the title of native window to title.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            BridgeConnector.Socket.Emit("browserWindowSetTitle", Id, title);
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

            BridgeConnector.Socket.Emit("browserWindowGetTitle", Id);

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
            BridgeConnector.Socket.Emit("browserWindowSetSheetOffset", Id, offsetY);
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
            BridgeConnector.Socket.Emit("browserWindowSetSheetOffset", Id, offsetY, offsetX);
        }

        /// <summary>
        /// Starts or stops flashing the window to attract user’s attention.
        /// </summary>
        /// <param name="flag"></param>
        public void FlashFrame(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindowFlashFrame", Id, flag);
        }

        /// <summary>
        /// Makes the window not show in the taskbar.
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipTaskbar(bool skip)
        {
            BridgeConnector.Socket.Emit("browserWindowSetSkipTaskbar", Id, skip);
        }

        /// <summary>
        /// Enters or leaves the kiosk mode.
        /// </summary>
        /// <param name="flag"></param>
        public void SetKiosk(bool flag)
        {
            BridgeConnector.Socket.Emit("browserWindowSetKiosk", Id, flag);
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

            BridgeConnector.Socket.Emit("browserWindowIsKiosk", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets the pathname of the file the window represents, 
        /// and the icon of the file will show in window’s title bar.
        /// </summary>
        /// <param name="filename"></param>
        public void SetRepresentedFilename(string filename)
        {
            BridgeConnector.Socket.Emit("browserWindowSetRepresentedFilename", Id, filename);
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

            BridgeConnector.Socket.Emit("browserWindowGetRepresentedFilename", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Specifies whether the window’s document has been edited, 
        /// and the icon in title bar will become gray when set to true.
        /// </summary>
        /// <param name="edited"></param>
        public void SetDocumentEdited(bool edited)
        {
            BridgeConnector.Socket.Emit("browserWindowSetDocumentEdited", Id, edited);
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

            BridgeConnector.Socket.Emit("browserWindowIsDocumentEdited", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Focuses the on web view.
        /// </summary>
        public void FocusOnWebView()
        {
            BridgeConnector.Socket.Emit("browserWindowFocusOnWebView", Id);
        }

        /// <summary>
        /// Blurs the web view.
        /// </summary>
        public void BlurWebView()
        {
            BridgeConnector.Socket.Emit("browserWindowBlurWebView", Id);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        public void LoadURL(string url)
        {
            BridgeConnector.Socket.Emit("browserWindowLoadURL", Id, url);
        }

        /// <summary>
        /// The url can be a remote address (e.g. http://) or 
        /// a path to a local HTML file using the file:// protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="options"></param>
        public void LoadURL(string url, LoadURLOptions options)
        {
            BridgeConnector.Socket.Emit("browserWindowLoadURL", Id, url, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.reload.
        /// </summary>
        public void Reload()
        {
            BridgeConnector.Socket.Emit("browserWindowReload", Id);
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public IReadOnlyCollection<MenuItem> MenuItems { get { return _items.AsReadOnly(); } }
        private List<MenuItem> _items = new List<MenuItem>();

        /// <summary>
        /// Sets the menu as the window’s menu bar, 
        /// setting it to null will remove the menu bar.
        /// </summary>
        /// <param name="menuItems"></param>
        public void SetMenu(MenuItem[] menuItems)
        {
            menuItems.AddMenuItemsId();
            BridgeConnector.Socket.Emit("browserWindowSetMenu", JArray.FromObject(menuItems, _jsonSerializer));
            _items.AddRange(menuItems);

            BridgeConnector.Socket.Off("windowMenuItemClicked");
            BridgeConnector.Socket.On("windowMenuItemClicked", (id) => {
                MenuItem menuItem = _items.GetMenuItem(id.ToString());
                menuItem?.Click();
            });
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
        public void SetProgressBar(int progress)
        {
            BridgeConnector.Socket.Emit("browserWindowSetProgressBar", Id, progress);
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
        public void SetProgressBar(int progress, ProgressBarOptions progressBarOptions)
        {
            BridgeConnector.Socket.Emit("browserWindowSetProgressBar", Id, progress, JObject.FromObject(progressBarOptions, _jsonSerializer));
        }

        /// <summary>
        /// Sets whether the window should have a shadow. On Windows and Linux does nothing.
        /// </summary>
        /// <param name="hasShadow"></param>
        public void SetHasShadow(bool hasShadow)
        {
            BridgeConnector.Socket.Emit("browserWindowSetHasShadow", Id, hasShadow);
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

            BridgeConnector.Socket.Emit("browserWindowHasShadow", Id);

            return taskCompletionSource.Task;
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
        public Task<bool> SetThumbarButtonsAsync(ThumbarButton[] thumbarButtons)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            BridgeConnector.Socket.On("browserWindowSetThumbarButtons-completed", (success) => {
                BridgeConnector.Socket.Off("browserWindowSetThumbarButtons-completed");

                taskCompletionSource.SetResult((bool)success);
            });

            thumbarButtons.AddThumbarButtonsId();
            BridgeConnector.Socket.Emit("browserWindowSetThumbarButtons", Id, JArray.FromObject(thumbarButtons, _jsonSerializer));
            _thumbarButtons.Clear();
            _thumbarButtons.AddRange(thumbarButtons);

            BridgeConnector.Socket.Off("thumbarButtonClicked");
            BridgeConnector.Socket.On("thumbarButtonClicked", (id) => {
                ThumbarButton thumbarButton = _thumbarButtons.GetThumbarButton(id.ToString());
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
        public void SetThumbnailClip(Rectangle rectangle)
        {
            BridgeConnector.Socket.Emit("browserWindowSetThumbnailClip", Id, rectangle);
        }

        /// <summary>
        /// Sets the toolTip that is displayed when hovering over the window thumbnail in the taskbar.
        /// </summary>
        /// <param name="tooltip"></param>
        public void SetThumbnailToolTip(string tooltip)
        {
            BridgeConnector.Socket.Emit("browserWindowSetThumbnailToolTip", Id, tooltip);
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
            BridgeConnector.Socket.Emit("browserWindowSetAppDetails", Id, JObject.FromObject(options, _jsonSerializer));
        }

        /// <summary>
        /// Same as webContents.showDefinitionForSelection().
        /// </summary>
        public void ShowDefinitionForSelection()
        {
            BridgeConnector.Socket.Emit("browserWindowShowDefinitionForSelection", Id);
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
            BridgeConnector.Socket.Emit("browserWindowSetAutoHideMenuBar", Id, hide);
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

            BridgeConnector.Socket.Emit("browserWindowIsMenuBarAutoHide", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Sets whether the menu bar should be visible. If the menu bar is auto-hide,
        /// users can still bring up the menu bar by pressing the single Alt key.
        /// </summary>
        /// <param name="visible"></param>
        public void SetMenuBarVisibility(bool visible)
        {
            BridgeConnector.Socket.Emit("browserWindowSetMenuBarVisibility", Id, visible);
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

            BridgeConnector.Socket.Emit("browserWindowIsMenuBarVisible", Id);

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
            BridgeConnector.Socket.Emit("browserWindowSetVisibleOnAllWorkspaces", Id, visible);
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

            BridgeConnector.Socket.Emit("browserWindowIsVisibleOnAllWorkspaces", Id);

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
            BridgeConnector.Socket.Emit("browserWindowSetIgnoreMouseEvents", Id, ignore);
        }

        /// <summary>
        /// Prevents the window contents from being captured by other apps.
        /// 
        /// On macOS it sets the NSWindow’s sharingType to NSWindowSharingNone. 
        /// On Windows it calls SetWindowDisplayAffinity with WDA_MONITOR.
        /// </summary>
        /// <param name="enable"></param>
        public void SetContentProtection(bool enable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetContentProtection", Id, enable);
        }

        /// <summary>
        /// Changes whether the window can be focused.
        /// </summary>
        /// <param name="focusable"></param>
        public void SetFocusable(bool focusable)
        {
            BridgeConnector.Socket.Emit("browserWindowSetFocusable", Id, focusable);
        }

        /// <summary>
        /// Sets parent as current window’s parent window, 
        /// passing null will turn current window into a top-level window.
        /// </summary>
        /// <param name="parent"></param>
        public void SetParentWindow(BrowserWindow parent)
        {
            BridgeConnector.Socket.Emit("browserWindowSetParentWindow", Id, JObject.FromObject(parent, _jsonSerializer));
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

            BridgeConnector.Socket.Emit("browserWindowGetParentWindow", Id);

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

            BridgeConnector.Socket.Emit("browserWindowGetChildWindows", Id);

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Controls whether to hide cursor when typing.
        /// </summary>
        /// <param name="autoHide"></param>
        public void SetAutoHideCursor(bool autoHide)
        {
            BridgeConnector.Socket.Emit("browserWindowSetAutoHideCursor", Id, autoHide);
        }

        /// <summary>
        /// Adds a vibrancy effect to the browser window. 
        /// Passing null or an empty string will remove the vibrancy effect on the window.
        /// </summary>
        /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
        /// menu, popover, sidebar, medium-light or ultra-dark. 
        /// See the macOS documentation for more details.</param>
        public void SetVibrancy(Vibrancy type)
        {
            BridgeConnector.Socket.Emit("browserWindowSetVibrancy", Id, type.GetDescription());
        }

        /// <summary>
        /// Render and control web pages.
        /// </summary>
        public WebContents WebContents { get; internal set; }

        private JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
