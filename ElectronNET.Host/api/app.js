"use strict";
let isQuitWindowAllClosed = true, electronSocket;
let appWindowAllClosedEventId;
module.exports = (socket, app) => {
    electronSocket = socket;
    // By default, quit when all windows are closed
    app.on('window-all-closed', () => {
        // On macOS it is common for applications and their menu bar
        // to stay active until the user quits explicitly with Cmd + Q
        if (process.platform !== 'darwin' && isQuitWindowAllClosed) {
            app.quit();
        }
        else if (appWindowAllClosedEventId) {
            // If the user is on macOS
            // - OR -
            // If the user has indicated NOT to quit when all windows are closed,
            // emit the event.
            electronSocket.emit('app-window-all-closed' + appWindowAllClosedEventId);
        }
    });
    socket.on('quit-app-window-all-closed-event', (quit) => {
        isQuitWindowAllClosed = quit;
    });
    socket.on('register-app-window-all-closed-event', (id) => {
        appWindowAllClosedEventId = id;
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
    socket.on('appFocus', (options) => {
        app.focus(options);
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
    socket.on('appSetAppLogsPath', (path) => {
        app.setAppLogsPath(path);
    });
    socket.on('appGetPath', (name) => {
        const path = app.getPath(name);
        electronSocket.emit('appGetPathCompleted', path);
    });
    socket.on('appGetFileIcon', async (path, options) => {
        let error = {};
        if (options) {
            const nativeImage = await app.getFileIcon(path, options).catch((errorFileIcon) => error = errorFileIcon);
            electronSocket.emit('appGetFileIconCompleted', [error, nativeImage]);
        }
        else {
            const nativeImage = await app.getFileIcon(path).catch((errorFileIcon) => error = errorFileIcon);
            electronSocket.emit('appGetFileIconCompleted', [error, nativeImage]);
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
        electronSocket.emit('appGetNameCompleted', app.name);
    });
    socket.on('appSetName', (name) => {
        app.name = name;
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
    socket.on('appHasSingleInstanceLock', () => {
        const hasLock = app.hasSingleInstanceLock();
        electronSocket.emit('appHasSingleInstanceLockCompleted', hasLock);
    });
    socket.on('appReleaseSingleInstanceLock', () => {
        app.releaseSingleInstanceLock();
    });
    socket.on('appSetUserActivity', (type, userInfo, webpageUrl) => {
        app.setUserActivity(type, userInfo, webpageUrl);
    });
    socket.on('appGetCurrentActivityType', () => {
        const activityType = app.getCurrentActivityType();
        electronSocket.emit('appGetCurrentActivityTypeCompleted', activityType);
    });
    socket.on('appInvalidateCurrentActivity', () => {
        app.invalidateCurrentActivity();
    });
    socket.on('appResignCurrentActivity', () => {
        app.resignCurrentActivity();
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
    socket.on('appSetAccessibilitySupportEnabled', (enabled) => {
        app.setAccessibilitySupportEnabled(enabled);
    });
    socket.on('appShowAboutPanel', () => {
        app.showAboutPanel();
    });
    socket.on('appSetAboutPanelOptions', (options) => {
        app.setAboutPanelOptions(options);
    });
    socket.on('appGetUserAgentFallback', () => {
        electronSocket.emit('appGetUserAgentFallbackCompleted', app.userAgentFallback);
    });
    socket.on('appSetUserAgentFallback', (userAgent) => {
        app.userAgentFallback = userAgent;
    });
    socket.on('register-app-on-event', (eventName, listenerName) => {
        app.on(eventName, (...args) => {
            if (args.length > 1) {
                electronSocket.emit(listenerName, args[1]);
            }
            else {
                electronSocket.emit(listenerName);
            }
        });
    });
    socket.on('register-app-once-event', (eventName, listenerName) => {
        app.once(eventName, (...args) => {
            if (args.length > 1) {
                electronSocket.emit(listenerName, args[1]);
            }
            else {
                electronSocket.emit(listenerName);
            }
        });
    });
};
//# sourceMappingURL=app.js.map