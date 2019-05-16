"use strict";
let isQuitWindowAllClosed = true, electronSocket;
module.exports = (socket, app) => {
    electronSocket = socket;
    // Quit when all windows are closed.
    app.on('window-all-closed', () => {
        // On macOS it is common for applications and their menu bar
        // to stay active until the user quits explicitly with Cmd + Q
        if (process.platform !== 'darwin' &&
            isQuitWindowAllClosed) {
            app.quit();
        }
    });
    socket.on('quit-app-window-all-closed-event', (quit) => {
        isQuitWindowAllClosed = quit;
    });
    socket.on('register-app-window-all-closed-event', (id) => {
        app.on('window-all-closed', () => {
            electronSocket.emit('app-window-all-closed' + id);
        });
    });
    socket.on('register-app-before-quit-event', (id) => {
        app.on('before-quit', (event) => {
            event.preventDefault();
            electronSocket.emit('app-before-quit' + id);
        });
    });
    socket.on('register-app-will-quit-event', (id) => {
        app.on('will-quit', (event) => {
            event.preventDefault();
            electronSocket.emit('app-will-quit' + id);
        });
    });
    socket.on('register-app-browser-window-blur-event', (id) => {
        app.on('browser-window-blur', () => {
            electronSocket.emit('app-browser-window-blur' + id);
        });
    });
    socket.on('register-app-browser-window-focus-event', (id) => {
        app.on('browser-window-focus', () => {
            electronSocket.emit('app-browser-window-focus' + id);
        });
    });
    socket.on('register-app-browser-window-created-event', (id) => {
        app.on('browser-window-created', () => {
            electronSocket.emit('app-browser-window-created' + id);
        });
    });
    socket.on('register-app-web-contents-created-event', (id) => {
        app.on('web-contents-created', () => {
            electronSocket.emit('app-web-contents-created' + id);
        });
    });
    socket.on('register-app-accessibility-support-changed-event', (id) => {
        app.on('accessibility-support-changed', (event, accessibilitySupportEnabled) => {
            electronSocket.emit('app-accessibility-support-changed' + id, accessibilitySupportEnabled);
        });
    });
    socket.on('appQuit', () => {
        app.quit();
    });
    socket.on('appExit', (exitCode = 0) => {
        app.exit(exitCode);
    });
    socket.on('appRelaunch', (options) => {
        app.relaunch(options);
    });
    socket.on('appFocus', () => {
        app.focus();
    });
    socket.on('appHide', () => {
        app.hide();
    });
    socket.on('appShow', () => {
        app.show();
    });
    socket.on('appGetAppPath', () => {
        const path = app.getAppPath();
        electronSocket.emit('appGetAppPathCompleted', path);
    });
    socket.on('appGetPath', (name) => {
        const path = app.getPath(name);
        electronSocket.emit('appGetPathCompleted', path);
    });
    // const nativeImages = {};
    // function addNativeImage(nativeImage: Electron.NativeImage) {
    //     if(Object.keys(nativeImages).length === 0) {
    //         nativeImage['1'] = nativeImage;
    //     } else {
    //         let indexCount = Object.keys(nativeImages).length + 1;
    //         nativeImage[indexCount] = nativeImage;
    //     }
    // }
    socket.on('appGetFileIcon', (path, options) => {
        if (options) {
            app.getFileIcon(path, options, (error, nativeImage) => {
                electronSocket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        }
        else {
            app.getFileIcon(path, (error, nativeImage) => {
                electronSocket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        }
    });
    socket.on('appSetPath', (name, path) => {
        app.setPath(name, path);
    });
    socket.on('appGetVersion', () => {
        const version = app.getVersion();
        electronSocket.emit('appGetVersionCompleted', version);
    });
    socket.on('appGetName', () => {
        const name = app.getName();
        electronSocket.emit('appGetNameCompleted', name);
    });
    socket.on('appSetName', (name) => {
        app.setName(name);
    });
    socket.on('appGetLocale', () => {
        const locale = app.getLocale();
        electronSocket.emit('appGetLocaleCompleted', locale);
    });
    socket.on('appAddRecentDocument', (path) => {
        app.addRecentDocument(path);
    });
    socket.on('appClearRecentDocuments', () => {
        app.clearRecentDocuments();
    });
    socket.on('appSetAsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.setAsDefaultProtocolClient(protocol, path, args);
        electronSocket.emit('appSetAsDefaultProtocolClientCompleted', success);
    });
    socket.on('appRemoveAsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.removeAsDefaultProtocolClient(protocol, path, args);
        electronSocket.emit('appRemoveAsDefaultProtocolClientCompleted', success);
    });
    socket.on('appIsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.isDefaultProtocolClient(protocol, path, args);
        electronSocket.emit('appIsDefaultProtocolClientCompleted', success);
    });
    socket.on('appSetUserTasks', (tasks) => {
        const success = app.setUserTasks(tasks);
        electronSocket.emit('appSetUserTasksCompleted', success);
    });
    socket.on('appGetJumpListSettings', () => {
        const jumpListSettings = app.getJumpListSettings();
        electronSocket.emit('appGetJumpListSettingsCompleted', jumpListSettings);
    });
    socket.on('appSetJumpList', (categories) => {
        app.setJumpList(categories);
    });
    socket.on('appRequestSingleInstanceLock', () => {
        app.on('second-instance', (args, workingDirectory) => {
            electronSocket.emit('secondInstance', [args, workingDirectory]);
        });
        const success = app.requestSingleInstanceLock();
        electronSocket.emit('appRequestSingleInstanceLockCompleted', success);
    });
    socket.on('appReleaseSingleInstanceLock', () => {
        app.releaseSingleInstanceLock();
    });
    socket.on('appSetUserActivity', (type, userInfo, webpageURL) => {
        app.setUserActivity(type, userInfo, webpageURL);
    });
    socket.on('appGetCurrentActivityType', () => {
        const activityType = app.getCurrentActivityType();
        electronSocket.emit('appGetCurrentActivityTypeCompleted', activityType);
    });
    socket.on('appSetAppUserModelId', (id) => {
        app.setAppUserModelId(id);
    });
    socket.on('appImportCertificate', (options) => {
        app.importCertificate(options, (result) => {
            electronSocket.emit('appImportCertificateCompleted', result);
        });
    });
    socket.on('appGetAppMetrics', () => {
        const processMetrics = app.getAppMetrics();
        electronSocket.emit('appGetAppMetricsCompleted', processMetrics);
    });
    socket.on('appGetGpuFeatureStatus', () => {
        const gpuFeatureStatus = app.getGPUFeatureStatus();
        electronSocket.emit('appGetGpuFeatureStatusCompleted', gpuFeatureStatus);
    });
    socket.on('appSetBadgeCount', (count) => {
        const success = app.setBadgeCount(count);
        electronSocket.emit('appSetBadgeCountCompleted', success);
    });
    socket.on('appGetBadgeCount', () => {
        const count = app.getBadgeCount();
        electronSocket.emit('appGetBadgeCountCompleted', count);
    });
    socket.on('appIsUnityRunning', () => {
        const isUnityRunning = app.isUnityRunning();
        electronSocket.emit('appIsUnityRunningCompleted', isUnityRunning);
    });
    socket.on('appGetLoginItemSettings', (options) => {
        const loginItemSettings = app.getLoginItemSettings(options);
        electronSocket.emit('appGetLoginItemSettingsCompleted', loginItemSettings);
    });
    socket.on('appSetLoginItemSettings', (settings) => {
        app.setLoginItemSettings(settings);
    });
    socket.on('appIsAccessibilitySupportEnabled', () => {
        const isAccessibilitySupportEnabled = app.isAccessibilitySupportEnabled();
        electronSocket.emit('appIsAccessibilitySupportEnabledCompleted', isAccessibilitySupportEnabled);
    });
    socket.on('appSetAboutPanelOptions', (options) => {
        app.setAboutPanelOptions(options);
    });
    socket.on('appCommandLineAppendSwitch', (theSwitch, value) => {
        app.commandLine.appendSwitch(theSwitch, value);
    });
    socket.on('appCommandLineAppendArgument', (value) => {
        app.commandLine.appendArgument(value);
    });
    socket.on('appDockBounce', (type) => {
        const id = app.dock.bounce(type);
        electronSocket.emit('appDockBounceCompleted', id);
    });
    socket.on('appDockCancelBounce', (id) => {
        app.dock.cancelBounce(id);
    });
    socket.on('appDockDownloadFinished', (filePath) => {
        app.dock.downloadFinished(filePath);
    });
    socket.on('appDockSetBadge', (text) => {
        app.dock.setBadge(text);
    });
    socket.on('appDockGetBadge', () => {
        const text = app.dock.getBadge();
        electronSocket.emit('appDockGetBadgeCompleted', text);
    });
    socket.on('appDockHide', () => {
        app.dock.hide();
    });
    socket.on('appDockShow', () => {
        app.dock.show();
    });
    socket.on('appDockIsVisible', () => {
        const isVisible = app.dock.isVisible();
        electronSocket.emit('appDockIsVisibleCompleted', isVisible);
    });
    // TODO: Menü Lösung muss noch implementiert werden
    socket.on('appDockSetMenu', (menu) => {
        app.dock.setMenu(menu);
    });
    socket.on('appDockSetIcon', (image) => {
        app.dock.setIcon(image);
    });
};
//# sourceMappingURL=app.js.map