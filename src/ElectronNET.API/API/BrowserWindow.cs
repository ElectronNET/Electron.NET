using ElectronNET.API.Entities;
using ElectronNET.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace ElectronNET.API;

using ElectronNET.Common;
using System.Text.Json;

/// <summary>
/// Create and control browser windows.
/// </summary>
public class BrowserWindow : ApiBase
{
    protected override SocketEventNameTypes SocketEventNameType => SocketEventNameTypes.DashesLowerFirst;

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public override int Id { get; protected set; }

    /// <summary>
    /// Emitted when the web page has been rendered (while not being shown) and 
    /// window can be displayed without a visual flash.
    /// </summary>
    public event Action OnReadyToShow
    {
        add => ApiEventManager.AddEvent("browserWindow-ready-to-show", Id, _readyToShow, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-ready-to-show", Id, _readyToShow, value);
    }

    private event Action _readyToShow;

    /// <summary>
    /// Emitted when the document changed its title.
    /// </summary>
    public event Action<string> OnPageTitleUpdated
    {
        add => ApiEventManager.AddEvent("browserWindow-page-title-updated", Id, _pageTitleUpdated, value, (args) => args.ToString());
        remove => ApiEventManager.RemoveEvent("browserWindow-page-title-updated", Id, _pageTitleUpdated, value);
    }

    private event Action<string> _pageTitleUpdated;

    /// <summary>
    /// Emitted when the window is going to be closed.
    /// </summary>
    public event Action OnClose
    {
        add => ApiEventManager.AddEvent("browserWindow-close", Id, _close, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-close", Id, _close, value);
    }

    private event Action _close;

    /// <summary>
    /// Emitted when the window is closed. 
    /// After you have received this event you should remove the 
    /// reference to the window and avoid using it any more.
    /// </summary>
    public event Action OnClosed
    {
        add => ApiEventManager.AddEvent("browserWindow-closed", Id, _closed, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-closed", Id, _closed, value);
    }

    private event Action _closed;

    /// <summary>
    /// Emitted when window session is going to end due to force shutdown or machine restart or session log off.
    /// </summary>
    public event Action OnSessionEnd
    {
        add => ApiEventManager.AddEvent("browserWindow-session-end", Id, _sessionEnd, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-session-end", Id, _sessionEnd, value);
    }

    private event Action _sessionEnd;

    /// <summary>
    /// Emitted when the web page becomes unresponsive.
    /// </summary>
    public event Action OnUnresponsive
    {
        add => ApiEventManager.AddEvent("browserWindow-unresponsive", Id, _unresponsive, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-unresponsive", Id, _unresponsive, value);
    }

    private event Action _unresponsive;

    /// <summary>
    /// Emitted when the unresponsive web page becomes responsive again.
    /// </summary>
    public event Action OnResponsive
    {
        add => ApiEventManager.AddEvent("browserWindow-responsive", Id, _responsive, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-responsive", Id, _responsive, value);
    }

    private event Action _responsive;

    /// <summary>
    /// Emitted when the window loses focus.
    /// </summary>
    public event Action OnBlur
    {
        add => ApiEventManager.AddEvent("browserWindow-blur", Id, _blur, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-blur", Id, _blur, value);
    }

    private event Action _blur;

    /// <summary>
    /// Emitted when the window gains focus.
    /// </summary>
    public event Action OnFocus
    {
        add => ApiEventManager.AddEvent("browserWindow-focus", Id, _focus, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-focus", Id, _focus, value);
    }

    private event Action _focus;

    /// <summary>
    /// Emitted when the window is shown.
    /// </summary>
    public event Action OnShow
    {
        add => ApiEventManager.AddEvent("browserWindow-show", Id, _show, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-show", Id, _show, value);
    }

    private event Action _show;

    /// <summary>
    /// Emitted when the window is hidden.
    /// </summary>
    public event Action OnHide
    {
        add => ApiEventManager.AddEvent("browserWindow-hide", Id, _hide, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-hide", Id, _hide, value);
    }

    private event Action _hide;

    /// <summary>
    /// Emitted when window is maximized.
    /// </summary>
    public event Action OnMaximize
    {
        add => ApiEventManager.AddEvent("browserWindow-maximize", Id, _maximize, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-maximize", Id, _maximize, value);
    }

    private event Action _maximize;

    /// <summary>
    /// Emitted when the window exits from a maximized state.
    /// </summary>
    public event Action OnUnmaximize
    {
        add => ApiEventManager.AddEvent("browserWindow-unmaximize", Id, _unmaximize, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-unmaximize", Id, _unmaximize, value);
    }

    private event Action _unmaximize;

    /// <summary>
    /// Emitted when the window is minimized.
    /// </summary>
    public event Action OnMinimize
    {
        add => ApiEventManager.AddEvent("browserWindow-minimize", Id, _minimize, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-minimize", Id, _minimize, value);
    }

    private event Action _minimize;

    /// <summary>
    /// Emitted when the window is restored from a minimized state.
    /// </summary>
    public event Action OnRestore
    {
        add => ApiEventManager.AddEvent("browserWindow-restore", Id, _restore, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-restore", Id, _restore, value);
    }

    private event Action _restore;

    /// <summary>
    /// Emitted when the window is being resized.
    /// </summary>
    public event Action OnResize
    {
        add => ApiEventManager.AddEvent("browserWindow-resize", Id, _resize, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-resize", Id, _resize, value);
    }

    private event Action _resize;

    /// <summary>
    /// Emitted when the window is being moved to a new position.
    /// 
    /// Note: On macOS this event is just an alias of moved.
    /// </summary>
    public event Action OnMove
    {
        add => ApiEventManager.AddEvent("browserWindow-move", Id, _move, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-move", Id, _move, value);
    }

    private event Action _move;

    /// <summary>
    /// macOS: Emitted once when the window is moved to a new position.
    /// </summary>
    public event Action OnMoved
    {
        add => ApiEventManager.AddEvent("browserWindow-moved", Id, _moved, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-moved", Id, _moved, value);
    }

    private event Action _moved;

    /// <summary>
    /// Emitted when the window enters a full-screen state.
    /// </summary>
    public event Action OnEnterFullScreen
    {
        add => ApiEventManager.AddEvent("browserWindow-enter-full-screen", Id, _enterFullScreen, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-enter-full-screen", Id, _enterFullScreen, value);
    }

    private event Action _enterFullScreen;

    /// <summary>
    /// Emitted when the window leaves a full-screen state.
    /// </summary>
    public event Action OnLeaveFullScreen
    {
        add => ApiEventManager.AddEvent("browserWindow-leave-full-screen", Id, _leaveFullScreen, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-leave-full-screen", Id, _leaveFullScreen, value);
    }

    private event Action _leaveFullScreen;

    /// <summary>
    /// Emitted when the window enters a full-screen state triggered by HTML API.
    /// </summary>
    public event Action OnEnterHtmlFullScreen
    {
        add => ApiEventManager.AddEvent("browserWindow-enter-html-full-screen", Id, _enterHtmlFullScreen, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-enter-html-full-screen", Id, _enterHtmlFullScreen, value);
    }

    private event Action _enterHtmlFullScreen;

    /// <summary>
    /// Emitted when the window leaves a full-screen state triggered by HTML API.
    /// </summary>
    public event Action OnLeaveHtmlFullScreen
    {
        add => ApiEventManager.AddEvent("browserWindow-leave-html-full-screen", Id, _leaveHtmlFullScreen, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-leave-html-full-screen", Id, _leaveHtmlFullScreen, value);
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
        add => ApiEventManager.AddEvent("browserWindow-app-command", Id, _appCommand, value, (args) => args.ToString());
        remove => ApiEventManager.RemoveEvent("browserWindow-app-command", Id, _appCommand, value);
    }

    private event Action<string> _appCommand;

    /// <summary>
    /// Emitted on 3-finger swipe. Possible directions are up, right, down, left.
    /// </summary>
    public event Action<string> OnSwipe
    {
        add => ApiEventManager.AddEvent("browserWindow-swipe", Id, _swipe, value, (args) => args.ToString());
        remove => ApiEventManager.RemoveEvent("browserWindow-swipe", Id, _swipe, value);
    }

    private event Action<string> _swipe;

    /// <summary>
    /// Emitted when the window opens a sheet.
    /// </summary>
    public event Action OnSheetBegin
    {
        add => ApiEventManager.AddEvent("browserWindow-sheet-begin", Id, _sheetBegin, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-sheet-begin", Id, _sheetBegin, value);
    }

    private event Action _sheetBegin;

    /// <summary>
    /// Emitted when the window has closed a sheet.
    /// </summary>
    public event Action OnSheetEnd
    {
        add => ApiEventManager.AddEvent("browserWindow-sheet-end", Id, _sheetEnd, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-sheet-end", Id, _sheetEnd, value);
    }

    private event Action _sheetEnd;

    /// <summary>
    /// Emitted when the native new tab button is clicked.
    /// </summary>
    public event Action OnNewWindowForTab
    {
        add => ApiEventManager.AddEvent("browserWindow-new-window-for-tab", Id, _newWindowForTab, value);
        remove => ApiEventManager.RemoveEvent("browserWindow-new-window-for-tab", Id, _newWindowForTab, value);
    }

    private event Action _newWindowForTab;

    internal BrowserWindow(int id)
    {
        Id = id;
        WebContents = new WebContents(id);
    }

    /// <summary>
    /// Force closing the window, the unload and beforeunload event won’t be 
    /// emitted for the web page, and close event will also not be emitted 
    /// for this window, but it guarantees the closed event will be emitted.
    /// </summary>
    public void Destroy() => this.CallMethod0();

    /// <summary>
    /// Try to close the window. This has the same effect as a user manually 
    /// clicking the close button of the window. The web page may cancel the close though. 
    /// </summary>
    public void Close() => this.CallMethod0();

    /// <summary>
    /// Focuses on the window.
    /// </summary>
    public void Focus() => this.CallMethod0();

    /// <summary>
    /// Removes focus from the window.
    /// </summary>
    public void Blur() => this.CallMethod0();

    /// <summary>
    /// Whether the window is focused.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsFocusedAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Whether the window is destroyed.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsDestroyedAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Shows and gives focus to the window.
    /// </summary>
    public void Show() => this.CallMethod0();

    /// <summary>
    /// Shows the window but doesn’t focus on it.
    /// </summary>
    public void ShowInactive() => this.CallMethod0();

    /// <summary>
    /// Hides the window.
    /// </summary>
    public void Hide() => this.CallMethod0();

    /// <summary>
    /// Whether the window is visible to the user.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsVisibleAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Whether current window is a modal window.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsModalAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Maximizes the window. This will also show (but not focus) the window if it isn’t being displayed already.
    /// </summary>
    public void Maximize() => this.CallMethod0();

    /// <summary>
    /// Unmaximizes the window.
    /// </summary>
    public void Unmaximize() => this.CallMethod0();

    /// <summary>
    /// Whether the window is maximized.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsMaximizedAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Minimizes the window. On some platforms the minimized window will be shown in the Dock.
    /// </summary>
    public void Minimize() => this.CallMethod0();

    /// <summary>
    /// Restores the window from minimized state to its previous state.
    /// </summary>
    public void Restore() => this.CallMethod0();

    /// <summary>
    /// Whether the window is minimized.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsMinimizedAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window should be in fullscreen mode.
    /// </summary>
    /// <param name="flag"></param>
    public void SetFullScreen(bool flag) => this.CallMethod1(flag);

    /// <summary>
    /// Whether the window is in fullscreen mode.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsFullScreenAsync() => this.GetPropertyAsync<bool>();

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
    public void SetAspectRatio(double aspectRatio, Size extraSize) =>
        this.CallMethod2(aspectRatio, extraSize);

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
    public void SetAspectRatio(int aspectRatio, Size extraSize) =>
        this.CallMethod2(aspectRatio, extraSize);

    /// <summary>
    /// Uses Quick Look to preview a file at a given path.
    /// </summary>
    /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
    /// Quick Look uses the file name and file extension on the path to determine the content type of the 
    /// file to open.</param>
    public void PreviewFile(string path) => this.CallMethod1(path);

    /// <summary>
    /// Uses Quick Look to preview a file at a given path.
    /// </summary>
    /// <param name="path">The absolute path to the file to preview with QuickLook. This is important as 
    /// Quick Look uses the file name and file extension on the path to determine the content type of the 
    /// file to open.</param>
    /// <param name="displayname">The name of the file to display on the Quick Look modal view. This is 
    /// purely visual and does not affect the content type of the file. Defaults to path.</param>
    public void PreviewFile(string path, string displayname) => this.CallMethod2(path, displayname);

    /// <summary>
    /// Closes the currently open Quick Look panel.
    /// </summary>
    public void CloseFilePreview() => this.CallMethod0();

    /// <summary>
    /// Resizes and moves the window to the supplied bounds
    /// </summary>
    /// <param name="bounds"></param>
    public void SetBounds(Rectangle bounds) => this.CallMethod1(bounds);

    /// <summary>
    /// Resizes and moves the window to the supplied bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="animate"></param>
    public void SetBounds(Rectangle bounds, bool animate) => this.CallMethod2(bounds, animate);

    /// <summary>
    /// Gets the bounds asynchronous.
    /// </summary>
    /// <returns></returns>
    public Task<Rectangle> GetBoundsAsync() => this.GetPropertyAsync<Rectangle>();

    /// <summary>
    /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
    /// </summary>
    /// <param name="bounds"></param>
    public void SetContentBounds(Rectangle bounds) => this.CallMethod1(bounds);

    /// <summary>
    /// Resizes and moves the window’s client area (e.g. the web page) to the supplied bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="animate"></param>
    public void SetContentBounds(Rectangle bounds, bool animate) => this.CallMethod2(bounds, animate);

    /// <summary>
    /// Gets the content bounds asynchronous.
    /// </summary>
    /// <returns></returns>
    public Task<Rectangle> GetContentBoundsAsync() => this.GetPropertyAsync<Rectangle>();

    /// <summary>
    /// Resizes the window to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetSize(int width, int height) => this.CallMethod2(width, height);

    /// <summary>
    /// Resizes the window to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="animate"></param>
    public void SetSize(int width, int height, bool animate) => this.CallMethod3(width, height, animate);

    /// <summary>
    /// Contains the window’s width and height.
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetSizeAsync() => this.GetPropertyAsync<int[]>();

    /// <summary>
    /// Resizes the window’s client area (e.g. the web page) to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetContentSize(int width, int height) => this.CallMethod2(width, height);

    /// <summary>
    /// Resizes the window’s client area (e.g. the web page) to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="animate"></param>
    public void SetContentSize(int width, int height, bool animate) => this.CallMethod3(width, height, animate);

    /// <summary>
    /// Contains the window’s client area’s width and height.
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetContentSizeAsync() => this.GetPropertyAsync<int[]>();

    /// <summary>
    /// Sets the minimum size of window to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetMinimumSize(int width, int height) => this.CallMethod2(width, height);

    /// <summary>
    /// Contains the window’s minimum width and height.
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetMinimumSizeAsync() => this.GetPropertyAsync<int[]>();

    /// <summary>
    /// Sets the maximum size of window to width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetMaximumSize(int width, int height) => this.CallMethod2(width, height);

    /// <summary>
    /// Contains the window’s maximum width and height.
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetMaximumSizeAsync() => this.GetPropertyAsync<int[]>();

    /// <summary>
    /// Sets whether the window can be manually resized by user.
    /// </summary>
    /// <param name="resizable"></param>
    public void SetResizable(bool resizable) => this.CallMethod1(resizable);

    /// <summary>
    /// Whether the window can be manually resized by user.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsResizableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window can be moved by user. On Linux does nothing.
    /// </summary>
    /// <param name="movable"></param>
    public void SetMovable(bool movable) => this.CallMethod1(movable);

    /// <summary>
    /// Whether the window can be moved by user.
    /// 
    /// On Linux always returns true.
    /// </summary>
    /// <returns>On Linux always returns true.</returns>
    public Task<bool> IsMovableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window can be manually minimized by user. On Linux does nothing.
    /// </summary>
    /// <param name="minimizable"></param>
    public void SetMinimizable(bool minimizable) => this.CallMethod1(minimizable);

    /// <summary>
    /// Whether the window can be manually minimized by user.
    /// 
    /// On Linux always returns true.
    /// </summary>
    /// <returns>On Linux always returns true.</returns>
    public Task<bool> IsMinimizableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window can be manually maximized by user. On Linux does nothing.
    /// </summary>
    /// <param name="maximizable"></param>
    public void SetMaximizable(bool maximizable) => this.CallMethod1(maximizable);

    /// <summary>
    /// Whether the window can be manually maximized by user.
    /// 
    /// On Linux always returns true.
    /// </summary>
    /// <returns>On Linux always returns true.</returns>
    public Task<bool> IsMaximizableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
    /// </summary>
    /// <param name="fullscreenable"></param>
    public void SetFullScreenable(bool fullscreenable) => this.CallMethod1(fullscreenable);

    /// <summary>
    /// Whether the maximize/zoom window button toggles fullscreen mode or maximizes the window.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsFullScreenableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window can be manually closed by user. On Linux does nothing.
    /// </summary>
    /// <param name="closable"></param>
    public void SetClosable(bool closable) => this.CallMethod1(closable);

    /// <summary>
    /// Whether the window can be manually closed by user.
    /// 
    /// On Linux always returns true.
    /// </summary>
    /// <returns>On Linux always returns true.</returns>
    public Task<bool> IsClosableAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window should show always on top of other windows. 
    /// After setting this, the window is still a normal window, not a toolbox 
    /// window which can not be focused on.
    /// </summary>
    /// <param name="flag"></param>
    public void SetAlwaysOnTop(bool flag) => this.CallMethod1(flag);

    /// <summary>
    /// Sets whether the window should show always on top of other windows. 
    /// After setting this, the window is still a normal window, not a toolbox 
    /// window which can not be focused on.
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="level">Values include normal, floating, torn-off-menu, modal-panel, main-menu, 
    /// status, pop-up-menu and screen-saver. The default is floating. 
    /// See the macOS docs</param>
    public void SetAlwaysOnTop(bool flag, OnTopLevel level) => this.CallMethod2(flag, level.GetDescription());

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
    public void SetAlwaysOnTop(bool flag, OnTopLevel level, int relativeLevel) => this.CallMethod3(flag, level.GetDescription(), relativeLevel);

    /// <summary>
    /// Whether the window is always on top of other windows.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsAlwaysOnTopAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Moves window to the center of the screen.
    /// </summary>
    public void Center() => this.CallMethod0();

    /// <summary>
    /// Moves window to x and y.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        // Workaround Windows 10 / Electron Bug
        // https://github.com/electron/electron/issues/4045
        //if (isWindows10())
        //{
        //    x = x - 7;
        //}
        this.CallMethod2(x, y);
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
        //if (isWindows10())
        //{
        //    x = x - 7;
        //}

        this.CallMethod3(x, y, animate);
    }

    private bool isWindows10()
    {
        return RuntimeInformation.OSDescription.Contains("Windows 10");
    }

    /// <summary>
    /// Contains the window’s current position.
    /// </summary>
    /// <returns></returns>
    public Task<int[]> GetPositionAsync() => this.GetPropertyAsync<int[]>();

    /// <summary>
    /// Changes the title of native window to title.
    /// </summary>
    /// <param name="title"></param>
    public void SetTitle(string title) => this.CallMethod1(title);

    /// <summary>
    /// The title of the native window.
    /// 
    /// Note: The title of web page can be different from the title of the native window.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetTitleAsync() => this.GetPropertyAsync<string>();

    /// <summary>
    /// Changes the attachment point for sheets on macOS. 
    /// By default, sheets are attached just below the window frame, 
    /// but you may want to display them beneath a HTML-rendered toolbar.
    /// </summary>
    /// <param name="offsetY"></param>
    public void SetSheetOffset(float offsetY) => this.CallMethod1(offsetY);

    /// <summary>
    /// Changes the attachment point for sheets on macOS. 
    /// By default, sheets are attached just below the window frame, 
    /// but you may want to display them beneath a HTML-rendered toolbar.
    /// </summary>
    /// <param name="offsetY"></param>
    /// <param name="offsetX"></param>
    public void SetSheetOffset(float offsetY, float offsetX) => this.CallMethod2(offsetY, offsetX);

    /// <summary>
    /// Starts or stops flashing the window to attract user’s attention.
    /// </summary>
    /// <param name="flag"></param>
    public void FlashFrame(bool flag) => this.CallMethod1(flag);

    /// <summary>
    /// Makes the window not show in the taskbar.
    /// </summary>
    /// <param name="skip"></param>
    public void SetSkipTaskbar(bool skip) => this.CallMethod1(skip);

    /// <summary>
    /// Enters or leaves the kiosk mode.
    /// </summary>
    /// <param name="flag"></param>
    public void SetKiosk(bool flag) => this.CallMethod1(flag);

    /// <summary>
    /// Whether the window is in kiosk mode.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsKioskAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Returns the native type of the handle is HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.
    /// </summary>
    /// <returns>string of the native handle obtained, HWND on Windows, NSView* on macOS, and Window (unsigned long) on Linux.</returns>
    public Task<string> GetNativeWindowHandle() => this.GetPropertyAsync<string>();

    /// <summary>
    /// Sets the pathname of the file the window represents, 
    /// and the icon of the file will show in window’s title bar.
    /// </summary>
    /// <param name="filename"></param>
    public void SetRepresentedFilename(string filename) => this.CallMethod1(filename);

    /// <summary>
    /// The pathname of the file the window represents.
    /// </summary>
    /// <returns></returns>
    public Task<string> GetRepresentedFilenameAsync() => this.GetPropertyAsync<string>();

    /// <summary>
    /// Specifies whether the window’s document has been edited, 
    /// and the icon in title bar will become gray when set to true.
    /// </summary>
    /// <param name="edited"></param>
    public void SetDocumentEdited(bool edited) => this.CallMethod1(edited);

    /// <summary>
    /// Whether the window’s document has been edited.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsDocumentEditedAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Focuses the on web view.
    /// </summary>
    public void FocusOnWebView() => this.CallMethod0();

    /// <summary>
    /// Blurs the web view.
    /// </summary>
    public void BlurWebView() => this.CallMethod0();

    /// <summary>
    /// The url can be a remote address (e.g. http://) or 
    /// a path to a local HTML file using the file:// protocol.
    /// </summary>
    /// <param name="url"></param>
    public void LoadURL(string url) => this.CallMethod1(url);

    /// <summary>
    /// The url can be a remote address (e.g. http://) or 
    /// a path to a local HTML file using the file:// protocol.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="options"></param>
    public void LoadURL(string url, LoadURLOptions options) => this.CallMethod2(url, options);

    /// <summary>
    /// Same as webContents.reload.
    /// </summary>
    public void Reload() => this.CallMethod0();

    /// <summary>
    /// Gets the menu items.
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
    /// Sets the menu as the window’s menu bar, 
    /// setting it to null will remove the menu bar.
    /// </summary>
    /// <param name="menuItems"></param>
    public void SetMenu(MenuItem[] menuItems)
    {
        menuItems.AddMenuItemsId();
        this.CallMethod1(menuItems);
        _items.AddRange(menuItems);

        BridgeConnector.Socket.Off("windowMenuItemClicked");
        BridgeConnector.Socket.On<string>("windowMenuItemClicked", (id) =>
        {
            MenuItem menuItem = _items.GetMenuItem(id);
            menuItem?.Click();
        });
    }

    /// <summary>
    /// Remove the window's menu bar.
    /// </summary>
    public void RemoveMenu() => this.CallMethod0();

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
    public void SetProgressBar(double progress) => this.CallMethod1(progress);

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
    public void SetProgressBar(double progress, ProgressBarOptions progressBarOptions) =>
        this.CallMethod2(progress, progressBarOptions);

    /// <summary>
    /// Sets whether the window should have a shadow. On Windows and Linux does nothing.
    /// </summary>
    /// <param name="hasShadow"></param>
    public void SetHasShadow(bool hasShadow) => this.CallMethod1(hasShadow);

    /// <summary>
    /// Whether the window has a shadow.
    /// 
    /// On Windows and Linux always returns true.
    /// </summary>
    /// <returns></returns>
    public Task<bool> HasShadowAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Gets the thumbar buttons.
    /// </summary>
    /// <value>
    /// The thumbar buttons.
    /// </value>
    public IReadOnlyCollection<ThumbarButton> ThumbarButtons
    {
        get
        {
            return _thumbarButtons.AsReadOnly();
        }
    }

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
        var tcs = new TaskCompletionSource<bool>();

        BridgeConnector.Socket.Once<bool>("browserWindowSetThumbarButtons-completed", tcs.SetResult);

        thumbarButtons.AddThumbarButtonsId();
        BridgeConnector.Socket.Emit("browserWindowSetThumbarButtons", Id, thumbarButtons);
        _thumbarButtons.Clear();
        _thumbarButtons.AddRange(thumbarButtons);

        BridgeConnector.Socket.Off("thumbarButtonClicked");
        BridgeConnector.Socket.On<string>("thumbarButtonClicked", (id) =>
        {
            ThumbarButton thumbarButton = _thumbarButtons.GetThumbarButton(id);
            thumbarButton?.Click();
        });

        return tcs.Task;
    }

    /// <summary>
    /// Sets the region of the window to show as the thumbnail image displayed when hovering over
    /// the window in the taskbar. You can reset the thumbnail to be the entire window by specifying
    /// an empty region: {x: 0, y: 0, width: 0, height: 0}.
    /// </summary>
    /// <param name="rectangle"></param>
    public void SetThumbnailClip(Rectangle rectangle) => this.CallMethod1(rectangle);

    /// <summary>
    /// Sets the toolTip that is displayed when hovering over the window thumbnail in the taskbar.
    /// </summary>
    /// <param name="tooltip"></param>
    public void SetThumbnailToolTip(string tooltip) => this.CallMethod1(tooltip);

    /// <summary>
    /// Sets the properties for the window’s taskbar button.
    /// 
    /// Note: relaunchCommand and relaunchDisplayName must always be set together. 
    /// If one of those properties is not set, then neither will be used.
    /// </summary>
    /// <param name="options"></param>
    public void SetAppDetails(AppDetailsOptions options) => this.CallMethod1(options);

    /// <summary>
    /// Same as webContents.showDefinitionForSelection().
    /// </summary>
    public void ShowDefinitionForSelection() => this.CallMethod0();

    /// <summary>
    /// Sets whether the window menu bar should hide itself automatically. 
    /// Once set the menu bar will only show when users press the single Alt key.
    /// 
    /// If the menu bar is already visible, calling setAutoHideMenuBar(true) won’t hide it immediately.
    /// </summary>
    /// <param name="hide"></param>
    public void SetAutoHideMenuBar(bool hide) => this.CallMethod1(hide);

    /// <summary>
    /// Whether menu bar automatically hides itself.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsMenuBarAutoHideAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the menu bar should be visible. If the menu bar is auto-hide,
    /// users can still bring up the menu bar by pressing the single Alt key.
    /// </summary>
    /// <param name="visible"></param>
    public void SetMenuBarVisibility(bool visible) => this.CallMethod1(visible);

    /// <summary>
    /// Whether the menu bar is visible.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsMenuBarVisibleAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Sets whether the window should be visible on all workspaces.
    /// 
    /// Note: This API does nothing on Windows.
    /// </summary>
    /// <param name="visible"></param>
    public void SetVisibleOnAllWorkspaces(bool visible) => this.CallMethod1(visible);

    /// <summary>
    /// Whether the window is visible on all workspaces.
    /// 
    /// Note: This API always returns false on Windows.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsVisibleOnAllWorkspacesAsync() => this.GetPropertyAsync<bool>();

    /// <summary>
    /// Makes the window ignore all mouse events.
    /// 
    /// All mouse events happened in this window will be passed to the window 
    /// below this window, but if this window has focus, it will still receive keyboard events.
    /// </summary>
    /// <param name="ignore"></param>
    public void SetIgnoreMouseEvents(bool ignore) => this.CallMethod1(ignore);

    /// <summary>
    /// Prevents the window contents from being captured by other apps.
    /// 
    /// On macOS it sets the NSWindow’s sharingType to NSWindowSharingNone. 
    /// On Windows it calls SetWindowDisplayAffinity with WDA_MONITOR.
    /// </summary>
    /// <param name="enable"></param>
    public void SetContentProtection(bool enable) => this.CallMethod1(enable);

    /// <summary>
    /// Changes whether the window can be focused.
    /// </summary>
    /// <param name="focusable"></param>
    public void SetFocusable(bool focusable) => this.CallMethod1(focusable);

    /// <summary>
    /// Sets parent as current window’s parent window, 
    /// passing null will turn current window into a top-level window.
    /// </summary>
    /// <param name="parent"></param>
    public void SetParentWindow(BrowserWindow parent)
    {
        if (parent == null)
        {
            BridgeConnector.Socket.Emit("browserWindowSetParentWindow", Id, null);
        }
        else
        {
            BridgeConnector.Socket.Emit("browserWindowSetParentWindow", Id, parent);
        }
    }

    /// <summary>
    /// The parent window.
    /// </summary>
    /// <returns></returns>
    public async Task<BrowserWindow> GetParentWindowAsync()
    {
        var browserWindowId = await this.GetPropertyAsync<int>().ConfigureAwait(false);
        var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == browserWindowId);
        return browserWindow;
    }

    /// <summary>
    /// All child windows.
    /// </summary>
    /// <returns></returns>
    public async Task<List<BrowserWindow>> GetChildWindowsAsync()
    {
        var browserWindowIds = await this.GetPropertyAsync<int[]>().ConfigureAwait(false);
        var browserWindows = new List<BrowserWindow>();

        foreach (var id in browserWindowIds)
        {
            var browserWindow = Electron.WindowManager.BrowserWindows.ToList().Single(x => x.Id == id);
            browserWindows.Add(browserWindow);
        }

        return browserWindows;
    }

    /// <summary>
    /// Controls whether to hide cursor when typing.
    /// </summary>
    /// <param name="autoHide"></param>
    public void SetAutoHideCursor(bool autoHide) => this.CallMethod1(autoHide);

    /// <summary>
    /// Adds a vibrancy effect to the browser window. 
    /// Passing null or an empty string will remove the vibrancy effect on the window.
    /// </summary>
    /// <param name="type">Can be appearance-based, light, dark, titlebar, selection, 
    /// menu, popover, sidebar, medium-light or ultra-dark. 
    /// See the macOS documentation for more details.</param>
    public void SetVibrancy(Vibrancy type) => this.CallMethod1(type.GetDescription());

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
        // This message name does not match the default ApiBase naming convention.
        BridgeConnector.Socket.Emit("browserWindow-setBrowserView", Id, browserView.Id);
    }
}