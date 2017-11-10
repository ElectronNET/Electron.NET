import { nativeImage as NativeImage } from 'electron';
let isQuitWindowAllClosed = true;

module.exports = (socket: SocketIO.Server, app: Electron.App) => {

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
            socket.emit('app-window-all-closed' + id);
        });
    });

    socket.on('register-app-before-quit-event', (id) => {
        app.on('before-quit', (event) => {
            event.preventDefault();

            socket.emit('app-before-quit' + id);
        });
    });

    socket.on('register-app-will-quit-event', (id) => {
        app.on('will-quit', (event) => {
            event.preventDefault();

            socket.emit('app-will-quit' + id);
        });
    });

    socket.on('register-app-browser-window-blur-event', (id) => {
        app.on('browser-window-blur', () => {
            socket.emit('app-browser-window-blur' + id);
        });
    });

    socket.on('register-app-browser-window-focus-event', (id) => {
        app.on('browser-window-focus', () => {
            socket.emit('app-browser-window-focus' + id);
        });
    });

    socket.on('register-app-browser-window-created-event', (id) => {
        app.on('browser-window-created', () => {
            socket.emit('app-browser-window-created' + id);
        });
    });

    socket.on('register-app-web-contents-created-event', (id) => {
        app.on('web-contents-created', () => {
            socket.emit('app-web-contents-created' + id);
        });
    });

    socket.on('register-app-accessibility-support-changed-event', (id) => {
        app.on('accessibility-support-changed', (event, accessibilitySupportEnabled) => {
            socket.emit('app-accessibility-support-changed' + id, accessibilitySupportEnabled);
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
        socket.emit('appGetAppPathCompleted', path);
    });

    socket.on('appGetPath', (name) => {
        const path = app.getPath(name);
        socket.emit('appGetPathCompleted', path);
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
                socket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        } else {
            app.getFileIcon(path, (error, nativeImage) => {
                socket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        }
    });

    socket.on('appSetPath', (name, path) => {
        app.setPath(name, path);
    });

    socket.on('appGetVersion', () => {
        const version = app.getVersion();
        socket.emit('appGetVersionCompleted', version);
    });

    socket.on('appGetName', () => {
        const name = app.getName();
        socket.emit('appGetNameCompleted', name);
    });

    socket.on('appSetName', (name) => {
        app.setName(name);
    });

    socket.on('appGetLocale', () => {
        const locale = app.getLocale();
        socket.emit('appGetLocaleCompleted', locale);
    });

    socket.on('appAddRecentDocument', (path) => {
        app.addRecentDocument(path);
    });

    socket.on('appClearRecentDocuments', () => {
        app.clearRecentDocuments();
    });

    socket.on('appSetAsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.setAsDefaultProtocolClient(protocol, path, args);
        socket.emit('appSetAsDefaultProtocolClientCompleted', success);
    });

    socket.on('appRemoveAsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.removeAsDefaultProtocolClient(protocol, path, args);
        socket.emit('appRemoveAsDefaultProtocolClientCompleted', success);
    });

    socket.on('appIsDefaultProtocolClient', (protocol, path, args) => {
        const success = app.isDefaultProtocolClient(protocol, path, args);
        socket.emit('appIsDefaultProtocolClientCompleted', success);
    });

    socket.on('appSetUserTasks', (tasks) => {
        const success = app.setUserTasks(tasks);
        socket.emit('appSetUserTasksCompleted', success);
    });

    socket.on('appGetJumpListSettings', () => {
        const jumpListSettings = app.getJumpListSettings();
        socket.emit('appGetJumpListSettingsCompleted', jumpListSettings);
    });

    socket.on('appSetJumpList', (categories) => {
        app.setJumpList(categories);
    });

    socket.on('appMakeSingleInstance', () => {
        const success = app.makeSingleInstance((args, workingDirectory) => {
            socket.emit('newInstanceOpened', [args, workingDirectory]);
        });
        socket.emit('appMakeSingleInstanceCompleted', success);
    });

    socket.on('appReleaseSingleInstance', () => {
        app.releaseSingleInstance();
    });

    socket.on('appSetUserActivity', (type, userInfo, webpageURL) => {
        app.setUserActivity(type, userInfo, webpageURL);
    });

    socket.on('appGetCurrentActivityType', () => {
        const activityType = app.getCurrentActivityType();
        socket.emit('appGetCurrentActivityTypeCompleted', activityType);
    });

    socket.on('appSetAppUserModelId', (id) => {
        app.setAppUserModelId(id);
    });

    socket.on('appImportCertificate', (options) => {
        app.importCertificate(options, (result) => {
            socket.emit('appImportCertificateCompleted', result);
        });
    });

    socket.on('appGetAppMetrics', () => {
        const processMetrics = app.getAppMetrics();
        socket.emit('appGetAppMetricsCompleted', processMetrics);
    });

    socket.on('appGetGpuFeatureStatus', () => {
        // TS Workaround - TS say getGpuFeatureStatus - but it is getGPUFeatureStatus
        let x = <any>app;
        const gpuFeatureStatus = x.getGPUFeatureStatus();
        socket.emit('appGetGpuFeatureStatusCompleted', gpuFeatureStatus);
    });

    socket.on('appSetBadgeCount', (count) => {
        const success = app.setBadgeCount(count);
        socket.emit('appSetBadgeCountCompleted', success);
    });

    socket.on('appGetBadgeCount', () => {
        const count = app.getBadgeCount();
        socket.emit('appGetBadgeCountCompleted', count);
    });

    socket.on('appIsUnityRunning', () => {
        const isUnityRunning = app.isUnityRunning();
        socket.emit('appIsUnityRunningCompleted', isUnityRunning);
    });

    socket.on('appGetLoginItemSettings', (options) => {
        const loginItemSettings = app.getLoginItemSettings(options);
        socket.emit('appGetLoginItemSettingsCompleted', loginItemSettings);
    });

    socket.on('appSetLoginItemSettings', (settings) => {
        app.setLoginItemSettings(settings);
    });

    socket.on('appIsAccessibilitySupportEnabled', () => {
        const isAccessibilitySupportEnabled = app.isAccessibilitySupportEnabled();
        socket.emit('appIsAccessibilitySupportEnabledCompleted', isAccessibilitySupportEnabled);
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

    socket.on('appEnableMixedSandbox', () => {
        app.enableMixedSandbox();
    });

    socket.on('appDockBounce', (type) => {
        const id = app.dock.bounce(type);
        socket.emit('appDockBounceCompleted', id);
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
        socket.emit('appDockGetBadgeCompleted', text);
    });

    socket.on('appDockHide', () => {
        app.dock.hide();
    });

    socket.on('appDockShow', () => {
        app.dock.show();
    });

    socket.on('appDockIsVisible', () => {
        const isVisible = app.dock.isVisible();
        socket.emit('appDockIsVisibleCompleted', isVisible);
    });

    // TODO: Menü Lösung muss noch implementiert werden
    socket.on('appDockSetMenu', (menu) => {
        app.dock.setMenu(menu);
    });

    socket.on('appDockSetIcon', (image) => {
        app.dock.setIcon(image);
    });
}