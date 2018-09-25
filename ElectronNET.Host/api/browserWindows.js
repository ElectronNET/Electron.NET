"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
var path = require('path');
var windows = [];
module.exports = function (socket) {
    socket.on('register-browserWindow-ready-to-show', function (id) {
        getWindowById(id).on('ready-to-show', function () {
            socket.emit('browserWindow-ready-to-show' + id);
        });
    });
    socket.on('register-browserWindow-page-title-updated', function (id) {
        getWindowById(id).on('page-title-updated', function (event, title) {
            socket.emit('browserWindow-page-title-updated' + id, title);
        });
    });
    socket.on('register-browserWindow-close', function (id) {
        getWindowById(id).on('close', function () {
            socket.emit('browserWindow-close' + id);
        });
    });
    socket.on('register-browserWindow-closed', function (id) {
        getWindowById(id).on('closed', function () {
            socket.emit('browserWindow-closed' + id);
        });
    });
    socket.on('register-browserWindow-session-end', function (id) {
        getWindowById(id).on('session-end', function () {
            socket.emit('browserWindow-session-end' + id);
        });
    });
    socket.on('register-browserWindow-unresponsive', function (id) {
        getWindowById(id).on('unresponsive', function () {
            socket.emit('browserWindow-unresponsive' + id);
        });
    });
    socket.on('register-browserWindow-responsive', function (id) {
        getWindowById(id).on('responsive', function () {
            socket.emit('browserWindow-responsive' + id);
        });
    });
    socket.on('register-browserWindow-blur', function (id) {
        getWindowById(id).on('blur', function () {
            socket.emit('browserWindow-blur' + id);
        });
    });
    socket.on('register-browserWindow-focus', function (id) {
        getWindowById(id).on('focus', function () {
            socket.emit('browserWindow-focus' + id);
        });
    });
    socket.on('register-browserWindow-show', function (id) {
        getWindowById(id).on('show', function () {
            socket.emit('browserWindow-show' + id);
        });
    });
    socket.on('register-browserWindow-hide', function (id) {
        getWindowById(id).on('hide', function () {
            socket.emit('browserWindow-hide' + id);
        });
    });
    socket.on('register-browserWindow-maximize', function (id) {
        getWindowById(id).on('maximize', function () {
            socket.emit('browserWindow-maximize' + id);
        });
    });
    socket.on('register-browserWindow-unmaximize', function (id) {
        getWindowById(id).on('unmaximize', function () {
            socket.emit('browserWindow-unmaximize' + id);
        });
    });
    socket.on('register-browserWindow-minimize', function (id) {
        getWindowById(id).on('minimize', function () {
            socket.emit('browserWindow-minimize' + id);
        });
    });
    socket.on('register-browserWindow-restore', function (id) {
        getWindowById(id).on('restore', function () {
            socket.emit('browserWindow-restore' + id);
        });
    });
    socket.on('register-browserWindow-resize', function (id) {
        getWindowById(id).on('resize', function () {
            socket.emit('browserWindow-resize' + id);
        });
    });
    socket.on('register-browserWindow-move', function (id) {
        getWindowById(id).on('move', function () {
            socket.emit('browserWindow-move' + id);
        });
    });
    socket.on('register-browserWindow-moved', function (id) {
        getWindowById(id).on('moved', function () {
            socket.emit('browserWindow-moved' + id);
        });
    });
    socket.on('register-browserWindow-enter-full-screen', function (id) {
        getWindowById(id).on('enter-full-screen', function () {
            socket.emit('browserWindow-enter-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-full-screen', function (id) {
        getWindowById(id).on('leave-full-screen', function () {
            socket.emit('browserWindow-leave-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-enter-html-full-screen', function (id) {
        getWindowById(id).on('enter-html-full-screen', function () {
            socket.emit('browserWindow-enter-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-html-full-screen', function (id) {
        getWindowById(id).on('leave-html-full-screen', function () {
            socket.emit('browserWindow-leave-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-app-command', function (id) {
        getWindowById(id).on('app-command', function (event, command) {
            socket.emit('browserWindow-app-command' + id, command);
        });
    });
    socket.on('register-browserWindow-scroll-touch-begin', function (id) {
        getWindowById(id).on('scroll-touch-begin', function () {
            socket.emit('browserWindow-scroll-touch-begin' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-end', function (id) {
        getWindowById(id).on('scroll-touch-end', function () {
            socket.emit('browserWindow-scroll-touch-end' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-edge', function (id) {
        getWindowById(id).on('scroll-touch-edge', function () {
            socket.emit('browserWindow-scroll-touch-edge' + id);
        });
    });
    socket.on('register-browserWindow-swipe', function (id) {
        getWindowById(id).on('swipe', function (event, direction) {
            socket.emit('browserWindow-swipe' + id, direction);
        });
    });
    socket.on('register-browserWindow-sheet-begin', function (id) {
        getWindowById(id).on('sheet-begin', function () {
            socket.emit('browserWindow-sheet-begin' + id);
        });
    });
    socket.on('register-browserWindow-sheet-end', function (id) {
        getWindowById(id).on('sheet-end', function () {
            socket.emit('browserWindow-sheet-end' + id);
        });
    });
    socket.on('register-browserWindow-new-window-for-tab', function (id) {
        getWindowById(id).on('new-window-for-tab', function () {
            socket.emit('browserWindow-new-window-for-tab' + id);
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
    socket.on('browserWindowDestroy', function (id) {
        getWindowById(id).destroy();
    });
    socket.on('browserWindowClose', function (id) {
        getWindowById(id).close();
    });
    socket.on('browserWindowFocus', function (id) {
        getWindowById(id).focus();
    });
    socket.on('browserWindowBlur', function (id) {
        getWindowById(id).blur();
    });
    socket.on('browserWindowIsFocused', function (id) {
        var isFocused = getWindowById(id).isFocused();
        socket.emit('browserWindow-isFocused-completed', isFocused);
    });
    socket.on('browserWindowIsDestroyed', function (id) {
        var isDestroyed = getWindowById(id).isDestroyed();
        socket.emit('browserWindow-isDestroyed-completed', isDestroyed);
    });
    socket.on('browserWindowShow', function (id) {
        getWindowById(id).show();
    });
    socket.on('browserWindowShowInactive', function (id) {
        getWindowById(id).showInactive();
    });
    socket.on('browserWindowHide', function (id) {
        getWindowById(id).hide();
    });
    socket.on('browserWindowIsVisible', function (id) {
        var isVisible = getWindowById(id).isVisible();
        socket.emit('browserWindow-isVisible-completed', isVisible);
    });
    socket.on('browserWindowIsModal', function (id) {
        var isModal = getWindowById(id).isModal();
        socket.emit('browserWindow-isModal-completed', isModal);
    });
    socket.on('browserWindowMaximize', function (id) {
        getWindowById(id).maximize();
    });
    socket.on('browserWindowUnmaximize', function (id) {
        getWindowById(id).unmaximize();
    });
    socket.on('browserWindowIsMaximized', function (id) {
        var isMaximized = getWindowById(id).isMaximized();
        socket.emit('browserWindow-isMaximized-completed', isMaximized);
    });
    socket.on('browserWindowMinimize', function (id) {
        getWindowById(id).minimize();
    });
    socket.on('browserWindowRestore', function (id) {
        getWindowById(id).restore();
    });
    socket.on('browserWindowIsMinimized', function (id) {
        var isMinimized = getWindowById(id).isMinimized();
        socket.emit('browserWindow-isMinimized-completed', isMinimized);
    });
    socket.on('browserWindowSetFullScreen', function (id, fullscreen) {
        getWindowById(id).setFullScreen(fullscreen);
    });
    socket.on('browserWindowIsFullScreen', function (id) {
        var isFullScreen = getWindowById(id).isFullScreen();
        socket.emit('browserWindow-isFullScreen-completed', isFullScreen);
    });
    socket.on('browserWindowSetAspectRatio', function (id, aspectRatio, extraSize) {
        getWindowById(id).setAspectRatio(aspectRatio, extraSize);
    });
    socket.on('browserWindowPreviewFile', function (id, path, displayname) {
        getWindowById(id).previewFile(path, displayname);
    });
    socket.on('browserWindowCloseFilePreview', function (id) {
        getWindowById(id).closeFilePreview();
    });
    socket.on('browserWindowSetBounds', function (id, bounds, animate) {
        getWindowById(id).setBounds(bounds, animate);
    });
    socket.on('browserWindowGetBounds', function (id) {
        var rectangle = getWindowById(id).getBounds();
        socket.emit('browserWindow-getBounds-completed', rectangle);
    });
    socket.on('browserWindowSetContentBounds', function (id, bounds, animate) {
        getWindowById(id).setContentBounds(bounds, animate);
    });
    socket.on('browserWindowGetContentBounds', function (id) {
        var rectangle = getWindowById(id).getContentBounds();
        socket.emit('browserWindow-getContentBounds-completed', rectangle);
    });
    socket.on('browserWindowSetSize', function (id, width, height, animate) {
        getWindowById(id).setSize(width, height, animate);
    });
    socket.on('browserWindowGetSize', function (id) {
        var size = getWindowById(id).getSize();
        socket.emit('browserWindow-getSize-completed', size);
    });
    socket.on('browserWindowSetContentSize', function (id, width, height, animate) {
        getWindowById(id).setContentSize(width, height, animate);
    });
    socket.on('browserWindowGetContentSize', function (id) {
        var size = getWindowById(id).getContentSize();
        socket.emit('browserWindow-getContentSize-completed', size);
    });
    socket.on('browserWindowSetMinimumSize', function (id, width, height) {
        getWindowById(id).setMinimumSize(width, height);
    });
    socket.on('browserWindowGetMinimumSize', function (id) {
        var size = getWindowById(id).getMinimumSize();
        socket.emit('browserWindow-getMinimumSize-completed', size);
    });
    socket.on('browserWindowSetMaximumSize', function (id, width, height) {
        getWindowById(id).setMaximumSize(width, height);
    });
    socket.on('browserWindowGetMaximumSize', function (id) {
        var size = getWindowById(id).getMaximumSize();
        socket.emit('browserWindow-getMaximumSize-completed', size);
    });
    socket.on('browserWindowSetResizable', function (id, resizable) {
        getWindowById(id).setResizable(resizable);
    });
    socket.on('browserWindowIsResizable', function (id) {
        var resizable = getWindowById(id).isResizable();
        socket.emit('browserWindow-isResizable-completed', resizable);
    });
    socket.on('browserWindowSetMovable', function (id, movable) {
        getWindowById(id).setMovable(movable);
    });
    socket.on('browserWindowIsMovable', function (id) {
        var movable = getWindowById(id).isMovable();
        socket.emit('browserWindow-isMovable-completed', movable);
    });
    socket.on('browserWindowSetMinimizable', function (id, minimizable) {
        getWindowById(id).setMinimizable(minimizable);
    });
    socket.on('browserWindowIsMinimizable', function (id) {
        var minimizable = getWindowById(id).isMinimizable();
        socket.emit('browserWindow-isMinimizable-completed', minimizable);
    });
    socket.on('browserWindowSetMaximizable', function (id, maximizable) {
        getWindowById(id).setMaximizable(maximizable);
    });
    socket.on('browserWindowIsMaximizable', function (id) {
        var maximizable = getWindowById(id).isMaximizable();
        socket.emit('browserWindow-isMaximizable-completed', maximizable);
    });
    socket.on('browserWindowSetFullScreenable', function (id, fullscreenable) {
        getWindowById(id).setFullScreenable(fullscreenable);
    });
    socket.on('browserWindowIsFullScreenable', function (id) {
        var fullscreenable = getWindowById(id).isFullScreenable();
        socket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
    });
    socket.on('browserWindowSetClosable', function (id, closable) {
        getWindowById(id).setClosable(closable);
    });
    socket.on('browserWindowIsClosable', function (id) {
        var closable = getWindowById(id).isClosable();
        socket.emit('browserWindow-isClosable-completed', closable);
    });
    socket.on('browserWindowSetAlwaysOnTop', function (id, flag, level, relativeLevel) {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });
    socket.on('browserWindowIsAlwaysOnTop', function (id) {
        var isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();
        socket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
    });
    socket.on('browserWindowCenter', function (id) {
        getWindowById(id).center();
    });
    socket.on('browserWindowSetPosition', function (id, x, y, animate) {
        getWindowById(id).setPosition(x, y, animate);
    });
    socket.on('browserWindowGetPosition', function (id) {
        var position = getWindowById(id).getPosition();
        socket.emit('browserWindow-getPosition-completed', position);
    });
    socket.on('browserWindowSetTitle', function (id, title) {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindowGetTitle', function (id) {
        var title = getWindowById(id).getTitle();
        socket.emit('browserWindow-getTitle-completed', title);
    });
    socket.on('browserWindowSetTitle', function (id, title) {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindowSetSheetOffset', function (id, offsetY, offsetX) {
        if (offsetX) {
            getWindowById(id).setSheetOffset(offsetY, offsetX);
        }
        else {
            getWindowById(id).setSheetOffset(offsetY);
        }
    });
    socket.on('browserWindowFlashFrame', function (id, flag) {
        getWindowById(id).flashFrame(flag);
    });
    socket.on('browserWindowSetSkipTaskbar', function (id, skip) {
        getWindowById(id).setSkipTaskbar(skip);
    });
    socket.on('browserWindowSetKiosk', function (id, flag) {
        getWindowById(id).setKiosk(flag);
    });
    socket.on('browserWindowIsKiosk', function (id) {
        var isKiosk = getWindowById(id).isKiosk();
        socket.emit('browserWindow-isKiosk-completed', isKiosk);
    });
    socket.on('browserWindowSetRepresentedFilename', function (id, filename) {
        getWindowById(id).setRepresentedFilename(filename);
    });
    socket.on('browserWindowGetRepresentedFilename', function (id) {
        var pathname = getWindowById(id).getRepresentedFilename();
        socket.emit('browserWindow-getRepresentedFilename-completed', pathname);
    });
    socket.on('browserWindowSetDocumentEdited', function (id, edited) {
        getWindowById(id).setDocumentEdited(edited);
    });
    socket.on('browserWindowIsDocumentEdited', function (id) {
        var edited = getWindowById(id).isDocumentEdited();
        socket.emit('browserWindow-isDocumentEdited-completed', edited);
    });
    socket.on('browserWindowFocusOnWebView', function (id) {
        getWindowById(id).focusOnWebView();
    });
    socket.on('browserWindowBlurWebView', function (id) {
        getWindowById(id).blurWebView();
    });
    socket.on('browserWindowLoadURL', function (id, url, options) {
        getWindowById(id).loadURL(url, options);
    });
    socket.on('browserWindowReload', function (id) {
        getWindowById(id).reload();
    });
    socket.on('browserWindowSetMenu', function (id, menuItems) {
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
    socket.on('browserWindowSetProgressBar', function (id, progress) {
        getWindowById(id).setProgressBar(progress);
    });
    socket.on('browserWindowSetHasShadow', function (id, hasShadow) {
        getWindowById(id).setHasShadow(hasShadow);
    });
    socket.on('browserWindowHasShadow', function (id) {
        var hasShadow = getWindowById(id).hasShadow();
        socket.emit('browserWindow-hasShadow-completed', hasShadow);
    });
    socket.on('browserWindowSetThumbarButtons', function (id, thumbarButtons) {
        thumbarButtons.forEach(function (thumbarButton) {
            var imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());
            thumbarButton.icon = electron_1.nativeImage.createFromPath(imagePath);
            thumbarButton.click = function () {
                socket.emit("thumbarButtonClicked", thumbarButton["id"]);
            };
        });
        var success = getWindowById(id).setThumbarButtons(thumbarButtons);
        socket.emit('browserWindowSetThumbarButtons-completed', success);
    });
    socket.on('browserWindowSetThumbnailClip', function (id, rectangle) {
        getWindowById(id).setThumbnailClip(rectangle);
    });
    socket.on('browserWindowSetThumbnailToolTip', function (id, toolTip) {
        getWindowById(id).setThumbnailToolTip(toolTip);
    });
    socket.on('browserWindowSetAppDetails', function (id, options) {
        getWindowById(id).setAppDetails(options);
    });
    socket.on('browserWindowShowDefinitionForSelection', function (id) {
        getWindowById(id).showDefinitionForSelection();
    });
    socket.on('browserWindowSetAutoHideMenuBar', function (id, hide) {
        getWindowById(id).setAutoHideMenuBar(hide);
    });
    socket.on('browserWindowIsMenuBarAutoHide', function (id) {
        var isMenuBarAutoHide = getWindowById(id).isMenuBarAutoHide();
        socket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
    });
    socket.on('browserWindowSetMenuBarVisibility', function (id, visible) {
        getWindowById(id).setMenuBarVisibility(visible);
    });
    socket.on('browserWindowIsMenuBarVisible', function (id) {
        var isMenuBarVisible = getWindowById(id).isMenuBarVisible();
        socket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
    });
    socket.on('browserWindowSetVisibleOnAllWorkspaces', function (id, visible) {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });
    socket.on('browserWindowIsVisibleOnAllWorkspaces', function (id) {
        var isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();
        socket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
    });
    socket.on('browserWindowSetIgnoreMouseEvents', function (id, ignore) {
        getWindowById(id).setIgnoreMouseEvents(ignore);
    });
    socket.on('browserWindowSetContentProtection', function (id, enable) {
        getWindowById(id).setContentProtection(enable);
    });
    socket.on('browserWindowSetFocusable', function (id, focusable) {
        getWindowById(id).setFocusable(focusable);
    });
    socket.on('browserWindowSetParentWindow', function (id, parent) {
        var browserWindow = electron_1.BrowserWindow.fromId(parent.id);
        getWindowById(id).setParentWindow(browserWindow);
    });
    socket.on('browserWindowGetParentWindow', function (id) {
        var browserWindow = getWindowById(id).getParentWindow();
        socket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
    });
    socket.on('browserWindowGetChildWindows', function (id) {
        var browserWindows = getWindowById(id).getChildWindows();
        var ids = [];
        browserWindows.forEach(function (x) {
            ids.push(x.id);
        });
        socket.emit('browserWindow-getChildWindows-completed', ids);
    });
    socket.on('browserWindowSetAutoHideCursor', function (id, autoHide) {
        getWindowById(id).setAutoHideCursor(autoHide);
    });
    socket.on('browserWindowSetVibrancy', function (id, type) {
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