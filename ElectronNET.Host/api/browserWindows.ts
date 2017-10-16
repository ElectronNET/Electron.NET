import { BrowserWindow, Menu } from "electron";
const windows: Electron.BrowserWindow[] = []

module.exports = (socket: SocketIO.Server) => {
    socket.on('register-browserWindow-ready-to-show', (id) => {
        getWindowById(id).on('ready-to-show', () => {
            socket.emit('browserWindow-ready-to-show');
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

    socket.on('browserWindow-destroy', (id) => {
        getWindowById(id).destroy();
    });

    socket.on('browserWindow-close', (id) => {
        getWindowById(id).close();
    });

    socket.on('browserWindow-focus', (id) => {
        getWindowById(id).focus();
    });

    socket.on('browserWindow-blur', (id) => {
        getWindowById(id).blur();
    });

    socket.on('browserWindow-isFocused', (id) => {
        const isFocused = getWindowById(id).isFocused();

        socket.emit('browserWindow-isFocused-completed', isFocused);
    });

    socket.on('browserWindow-isDestroyed', (id) => {
        const isDestroyed = getWindowById(id).isDestroyed();

        socket.emit('browserWindow-isDestroyed-completed', isDestroyed);
    });

    socket.on('browserWindow-show', (id) => {
        getWindowById(id).show();
    });

    socket.on('browserWindow-showInactive', (id) => {
        getWindowById(id).showInactive();
    });

    socket.on('browserWindow-hide', (id) => {
        getWindowById(id).hide();
    });

    socket.on('browserWindow-isVisible', (id) => {
        const isVisible = getWindowById(id).isVisible();

        socket.emit('browserWindow-isVisible-completed', isVisible);
    });

    socket.on('browserWindow-isModal', (id) => {
        const isModal = getWindowById(id).isModal();

        socket.emit('browserWindow-isModal-completed', isModal);
    });

    socket.on('browserWindow-maximize', (id) => {
        getWindowById(id).maximize();
    });

    socket.on('browserWindow-unmaximize', (id) => {
        getWindowById(id).unmaximize();
    });

    socket.on('browserWindow-isMaximized', (id) => {
        const isMaximized = getWindowById(id).isMaximized();

        socket.emit('browserWindow-isMaximized-completed', isMaximized);
    });

    socket.on('browserWindow-minimize', (id) => {
        getWindowById(id).minimize();
    });

    socket.on('browserWindow-restore', (id) => {
        getWindowById(id).restore();
    });

    socket.on('browserWindow-isMinimized', (id) => {
        const isMinimized = getWindowById(id).isMinimized();

        socket.emit('browserWindow-isMinimized-completed', isMinimized);
    });

    socket.on('browserWindow-setFullScreen', (id, fullscreen) => {
        getWindowById(id).setFullScreen(fullscreen);
    });

    socket.on('browserWindow-isFullScreen', (id) => {
        const isFullScreen = getWindowById(id).isFullScreen();

        socket.emit('browserWindow-isFullScreen-completed', isFullScreen);
    });

    socket.on('browserWindow-setAspectRatio', (id, aspectRatio, extraSize) => {
        getWindowById(id).setAspectRatio(aspectRatio, extraSize);
    });

    socket.on('browserWindow-previewFile', (id, path, displayname) => {
        getWindowById(id).previewFile(path, displayname);
    });

    socket.on('browserWindow-closeFilePreview', (id) => {
        getWindowById(id).closeFilePreview();
    });

    socket.on('browserWindow-setBounds', (id, bounds, animate) => {
        getWindowById(id).setBounds(bounds, animate);
    });

    socket.on('browserWindow-getBounds', (id) => {
        const rectangle = getWindowById(id).getBounds();

        socket.emit('browserWindow-getBounds-completed', rectangle);
    });

    socket.on('browserWindow-setContentBounds', (id, bounds, animate) => {
        getWindowById(id).setContentBounds(bounds, animate);
    });

    socket.on('browserWindow-getContentBounds', (id) => {
        const rectangle = getWindowById(id).getContentBounds();

        socket.emit('browserWindow-getContentBounds-completed', rectangle);
    });

    socket.on('browserWindow-setSize', (id, width, height, animate) => {
        getWindowById(id).setSize(width, height, animate);
    });

    socket.on('browserWindow-getSize', (id) => {
        const size = getWindowById(id).getSize();

        socket.emit('browserWindow-getSize-completed', size);
    });

    socket.on('browserWindow-setContentSize', (id, width, height, animate) => {
        getWindowById(id).setContentSize(width, height, animate);
    });

    socket.on('browserWindow-getContentSize', (id) => {
        const size = getWindowById(id).getContentSize();

        socket.emit('browserWindow-getContentSize-completed', size);
    });

    socket.on('browserWindow-setMinimumSize', (id, width, height) => {
        getWindowById(id).setMinimumSize(width, height);
    });

    socket.on('browserWindow-getMinimumSize', (id) => {
        const size = getWindowById(id).getMinimumSize();

        socket.emit('browserWindow-getMinimumSize-completed', size);
    });

    socket.on('browserWindow-setMaximumSize', (id, width, height) => {
        getWindowById(id).setMaximumSize(width, height);
    });
    
    socket.on('browserWindow-getMaximumSize', (id) => {
        const size = getWindowById(id).getMaximumSize();
        
        socket.emit('browserWindow-getMaximumSize-completed', size);
    });
    
    socket.on('browserWindow-setResizable', (id, resizable) => {
        getWindowById(id).setResizable(resizable);
    });
    
    socket.on('browserWindow-isResizable', (id) => {
        const resizable = getWindowById(id).isResizable();
        
        socket.emit('browserWindow-isResizable-completed', resizable);
    });
    
    socket.on('browserWindow-setMovable', (id, movable) => {
        getWindowById(id).setMovable(movable);
    });
    
    socket.on('browserWindow-isMovable', (id) => {
        const movable = getWindowById(id).isMovable();
        
        socket.emit('browserWindow-isMovable-completed', movable);
    });
    
    socket.on('browserWindow-setMinimizable', (id, minimizable) => {
        getWindowById(id).setMinimizable(minimizable);
    });
    
    socket.on('browserWindow-isMinimizable', (id) => {
        const minimizable = getWindowById(id).isMinimizable();
        
        socket.emit('browserWindow-isMinimizable-completed', minimizable);
    });
    
    socket.on('browserWindow-setMaximizable', (id, maximizable) => {
        getWindowById(id).setMaximizable(maximizable);
    });
    
    socket.on('browserWindow-isMaximizable', (id) => {
        const maximizable = getWindowById(id).isMaximizable();
        
        socket.emit('browserWindow-isMaximizable-completed', maximizable);
    });
    
    socket.on('browserWindow-setFullScreenable', (id, fullscreenable) => {
        getWindowById(id).setFullScreenable(fullscreenable);
    });
    
    socket.on('browserWindow-isFullScreenable', (id) => {
        const fullscreenable = getWindowById(id).isFullScreenable();
        
        socket.emit('browserWindow-isFullScreenable-completed', fullscreenable);
    });

    socket.on('browserWindow-setClosable', (id, closable) => {
        getWindowById(id).setClosable(closable);
    });
    
    socket.on('browserWindow-isClosable', (id) => {
        const closable = getWindowById(id).isClosable();
        
        socket.emit('browserWindow-isClosable-completed', closable);
    });
    
    socket.on('browserWindow-setAlwaysOnTop', (id, flag, level, relativeLevel) => {
        getWindowById(id).setAlwaysOnTop(flag, level, relativeLevel);
    });
    
    socket.on('browserWindow-isAlwaysOnTop', (id) => {
        const isAlwaysOnTop = getWindowById(id).isAlwaysOnTop();
        
        socket.emit('browserWindow-isAlwaysOnTop-completed', isAlwaysOnTop);
    });
    
    socket.on('browserWindow-center', (id) => {
        getWindowById(id).center();
    });
    
    socket.on('browserWindow-setPosition', (id, x, y, animate) => {
        getWindowById(id).setPosition(x, y, animate);
    });
    
    socket.on('browserWindow-getPosition', (id) => {
        const position = getWindowById(id).getPosition();
        
        socket.emit('browserWindow-getPosition-completed', position);
    });
    
    socket.on('browserWindow-setTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    
    socket.on('browserWindow-getTitle', (id) => {
        const title = getWindowById(id).getTitle();
        
        socket.emit('browserWindow-getTitle-completed', title);
    });

    socket.on('browserWindow-setTitle', (id, title) => {
        getWindowById(id).setTitle(title);
    });
    
    socket.on('browserWindow-setSheetOffset', (id, offsetY, offsetX) => {
        if(offsetX) {
            getWindowById(id).setSheetOffset(offsetY, offsetX);
        } else {
            getWindowById(id).setSheetOffset(offsetY);
        }
    });
    
    socket.on('browserWindow-flashFrame', (id, flag) => {
        getWindowById(id).flashFrame(flag);
    });

    socket.on('browserWindow-setSkipTaskbar', (id, skip) => {
        getWindowById(id).setSkipTaskbar(skip);
    });

    socket.on('browserWindow-setKiosk', (id, flag) => {
        getWindowById(id).setKiosk(flag);
    });
    
    socket.on('browserWindow-isKiosk', (id) => {
        const isKiosk = getWindowById(id).isKiosk();
        
        socket.emit('browserWindow-isKiosk-completed', isKiosk);
    });
    
    socket.on('browserWindow-setRepresentedFilename', (id, filename) => {
        getWindowById(id).setRepresentedFilename(filename);
    });
    
    socket.on('browserWindow-getRepresentedFilename', (id) => {
        const pathname = getWindowById(id).getRepresentedFilename();
        
        socket.emit('browserWindow-getRepresentedFilename-completed', pathname);
    });
    
    socket.on('browserWindow-setDocumentEdited', (id, edited) => {
        getWindowById(id).setDocumentEdited(edited);
    });
    
    socket.on('browserWindow-isDocumentEdited', (id) => {
        const edited = getWindowById(id).isDocumentEdited();
        
        socket.emit('browserWindow-isDocumentEdited-completed', edited);
    });
    
    socket.on('browserWindow-focusOnWebView', (id) => {
        getWindowById(id).focusOnWebView();
    });

    socket.on('browserWindow-blurWebView', (id) => {
        getWindowById(id).blurWebView();
    });

    socket.on('browserWindow-loadURL', (id, url, options) => {
        getWindowById(id).loadURL(url, options);
    });

    socket.on('browserWindow-reload', (id) => {
        getWindowById(id).reload();
    });
    
    socket.on('browserWindow-setMenu', (id, menuItems) => {
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
    
    socket.on('browserWindow-setProgressBar', (id, progress) => {
        getWindowById(id).setProgressBar(progress);
    });

    socket.on('browserWindow-setHasShadow', (id, hasShadow) => {
        getWindowById(id).setHasShadow(hasShadow);
    });
    
    socket.on('browserWindow-hasShadow', (id) => {
        const hasShadow = getWindowById(id).hasShadow();
        
        socket.emit('browserWindow-hasShadow-completed', hasShadow);
    });
    
    socket.on('browserWindow-setAppDetails', (id, options) => {
        getWindowById(id).setAppDetails(options);
    });
    
    socket.on('browserWindow-showDefinitionForSelection', (id) => {
        getWindowById(id).showDefinitionForSelection();
    });
    
    socket.on('browserWindow-setAutoHideMenuBar', (id, hide) => {
        getWindowById(id).setAutoHideMenuBar(hide);
    });
    
    socket.on('browserWindow-isMenuBarAutoHide', (id) => {
        const isMenuBarAutoHide = getWindowById(id).isMenuBarAutoHide();
        
        socket.emit('browserWindow-isMenuBarAutoHide-completed', isMenuBarAutoHide);
    });
    
    socket.on('browserWindow-setMenuBarVisibility', (id, visible) => {
        getWindowById(id).setMenuBarVisibility(visible);
    });
    
    socket.on('browserWindow-isMenuBarVisible', (id) => {
        const isMenuBarVisible = getWindowById(id).isMenuBarVisible();
        
        socket.emit('browserWindow-isMenuBarVisible-completed', isMenuBarVisible);
    });
    
    socket.on('browserWindow-setVisibleOnAllWorkspaces', (id, visible) => {
        getWindowById(id).setVisibleOnAllWorkspaces(visible);
    });
    
    socket.on('browserWindow-isVisibleOnAllWorkspaces', (id) => {
        const isVisibleOnAllWorkspaces = getWindowById(id).isVisibleOnAllWorkspaces();
        
        socket.emit('browserWindow-isVisibleOnAllWorkspaces-completed', isVisibleOnAllWorkspaces);
    });
    
    socket.on('browserWindow-setIgnoreMouseEvents', (id, ignore) => {
        getWindowById(id).setIgnoreMouseEvents(ignore);
    });

    socket.on('browserWindow-setContentProtection', (id, enable) => {
        getWindowById(id).setContentProtection(enable);
    });

    socket.on('browserWindow-setFocusable', (id, focusable) => {
        getWindowById(id).setFocusable(focusable);
    });

    socket.on('browserWindow-setParentWindow', (id, parent) => {
        const browserWindow = BrowserWindow.fromId(parent.id);

        getWindowById(id).setParentWindow(browserWindow);
    });

    socket.on('browserWindow-getParentWindow', (id) => {
        const browserWindow = getWindowById(id).getParentWindow();
        
        socket.emit('browserWindow-getParentWindow-completed', browserWindow.id);
    });

    socket.on('browserWindow-getChildWindows', (id) => {
        const browserWindows = getWindowById(id).getChildWindows();

        const ids = [];

        browserWindows.forEach(x => {
            ids.push(x.id);
        })
        
        socket.emit('browserWindow-getChildWindows-completed', ids);
    });

    socket.on('browserWindow-setAutoHideCursor', (id, autoHide) => {
        getWindowById(id).setAutoHideCursor(autoHide);
    });

    socket.on('browserWindow-setVibrancy', (id, type) => {
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