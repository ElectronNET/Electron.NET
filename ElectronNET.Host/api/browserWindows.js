"use strict";
const electron_1 = require("electron");
const browserView_1 = require("./browserView");
const path = require('path');
const windows = (global['browserWindows'] = global['browserWindows'] || []);
let readyToShowWindowsIds = [];
let window, lastOptions, electronSocket;
let mainWindowURL;
module.exports = (socket, app) => {
    electronSocket = socket;
    socket.on('register-browserWindow-ready-to-show', (id) => {
        if (readyToShowWindowsIds.includes(id)) {
            readyToShowWindowsIds = readyToShowWindowsIds.filter(value => value !== id);
            electronSocket.emit('browserWindow-ready-to-show' + id);
        }
        getWindowById(id).on('ready-to-show', () => {
            readyToShowWindowsIds.push(id);
            electronSocket.emit('browserWindow-ready-to-show' + id);
        });
    });
    socket.on('register-browserWindow-page-title-updated', (id) => {
        getWindowById(id).on('page-title-updated', (event, title) => {
            electronSocket.emit('browserWindow-page-title-updated' + id, title);
        });
    });
    socket.on('register-browserWindow-close', (id) => {
        getWindowById(id).on('close', () => {
            electronSocket.emit('browserWindow-close' + id);
        });
    });
    socket.on('register-browserWindow-closed', (id) => {
        getWindowById(id).on('closed', () => {
            electronSocket.emit('browserWindow-closed' + id);
        });
    });
    socket.on('register-browserWindow-session-end', (id) => {
        getWindowById(id).on('session-end', () => {
            electronSocket.emit('browserWindow-session-end' + id);
        });
    });
    socket.on('register-browserWindow-unresponsive', (id) => {
        getWindowById(id).on('unresponsive', () => {
            electronSocket.emit('browserWindow-unresponsive' + id);
        });
    });
    socket.on('register-browserWindow-responsive', (id) => {
        getWindowById(id).on('responsive', () => {
            electronSocket.emit('browserWindow-responsive' + id);
        });
    });
    socket.on('register-browserWindow-blur', (id) => {
        getWindowById(id).on('blur', () => {
            electronSocket.emit('browserWindow-blur' + id);
        });
    });
    socket.on('register-browserWindow-focus', (id) => {
        getWindowById(id).on('focus', () => {
            electronSocket.emit('browserWindow-focus' + id);
        });
    });
    socket.on('register-browserWindow-show', (id) => {
        getWindowById(id).on('show', () => {
            electronSocket.emit('browserWindow-show' + id);
        });
    });
    socket.on('register-browserWindow-hide', (id) => {
        getWindowById(id).on('hide', () => {
            electronSocket.emit('browserWindow-hide' + id);
        });
    });
    socket.on('register-browserWindow-maximize', (id) => {
        getWindowById(id).on('maximize', () => {
            electronSocket.emit('browserWindow-maximize' + id);
        });
    });
    socket.on('register-browserWindow-unmaximize', (id) => {
        getWindowById(id).on('unmaximize', () => {
            electronSocket.emit('browserWindow-unmaximize' + id);
        });
    });
    socket.on('register-browserWindow-minimize', (id) => {
        getWindowById(id).on('minimize', () => {
            electronSocket.emit('browserWindow-minimize' + id);
        });
    });
    socket.on('register-browserWindow-restore', (id) => {
        getWindowById(id).on('restore', () => {
            electronSocket.emit('browserWindow-restore' + id);
        });
    });
    socket.on('register-browserWindow-resize', (id) => {
        getWindowById(id).on('resize', () => {
            electronSocket.emit('browserWindow-resize' + id);
        });
    });
    socket.on('register-browserWindow-move', (id) => {
        getWindowById(id).on('move', () => {
            electronSocket.emit('browserWindow-move' + id);
        });
    });
    socket.on('register-browserWindow-moved', (id) => {
        getWindowById(id).on('moved', () => {
            electronSocket.emit('browserWindow-moved' + id);
        });
    });
    socket.on('register-browserWindow-enter-full-screen', (id) => {
        getWindowById(id).on('enter-full-screen', () => {
            electronSocket.emit('browserWindow-enter-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-full-screen', (id) => {
        getWindowById(id).on('leave-full-screen', () => {
            electronSocket.emit('browserWindow-leave-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-enter-html-full-screen', (id) => {
        getWindowById(id).on('enter-html-full-screen', () => {
            electronSocket.emit('browserWindow-enter-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-html-full-screen', (id) => {
        getWindowById(id).on('leave-html-full-screen', () => {
            electronSocket.emit('browserWindow-leave-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-app-command', (id) => {
        getWindowById(id).on('app-command', (event, command) => {
            electronSocket.emit('browserWindow-app-command' + id, command);
        });
    });
    socket.on('register-browserWindow-scroll-touch-begin', (id) => {
        getWindowById(id).on('scroll-touch-begin', () => {
            electronSocket.emit('browserWindow-scroll-touch-begin' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-end', (id) => {
        getWindowById(id).on('scroll-touch-end', () => {
            electronSocket.emit('browserWindow-scroll-touch-end' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-edge', (id) => {
        getWindowById(id).on('scroll-touch-edge', () => {
            electronSocket.emit('browserWindow-scroll-touch-edge' + id);
        });
    });
    socket.on('register-browserWindow-swipe', (id) => {
        getWindowById(id).on('swipe', (event, direction) => {
            electronSocket.emit('browserWindow-swipe' + id, direction);
        });
    });
    socket.on('register-browserWindow-sheet-begin', (id) => {
        getWindowById(id).on('sheet-begin', () => {
            electronSocket.emit('browserWindow-sheet-begin' + id);
        });
    });
    socket.on('register-browserWindow-sheet-end', (id) => {
        getWindowById(id).on('sheet-end', () => {
            electronSocket.emit('browserWindow-sheet-end' + id);
        });
    });
    socket.on('register-browserWindow-new-window-for-tab', (id) => {
        getWindowById(id).on('new-window-for-tab', () => {
            electronSocket.emit('browserWindow-new-window-for-tab' + id);
        });
    });
    socket.on('createBrowserWindow', (options, loadUrl) => {
        if (options.webPreferences && !('nodeIntegration' in options.webPreferences)) {
            options = { ...options, webPreferences: { ...options.webPreferences, nodeIntegration: true } };
        }
        else if (!options.webPreferences) {
            options = { ...options, webPreferences: { nodeIntegration: true } };
        }
        // we dont want to recreate the window when watch is ready.
        if (app.commandLine.hasSwitch('watch') && app['mainWindowURL'] === loadUrl) {
            window = app['mainWindow'];
            if (window) {
                window.reload();
                windows.push(window);
                electronSocket.emit('BrowserWindowCreated', window.id);
                return;
            }
        }
        else {
            window = new electron_1.BrowserWindow(options);
        }
        window.on('ready-to-show', () => {
            if (readyToShowWindowsIds.includes(window.id)) {
                readyToShowWindowsIds = readyToShowWindowsIds.filter(value => value !== window.id);
            }
            else {
                readyToShowWindowsIds.push(window.id);
            }
        });
        lastOptions = options;
        window.on('closed', (sender) => {
            for (let index = 0; index < windows.length; index++) {
                const windowItem = windows[index];
                try {
                    windowItem.id;
                }
                catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);
                        const ids = [];
                        windows.forEach(x => ids.push(x.id));
                        electronSocket.emit('BrowserWindowClosed', ids);
                    }
                }
            }
        });
        app.on('activate', () => {
            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            if (window === null && lastOptions) {
                window = new electron_1.BrowserWindow(lastOptions);
            }
        });
        if (loadUrl) {
            window.loadURL(loadUrl);
        }
        if (app.commandLine.hasSwitch('clear-cache') &&
            app.commandLine.getSwitchValue('clear-cache')) {
            window.webContents.session.clearCache();
            console.log('auto clear-cache active for new window.');
        }
        // set main window url
        if (app['mainWindowURL'] == undefined || app['mainWindowURL'] == "") {
            app['mainWindowURL'] = loadUrl;
            app['mainWindow'] = window;
        }
        windows.push(window);
        electronSocket.emit('BrowserWindowCreated', window.id);
    });
    socket.on('browserWindowDestroy', (id) => {
        getWindowById(id).destroy();
    });
    socket.on('browserWindowClose', (id) => {
        getWindowById(id).close();
    });
    socket.on('browserWindowFocus', (id) => {
        getWindowById(id).focus();
    });
    socket.on('browserWindowBlur', (id) => {
        getWindowById(id).blur();
    });
    socket.on('browserWindowIsFocused', (id) => {
        const isFocused = getWindowById(id).isFocused();
        electronSocket.emit('browserWindow-isFocused-completed', isFocused);
    });
    socket.on('browserWindowIsDestroyed', (id) => {
        const isDestroyed = getWindowById(id).isDestroyed();
        electronSocket.emit('browserWindow-isDestroyed-completed', isDestroyed);
    });
    socket.on('browserWindowShow', (id) => {
        getWindowById(id).show();
    });
    socket.on('browserWindowShowInactive', (id) => {
        getWindowById(id).showInactive();
    });
    socket.on('browserWindowHide', (id) => {
        getWindowById(id).hide();
    });
    socket.on('browserWindowIsVisible', (id) => {
        const isVisible = getWindowById(id).isVisible();
        electronSocket.emit('browserWindow-isVisible-completed', isVisible);
    });
    socket.on('browserWindowIsModal', (id) => {
        const isModal = getWindowById(id).isModal();
        electronSocket.emit('browserWindow-isModal-completed', isModal);
    });
    socket.on('browserWindowMaximize', (id) => {
        getWindowById(id).maximize();
    });
    socket.on('browserWindowUnmaximize', (id) => {
        getWindowById(id).unmaximize();
    });
    socket.on('browserWindowIsMaximized', (id) => {
        const isMaximized = getWindowById(id).isMaximized();
        electronSocket.emit('browserWindow-isMaximized-completed', isMaximized);
    });
    socket.on('browserWindowMinimize', (id) => {
        getWindowById(id).minimize();
    });
    socket.on('browserWindowRestore', (id) => {
        getWindowById(id).restore();
    });
    socket.on('browserWindowIsMinimized', (id) => {
        const isMinimized = getWindowById(id).isMinimized();
        electronSocket.emit('browserWindow-isMinimized-completed', isMinimized);
    });
    socket.on('browserWindowSetFullScreen', (id, fullscreen) => {
        getWindowById(id).setFullScreen(fullscreen);
    });
    socket.on('browserWindowIsFullScreen', (id) => {
        const isFullScreen = getWindowById(id).isFullScreen();
        electronSocket.emit('browserWindow-isFullScreen-completed', isFullScreen);
    });
    socket.on('browserWindowSetAspectRatio', (id, aspectRatio, extraSize) => {
        getWindowById(id).setAspectRatio(aspectRatio, extraSize);
    });
    socket.on('browserWindowPreviewFile', (id, path, displayname) => {
        getWindowById(id).previewFile(path, displayname);
    });
    socket.on('browserWindowCloseFilePreview', (id) => {
        getWindowById(id).closeFilePreview();
    });
    socket.on('browserWindowSetBounds', (id, bounds, animate) => {
        getWindowById(id).setBounds(bounds, animate);
    });
    socket.on('browserWindowGetBounds', (id) => {
        const rectangle = getWindowById(id).getBounds();
        electronSocket.emit('browserWindow-getBounds-completed', rectangle);
    });
    socket.on('browserWindowSetContentBounds', (id, bounds, animate) => {
        getWindowById(id).setContentBounds(bounds, animate);
    });
    socket.on('browserWindowGetContentBounds', (id) => {
        const rectangle = getWindowById(id).getContentBounds();
        electronSocket.emit('browserWindow-getContentBounds-completed', rectangle);
    });
    socket.on('browserWindowSetSize', (id, width, height, animate) => {
        getWindowById(id).setSize(width, height, animate);
    });
    socket.on('browserWindowGetSize', (id) => {
        const size = getWindowById(id).getSize();
        electronSocket.emit('browserWindow-getSize-completed', size);
    });
    socket.on('browserWindowSetContentSize', (id, width, height, animate) => {
        getWindowById(id).setContentSize(width, height, animate);
    });
    socket.on('browserWindowGetContentSize', (id) => {
        const size = getWindowById(id).getContentSize();
        electronSocket.emit('browserWindow-getContentSize-completed', size);
    });
    socket.on('browserWindowSetMinimumSize', (id, width, height) => {
        getWindowById(id).setMinimumSize(width, height);
    });
    socket.on('browserWindowGetMinimumSize', (id) => {
        const size = getWindowById(id).getMinimumSize();
        electronSocket.emit('browserWindow-getMinimumSize-completed', size);
    });
    socket.on('browserWindowSetMaximumSize', (id, width, height) => {
        getWindowById(id).setMaximumSize(width, height);
    });
    socket.on('browserWindowGetMaximumSize', (id) => {
        const size = getWindowById(id).getMaximumSize();
        electronSocket.emit('browserWindow-getMaximumSize-completed', size);
    });
    socket.on('browserWindowSetResizable', (id, resizable) => {
        getWindowById(id).setResizable(resizable);
    });
    socket.on('browserWindowIsResizable', (id) => {
        const resizable = getWindowById(id).isResizable();
        electronSocket.emit('browserWindow-isResizable-completed', resizable);
    });
    socket.on('browserWindowSetMovable', (id, movable) => {
        getWindowById(id).setMovable(movable);
    });
    socket.on('browserWindowIsMovable', (id) => {
        const movable = getWindowById(id).isMovable();
        electronSocket.emit('browserWindow-isMovable-completed', movable);
    });
    socket.on('browserWindowSetMinimizable', (id, minimizable) => {
        getWindowById(id).setMinimizable(minimizable);
    });
    socket.on('browserWindowIsMinimizable', (id) => {
        const minimizable = getWindowById(id).isMinimizable();
        electronSocket.emit('browserWindow-isMinimizable-completed', minimizable);
    });
    socket.on('browserWindowSetMaximizable', (id, maximizable) => {
        getWindowById(id).setMaximizable(maximizable);
    });
    socket.on('browserWindowIsMaximizable', (id) => {
        const maximizable = getWindowById(id).isMaximizable();
        electronSocket.emit('browserWindow-isMaximizable-completed', maximizable);
    });
    socket.on('browserWindowSetFullScreenable', (id, fullscreenable) => {
        getWindowById(id).setFullScreenable(fullscreenable);
    });
    socket.on('browserWindowIsFullScreenable', (id) => {
        const fullscreenable = getWindowById(id).isFullScreenable();
        electronSocket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
    });
    socket.on('browserWindowSetClosable', (id, closable) => {
        getWindowById(id).setClosable(closable);
    });
    socket.on('browserWindowIsClosable', (id) => {
        const closable = getWindowById(id).isClosable();
        electronSocket.emit('browserWindow-isClosable-completed', closable);
    });
    socket.on('browserWindowSetAlwaysOnTop', (id, flag, level, relativeLevel) => {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });
    socket.on('browserWindowIsAlwaysOnTop', (id) => {
        const isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();
        electronSocket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
    });
    socket.on('browserWindowCenter', (id) => {
        getWindowById(id).center();
    });
    socket.on('browserWindowSetPosition', (id, x, y, animate) => {
        getWindowById(id).setPosition(x, y, animate);
    });
    socket.on('browserWindowGetPosition', (id) => {
        const position = getWindowById(id).getPosition();
        electronSocket.emit('browserWindow-getPosition-completed', position);
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindowGetTitle', (id) => {
        const title = getWindowById(id).getTitle();
        electronSocket.emit('browserWindow-getTitle-completed', title);
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    socket.on('browserWindowSetSheetOffset', (id, offsetY, offsetX) => {
        if (offsetX) {
            getWindowById(id).setSheetOffset(offsetY, offsetX);
        }
        else {
            getWindowById(id).setSheetOffset(offsetY);
        }
    });
    socket.on('browserWindowFlashFrame', (id, flag) => {
        getWindowById(id).flashFrame(flag);
    });
    socket.on('browserWindowSetSkipTaskbar', (id, skip) => {
        getWindowById(id).setSkipTaskbar(skip);
    });
    socket.on('browserWindowSetKiosk', (id, flag) => {
        getWindowById(id).setKiosk(flag);
    });
    socket.on('browserWindowIsKiosk', (id) => {
        const isKiosk = getWindowById(id).isKiosk();
        electronSocket.emit('browserWindow-isKiosk-completed', isKiosk);
    });
    socket.on('browserWindowGetNativeWindowHandle', (id) => {
        const nativeWindowHandle = getWindowById(id).getNativeWindowHandle().readInt32LE(0).toString(16);
        electronSocket.emit('browserWindow-getNativeWindowHandle-completed', nativeWindowHandle);
    });
    socket.on('browserWindowSetRepresentedFilename', (id, filename) => {
        getWindowById(id).setRepresentedFilename(filename);
    });
    socket.on('browserWindowGetRepresentedFilename', (id) => {
        const pathname = getWindowById(id).getRepresentedFilename();
        electronSocket.emit('browserWindow-getRepresentedFilename-completed', pathname);
    });
    socket.on('browserWindowSetDocumentEdited', (id, edited) => {
        getWindowById(id).setDocumentEdited(edited);
    });
    socket.on('browserWindowIsDocumentEdited', (id) => {
        const edited = getWindowById(id).isDocumentEdited();
        electronSocket.emit('browserWindow-isDocumentEdited-completed', edited);
    });
    socket.on('browserWindowFocusOnWebView', (id) => {
        getWindowById(id).focusOnWebView();
    });
    socket.on('browserWindowBlurWebView', (id) => {
        getWindowById(id).blurWebView();
    });
    socket.on('browserWindowLoadURL', (id, url, options) => {
        getWindowById(id).loadURL(url, options);
    });
    socket.on('browserWindowReload', (id) => {
        getWindowById(id).reload();
    });
    socket.on('browserWindowSetMenu', (id, menuItems) => {
        let menu = null;
        if (menuItems) {
            menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                electronSocket.emit('windowMenuItemClicked', id);
            });
        }
        getWindowById(id).setMenu(menu);
    });
    socket.on('browserWindowRemoveMenu', (id) => {
        getWindowById(id).removeMenu();
    });
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if (item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            if ('id' in item && item.id) {
                item.click = () => { callback(item.id); };
            }
        });
    }
    socket.on('browserWindowSetProgressBar', (id, progress) => {
        getWindowById(id).setProgressBar(progress);
    });
    socket.on('browserWindowSetProgressBar', (id, progress, options) => {
        getWindowById(id).setProgressBar(progress, options);
    });
    socket.on('browserWindowSetHasShadow', (id, hasShadow) => {
        getWindowById(id).setHasShadow(hasShadow);
    });
    socket.on('browserWindowHasShadow', (id) => {
        const hasShadow = getWindowById(id).hasShadow();
        electronSocket.emit('browserWindow-hasShadow-completed', hasShadow);
    });
    socket.on('browserWindowSetThumbarButtons', (id, thumbarButtons) => {
        thumbarButtons.forEach(thumbarButton => {
            const imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());
            thumbarButton.icon = electron_1.nativeImage.createFromPath(imagePath);
            thumbarButton.click = () => {
                electronSocket.emit('thumbarButtonClicked', thumbarButton['id']);
            };
        });
        const success = getWindowById(id).setThumbarButtons(thumbarButtons);
        electronSocket.emit('browserWindowSetThumbarButtons-completed', success);
    });
    socket.on('browserWindowSetThumbnailClip', (id, rectangle) => {
        getWindowById(id).setThumbnailClip(rectangle);
    });
    socket.on('browserWindowSetThumbnailToolTip', (id, toolTip) => {
        getWindowById(id).setThumbnailToolTip(toolTip);
    });
    socket.on('browserWindowSetAppDetails', (id, options) => {
        getWindowById(id).setAppDetails(options);
    });
    socket.on('browserWindowShowDefinitionForSelection', (id) => {
        getWindowById(id).showDefinitionForSelection();
    });
    socket.on('browserWindowSetAutoHideMenuBar', (id, hide) => {
        getWindowById(id).setAutoHideMenuBar(hide);
    });
    socket.on('browserWindowIsMenuBarAutoHide', (id) => {
        const isMenuBarAutoHide = getWindowById(id).isMenuBarAutoHide();
        electronSocket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
    });
    socket.on('browserWindowSetMenuBarVisibility', (id, visible) => {
        getWindowById(id).setMenuBarVisibility(visible);
    });
    socket.on('browserWindowIsMenuBarVisible', (id) => {
        const isMenuBarVisible = getWindowById(id).isMenuBarVisible();
        electronSocket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
    });
    socket.on('browserWindowSetVisibleOnAllWorkspaces', (id, visible) => {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });
    socket.on('browserWindowIsVisibleOnAllWorkspaces', (id) => {
        const isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();
        electronSocket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
    });
    socket.on('browserWindowSetIgnoreMouseEvents', (id, ignore) => {
        getWindowById(id).setIgnoreMouseEvents(ignore);
    });
    socket.on('browserWindowSetContentProtection', (id, enable) => {
        getWindowById(id).setContentProtection(enable);
    });
    socket.on('browserWindowSetFocusable', (id, focusable) => {
        getWindowById(id).setFocusable(focusable);
    });
    socket.on('browserWindowSetParentWindow', (id, parent) => {
        const browserWindow = electron_1.BrowserWindow.fromId(parent.id);
        getWindowById(id).setParentWindow(browserWindow);
    });
    socket.on('browserWindowGetParentWindow', (id) => {
        const browserWindow = getWindowById(id).getParentWindow();
        electronSocket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
    });
    socket.on('browserWindowGetChildWindows', (id) => {
        const browserWindows = getWindowById(id).getChildWindows();
        const ids = [];
        browserWindows.forEach(x => {
            ids.push(x.id);
        });
        electronSocket.emit('browserWindow-getChildWindows-completed', ids);
    });
    socket.on('browserWindowSetAutoHideCursor', (id, autoHide) => {
        getWindowById(id).setAutoHideCursor(autoHide);
    });
    socket.on('browserWindowSetVibrancy', (id, type) => {
        getWindowById(id).setVibrancy(type);
    });
    socket.on('browserWindowAddExtension', (path) => {
        const extensionName = electron_1.BrowserWindow.addExtension(path);
        electronSocket.emit('browserWindow-addExtension-completed', extensionName);
    });
    socket.on('browserWindowRemoveExtension', (name) => {
        electron_1.BrowserWindow.removeExtension(name);
    });
    socket.on('browserWindowGetExtensions', () => {
        const extensionsList = electron_1.BrowserWindow.getExtensions();
        const chromeExtensionInfo = [];
        Object.keys(extensionsList).forEach(key => {
            chromeExtensionInfo.push(extensionsList[key]);
        });
        electronSocket.emit('browserWindow-getExtensions-completed', chromeExtensionInfo);
    });
    socket.on('browserWindow-setBrowserView', (id, browserViewId) => {
        getWindowById(id).setBrowserView(browserView_1.browserViewMediateService(browserViewId));
    });
    function getWindowById(id) {
        for (let index = 0; index < windows.length; index++) {
            const element = windows[index];
            if (element.id === id) {
                return element;
            }
        }
    }
};
//# sourceMappingURL=browserWindows.js.map