import { BrowserWindow } from "electron";
const windows: Electron.BrowserWindow[] = []

module.exports = (socket: SocketIO.Server) => {
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
    
    function getWindowById(id: number): Electron.BrowserWindow {
        for (var index = 0; index < windows.length; index++) {
            var element = windows[index];
            if (element.id == id) {
                return element;
            }
        }
    }
}