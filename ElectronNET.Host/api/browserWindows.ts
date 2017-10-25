import { BrowserWindow, Menu, nativeImage } from "electron";
const path = require('path');
const windows: Electron.BrowserWindow[] = []

module.exports = (socket: SocketIO.Server) => {

    socket.on('register-browserWindow-ready-to-show', (id) => {
        getWindowById(id).on('ready-to-show', () => {
            socket.emit('browserWindow-ready-to-show' + id);
        });
    });

    socket.on('register-browserWindow-page-title-updated', (id) => {
        getWindowById(id).on('page-title-updated', (event, title) => {
            socket.emit('browserWindow-page-title-updated' + id, title);
        });
    });

    socket.on('register-browserWindow-close', (id) => {
        getWindowById(id).on('close', () => {
            socket.emit('browserWindow-close' + id);
        });
    });

    socket.on('register-browserWindow-closed', (id) => {
        getWindowById(id).on('closed', () => {
            socket.emit('browserWindow-closed' + id);
        });
    });

    socket.on('register-browserWindow-session-end', (id) => {
        getWindowById(id).on('session-end', () => {
            socket.emit('browserWindow-session-end' + id);
        });
    });

    socket.on('register-browserWindow-unresponsive', (id) => {
        getWindowById(id).on('unresponsive', () => {
            socket.emit('browserWindow-unresponsive' + id);
        });
    });

    socket.on('register-browserWindow-responsive', (id) => {
        getWindowById(id).on('responsive', () => {
            socket.emit('browserWindow-responsive' + id);
        });
    });

    socket.on('register-browserWindow-blur', (id) => {
        getWindowById(id).on('blur', () => {
            socket.emit('browserWindow-blur' + id);
        });
    });

    socket.on('register-browserWindow-focus', (id) => {
        getWindowById(id).on('focus', () => {
            socket.emit('browserWindow-focus' + id);
        });
    });

    socket.on('register-browserWindow-show', (id) => {
        getWindowById(id).on('show', () => {
            socket.emit('browserWindow-show' + id);
        });
    });

    socket.on('register-browserWindow-hide', (id) => {
        getWindowById(id).on('hide', () => {
            socket.emit('browserWindow-hide' + id);
        });
    });

    socket.on('register-browserWindow-maximize', (id) => {
        getWindowById(id).on('maximize', () => {
            socket.emit('browserWindow-maximize' + id);
        });
    });

    socket.on('register-browserWindow-unmaximize', (id) => {
        getWindowById(id).on('unmaximize', () => {
            socket.emit('browserWindow-unmaximize' + id);
        });
    });

    socket.on('register-browserWindow-minimize', (id) => {
        getWindowById(id).on('minimize', () => {
            socket.emit('browserWindow-minimize' + id);
        });
    });

    socket.on('register-browserWindow-restore', (id) => {
        getWindowById(id).on('restore', () => {
            socket.emit('browserWindow-restore' + id);
        });
    });

    socket.on('register-browserWindow-resize', (id) => {
        getWindowById(id).on('resize', () => {
            socket.emit('browserWindow-resize' + id);
        });
    });

    socket.on('register-browserWindow-move', (id) => {
        getWindowById(id).on('move', () => {
            socket.emit('browserWindow-move' + id);
        });
    });

    socket.on('register-browserWindow-moved', (id) => {
        getWindowById(id).on('moved', () => {
            socket.emit('browserWindow-moved' + id);
        });
    });

    socket.on('register-browserWindow-enter-full-screen', (id) => {
        getWindowById(id).on('enter-full-screen', () => {
            socket.emit('browserWindow-enter-full-screen' + id);
        });
    });

    socket.on('register-browserWindow-leave-full-screen', (id) => {
        getWindowById(id).on('leave-full-screen', () => {
            socket.emit('browserWindow-leave-full-screen' + id);
        });
    });

    socket.on('register-browserWindow-enter-html-full-screen', (id) => {
        getWindowById(id).on('enter-html-full-screen', () => {
            socket.emit('browserWindow-enter-html-full-screen' + id);
        });
    });

    socket.on('register-browserWindow-leave-html-full-screen', (id) => {
        getWindowById(id).on('leave-html-full-screen', () => {
            socket.emit('browserWindow-leave-html-full-screen' + id);
        });
    });
    
    socket.on('register-browserWindow-app-command', (id) => {
        getWindowById(id).on('app-command', (event, command) => {
            socket.emit('browserWindow-app-command' + id, command);
        });
    });
    
    socket.on('register-browserWindow-scroll-touch-begin', (id) => {
        getWindowById(id).on('scroll-touch-begin', () => {
            socket.emit('browserWindow-scroll-touch-begin' + id);
        });
    });

    socket.on('register-browserWindow-scroll-touch-end', (id) => {
        getWindowById(id).on('scroll-touch-end', () => {
            socket.emit('browserWindow-scroll-touch-end' + id);
        });
    });

    socket.on('register-browserWindow-scroll-touch-edge', (id) => {
        getWindowById(id).on('scroll-touch-edge', () => {
            socket.emit('browserWindow-scroll-touch-edge' + id);
        });
    });

    socket.on('register-browserWindow-swipe', (id) => {
        getWindowById(id).on('swipe', (event, direction) => {
            socket.emit('browserWindow-swipe' + id, direction);
        });
    });

    socket.on('register-browserWindow-sheet-begin', (id) => {
        getWindowById(id).on('sheet-begin', () => {
            socket.emit('browserWindow-sheet-begin' + id);
        });
    });

    socket.on('register-browserWindow-sheet-end', (id) => {
        getWindowById(id).on('sheet-end', () => {
            socket.emit('browserWindow-sheet-end' + id);
        });
    });

    socket.on('register-browserWindow-new-window-for-tab', (id) => {
        getWindowById(id).on('new-window-for-tab', () => {
            socket.emit('browserWindow-new-window-for-tab' + id);
        });
    });

    socket.on('createBrowserWindow', (options, loadUrl) => {
        let window = new BrowserWindow(options);

        window.on('closed', (sender) => {
            for (var index = 0; index < windows.length; index++) {
                var windowItem = windows[index];
                try {
                    windowItem.id;
                } catch (error) {
                    if (error.message === 'Object has been destroyed') {
                        windows.splice(index, 1);

                        const ids = [];
                        windows.forEach(x => ids.push(x.id));
                        socket.emit('BrowserWindowClosed', ids);
                    }
                }
            }
        });

        if (loadUrl) {
            window.loadURL(loadUrl);
        }

        windows.push(window);
        socket.emit('BrowserWindowCreated', window.id);
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

        socket.emit('browserWindow-isFocused-completed', isFocused);
    });

    socket.on('browserWindowIsDestroyed', (id) => {
        const isDestroyed = getWindowById(id).isDestroyed();

        socket.emit('browserWindow-isDestroyed-completed', isDestroyed);
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

        socket.emit('browserWindow-isVisible-completed', isVisible);
    });

    socket.on('browserWindowIsModal', (id) => {
        const isModal = getWindowById(id).isModal();

        socket.emit('browserWindow-isModal-completed', isModal);
    });

    socket.on('browserWindowMaximize', (id) => {
        getWindowById(id).maximize();
    });

    socket.on('browserWindowUnmaximize', (id) => {
        getWindowById(id).unmaximize();
    });

    socket.on('browserWindowIsMaximized', (id) => {
        const isMaximized = getWindowById(id).isMaximized();

        socket.emit('browserWindow-isMaximized-completed', isMaximized);
    });

    socket.on('browserWindowMinimize', (id) => {
        getWindowById(id).minimize();
    });

    socket.on('browserWindowRestore', (id) => {
        getWindowById(id).restore();
    });

    socket.on('browserWindowIsMinimized', (id) => {
        const isMinimized = getWindowById(id).isMinimized();

        socket.emit('browserWindow-isMinimized-completed', isMinimized);
    });

    socket.on('browserWindowSetFullScreen', (id, fullscreen) => {
        getWindowById(id).setFullScreen(fullscreen);
    });

    socket.on('browserWindowIsFullScreen', (id) => {
        const isFullScreen = getWindowById(id).isFullScreen();

        socket.emit('browserWindow-isFullScreen-completed', isFullScreen);
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

        socket.emit('browserWindow-getBounds-completed', rectangle);
    });

    socket.on('browserWindowSetContentBounds', (id, bounds, animate) => {
        getWindowById(id).setContentBounds(bounds, animate);
    });

    socket.on('browserWindowGetContentBounds', (id) => {
        const rectangle = getWindowById(id).getContentBounds();

        socket.emit('browserWindow-getContentBounds-completed', rectangle);
    });

    socket.on('browserWindowSetSize', (id, width, height, animate) => {
        getWindowById(id).setSize(width, height, animate);
    });

    socket.on('browserWindowGetSize', (id) => {
        const size = getWindowById(id).getSize();

        socket.emit('browserWindow-getSize-completed', size);
    });

    socket.on('browserWindowSetContentSize', (id, width, height, animate) => {
        getWindowById(id).setContentSize(width, height, animate);
    });

    socket.on('browserWindowGetContentSize', (id) => {
        const size = getWindowById(id).getContentSize();

        socket.emit('browserWindow-getContentSize-completed', size);
    });

    socket.on('browserWindowSetMinimumSize', (id, width, height) => {
        getWindowById(id).setMinimumSize(width, height);
    });

    socket.on('browserWindowGetMinimumSize', (id) => {
        const size = getWindowById(id).getMinimumSize();

        socket.emit('browserWindow-getMinimumSize-completed', size);
    });

    socket.on('browserWindowSetMaximumSize', (id, width, height) => {
        getWindowById(id).setMaximumSize(width, height);
    });
    
    socket.on('browserWindowGetMaximumSize', (id) => {
        const size = getWindowById(id).getMaximumSize();
        
        socket.emit('browserWindow-getMaximumSize-completed', size);
    });
    
    socket.on('browserWindowSetResizable', (id, resizable) => {
        getWindowById(id).setResizable(resizable);
    });
    
    socket.on('browserWindowIsResizable', (id) => {
        const resizable = getWindowById(id).isResizable();
        
        socket.emit('browserWindow-isResizable-completed', resizable);
    });
    
    socket.on('browserWindowSetMovable', (id, movable) => {
        getWindowById(id).setMovable(movable);
    });
    
    socket.on('browserWindowIsMovable', (id) => {
        const movable = getWindowById(id).isMovable();
        
        socket.emit('browserWindow-isMovable-completed', movable);
    });
    
    socket.on('browserWindowSetMinimizable', (id, minimizable) => {
        getWindowById(id).setMinimizable(minimizable);
    });
    
    socket.on('browserWindowIsMinimizable', (id) => {
        const minimizable = getWindowById(id).isMinimizable();
        
        socket.emit('browserWindow-isMinimizable-completed', minimizable);
    });
    
    socket.on('browserWindowSetMaximizable', (id, maximizable) => {
        getWindowById(id).setMaximizable(maximizable);
    });
    
    socket.on('browserWindowIsMaximizable', (id) => {
        const maximizable = getWindowById(id).isMaximizable();
        
        socket.emit('browserWindow-isMaximizable-completed', maximizable);
    });
    
    socket.on('browserWindowSetFullScreenable', (id, fullscreenable) => {
        getWindowById(id).setFullScreenable(fullscreenable);
    });
    
    socket.on('browserWindowIsFullScreenable', (id) => {
        const fullscreenable = getWindowById(id).isFullScreenable();
        
        socket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
    });

    socket.on('browserWindowSetClosable', (id, closable) => {
        getWindowById(id).setClosable(closable);
    });
    
    socket.on('browserWindowIsClosable', (id) => {
        const closable = getWindowById(id).isClosable();
        
        socket.emit('browserWindow-isClosable-completed', closable);
    });
    
    socket.on('browserWindowSetAlwaysOnTop', (id, flag, level, relativeLevel) => {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });
    
    socket.on('browserWindowIsAlwaysOnTop', (id) => {
        const isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();
        
        socket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
    });
    
    socket.on('browserWindowCenter', (id) => {
        getWindowById(id).center();
    });
    
    socket.on('browserWindowSetPosition', (id, x, y, animate) => {
        getWindowById(id).setPosition(x, y, animate);
    });
    
    socket.on('browserWindowGetPosition', (id) => {
        const position = getWindowById(id).getPosition();
        
        socket.emit('browserWindow-getPosition-completed', position);
    });
    
    socket.on('browserWindowSetTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    
    socket.on('browserWindowGetTitle', (id) => {
        const title = getWindowById(id).getTitle();
        
        socket.emit('browserWindow-getTitle-completed', title);
    });

    socket.on('browserWindowSetTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    
    socket.on('browserWindowSetSheetOffset', (id, offsetY, offsetX) => {
        if(offsetX) {
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
    
    socket.on('browserWindowIsKiosk', (id) => {
        const isKiosk = getWindowById(id).isKiosk();
        
        socket.emit('browserWindow-isKiosk-completed', isKiosk);
    });
    
    socket.on('browserWindowSetRepresentedFilename', (id, filename) => {
        getWindowById(id).setRepresentedFilename(filename);
    });
    
    socket.on('browserWindowGetRepresentedFilename', (id) => {
        const pathname = getWindowById(id).getRepresentedFilename();
        
        socket.emit('browserWindow-getRepresentedFilename-completed', pathname);
    });
    
    socket.on('browserWindowSetDocumentEdited', (id, edited) => {
        getWindowById(id).setDocumentEdited(edited);
    });
    
    socket.on('browserWindowIsDocumentEdited', (id) => {
        const edited = getWindowById(id).isDocumentEdited();
        
        socket.emit('browserWindow-isDocumentEdited-completed', edited);
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
        
        if(menuItems) {
            menu = Menu.buildFromTemplate(menuItems);
            
            addMenuItemClickConnector(menu.items, (id) => {
                socket.emit("windowMenuItemClicked", id);
            });
        }
        
        getWindowById(id).setMenu(menu);
    });
    
    function addMenuItemClickConnector(menuItems, callback) {
        menuItems.forEach((item) => {
            if(item.submenu && item.submenu.items.length > 0) {
                addMenuItemClickConnector(item.submenu.items, callback);
            }
            
            if("id" in item && item.id) {
                item.click = () => { callback(item.id); };
            }
        });
    }
    
    socket.on('browserWindowSetProgressBar', (id, progress) => {
        getWindowById(id).setProgressBar(progress);
    });

    socket.on('browserWindowSetHasShadow', (id, hasShadow) => {
        getWindowById(id).setHasShadow(hasShadow);
    });
    
    socket.on('browserWindowHasShadow', (id) => {
        const hasShadow = getWindowById(id).hasShadow();
        
        socket.emit('browserWindow-hasShadow-completed', hasShadow);
    });

    socket.on('browserWindowSetThumbarButtons', (id, thumbarButtons: Electron.ThumbarButton[]) => {
        thumbarButtons.forEach(thumbarButton => {
            const imagePath = path.join(__dirname.replace('api', ''), 'bin', thumbarButton.icon.toString());            
            thumbarButton.icon = nativeImage.createFromPath(imagePath);
            thumbarButton.click = () => {
                socket.emit("thumbarButtonClicked", thumbarButton["id"]);
            };
        });

        const success = getWindowById(id).setThumbarButtons(thumbarButtons);
        socket.emit('browserWindowSetThumbarButtons-completed', success);
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
        
        socket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
    });
    
    socket.on('browserWindowSetMenuBarVisibility', (id, visible) => {
        getWindowById(id).setMenuBarVisibility(visible);
    });
    
    socket.on('browserWindowIsMenuBarVisible', (id) => {
        const isMenuBarVisible = getWindowById(id).isMenuBarVisible();
        
        socket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
    });
    
    socket.on('browserWindowSetVisibleOnAllWorkspaces', (id, visible) => {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });
    
    socket.on('browserWindowIsVisibleOnAllWorkspaces', (id) => {
        const isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();
        
        socket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
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

    socket.on('browserWindowGetParentWindow', (id) => {
        const browserWindow = getWindowById(id).getParentWindow();
        
        socket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
    });

    socket.on('browserWindowGetChildWindows', (id) => {
        const browserWindows = getWindowById(id).getChildWindows();

        const ids = [];

        browserWindows.forEach(x => {
            ids.push(x.id);
        })
        
        socket.emit('browserWindow-getChildWindows-completed', ids);
    });

    socket.on('browserWindowSetAutoHideCursor', (id, autoHide) => {
        getWindowById(id).setAutoHideCursor(autoHide);
    });

    socket.on('browserWindowSetVibrancy', (id, type) => {
        getWindowById(id).setVibrancy(type);
    });

    function getWindowById(id: number): Electron.BrowserWindow {
        for (var index = 0; index < windows.length; index++) {
            var element = windows[index];
            if (element.id == id) {
                return element;
            }
        }
    }
}