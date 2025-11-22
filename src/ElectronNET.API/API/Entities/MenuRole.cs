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
        undo = 1,

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
        /// The pasteAndMatchStyle
        /// </summary>
        pasteAndMatchStyle,

        /// <summary>
        /// The selectAll
        /// </summary>
        selectAll,

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
        forceReload,

        /// <summary>
        /// Toggle developer tools in the current window
        /// </summary>
        toggleDevTools,

        /// <summary>
        /// Toggle full screen mode on the current window
        /// </summary>
        togglefullscreen,

        /// <summary>
        /// Reset the focused page’s zoom level to the original size
        /// </summary>
        resetZoom,

        /// <summary>
        /// Zoom in the focused page by 10%
        /// </summary>
        zoomIn,

        /// <summary>
        /// Zoom out the focused page by 10%
        /// </summary>
        zoomOut,

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
        hideOthers,

        /// <summary>
        /// Only macOS: Map to the unhideAllApplications action
        /// </summary>
        unhide,

        /// <summary>
        /// Only macOS: Map to the startSpeaking action
        /// </summary>
        startSpeaking,

        /// <summary>
        /// Only macOS: Map to the stopSpeaking action
        /// </summary>
        stopSpeaking,

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
        services,

        /// <summary>
        /// Toggle built-in spellchecker.
        /// </summary>
        toggleSpellChecker,

        /// <summary>
        /// The submenu is a "File" menu.
        /// </summary>
        fileMenu,

        /// <summary>
        /// The submenu is a "View" menu.
        /// </summary>
        viewMenu,

        /// <summary>
        /// The application menu.
        /// </summary>
        appMenu,

        /// <summary>
        /// The submenu is a "Share" menu.
        /// </summary>
        shareMenu,

        /// <summary>
        /// Displays a list of files recently opened by the app.
        /// </summary>
        recentDocuments,

        /// <summary>
        /// Clear the recent documents list.
        /// </summary>
        clearRecentDocuments,

        /// <summary>
        /// Toggle the tab bar (macOS).
        /// </summary>
        toggleTabBar,

        /// <summary>
        /// Select the next tab (macOS).
        /// </summary>
        selectNextTab,

        /// <summary>
        /// Select the previous tab (macOS).
        /// </summary>
        selectPreviousTab,

        /// <summary>
        /// Show all tabs (macOS).
        /// </summary>
        showAllTabs,

        /// <summary>
        /// Merge all windows (macOS).
        /// </summary>
        mergeAllWindows,

        /// <summary>
        /// Move the current tab to a new window (macOS).
        /// </summary>
        moveTabToNewWindow,

        /// <summary>
        /// Show substitutions panel (macOS).
        /// </summary>
        showSubstitutions,

        /// <summary>
        /// Toggle smart quotes (macOS).
        /// </summary>
        toggleSmartQuotes,

        /// <summary>
        /// Toggle smart dashes (macOS).
        /// </summary>
        toggleSmartDashes,

        /// <summary>
        /// Toggle text replacement (macOS).
        /// </summary>
        toggleTextReplacement,

        // Backwards-compatibility aliases (old identifiers) to avoid breaking existing code.
        // These map to the same enum values as their official values.
        pasteandmatchstyle = pasteAndMatchStyle,
        selectall = selectAll,
        forcereload = forceReload,
        toggledevtools = toggleDevTools,
        resetzoom = resetZoom,
        zoomin = zoomIn,
        zoomout = zoomOut,
        hideothers = hideOthers,
        startspeaking = startSpeaking,
        stopspeaking = stopSpeaking,
        togglespellchecker = toggleSpellChecker,
        togglesmartquotes = toggleSmartQuotes,
        togglesmartdashes = toggleSmartDashes,
        toggletextreplacement = toggleTextReplacement
    }
}