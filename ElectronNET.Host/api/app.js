"use strict";
let isQuitWindowAllClosed = true;
let appWindowAllClosedEventId;
module.exports = (socket, app) => {
    // By default, quit when all windows are closed
    app.on('window-all-closed', () => {
        // On macOS it is common for applications and their menu bar
        // to stay active until the user quits explicitly with Cmd + Q
        if (process.platform !== 'darwin' && isQuitWindowAllClosed) {
            socket.invoke('AppWindowAllClosed', 0);
            app.quit();
        }
        else if (appWindowAllClosedEventId) {
            // If the user is on macOS
            // - OR -
            // If the user has indicated NOT to quit when all windows are closed,
            // emit the event.
            socket.invoke('AppWindowAllClosed', appWindowAllClosedEventId);
        }
    });
    socket.on('quit-app-window-all-closed-event', (quit) => {
        isQuitWindowAllClosed = quit;
    });
    socket.on('register-app-window-all-closed-event', (id) => {
        socket.invoke('AppWindowAllClosed', id);
    });
    socket.on('register-app-before-quit-event', (id) => {
        app.on('before-quit', (event) => {
            event.preventDefault();
            socket.invoke('AppBeforeQuit', id);
        });
    });
    socket.on('register-app-will-quit-event', (id) => {
        app.on('will-quit', (event) => {
            event.preventDefault();
            socket.invoke('AppWillQuit', id);
        });
    });
    socket.on('register-app-browser-window-blur-event', (id) => {
        app.on('browser-window-blur', () => {
            socket.invoke('AppBrowserWindowBlur', id);
        });
    });
    socket.on('register-app-browser-window-focus-event', (id) => {
        app.on('browser-window-focus', () => {
            socket.invoke('AppBrowserWindowFocus', id);
        });
    });
    socket.on('register-app-browser-window-created-event', (id) => {
        app.on('browser-window-created', () => {
            socket.invoke('AppBrowserWindowCreated', id);
        });
    });
    socket.on('register-app-web-contents-created-event', (id) => {
        app.on('web-contents-created', () => {
            socket.invoke('AppWebContentsCreated', id);
        });
    });
    socket.on('register-app-accessibility-support-changed-event', (id) => {
        app.on('accessibility-support-changed', (event, accessibilitySupportEnabled) => {
            socket.invoke('AppAccessibilitySupportChanged', id, accessibilitySupportEnabled);
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
    socket.on('appGetAppPath', (guid) => {
        const path = app.getAppPath();
        socket.invoke('SendClientResponseString', guid, path);
    });
    socket.on('appSetAppLogsPath', (path) => {
        app.setAppLogsPath(path);
    });
    socket.on('appGetPath', (guid, name) => {
        const path = app.getPath(name);
        socket.invoke('SendClientResponseString', guid, path);
    });
    socket.on('appGetFileIcon', async (path, options) => {
        let error = {};
        if (options) {
            const nativeImage = await app.getFileIcon(path, options).catch((errorFileIcon) => error = errorFileIcon);
            socket.invoke('appGetFileIconCompleted', [error, nativeImage]);
        }
        else {
            const nativeImage = await app.getFileIcon(path).catch((errorFileIcon) => error = errorFileIcon);
            socket.invoke('appGetFileIconCompleted', [error, nativeImage]);
        }
    });
    socket.on('appSetPath', (name, path) => {
        app.setPath(name, path);
    });
    socket.on('appGetVersion', (guid) => {
        const version = app.getVersion();
        socket.invoke('SendClientResponseString', guid, version);
    });
    socket.on('appGetName', (guid) => {
        socket.invoke('SendClientResponseString', guid, name);
    });
    socket.on('appSetName', (name) => {
        app.name = name;
    });
    socket.on('appGetLocale', (guid) => {
        const locale = app.getLocale();
        socket.invoke('SendClientResponseString', guid, locale);
    });
    socket.on('appAddRecentDocument', (path) => {
        app.addRecentDocument(path);
    });
    socket.on('appClearRecentDocuments', () => {
        app.clearRecentDocuments();
    });
    socket.on('appSetAsDefaultProtocolClient', (guid, protocol, path, args) => {
        const success = app.setAsDefaultProtocolClient(protocol, path, args);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('appRemoveAsDefaultProtocolClient', (guid, protocol, path, args) => {
        const success = app.removeAsDefaultProtocolClient(protocol, path, args);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('appIsDefaultProtocolClient', (guid, protocol, path, args) => {
        const success = app.isDefaultProtocolClient(protocol, path, args);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('appSetUserTasks', (guid, tasks) => {
        const success = app.setUserTasks(tasks);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('appGetJumpListSettings', (guid) => {
        const jumpListSettings = app.getJumpListSettings();
        socket.invoke('SendClientResponseJObject', guid, jumpListSettings);
    });
    socket.on('appSetJumpList', (categories) => {
        app.setJumpList(categories);
    });
    socket.on('appRequestSingleInstanceLock', (guid) => {
        const success = app.requestSingleInstanceLock();
        socket.invoke('SendClientResponseBool', guid, success);
        app.on('second-instance', (event, args = [], workingDirectory = '') => {
            socket.invoke('SendClientResponseJArray', guid, [args, workingDirectory]);
        });
    });
    socket.on('appHasSingleInstanceLock', (guid) => {
        const hasLock = app.hasSingleInstanceLock();
        socket.invoke('SendClientResponseBool', guid, hasLock);
    });
    socket.on('appReleaseSingleInstanceLock', () => {
        app.releaseSingleInstanceLock();
    });
    socket.on('appSetUserActivity', (type, userInfo, webpageUrl) => {
        app.setUserActivity(type, userInfo, webpageUrl);
    });
    socket.on('appGetCurrentActivityType', (guid) => {
        const activityType = app.getCurrentActivityType();
        socket.invoke('SendClientResponseString', guid, activityType);
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
    socket.on('appImportCertificate', (guid, options) => {
        app.importCertificate(options, (result) => {
            socket.invoke('SendClientResponseString', guid, result);
        });
    });
    socket.on('appGetAppMetrics', (guid) => {
        const processMetrics = app.getAppMetrics();
        socket.invoke('SendClientResponseJArray', guid, processMetrics);
    });
    socket.on('appGetGpuFeatureStatus', (guid) => {
        const gpuFeatureStatus = app.getGPUFeatureStatus();
        socket.invoke('SendClientResponseJObject', guid, gpuFeatureStatus);
    });
    socket.on('appSetBadgeCount', (guid, count) => {
        const success = app.setBadgeCount(count);
        socket.invoke('SendClientResponseBool', guid, success);
    });
    socket.on('appGetBadgeCount', (guid) => {
        const count = app.getBadgeCount();
        socket.invoke('SendClientResponseString', guid, count);
    });
    socket.on('appIsUnityRunning', (guid) => {
        const isUnityRunning = app.isUnityRunning();
        socket.invoke('SendClientResponseBool', guid, isUnityRunning);
    });
    socket.on('appGetLoginItemSettings', (guid) => {
        const loginItemSettings = app.getLoginItemSettings();
        socket.invoke('SendClientResponseJObject', guid, loginItemSettings);
    });
    socket.on('appGetLoginItemSettingsWithOptions', (guid, options) => {
        const loginItemSettings = app.getLoginItemSettings(options);
        socket.invoke('SendClientResponseJObject', guid, loginItemSettings);
    });
    socket.on('appSetLoginItemSettings', (settings) => {
        app.setLoginItemSettings(settings);
    });
    socket.on('appIsAccessibilitySupportEnabled', (guid) => {
        const isAccessibilitySupportEnabled = app.isAccessibilitySupportEnabled();
        socket.invoke('SendClientResponseBool', guid, isAccessibilitySupportEnabled);
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
    socket.on('appGetUserAgentFallback', (guid) => {
        socket.invoke('SendClientResponseString', guid, app.userAgentFallback);
    });
    socket.on('appSetUserAgentFallback', (userAgent) => {
        app.userAgentFallback = userAgent;
    });
    // Testing, this is potentially dangerous
    socket.on('appEval', (evalString) => {
        console.log("######################");
        console.log(evalString);
        console.log("######################");
        eval(evalString);
    });
    //ToDo: We dont know what type we need to return here
    socket.on('register-app-on-event', (eventName, listenerName) => {
        app.on(eventName, (...args) => {
            console.log(listenerName);
            if (args.length > 1) {
                socket.invoke(listenerName, args[1]);
            }
            else {
                socket.invoke(listenerName);
            }
        });
    });
    socket.on('register-app-once-event', (eventName, listenerName) => {
        app.once(eventName, (...args) => {
            if (args.length > 1) {
                socket.invoke(listenerName, args[1]);
            }
            else {
                socket.invoke(listenerName);
            }
        });
    });
};
//# sourceMappingURL=app.js.map