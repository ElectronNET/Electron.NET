import { BrowserWindow, Menu, nativeImage } from 'electron';
import { browserViewMediateService } from './browserView';
const path = require('path');
const windows: Electron.BrowserWindow[] = (global['browserWindows'] = global['browserWindows'] || []) as Electron.BrowserWindow[];
let readyToShowWindowsIds: number[] = [];
let window, lastOptions;
let mainWindowURL;
const proxyToCredentialsMap: { [proxy: string]: string } = (global['proxyToCredentialsMap'] = global['proxyToCredentialsMap'] || []) as { [proxy: string]: string };

export = (socket: SignalR.Hub.Proxy, app: Electron.App) => {

    app.on('login', (event, webContents, request, authInfo, callback) => {
        if (authInfo.isProxy) {
            let proxy = `${authInfo.host}:${authInfo.port}`
            if (proxy in proxyToCredentialsMap && proxyToCredentialsMap[proxy].split(':').length === 2) {
                event.preventDefault()
                let user = proxyToCredentialsMap[proxy].split(':')[0]
                let pass = proxyToCredentialsMap[proxy].split(':')[1]
                callback(user, pass)
            }
        }
    })

    socket.on('register-browserWindow-ready-to-show', (id) => {
        if (readyToShowWindowsIds.includes(id)) {
            readyToShowWindowsIds = readyToShowWindowsIds.filter(value => value !== id);
            socket.invoke('browserWindow-ready-to-show' + id);
        }

        getWindowById(id).on('ready-to-show', () => {
            readyToShowWindowsIds.push(id);
            socket.invoke('BrowserWindowReadyToShow', id);
        });
    });

    socket.on('register-browserWindow-page-title-updated', (id) => {
        getWindowById(id).on('page-title-updated', (event, title) => {
            socket.invoke('BrowserWindowPageTitleUpdated', id, title);
        });
    });

    socket.on('register-browserWindow-close', (id) => {
        getWindowById(id).on('close', () => {
            socket.invoke('BrowserWindowClose', id);
        });
    });

    socket.on('register-browserWindow-closed', (id) => {
        getWindowById(id).on('closed', () => {
            console.log(id);
            socket.invoke('BrowserWindowClosed', id);
        });
    });

    socket.on('register-browserWindow-session-end', (id) => {
        getWindowById(id).on('session-end', () => {
            socket.invoke('BrowserWindowSessionEnd', id);
        });
    });

    socket.on('register-browserWindow-unresponsive', (id) => {
        getWindowById(id).on('unresponsive', () => {
            socket.invoke('BrowserWindowUnresponsive', id);
        });
    });

    socket.on('register-browserWindow-responsive', (id) => {
        getWindowById(id).on('responsive', () => {
            socket.invoke('BrowserWindowResponsive', id);
        });
    });

    //Testing
    socket.on('register-browserWindow-blur', (id) => {
        getWindowById(id).on('blur', () => {
            console.log("BLUR");
            socket.invoke('BrowserWindowBlur', id);
        });
    });

    socket.on('register-browserWindow-focus', (id) => {
        getWindowById(id).on('focus', () => {
            socket.invoke('BrowserWindowFocus', id);
        });
    });

    socket.on('register-browserWindow-show', (id) => {
        getWindowById(id).on('show', () => {
            socket.invoke('BrowserWindowShow', id);
        });
    });

    socket.on('register-browserWindow-hide', (id) => {
        getWindowById(id).on('hide', () => {
            socket.invoke('BrowserWindowHide', id);
        });
    });

    socket.on('register-browserWindow-maximize', (id) => {
        getWindowById(id).on('maximize', () => {
            socket.invoke('BrowserWindowMaximize', id);
        });
    });

    socket.on('register-browserWindow-unmaximize', (id) => {
        getWindowById(id).on('unmaximize', () => {
            socket.invoke('BrowserWindowUnmaximize', id);
        });
    });

    socket.on('register-browserWindow-minimize', (id) => {
        getWindowById(id).on('minimize', () => {
            socket.invoke('BrowserWindowMinimize', id);
        });
    });

    socket.on('register-browserWindow-restore', (id) => {
        getWindowById(id).on('restore', () => {
            socket.invoke('BrowserWindowRestore', id);
        });
    });

    socket.on('register-browserWindow-resize', (id) => {
        getWindowById(id).on('resize', () => {
            socket.invoke('BrowserWindowResize', id);
        });
    });

    socket.on('register-browserWindow-move', (id) => {
        getWindowById(id).on('move', () => {
            socket.invoke('BrowserWindowMove', id);
        });
    });

    socket.on('register-browserWindow-moved', (id) => {
        getWindowById(id).on('moved', () => {
            socket.invoke('BrowserWindowMoved', id);
        });
    });

    socket.on('register-browserWindow-enter-full-screen', (id) => {
        getWindowById(id).on('enter-full-screen', () => {
            socket.invoke('BrowserWindowEnterFullScreen', id);
        });
    });

    socket.on('register-browserWindow-leave-full-screen', (id) => {
        getWindowById(id).on('leave-full-screen', () => {
            socket.invoke('BrowserWindowLeaveFullScreen', id);
        });
    });

    socket.on('register-browserWindow-enter-html-full-screen', (id) => {
        getWindowById(id).on('enter-html-full-screen', () => {
            socket.invoke('BrowserWindowEnterHtmlFullScreen', id);
        });
    });

    socket.on('register-browserWindow-leave-html-full-screen', (id) => {
        getWindowById(id).on('leave-html-full-screen', () => {
            socket.invoke('BrowserWindowLeaveHtmlFullScreen', id);
        });
    });

    socket.on('register-browserWindow-app-command', (id) => {
        getWindowById(id).on('app-command', (event, command) => {
            socket.invoke('BrowserWindowAppCommand', id, command);
        });
    });

    socket.on('register-browserWindow-scroll-touch-begin', (id) => {
        getWindowById(id).on('scroll-touch-begin', () => {
            socket.invoke('BrowserWindowScrollTouchBegin', id);
        });
    });

    socket.on('register-browserWindow-scroll-touch-end', (id) => {
        getWindowById(id).on('scroll-touch-end', () => {
            socket.invoke('BrowserWindowScrollTouchEnd', id);
        });
    });

    socket.on('register-browserWindow-scroll-touch-edge', (id) => {
        getWindowById(id).on('scroll-touch-edge', () => {
            socket.invoke('BrowserWindowScrollTouchEdge', id);
        });
    });

    socket.on('register-browserWindow-swipe', (id) => {
        getWindowById(id).on('swipe', (event, direction) => {
            socket.invoke('BrowserWindowSwipe', id, direction);
        });
    });

    socket.on('register-browserWindow-sheet-begin', (id) => {
        getWindowById(id).on('sheet-begin', () => {
            socket.invoke('BrowserWindowSheetBegin', id);
        });
    });

    socket.on('register-browserWindow-sheet-end', (id) => {
        getWindowById(id).on('sheet-end', () => {
            socket.invoke('BrowserWindowSheetEnd', id);
        });
    });

    socket.on('register-browserWindow-new-window-for-tab', (id) => {
        getWindowById(id).on('new-window-for-tab', () => {
            socket.invoke('BrowserWindowNewWindowForTab', id);
        });
    });

    socket.on('createBrowserWindow', (guid, options, loadUrl) => {
        if (options.webPreferences && !('nodeIntegration' in options.webPreferences)) {
            options = { ...options, webPreferences: { ...options.webPreferences, nodeIntegration: true, contextIsolation: false } };
        } else if (!options.webPreferences) {
            options = { ...options, webPreferences: { nodeIntegration: true, contextIsolation: false } };
        }

        // we dont want to recreate the window when watch is ready.
        if (app.commandLine.hasSwitch('watch') && app['mainWindowURL'] === loadUrl) {
            window = app['mainWindow'];
            if (window) {
                window.reload();
                windows.push(window);
                socket.invoke('SendClientResponseString', guid, window.id);
                return;
            }
        } else {
            window = new BrowserWindow(options);
        }

        if (options.proxy) {
            window.webContents.session.setProxy({proxyRules: options.proxy});
        }

        if (options.proxy && options.proxyCredentials) {
            proxyToCredentialsMap[options.proxy] = options.proxyCredentials;
        }

        window.on('ready-to-show', () => {
            if (readyToShowWindowsIds.includes(window.id)) {
                readyToShowWindowsIds = readyToShowWindowsIds.filter(value => value !== window.id);
            } else {
                readyToShowWindowsIds.push(window.id);
            }
        });

        lastOptions = options;

        window.on('closed', (sender) => {
            for (let index = 0; index < windows.length; index++) {
                const windowItem = windows[index];
                try {
                    windowItem.id;
                } catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);

                        const ids = [];
                        windows.forEach(x => ids.push(x.id));
                        console.log("------------------------------------");
                        console.log(ids);
                        socket.invoke('BrowserWindowsClosed', ids);
                    }
                }
            }
        });

        app.on('activate', () => {
            // On macOS it's common to re-create a window in the app when the
            // dock icon is clicked and there are no other windows open.
            if (window === null && lastOptions) {
                window = new BrowserWindow(lastOptions);
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
        socket.invoke('SendClientResponseString', guid, window.id);
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

    socket.on('browserWindowIsFocused', (guid, id) => {
        const isFocused = getWindowById(id).isFocused();

        socket.invoke('SendClientResponseBool', guid, isFocused);
    });

    socket.on('browserWindowIsDestroyed', (guid, id) => {
        const isDestroyed = getWindowById(id).isDestroyed();

        socket.invoke('SendClientResponseBool', guid, isDestroyed);
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

    socket.on('browserWindowIsVisible', (guid, id) => {
        const isVisible = getWindowById(id).isVisible();

        socket.invoke('SendClientResponseBool', guid, isVisible);
    });

    socket.on('browserWindowIsModal', (guid, id) => {
        const isModal = getWindowById(id).isModal();

        socket.invoke('SendClientResponseBool', guid, isModal);
    });

    socket.on('browserWindowMaximize', (id) => {
        getWindowById(id).maximize();
    });

    socket.on('browserWindowUnmaximize', (id) => {
        getWindowById(id).unmaximize();
    });

    socket.on('browserWindowIsMaximized', (guid, id) => {
        const isMaximized = getWindowById(id).isMaximized();

        socket.invoke('SendClientResponseBool', guid, isMaximized);
    });

    socket.on('browserWindowMinimize', (id) => {
        getWindowById(id).minimize();
    });

    socket.on('browserWindowRestore', (id) => {
        getWindowById(id).restore();
    });

    socket.on('browserWindowIsMinimized', (guid, id) => {
        const isMinimized = getWindowById(id).isMinimized();

        socket.invoke('SendClientResponseBool', guid, isMinimized);
    });

    socket.on('browserWindowSetFullScreen', (id, fullscreen) => {
        getWindowById(id).setFullScreen(fullscreen);
    });

    socket.on('browserWindowIsFullScreen', (guid, id) => {
        const isFullScreen = getWindowById(id).isFullScreen();

        socket.invoke('SendClientResponseBool', guid, isFullScreen);
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

    socket.on('browserWindowGetBounds', (guid, id) => {
        const rectangle = getWindowById(id).getBounds();

        socket.invoke('SendClientResponseJObject', guid, rectangle);
    });

    socket.on('browserWindowSetContentBounds', (id, bounds, animate) => {
        getWindowById(id).setContentBounds(bounds, animate);
    });

    socket.on('browserWindowGetContentBounds', (guid, id) => {
        const rectangle = getWindowById(id).getContentBounds();

        socket.invoke('SendClientResponseBool', guid, rectangle);
    });

    socket.on('browserWindowSetSize', (id, width, height, animate) => {
        getWindowById(id).setSize(width, height, animate);
    });

    socket.on('browserWindowGetSize', (guid, id) => {
        const size = getWindowById(id).getSize();

        socket.invoke('SendClientResponseJArray', guid, size);
    });

    socket.on('browserWindowSetContentSize', (id, width, height, animate) => {
        getWindowById(id).setContentSize(width, height, animate);
    });

    socket.on('browserWindowGetContentSize', (guid, id) => {
        const size = getWindowById(id).getContentSize();

        socket.invoke('SendClientResponseJArray', guid, size);
    });

    socket.on('browserWindowSetMinimumSize', (id, width, height) => {
        getWindowById(id).setMinimumSize(width, height);
    });

    socket.on('browserWindowGetMinimumSize', (guid, id) => {
        const size = getWindowById(id).getMinimumSize();

        socket.invoke('SendClientResponseJArray', guid, size);
    });

    socket.on('browserWindowSetMaximumSize', (id, width, height) => {
        getWindowById(id).setMaximumSize(width, height);
    });

    socket.on('browserWindowGetMaximumSize', (guid, id) => {
        const size = getWindowById(id).getMaximumSize();

        socket.invoke('SendClientResponseJArray', guid, size);
    });

    socket.on('browserWindowSetResizable', (id, resizable) => {
        getWindowById(id).setResizable(resizable);
    });

    socket.on('browserWindowIsResizable', (guid, id) => {
        const resizable = getWindowById(id).isResizable();

        socket.invoke('SendClientResponseBool', guid, resizable);
    });

    socket.on('browserWindowSetMovable', (id, movable) => {
        getWindowById(id).setMovable(movable);
    });

    socket.on('browserWindowIsMovable', (guid, id) => {
        const movable = getWindowById(id).isMovable();

        socket.invoke('SendClientResponseBool', guid, movable);
    });

    socket.on('browserWindowSetMinimizable', (id, minimizable) => {
        getWindowById(id).setMinimizable(minimizable);
    });

    socket.on('browserWindowIsMinimizable', (guid, id) => {
        const minimizable = getWindowById(id).isMinimizable();

        socket.invoke('SendClientResponseBool', guid, minimizable);
    });

    socket.on('browserWindowSetMaximizable', (id, maximizable) => {
        getWindowById(id).setMaximizable(maximizable);
    });

    socket.on('browserWindowIsMaximizable', (guid, id) => {
        const maximizable = getWindowById(id).isMaximizable();

        socket.invoke('SendClientResponseBool', guid, maximizable);
    });

    socket.on('browserWindowSetFullScreenable', (id, fullscreenable) => {
        getWindowById(id).setFullScreenable(fullscreenable);
    });

    socket.on('browserWindowIsFullScreenable', (guid, id) => {
        const fullscreenable = getWindowById(id).isFullScreenable();

        socket.invoke('SendClientResponseBool', guid, fullscreenable);
    });

    socket.on('browserWindowSetClosable', (id, closable) => {
        getWindowById(id).setClosable(closable);
    });

    socket.on('browserWindowIsClosable', (guid, id) => {
        const closable = getWindowById(id).isClosable();

        socket.invoke('SendClientResponseBool', guid, closable);
    });

    socket.on('browserWindowSetAlwaysOnTop', (id, flag, level, relativeLevel) => {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });

    socket.on('browserWindowIsAlwaysOnTop', (guid, id) => {
        const isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();

        socket.invoke('SendClientResponseBool', guid, isAlwaysOnTop);
    });

    socket.on('browserWindowCenter', (id) => {
        getWindowById(id).center();
    });

    socket.on('browserWindowSetPosition', (id, x, y, animate) => {
        getWindowById(id).setPosition(x, y, animate);
    });

    socket.on('browserWindowGetPosition', (guid, id) => {
        const position = getWindowById(id).getPosition();

        socket.invoke('SendClientResponseJArray', guid, position);
    });

    socket.on('browserWindowSetTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });

    socket.on('browserWindowGetTitle', (guid, id) => {
        const title = getWindowById(id).getTitle();

        socket.invoke('SendClientResponseString', guid, title);
    });

    socket.on('browserWindowSetSheetOffset', (id, offsetY, offsetX) => {
        if (offsetX) {
            getWindowById(id).setSheetOffset(offsetY, offsetX);
        } else {
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

    socket.on('browserWindowIsKiosk', (guid, id) => {
        const isKiosk = getWindowById(id).isKiosk();

        socket.invoke('SendClientResponseBool', guid, isKiosk);
    });

    socket.on('browserWindowGetNativeWindowHandle', (id) => {
        const nativeWindowHandle = getWindowById(id).getNativeWindowHandle().readInt32LE(0).toString(16);
        socket.invoke('SendClientResponseString', nativeWindowHandle);
    });

    socket.on('browserWindowSetRepresentedFilename', (id, filename) => {
        getWindowById(id).setRepresentedFilename(filename);
    });

    socket.on('browserWindowGetRepresentedFilename', (guid, id) => {
        const pathname = getWindowById(id).getRepresentedFilename();

        socket.invoke('SendClientResponseString', guid, pathname);
    });

    socket.on('browserWindowSetDocumentEdited', (id, edited) => {
        getWindowById(id).setDocumentEdited(edited);
    });

    socket.on('browserWindowIsDocumentEdited', (guid, id) => {
        const edited = getWindowById(id).isDocumentEdited();

        socket.invoke('SendClientResponseBool', guid, edited);
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
            menu = Menu.buildFromTemplate(menuItems);

            addMenuItemClickConnector(menu.items, (id) => {
                socket.invoke('BrowserWindowMenuItemClicked', id);
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

    socket.on('browserWindowHasShadow', (guid, id) => {
        const hasShadow = getWindowById(id).hasShadow();

        socket.invoke('SendClientResponseBool', guid, hasShadow);
    });

    socket.on('browserWindowSetThumbarButtons', (id, thumbarButtons: Electron.ThumbarButton[]) => {
        thumbarButtons.forEach(thumbarButton => {
            const imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());
            thumbarButton.icon = nativeImage.createFromPath(imagePath);
            thumbarButton.click = () => {
                socket.invoke('BrowserWindowThumbbarButtonClicked', thumbarButton['id']);
            };
        });

        const success = getWindowById(id).setThumbarButtons(thumbarButtons);
        socket.invoke('SendClientResponseBool', success);
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

    socket.on('browserWindowIsMenuBarAutoHide', (guid, id) => {
        const isMenuBarAutoHide = getWindowById(id).isMenuBarAutoHide();

        socket.invoke('SendClientResponseBool', guid, isMenuBarAutoHide);
    });

    socket.on('browserWindowSetMenuBarVisibility', (id, visible) => {
        getWindowById(id).setMenuBarVisibility(visible);
    });

    socket.on('browserWindowIsMenuBarVisible', (guid, id) => {
        const isMenuBarVisible = getWindowById(id).isMenuBarVisible();

        socket.invoke('SendClientResponseBool', guid, isMenuBarVisible);
    });

    socket.on('browserWindowSetVisibleOnAllWorkspaces', (id, visible) => {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });

    socket.on('browserWindowIsVisibleOnAllWorkspaces', (guid, id) => {
        const isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();

        socket.invoke('SendClientResponseBool', guid, isVisibleOnAllWorkspaces);
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
        const browserWindow = BrowserWindow.fromId(parent.id);

        getWindowById(id).setParentWindow(browserWindow);
    });

    socket.on('browserWindowGetParentWindow', (guid, id) => {
        const browserWindow = getWindowById(id).getParentWindow();

        socket.invoke('SendClientResponseString', guid, browserWindow.id);
    });

    socket.on('browserWindowGetChildWindows', (guid, id) => {
        const browserWindows = getWindowById(id).getChildWindows();

        const ids = [];

        browserWindows.forEach(x => {
            ids.push(x.id);
        });

        socket.invoke('SendClientResponseJArray', guid, ids);
    });

    socket.on('browserWindowSetAutoHideCursor', (id, autoHide) => {
        getWindowById(id).setAutoHideCursor(autoHide);
    });

    socket.on('browserWindowSetVibrancy', (id, type) => {
        getWindowById(id).setVibrancy(type);
    });

    socket.on('browserWindow-setBrowserView', (id, browserViewId) => {
        getWindowById(id).setBrowserView(browserViewMediateService(browserViewId));
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        for (let index = 0; index < windows.length; index++) {
            const element = windows[index];
            if (element.id === id) {
                return element;
            }
        }
    }
};
