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
        var _a;
        const index = readyToShowWindowsIds.indexOf(id);
        if (index > -1) {
            readyToShowWindowsIds.splice(index, 1);
            electronSocket.emit('browserWindow-ready-to-show' + id);
        }
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('ready-to-show', () => {
            readyToShowWindowsIds.push(id);
            electronSocket.emit('browserWindow-ready-to-show' + id);
        });
    });
    socket.on('register-browserWindow-page-title-updated', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('page-title-updated', (event, title) => {
            electronSocket.emit('browserWindow-page-title-updated' + id, title);
        });
    });
    socket.on('register-browserWindow-close', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('close', () => {
            electronSocket.emit('browserWindow-close' + id);
        });
    });
    socket.on('register-browserWindow-closed', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('closed', () => {
            electronSocket.emit('browserWindow-closed' + id);
        });
    });
    socket.on('register-browserWindow-session-end', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('session-end', () => {
            electronSocket.emit('browserWindow-session-end' + id);
        });
    });
    socket.on('register-browserWindow-unresponsive', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('unresponsive', () => {
            electronSocket.emit('browserWindow-unresponsive' + id);
        });
    });
    socket.on('register-browserWindow-responsive', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('responsive', () => {
            electronSocket.emit('browserWindow-responsive' + id);
        });
    });
    socket.on('register-browserWindow-blur', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('blur', () => {
            electronSocket.emit('browserWindow-blur' + id);
        });
    });
    socket.on('register-browserWindow-focus', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('focus', () => {
            electronSocket.emit('browserWindow-focus' + id);
        });
    });
    socket.on('register-browserWindow-show', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('show', () => {
            electronSocket.emit('browserWindow-show' + id);
        });
    });
    socket.on('register-browserWindow-hide', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('hide', () => {
            electronSocket.emit('browserWindow-hide' + id);
        });
    });
    socket.on('register-browserWindow-maximize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('maximize', () => {
            electronSocket.emit('browserWindow-maximize' + id);
        });
    });
    socket.on('register-browserWindow-unmaximize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('unmaximize', () => {
            electronSocket.emit('browserWindow-unmaximize' + id);
        });
    });
    socket.on('register-browserWindow-minimize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('minimize', () => {
            electronSocket.emit('browserWindow-minimize' + id);
        });
    });
    socket.on('register-browserWindow-restore', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('restore', () => {
            electronSocket.emit('browserWindow-restore' + id);
        });
    });
    socket.on('register-browserWindow-resize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('resize', () => {
            electronSocket.emit('browserWindow-resize' + id);
        });
    });
    socket.on('register-browserWindow-move', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('move', () => {
            electronSocket.emit('browserWindow-move' + id);
        });
    });
    socket.on('register-browserWindow-moved', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('moved', () => {
            electronSocket.emit('browserWindow-moved' + id);
        });
    });
    socket.on('register-browserWindow-enter-full-screen', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('enter-full-screen', () => {
            electronSocket.emit('browserWindow-enter-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-full-screen', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('leave-full-screen', () => {
            electronSocket.emit('browserWindow-leave-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-enter-html-full-screen', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('enter-html-full-screen', () => {
            electronSocket.emit('browserWindow-enter-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-leave-html-full-screen', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('leave-html-full-screen', () => {
            electronSocket.emit('browserWindow-leave-html-full-screen' + id);
        });
    });
    socket.on('register-browserWindow-app-command', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('app-command', (event, command) => {
            electronSocket.emit('browserWindow-app-command' + id, command);
        });
    });
    socket.on('register-browserWindow-scroll-touch-begin', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('scroll-touch-begin', () => {
            electronSocket.emit('browserWindow-scroll-touch-begin' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-end', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('scroll-touch-end', () => {
            electronSocket.emit('browserWindow-scroll-touch-end' + id);
        });
    });
    socket.on('register-browserWindow-scroll-touch-edge', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('scroll-touch-edge', () => {
            electronSocket.emit('browserWindow-scroll-touch-edge' + id);
        });
    });
    socket.on('register-browserWindow-swipe', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('swipe', (event, direction) => {
            electronSocket.emit('browserWindow-swipe' + id, direction);
        });
    });
    socket.on('register-browserWindow-sheet-begin', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('sheet-begin', () => {
            electronSocket.emit('browserWindow-sheet-begin' + id);
        });
    });
    socket.on('register-browserWindow-sheet-end', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('sheet-end', () => {
            electronSocket.emit('browserWindow-sheet-end' + id);
        });
    });
    socket.on('register-browserWindow-new-window-for-tab', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.on('new-window-for-tab', () => {
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
        if (options.parent) {
            options.parent = electron_1.BrowserWindow.fromId(options.parent.id);
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
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.destroy();
    });
    socket.on('browserWindowClose', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.close();
    });
    socket.on('browserWindowFocus', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.focus();
    });
    socket.on('browserWindowBlur', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.blur();
    });
    socket.on('browserWindowIsFocused', (id) => {
        var _a, _b;
        const isFocused = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isFocused()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isFocused-completed' + id, isFocused);
    });
    socket.on('browserWindowIsDestroyed', (id) => {
        const w = getWindowById(id);
        if (w) {
            const isDestroyed = w.isDestroyed();
            electronSocket.emit('browserWindow-isDestroyed-completed', isDestroyed);
        }
    });
    socket.on('browserWindowShow', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.show();
    });
    socket.on('browserWindowShowInactive', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.showInactive();
    });
    socket.on('browserWindowHide', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.hide();
    });
    socket.on('browserWindowIsVisible', (id) => {
        var _a, _b;
        const isVisible = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isVisible()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isVisible-completed' + id, isVisible);
    });
    socket.on('browserWindowIsModal', (id) => {
        var _a, _b;
        const isModal = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isModal()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isModal-completed' + id, isModal);
    });
    socket.on('browserWindowMaximize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.maximize();
    });
    socket.on('browserWindowUnmaximize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.unmaximize();
    });
    socket.on('browserWindowIsMaximized', (id) => {
        var _a, _b;
        const isMaximized = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMaximized()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMaximized-completed' + id, isMaximized);
    });
    socket.on('browserWindowMinimize', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.minimize();
    });
    socket.on('browserWindowRestore', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.restore();
    });
    socket.on('browserWindowIsMinimized', (id) => {
        var _a, _b;
        const isMinimized = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMinimized()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMinimized-completed' + id, isMinimized);
    });
    socket.on('browserWindowSetFullScreen', (id, fullscreen) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setFullScreen(fullscreen);
    });
    socket.on('browserWindowIsFullScreen', (id) => {
        var _a, _b;
        const isFullScreen = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isFullScreen()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isFullScreen-completed' + id, isFullScreen);
    });
    socket.on('browserWindowSetAspectRatio', (id, aspectRatio, extraSize) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setAspectRatio(aspectRatio, extraSize);
    });
    socket.on('browserWindowPreviewFile', (id, path, displayname) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.previewFile(path, displayname);
    });
    socket.on('browserWindowCloseFilePreview', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.closeFilePreview();
    });
    socket.on('browserWindowSetBounds', (id, bounds, animate) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setBounds(bounds, animate);
    });
    socket.on('browserWindowGetBounds', (id) => {
        var _a, _b;
        const rectangle = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getBounds()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getBounds-completed' + id, rectangle);
    });
    socket.on('browserWindowSetContentBounds', (id, bounds, animate) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setContentBounds(bounds, animate);
    });
    socket.on('browserWindowGetContentBounds', (id) => {
        var _a, _b;
        const rectangle = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getContentBounds()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getContentBounds-completed' + id, rectangle);
    });
    socket.on('browserWindowSetSize', (id, width, height, animate) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setSize(width, height, animate);
    });
    socket.on('browserWindowGetSize', (id) => {
        var _a, _b;
        const size = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getSize()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getSize-completed' + id, size);
    });
    socket.on('browserWindowSetContentSize', (id, width, height, animate) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setContentSize(width, height, animate);
    });
    socket.on('browserWindowGetContentSize', (id) => {
        var _a, _b;
        const size = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getContentSize()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getContentSize-completed' + id, size);
    });
    socket.on('browserWindowSetMinimumSize', (id, width, height) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMinimumSize(width, height);
    });
    socket.on('browserWindowGetMinimumSize', (id) => {
        var _a, _b;
        const size = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getMinimumSize()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getMinimumSize-completed' + id, size);
    });
    socket.on('browserWindowSetMaximumSize', (id, width, height) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMaximumSize(width, height);
    });
    socket.on('browserWindowGetMaximumSize', (id) => {
        var _a, _b;
        const size = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getMaximumSize()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getMaximumSize-completed' + id, size);
    });
    socket.on('browserWindowSetResizable', (id, resizable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setResizable(resizable);
    });
    socket.on('browserWindowIsResizable', (id) => {
        var _a, _b;
        const resizable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isResizable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isResizable-completed' + id, resizable);
    });
    socket.on('browserWindowSetMovable', (id, movable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMovable(movable);
    });
    socket.on('browserWindowIsMovable', (id) => {
        var _a, _b;
        const movable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMovable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMovable-completed' + id, movable);
    });
    socket.on('browserWindowSetMinimizable', (id, minimizable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMinimizable(minimizable);
    });
    socket.on('browserWindowIsMinimizable', (id) => {
        var _a, _b;
        const minimizable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMinimizable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMinimizable-completed' + id, minimizable);
    });
    socket.on('browserWindowSetMaximizable', (id, maximizable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMaximizable(maximizable);
    });
    socket.on('browserWindowIsMaximizable', (id) => {
        var _a, _b;
        const maximizable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMaximizable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMaximizable-completed' + id, maximizable);
    });
    socket.on('browserWindowSetFullScreenable', (id, fullscreenable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setFullScreenable(fullscreenable);
    });
    socket.on('browserWindowIsFullScreenable', (id) => {
        var _a, _b;
        const fullscreenable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isFullScreenable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isFullScreenable-completed' + id, fullscreenable);
    });
    socket.on('browserWindowSetClosable', (id, closable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setClosable(closable);
    });
    socket.on('browserWindowIsClosable', (id) => {
        var _a, _b;
        const closable = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isClosable()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isClosable-completed' + id, closable);
    });
    socket.on('browserWindowSetAlwaysOnTop', (id, flag, level, relativeLevel) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setAlwaysOnTop(flag, level, relativeLevel);
    });
    socket.on('browserWindowIsAlwaysOnTop', (id) => {
        var _a, _b;
        const isAlwaysOnTop = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isAlwaysOnTop()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isAlwaysOnTop-completed' + id, isAlwaysOnTop);
    });
    socket.on('browserWindowCenter', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.center();
    });
    socket.on('browserWindowSetPosition', (id, x, y, animate) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setPosition(x, y, animate);
    });
    socket.on('browserWindowGetPosition', (id) => {
        var _a, _b;
        const position = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getPosition()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getPosition-completed' + id, position);
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setTitle(title);
    });
    socket.on('browserWindowGetTitle', (id) => {
        var _a, _b;
        const title = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getTitle()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getTitle-completed' + id, title);
    });
    socket.on('browserWindowSetTitle', (id, title) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setTitle(title);
    });
    socket.on('browserWindowSetSheetOffset', (id, offsetY, offsetX) => {
        var _a, _b;
        if (offsetX) {
            (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setSheetOffset(offsetY, offsetX);
        }
        else {
            (_b = getWindowById(id)) === null || _b === void 0 ? void 0 : _b.setSheetOffset(offsetY);
        }
    });
    socket.on('browserWindowFlashFrame', (id, flag) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.flashFrame(flag);
    });
    socket.on('browserWindowSetSkipTaskbar', (id, skip) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setSkipTaskbar(skip);
    });
    socket.on('browserWindowSetKiosk', (id, flag) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setKiosk(flag);
    });
    socket.on('browserWindowIsKiosk', (id) => {
        var _a, _b;
        const isKiosk = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isKiosk()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isKiosk-completed' + id, isKiosk);
    });
    socket.on('browserWindowGetNativeWindowHandle', (id) => {
        var _a, _b, _c, _d;
        const nativeWindowHandle = (_d = (_c = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getNativeWindowHandle()) === null || _b === void 0 ? void 0 : _b.readInt32LE(0)) === null || _c === void 0 ? void 0 : _c.toString(16)) !== null && _d !== void 0 ? _d : null;
        electronSocket.emit('browserWindow-getNativeWindowHandle-completed' + id, nativeWindowHandle);
    });
    socket.on('browserWindowSetRepresentedFilename', (id, filename) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setRepresentedFilename(filename);
    });
    socket.on('browserWindowGetRepresentedFilename', (id) => {
        var _a, _b;
        const pathname = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getRepresentedFilename()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getRepresentedFilename-completed' + id, pathname);
    });
    socket.on('browserWindowSetDocumentEdited', (id, edited) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setDocumentEdited(edited);
    });
    socket.on('browserWindowIsDocumentEdited', (id) => {
        var _a, _b;
        const edited = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isDocumentEdited()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isDocumentEdited-completed' + id, edited);
    });
    socket.on('browserWindowFocusOnWebView', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.focusOnWebView();
    });
    socket.on('browserWindowBlurWebView', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.blurWebView();
    });
    socket.on('browserWindowLoadURL', (id, url, options) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.loadURL(url, options);
    });
    socket.on('browserWindowReload', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.reload();
    });
    socket.on('browserWindowSetMenu', (id, menuItems) => {
        var _a;
        let menu = null;
        if (menuItems) {
            menu = electron_1.Menu.buildFromTemplate(menuItems);
            addMenuItemClickConnector(menu.items, (id) => {
                electronSocket.emit('windowMenuItemClicked', id);
            });
        }
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMenu(menu);
    });
    socket.on('browserWindowRemoveMenu', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.removeMenu();
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
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setProgressBar(progress);
    });
    socket.on('browserWindowSetProgressBar', (id, progress, options) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setProgressBar(progress, options);
    });
    socket.on('browserWindowSetHasShadow', (id, hasShadow) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setHasShadow(hasShadow);
    });
    socket.on('browserWindowHasShadow', (id) => {
        var _a, _b;
        const hasShadow = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.hasShadow()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-hasShadow-completed' + id, hasShadow);
    });
    socket.on('browserWindowSetThumbarButtons', (id, thumbarButtons) => {
        var _a, _b;
        thumbarButtons.forEach(thumbarButton => {
            const imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());
            thumbarButton.icon = electron_1.nativeImage.createFromPath(imagePath);
            thumbarButton.click = () => {
                electronSocket.emit('thumbarButtonClicked', thumbarButton['id']);
            };
        });
        const success = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setThumbarButtons(thumbarButtons)) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindowSetThumbarButtons-completed' + id, success);
    });
    socket.on('browserWindowSetThumbnailClip', (id, rectangle) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setThumbnailClip(rectangle);
    });
    socket.on('browserWindowSetThumbnailToolTip', (id, toolTip) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setThumbnailToolTip(toolTip);
    });
    socket.on('browserWindowSetAppDetails', (id, options) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setAppDetails(options);
    });
    socket.on('browserWindowShowDefinitionForSelection', (id) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.showDefinitionForSelection();
    });
    socket.on('browserWindowSetAutoHideMenuBar', (id, hide) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setAutoHideMenuBar(hide);
    });
    socket.on('browserWindowIsMenuBarAutoHide', (id) => {
        var _a, _b;
        const isMenuBarAutoHide = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMenuBarAutoHide()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMenuBarAutoHide-completed' + id, isMenuBarAutoHide);
    });
    socket.on('browserWindowSetMenuBarVisibility', (id, visible) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setMenuBarVisibility(visible);
    });
    socket.on('browserWindowIsMenuBarVisible', (id) => {
        var _a, _b;
        const isMenuBarVisible = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isMenuBarVisible()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isMenuBarVisible-completed' + id, isMenuBarVisible);
    });
    socket.on('browserWindowSetVisibleOnAllWorkspaces', (id, visible) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setVisibleOnAllWorkspaces(visible);
    });
    socket.on('browserWindowIsVisibleOnAllWorkspaces', (id) => {
        var _a, _b;
        const isVisibleOnAllWorkspaces = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.isVisibleOnAllWorkspaces()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-isVisibleOnAllWorkspaces-completed' + id, isVisibleOnAllWorkspaces);
    });
    socket.on('browserWindowSetIgnoreMouseEvents', (id, ignore) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setIgnoreMouseEvents(ignore);
    });
    socket.on('browserWindowSetContentProtection', (id, enable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setContentProtection(enable);
    });
    socket.on('browserWindowSetFocusable', (id, focusable) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setFocusable(focusable);
    });
    socket.on('browserWindowSetParentWindow', (id, parent) => {
        var _a;
        const browserWindow = electron_1.BrowserWindow.fromId(parent.id);
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setParentWindow(browserWindow);
    });
    socket.on('browserWindowGetParentWindow', (id) => {
        var _a, _b;
        const browserWindow = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getParentWindow()) !== null && _b !== void 0 ? _b : null;
        electronSocket.emit('browserWindow-getParentWindow-completed' + id, browserWindow.id);
    });
    socket.on('browserWindowGetChildWindows', (id) => {
        var _a, _b;
        const browserWindows = (_b = (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.getChildWindows()) !== null && _b !== void 0 ? _b : null;
        const ids = [];
        browserWindows.forEach(x => {
            ids.push(x.id);
        });
        electronSocket.emit('browserWindow-getChildWindows-completed' + id, ids);
    });
    socket.on('browserWindowSetAutoHideCursor', (id, autoHide) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setAutoHideCursor(autoHide);
    });
    socket.on('browserWindowSetVibrancy', (id, type) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setVibrancy(type);
    });
    socket.on('browserWindow-setBrowserView', (id, browserViewId) => {
        var _a;
        (_a = getWindowById(id)) === null || _a === void 0 ? void 0 : _a.setBrowserView(browserView_1.browserViewMediateService(browserViewId));
    });
    function getWindowById(id) {
        for (let index = 0; index < windows.length; index++) {
            const element = windows[index];
            if (element.id === id) {
                return element;
            }
        }
        return null;
    }
};
//# sourceMappingURL=browserWindows.js.map