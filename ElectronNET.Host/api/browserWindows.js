"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var windows = [];
module.exports = function (socket) {
    socket.on('register-browserWindow-ready-to-show', function (id) {
        getWindowById(id).on('ready-to-show', function () {
            socket.emit('browserWindow-ready-to-show');
        });
    });
    socket.on('createBrowserWindow', function (options, loadUrl) {
        var window = new electron_1.BrowserWindow(options);
        window.on('closed', function (sender) {
            var _loop_1 = function () {
                windowItem = windows[index];
                try {
                    windowItem.id;
                }
                catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);
                        var ids_1 = [];
                        windows.forEach(function (x) { return ids_1.push(x.id); });
                        socket.emit('BrowserWindowClosed', ids_1);
                    }
                }
            };
            var windowItem;
            for (var index = 0; index < windows.length; index++) {
                _loop_1();
            }
        });
        if (loadUrl) {
            window.loadURL(loadUrl);
        }
        windows.push(window);
        socket.emit('BrowserWindowCreated', window.id);
    });
    socket.on('browserWindow-destroy', function (id) {
        getWindowById(id).destroy();
    });
    socket.on('browserWindow-close', function (id) {
        getWindowById(id).close();
    });
    socket.on('browserWindow-focus', function (id) {
        getWindowById(id).focus();
    });
    socket.on('browserWindow-blur', function (id) {
        getWindowById(id).blur();
    });
    socket.on('browserWindow-isFocused', function (id) {
        var isFocused = getWindowById(id).isFocused();
        socket.emit('browserWindow-isFocused-completed', isFocused);
    });
    socket.on('browserWindow-isDestroyed', function (id) {
        var isDestroyed = getWindowById(id).isDestroyed();
        socket.emit('browserWindow-isDestroyed-completed', isDestroyed);
    });
    socket.on('browserWindow-show', function (id) {
        getWindowById(id).show();
    });
    socket.on('browserWindow-showInactive', function (id) {
        getWindowById(id).showInactive();
    });
    socket.on('browserWindow-hide', function (id) {
        getWindowById(id).hide();
    });
    socket.on('browserWindow-isVisible', function (id) {
        var isVisible = getWindowById(id).isVisible();
        socket.emit('browserWindow-isVisible-completed', isVisible);
    });
    socket.on('browserWindow-isModal', function (id) {
        var isModal = getWindowById(id).isModal();
        socket.emit('browserWindow-isModal-completed', isModal);
    });
    socket.on('browserWindow-maximize', function (id) {
        getWindowById(id).maximize();
    });
    socket.on('browserWindow-unmaximize', function (id) {
        getWindowById(id).unmaximize();
    });
    socket.on('browserWindow-isMaximized', function (id) {
        var isMaximized = getWindowById(id).isMaximized();
        socket.emit('browserWindow-isMaximized-completed', isMaximized);
    });
    socket.on('browserWindow-minimize', function (id) {
        getWindowById(id).minimize();
    });
    socket.on('browserWindow-restore', function (id) {
        getWindowById(id).restore();
    });
    socket.on('browserWindow-isMinimized', function (id) {
        var isMinimized = getWindowById(id).isMinimized();
        socket.emit('browserWindow-isMinimized-completed', isMinimized);
    });
    socket.on('browserWindow-setFullScreen', function (id, fullscreen) {
        getWindowById(id).setFullScreen(fullscreen);
    });
    socket.on('browserWindow-isFullScreen', function (id) {
        var isFullScreen = getWindowById(id).isFullScreen();
        socket.emit('browserWindow-isFullScreen-completed', isFullScreen);
    });
    socket.on('browserWindow-setAspectRatio', function (id, aspectRatio, extraSize) {
        getWindowById(id).setAspectRatio(aspectRatio, extraSize);
    });
    socket.on('browserWindow-previewFile', function (id, path, displayname) {
        getWindowById(id).previewFile(path, displayname);
    });
    socket.on('browserWindow-closeFilePreview', function (id) {
        getWindowById(id).closeFilePreview();
    });
    socket.on('browserWindow-setBounds', function (id, bounds, animate) {
        getWindowById(id).setBounds(bounds, animate);
    });
    socket.on('browserWindow-getBounds', function (id) {
        var rectangle = getWindowById(id).getBounds();
        socket.emit('browserWindow-getBounds-completed', rectangle);
    });
    socket.on('browserWindow-setContentBounds', function (id, bounds, animate) {
        getWindowById(id).setContentBounds(bounds, animate);
    });
    socket.on('browserWindow-getContentBounds', function (id) {
        var rectangle = getWindowById(id).getContentBounds();
        socket.emit('browserWindow-getContentBounds-completed', rectangle);
    });
    socket.on('browserWindow-setSize', function (id, width, height, animate) {
        getWindowById(id).setSize(width, height, animate);
    });
    socket.on('browserWindow-getSize', function (id) {
        var size = getWindowById(id).getSize();
        socket.emit('browserWindow-getSize-completed', size);
    });
    socket.on('browserWindow-setContentSize', function (id, width, height, animate) {
        getWindowById(id).setContentSize(width, height, animate);
    });
    socket.on('browserWindow-getContentSize', function (id) {
        var size = getWindowById(id).getContentSize();
        socket.emit('browserWindow-getContentSize-completed', size);
    });
    socket.on('browserWindow-setMinimumSize', function (id, width, height) {
        getWindowById(id).setMinimumSize(width, height);
    });
    socket.on('browserWindow-getMinimumSize', function (id) {
        var size = getWindowById(id).getMinimumSize();
        socket.emit('browserWindow-getMinimumSize-completed', size);
    });
    socket.on('browserWindow-setMaximumSize', function (id, width, height) {
        getWindowById(id).setMaximumSize(width, height);
    });
    socket.on('browserWindow-getMaximumSize', function (id) {
        var size = getWindowById(id).getMaximumSize();
        socket.emit('browserWindow-getMaximumSize-completed', size);
    });
    socket.on('browserWindow-setResizable', function (id, resizable) {
        getWindowById(id).setResizable(resizable);
    });
    socket.on('browserWindow-isResizable', function (id) {
        var resizable = getWindowById(id).isResizable();
        socket.emit('browserWindow-isResizable-completed', resizable);
    });
    socket.on('browserWindow-setMovable', function (id, movable) {
        getWindowById(id).setMovable(movable);
    });
    socket.on('browserWindow-isMovable', function (id) {
        var movable = getWindowById(id).isMovable();
        socket.emit('browserWindow-isMovable-completed', movable);
    });
    socket.on('browserWindow-setMinimizable', function (id, minimizable) {
        getWindowById(id).setMinimizable(minimizable);
    });
    socket.on('browserWindow-isMinimizable', function (id) {
        var minimizable = getWindowById(id).isMinimizable();
        socket.emit('browserWindow-isMinimizable-completed', minimizable);
    });
    socket.on('browserWindow-setMaximizable', function (id, maximizable) {
        getWindowById(id).setMaximizable(maximizable);
    });
    socket.on('browserWindow-isMaximizable', function (id) {
        var maximizable = getWindowById(id).isMaximizable();
        socket.emit('browserWindow-isMaximizable-completed', maximizable);
    });
    socket.on('browserWindow-setFullScreenable', function (id, fullscreenable) {
        getWindowById(id).setFullScreenable(fullscreenable);
    });
    socket.on('browserWindow-isFullScreenable', function (id) {
        var fullscreenable = getWindowById(id).isFullScreenable();
        socket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
    });
    socket.on('browserWindow-setClosable', function (id, closable) {
        getWindowById(id).setClosable(closable);
    });
    socket.on('browserWindow-isClosable', function (id) {
        var closable = getWindowById(id).isClosable();
        socket.emit('browserWindow-isClosable-completed', closable);
    });
    socket.on('browserWindow-setAlwaysOnTop', function (id, flag, level, relativeLevel) {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });
    socket.on('browserWindow-isAlwaysOnTop', function (id) {
        var isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();
        socket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
    });
    socket.on('browserWindow-center', function (id) {
        getWindowById(id).center();
    });
    socket.on('browserWindow-setPosition', function (id, x, y, animate) {
        getWindowById(id).setPosition(x, y, animate);
    });
    socket.on('browserWindow-getPosition', function (id) {
        var position = getWindowById(id).getPosition();
        socket.emit('browserWindow-getPosition-completed', position);
    });
    socket.on('browserWindow-setTitle', function (id, title) {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindow-getTitle', function (id) {
        var title = getWindowById(id).getTitle();
        socket.emit('browserWindow-getTitle-completed', title);
    });
    socket.on('browserWindow-setTitle', function (id, title) {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindow-setSheetOffset', function (id, offsetY, offsetX) {
        if (offsetX) {
            getWindowById(id).setSheetOffset(offsetY, offsetX);
        }
        else {
            getWindowById(id).setSheetOffset(offsetY);
        }
    });
    socket.on('browserWindow-flashFrame', function (id, flag) {
        getWindowById(id).flashFrame(flag);
    });
    socket.on('browserWindow-setSkipTaskbar', function (id, skip) {
        getWindowById(id).setSkipTaskbar(skip);
    });
    socket.on('browserWindow-setKiosk', function (id, flag) {
        getWindowById(id).setKiosk(flag);
    });
    socket.on('browserWindow-isKiosk', function (id) {
        var isKiosk = getWindowById(id).isKiosk();
        socket.emit('browserWindow-isKiosk-completed', isKiosk);
    });
    socket.on('browserWindow-setRepresentedFilename', function (id, filename) {
        getWindowById(id).setRepresentedFilename(filename);
    });
    socket.on('browserWindow-getRepresentedFilename', function (id) {
        var pathname = getWindowById(id).getRepresentedFilename();
        socket.emit('browserWindow-getRepresentedFilename-completed', pathname);
    });
    socket.on('browserWindow-setDocumentEdited', function (id, edited) {
        getWindowById(id).setDocumentEdited(edited);
    });
    socket.on('browserWindow-isDocumentEdited', function (id) {
        var edited = getWindowById(id).isDocumentEdited();
        socket.emit('browserWindow-isDocumentEdited-completed', edited);
    });
    socket.on('browserWindow-focusOnWebView', function (id) {
        getWindowById(id).focusOnWebView();
    });
    socket.on('browserWindow-blurWebView', function (id) {
        getWindowById(id).blurWebView();
    });
    socket.on('browserWindow-loadURL', function (id, url, options) {
        getWindowById(id).loadURL(url, options);
    });
    socket.on('browserWindow-reload', function (id) {
        getWindowById(id).reload();
    });
    socket.on('browserWindow-setMenu', function (id, menuItems) {
        var menu = null;
        if (menuItems) {
            menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, function (id) {
                socket.emit("windowMenuItemClicked", id);
            });
        }
        getWindowById(id).setMenu(menu);
    });
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach(function (item) {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ("id" in item && item.id) {
                item.click = function () { callback(item.id); };
            }
        });
    }
    socket.on('browserWindow-setProgressBar', function (id, progress) {
        getWindowById(id).setProgressBar(progress);
    });
    socket.on('browserWindow-setHasShadow', function (id, hasShadow) {
        getWindowById(id).setHasShadow(hasShadow);
    });
    socket.on('browserWindow-hasShadow', function (id) {
        var hasShadow = getWindowById(id).hasShadow();
        socket.emit('browserWindow-hasShadow-completed', hasShadow);
    });
    socket.on('browserWindow-setAppDetails', function (id, options) {
        getWindowById(id).setAppDetails(options);
    });
    socket.on('browserWindow-showDefinitionForSelection', function (id) {
        getWindowById(id).showDefinitionForSelection();
    });
    socket.on('browserWindow-setAutoHideMenuBar', function (id, hide) {
        getWindowById(id).setAutoHideMenuBar(hide);
    });
    socket.on('browserWindow-isMenuBarAutoHide', function (id) {
        var isMenuBarAutoHide = getWindowById(id).isMenuBarAutoHide();
        socket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
    });
    socket.on('browserWindow-setMenuBarVisibility', function (id, visible) {
        getWindowById(id).setMenuBarVisibility(visible);
    });
    socket.on('browserWindow-isMenuBarVisible', function (id) {
        var isMenuBarVisible = getWindowById(id).isMenuBarVisible();
        socket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
    });
    socket.on('browserWindow-setVisibleOnAllWorkspaces', function (id, visible) {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });
    socket.on('browserWindow-isVisibleOnAllWorkspaces', function (id) {
        var isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();
        socket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
    });
    socket.on('browserWindow-setIgnoreMouseEvents', function (id, ignore) {
        getWindowById(id).setIgnoreMouseEvents(ignore);
    });
    socket.on('browserWindow-setContentProtection', function (id, enable) {
        getWindowById(id).setContentProtection(enable);
    });
    socket.on('browserWindow-setFocusable', function (id, focusable) {
        getWindowById(id).setFocusable(focusable);
    });
    socket.on('browserWindow-setParentWindow', function (id, parent) {
        var browserWindow = electron_1.BrowserWindow.fromId(parent.id);
        getWindowById(id).setParentWindow(browserWindow);
    });
    socket.on('browserWindow-getParentWindow', function (id) {
        var browserWindow = getWindowById(id).getParentWindow();
        socket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
    });
    socket.on('browserWindow-getChildWindows', function (id) {
        var browserWindows = getWindowById(id).getChildWindows();
        var ids = [];
        browserWindows.forEach(function (x) {
            ids.push(x.id);
        });
        socket.emit('browserWindow-getChildWindows-completed', ids);
    });
    socket.on('browserWindow-setAutoHideCursor', function (id, autoHide) {
        getWindowById(id).setAutoHideCursor(autoHide);
    });
    socket.on('browserWindow-setVibrancy', function (id, type) {
        getWindowById(id).setVibrancy(type);
    });
    function getWindowById(id) {
        for (var index = 0; index < windows.length; index++) {
            var element = windows[index];
            if (element.id == id) {
                return element;
            }
        }
    }
};
//# sourceMappingURL=browserWindows.js.map