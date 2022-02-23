"use strict";
const electron_1 = require("electron");
const browserView_1 = require("./browserView");
const path = require('path');
const windows = (global['browserWindows'] = global['browserWindows'] || []);
const readyToShowWindowsIds = (global['readyToShowWindowsIds'] = global['readyToShowWindowsIds'] || []);
const proxyToCredentialsMap = (global['proxyToCredentialsMap'] = global['proxyToCredentialsMap'] || []);
let window, lastOptions, electronSocket;
module.exports = (socket, app) => {
    electronSocket = socket;
    app.on('login', (event, webContents, request, authInfo, callback) => {
        if (authInfo.isProxy) {
            let proxy = `${authInfo.host}:${authInfo.port}`;
            if (proxy in proxyToCredentialsMap && proxyToCredentialsMap[proxy].split(':').length === 2) {
                event.preventDefault();
                let user = proxyToCredentialsMap[proxy].split(':')[0];
                let pass = proxyToCredentialsMap[proxy].split(':')[1];
                callback(user, pass);
            }
        }
    });
    socket.on('register-browserWindow-ready-to-show', (id) => {
        const index = readyToShowWindowsIds.indexOf(id);
        if (index > -1) {
            readyToShowWindowsIds.splice(index, 1);
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
            options = { ...options, webPreferences: { ...options.webPreferences, nodeIntegration: true, contextIsolation: false } };
        }
        else if (!options.webPreferences) {
            options = { ...options, webPreferences: { nodeIntegration: true, contextIsolation: false } };
        }
        // we dont want to recreate the window when watch is ready.
        if (app.commandLine.hasSwitch('watch') && app['mainWindowURL'] === loadUrl) {
            window = app['mainWindow'];
            if (window) {
                window.reload();
                if (windows.findIndex(i => i.id == window.id) == -1) {
                    windows.push(window);
                }
                electronSocket.emit('BrowserWindowCreated', window.id);
                return;
            }
        }
        else {
            window = new electron_1.BrowserWindow(options);
        }
        const thisWindow = window;
        if (options.proxy) {
            thisWindow.webContents.session.setProxy({ proxyRules: options.proxy });
        }
        if (options.proxy && options.proxyCredentials) {
            proxyToCredentialsMap[options.proxy] = options.proxyCredentials;
        }
        thisWindow.on('ready-to-show', () => {
            const index = readyToShowWindowsIds.indexOf(thisWindow.id);
            if (index > -1) {
                readyToShowWindowsIds.splice(index, 1);
            }
            else {
                readyToShowWindowsIds.push(thisWindow.id);
            }
        });
        lastOptions = options;
        thisWindow.on('closed', (sender) => {
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
        // this seems dangerous to assume
        app.on('activate', () => {
            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            if (window === null && lastOptions) {
                window = new electron_1.BrowserWindow(lastOptions);
            }
        });
        if (loadUrl) {
            thisWindow.loadURL(loadUrl);
        }
        if (app.commandLine.hasSwitch('clear-cache') &&
            app.commandLine.getSwitchValue('clear-cache')) {
            thisWindow.webContents.session.clearCache();
            console.log('auto clear-cache active for new window.');
        }
        // set main window url
        if (app['mainWindowURL'] == undefined || app['mainWindowURL'] == "") {
            app['mainWindowURL'] = loadUrl;
            app['mainWindow'] = thisWindow;
        }
        windows.push(thisWindow);
        electronSocket.emit('BrowserWindowCreated', thisWindow.id);
    });
    socket.on('browserWindowDestroy', (id) => {
        const w = getWindowById(id);
        if (w)
            w.destroy();
    });
    socket.on('browserWindowClose', (id) => {
        const w = getWindowById(id);
        if (w)
            w.close();
    });
    socket.on('browserWindowFocus', (id) => {
        const w = getWindowById(id);
        if (w)
            w.focus();
    });
    socket.on('browserWindowBlur', (id) => {
        const w = getWindowById(id);
        if (w)
            w.blur();
    });
    socket.on('browserWindowIsFocused', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isFocused = w.isFocused();
            electronSocket.emit('browserWindow-isFocused-completed', isFocused);
        }
    });
    socket.on('browserWindowIsDestroyed', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isDestroyed = w.isDestroyed();
            electronSocket.emit('browserWindow-isDestroyed-completed', isDestroyed);
        }
    });
    socket.on('browserWindowShow', (id) => {
        const w = getWindowById(id);
        if (w)
            w.show();
    });
    socket.on('browserWindowShowInactive', (id) => {
        const w = getWindowById(id);
        if (w)
            w.showInactive();
    });
    socket.on('browserWindowHide', (id) => {
        const w = getWindowById(id);
        if (w)
            w.hide();
    });
    socket.on('browserWindowIsVisible', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isVisible = w.isVisible();
            electronSocket.emit('browserWindow-isVisible-completed', isVisible);
        }
    });
    socket.on('browserWindowIsModal', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isModal = w.isModal();
            electronSocket.emit('browserWindow-isModal-completed', isModal);
        }
    });
    socket.on('browserWindowMaximize', (id) => {
        const w = getWindowById(id);
        if (w)
            w.maximize();
    });
    socket.on('browserWindowUnmaximize', (id) => {
        const w = getWindowById(id);
        if (w)
            w.unmaximize();
    });
    socket.on('browserWindowIsMaximized', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isMaximized = w.isMaximized();
            electronSocket.emit('browserWindow-isMaximized-completed', isMaximized);
        }
    });
    socket.on('browserWindowMinimize', (id) => {
        const w = getWindowById(id);
        if (w)
            w.minimize();
    });
    socket.on('browserWindowRestore', (id) => {
        const w = getWindowById(id);
        if (w)
            w.restore();
    });
    socket.on('browserWindowIsMinimized', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isMinimized = w.isMinimized();
            electronSocket.emit('browserWindow-isMinimized-completed', isMinimized);
        }
    });
    socket.on('browserWindowSetFullScreen', (id, fullscreen) => {
        const w = getWindowById(id);
        if (w)
            w.setFullScreen(fullscreen);
    });
    socket.on('browserWindowIsFullScreen', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isFullScreen = w.isFullScreen();
            electronSocket.emit('browserWindow-isFullScreen-completed', isFullScreen);
        }
    });
    socket.on('browserWindowSetAspectRatio', (id, aspectRatio, extraSize) => {
        const w = getWindowById(id);
        if (w)
            w.setAspectRatio(aspectRatio, extraSize);
    });
    socket.on('browserWindowPreviewFile', (id, path, displayname) => {
        const w = getWindowById(id);
        if (w)
            w.previewFile(path, displayname);
    });
    socket.on('browserWindowCloseFilePreview', (id) => {
        const w = getWindowById(id);
        if (w)
            w.closeFilePreview();
    });
    socket.on('browserWindowSetBounds', (id, bounds, animate) => {
        const w = getWindowById(id);
        if (w)
            w.setBounds(bounds, animate);
    });
    socket.on('browserWindowGetBounds', (id) => {
        const w = getWindowById(id);
        if (w) {
            const rectangle = w.getBounds();
            electronSocket.emit('browserWindow-getBounds-completed', rectangle);
        }
    });
    socket.on('browserWindowSetContentBounds', (id, bounds, animate) => {
        const w = getWindowById(id);
        if (w)
            w.setContentBounds(bounds, animate);
    });
    socket.on('browserWindowGetContentBounds', (id) => {
        const w = getWindowById(id);
        if (w) {
            const rectangle = w.getContentBounds();
            electronSocket.emit('browserWindow-getContentBounds-completed', rectangle);
        }
    });
    socket.on('browserWindowSetSize', (id, width, height, animate) => {
        const w = getWindowById(id);
        if (w)
            w.setSize(width, height, animate);
    });
    socket.on('browserWindowGetSize', (id) => {
        const w = getWindowById(id);
        if (w) {
            const size = w.getSize();
            electronSocket.emit('browserWindow-getSize-completed', size);
        }
    });
    socket.on('browserWindowSetContentSize', (id, width, height, animate) => {
        const w = getWindowById(id);
        if (w)
            w.setContentSize(width, height, animate);
    });
    socket.on('browserWindowGetContentSize', (id) => {
        const w = getWindowById(id);
        if (w) {
            const size = getWindowById(id).getContentSize();
            electronSocket.emit('browserWindow-getContentSize-completed', size);
        }
    });
    socket.on('browserWindowSetMinimumSize', (id, width, height) => {
        const w = getWindowById(id);
        if (w)
            w.setMinimumSize(width, height);
    });
    socket.on('browserWindowGetMinimumSize', (id) => {
        const w = getWindowById(id);
        if (w) {
            const size = w.getMinimumSize();
            electronSocket.emit('browserWindow-getMinimumSize-completed', size);
        }
    });
    socket.on('browserWindowSetMaximumSize', (id, width, height) => {
        const w = getWindowById(id);
        if (w)
            w.setMaximumSize(width, height);
    });
    socket.on('browserWindowGetMaximumSize', (id) => {
        const w = getWindowById(id);
        if (w) {
            const size = w.getMaximumSize();
            electronSocket.emit('browserWindow-getMaximumSize-completed', size);
        }
    });
    socket.on('browserWindowSetResizable', (id, resizable) => {
        const w = getWindowById(id);
        if (w)
            w.setResizable(resizable);
    });
    socket.on('browserWindowIsResizable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const resizable = w.isResizable();
            electronSocket.emit('browserWindow-isResizable-completed', resizable);
        }
    });
    socket.on('browserWindowSetMovable', (id, movable) => {
        const w = getWindowById(id);
        if (w)
            w.setMovable(movable);
    });
    socket.on('browserWindowIsMovable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const movable = w.isMovable();
            electronSocket.emit('browserWindow-isMovable-completed', movable);
        }
    });
    socket.on('browserWindowSetMinimizable', (id, minimizable) => {
        const w = getWindowById(id);
        if (w)
            w.setMinimizable(minimizable);
    });
    socket.on('browserWindowIsMinimizable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const minimizable = w.isMinimizable();
            electronSocket.emit('browserWindow-isMinimizable-completed', minimizable);
        }
    });
    socket.on('browserWindowSetMaximizable', (id, maximizable) => {
        const w = getWindowById(id);
        if (w)
            w.setMaximizable(maximizable);
    });
    socket.on('browserWindowIsMaximizable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const maximizable = w.isMaximizable();
            electronSocket.emit('browserWindow-isMaximizable-completed', maximizable);
        }
    });
    socket.on('browserWindowSetFullScreenable', (id, fullscreenable) => {
        const w = getWindowById(id);
        if (w)
            w.setFullScreenable(fullscreenable);
    });
    socket.on('browserWindowIsFullScreenable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const fullscreenable = w.isFullScreenable();
            electronSocket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
        }
    });
    socket.on('browserWindowSetClosable', (id, closable) => {
        const w = getWindowById(id);
        if (w)
            w.setClosable(closable);
    });
    socket.on('browserWindowIsClosable', (id) => {
        const w = getWindowById(id);
        if (w) {
            const closable = w.isClosable();
            electronSocket.emit('browserWindow-isClosable-completed', closable);
        }
    });
    socket.on('browserWindowSetAlwaysOnTop', (id, flag, level, relativeLevel) => {
        const w = getWindowById(id);
        if (w)
            w.setAlwaysOnTop(flag, level, relativeLevel);
    });
    socket.on('browserWindowIsAlwaysOnTop', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isAlwaysOnTop = w.isAlwaysOnTop();
            electronSocket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
        }
    });
    socket.on('browserWindowCenter', (id) => {
        const w = getWindowById(id);
        if (w)
            w.center();
    });
    socket.on('browserWindowSetPosition', (id, x, y, animate) => {
        const w = getWindowById(id);
        if (w)
            w.setPosition(x, y, animate);
    });
    socket.on('browserWindowGetPosition', (id) => {
        const w = getWindowById(id);
        if (w) {
            const position = w.getPosition();
            electronSocket.emit('browserWindow-getPosition-completed', position);
        }
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        const w = getWindowById(id);
        if (w)
            w.setTitle(title);
    });
    socket.on('browserWindowGetTitle', (id) => {
        const w = getWindowById(id);
        if (w) {
            const title = w.getTitle();
            electronSocket.emit('browserWindow-getTitle-completed', title);
        }
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        const w = getWindowById(id);
        if (w)
            w.setTitle(title);
    });
    socket.on('browserWindowSetSheetOffset', (id, offsetY, offsetX) => {
        const w = getWindowById(id);
        if (w) {
            if (offsetX) {
                w.setSheetOffset(offsetY, offsetX);
            }
            else {
                w.setSheetOffset(offsetY);
            }
        }
    });
    socket.on('browserWindowFlashFrame', (id, flag) => {
        const w = getWindowById(id);
        if (w)
            w.flashFrame(flag);
    });
    socket.on('browserWindowSetSkipTaskbar', (id, skip) => {
        const w = getWindowById(id);
        if (w)
            w.setSkipTaskbar(skip);
    });
    socket.on('browserWindowSetKiosk', (id, flag) => {
        const w = getWindowById(id);
        if (w)
            w.setKiosk(flag);
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
        const w = getWindowById(id);
        if (w)
            w.setRepresentedFilename(filename);
    });
    socket.on('browserWindowGetRepresentedFilename', (id) => {
        const w = getWindowById(id);
        if (w) {
            const pathname = w.getRepresentedFilename();
            electronSocket.emit('browserWindow-getRepresentedFilename-completed', pathname);
        }
    });
    socket.on('browserWindowSetDocumentEdited', (id, edited) => {
        const w = getWindowById(id);
        if (w)
            w.setDocumentEdited(edited);
    });
    socket.on('browserWindowIsDocumentEdited', (id) => {
        const w = getWindowById(id);
        if (w) {
            const edited = w.isDocumentEdited();
            electronSocket.emit('browserWindow-isDocumentEdited-completed', edited);
        }
    });
    socket.on('browserWindowFocusOnWebView', (id) => {
        const w = getWindowById(id);
        if (w)
            w.focusOnWebView();
    });
    socket.on('browserWindowBlurWebView', (id) => {
        const w = getWindowById(id);
        if (w)
            w.blurWebView();
    });
    socket.on('browserWindowLoadURL', (id, url, options) => {
        const w = getWindowById(id);
        if (w)
            w.loadURL(url, options);
    });
    socket.on('browserWindowReload', (id) => {
        const w = getWindowById(id);
        if (w)
            w.reload();
    });
    socket.on('browserWindowSetMenu', (id, menuItems) => {
        let menu = null;
        if (menuItems) {
            menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                electronSocket.emit('windowMenuItemClicked', id);
            });
        }
        const w = getWindowById(id);
        if (w)
            w.setMenu(menu);
    });
    socket.on('browserWindowRemoveMenu', (id) => {
        const w = getWindowById(id);
        if (w)
            w.removeMenu();
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
        const w = getWindowById(id);
        if (w)
            w.setProgressBar(progress);
    });
    socket.on('browserWindowSetProgressBar', (id, progress, options) => {
        const w = getWindowById(id);
        if (w)
            w.setProgressBar(progress, options);
    });
    socket.on('browserWindowSetHasShadow', (id, hasShadow) => {
        const w = getWindowById(id);
        if (w)
            w.setHasShadow(hasShadow);
    });
    socket.on('browserWindowHasShadow', (id) => {
        const w = getWindowById(id);
        if (w) {
            const hasShadow = w.hasShadow();
            electronSocket.emit('browserWindow-hasShadow-completed', hasShadow);
        }
    });
    socket.on('browserWindowSetThumbarButtons', (id, thumbarButtons) => {
        const w = getWindowById(id);
        if (w) {
            thumbarButtons.forEach(thumbarButton => {
                const imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());
                thumbarButton.icon = electron_1.nativeImage.createFromPath(imagePath);
                thumbarButton.click = () => {
                    electronSocket.emit('thumbarButtonClicked', thumbarButton['id']);
                };
            });
            const success = w.setThumbarButtons(thumbarButtons);
            electronSocket.emit('browserWindowSetThumbarButtons-completed', success);
        }
    });
    socket.on('browserWindowSetThumbnailClip', (id, rectangle) => {
        const w = getWindowById(id);
        if (w)
            w.setThumbnailClip(rectangle);
    });
    socket.on('browserWindowSetThumbnailToolTip', (id, toolTip) => {
        const w = getWindowById(id);
        if (w)
            w.setThumbnailToolTip(toolTip);
    });
    socket.on('browserWindowSetAppDetails', (id, options) => {
        const w = getWindowById(id);
        if (w)
            w.setAppDetails(options);
    });
    socket.on('browserWindowShowDefinitionForSelection', (id) => {
        const w = getWindowById(id);
        if (w)
            w.showDefinitionForSelection();
    });
    socket.on('browserWindowSetAutoHideMenuBar', (id, hide) => {
        const w = getWindowById(id);
        if (w)
            w.setAutoHideMenuBar(hide);
    });
    socket.on('browserWindowIsMenuBarAutoHide', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isMenuBarAutoHide = w.isMenuBarAutoHide();
            electronSocket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
        }
    });
    socket.on('browserWindowSetMenuBarVisibility', (id, visible) => {
        const w = getWindowById(id);
        if (w)
            w.setMenuBarVisibility(visible);
    });
    socket.on('browserWindowIsMenuBarVisible', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isMenuBarVisible = w.isMenuBarVisible();
            electronSocket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
        }
    });
    socket.on('browserWindowSetVisibleOnAllWorkspaces', (id, visible) => {
        const w = getWindowById(id);
        if (w)
            w.setVisibleOnAllWorkspaces(visible);
    });
    socket.on('browserWindowIsVisibleOnAllWorkspaces', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isVisibleOnAllWorkspaces = w.isVisibleOnAllWorkspaces();
            electronSocket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
        }
    });
    socket.on('browserWindowSetIgnoreMouseEvents', (id, ignore) => {
        const w = getWindowById(id);
        if (w)
            w.setIgnoreMouseEvents(ignore);
    });
    socket.on('browserWindowSetContentProtection', (id, enable) => {
        const w = getWindowById(id);
        if (w)
            w.setContentProtection(enable);
    });
    socket.on('browserWindowSetFocusable', (id, focusable) => {
        getWindowById(id).setFocusable(focusable);
    });
    socket.on('browserWindowSetParentWindow', (id, parent) => {
        const browserWindow = electron_1.BrowserWindow.fromId(parent.id);
        const w = getWindowById(id);
        if (w)
            w.setParentWindow(browserWindow);
    });
    socket.on('browserWindowGetParentWindow', (id) => {
        const w = getWindowById(id);
        if (w) {
            const browserWindow = w.getParentWindow();
            electronSocket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
        }
    });
    socket.on('browserWindowGetChildWindows', (id) => {
        const w = getWindowById(id);
        if (w) {
            const browserWindows = w.getChildWindows();
            const ids = [];
            browserWindows.forEach(x => {
                ids.push(x.id);
            });
            electronSocket.emit('browserWindow-getChildWindows-completed', ids);
        }
    });
    socket.on('browserWindowSetAutoHideCursor', (id, autoHide) => {
        const w = getWindowById(id);
        if (w)
            w.setAutoHideCursor(autoHide);
    });
    socket.on('browserWindowSetVibrancy', (id, type) => {
        const w = getWindowById(id);
        if (w)
            w.setVibrancy(type);
    });
    socket.on('browserWindow-setBrowserView', (id, browserViewId) => {
        const w = getWindowById(id);
        if (w)
            w.setBrowserView(browserView_1.browserViewMediateService(browserViewId));
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