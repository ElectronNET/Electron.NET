namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public enum MenuRole
    {
        /// <summary>
        /// The undo
        /// </summary>
        undo,

        /// <summary>
        /// The redo
        /// </summary>
        redo,

        /// <summary>
        /// The cut
        /// </summary>
        cut,

        /// <summary>
        /// The copy
        /// </summary>
        copy,

        /// <summary>
        /// The paste
        /// </summary>
        paste,

        /// <summary>
        /// The pasteandmatchstyle
        /// </summary>
        pasteandmatchstyle,

        /// <summary>
        /// The selectall
        /// </summary>
        selectall,

        /// <summary>
        /// The delete
        /// </summary>
        delete,

        /// <summary>
        /// Minimize current window
        /// </summary>
        minimize,

        /// <summary>
        /// Close current window
        /// </summary>
        close,

        /// <summary>
        /// Quit the application
        /// </summary>
        quit,

        /// <summary>
        /// Reload the current window
        /// </summary>
        reload,

        /// <summary>
        /// Reload the current window ignoring the cache.
        /// </summary>
        forcereload,

        /// <summary>
        /// Toggle developer tools in the current window
        /// </summary>
        toggledevtools,

        /// <summary>
        /// Toggle full screen mode on the current window
        /// </summary>
        togglefullscreen,

        /// <summary>
        /// Reset the focused page’s zoom level to the original size
        /// </summary>
        resetzoom,

        /// <summary>
        /// Zoom in the focused page by 10%
        /// </summary>
        zoomin,

        /// <summary>
        /// Zoom out the focused page by 10%
        /// </summary>
        zoomout,

        /// <summary>
        /// Whole default “Edit” menu (Undo, Copy, etc.)
        /// </summary>
        editMenu,

        /// <summary>
        /// Whole default “Window” menu (Minimize, Close, etc.)
        /// </summary>
        windowMenu,

        /// <summary>
        /// Only macOS: Map to the orderFrontStandardAboutPanel action
        /// </summary>
        about,

        /// <summary>
        /// Only macOS: Map to the hide action
        /// </summary>
        hide,

        /// <summary>
        /// Only macOS: Map to the hideOtherApplications action
        /// </summary>
        hideothers,

        /// <summary>
        /// Only macOS: Map to the unhideAllApplications action
        /// </summary>
        unhide,

        /// <summary>
        /// Only macOS: Map to the startSpeaking action
        /// </summary>
        startspeaking,

        /// <summary>
        /// Only macOS: Map to the stopSpeaking action
        /// </summary>
        stopspeaking,

        /// <summary>
        /// Only macOS: Map to the arrangeInFront action
        /// </summary>
        front,

        /// <summary>
        /// Only macOS: Map to the performZoom action
        /// </summary>
        zoom,

        /// <summary>
        /// Only macOS: The submenu is a “Window” menu
        /// </summary>
        window,

        /// <summary>
        /// Only macOS: The submenu is a “Help” menu
        /// </summary>
        help,

        /// <summary>
        /// Only macOS: The submenu is a “Services” menu
        /// </summary>
        services
    }
}