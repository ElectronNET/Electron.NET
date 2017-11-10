"use strict";
exports.__esModule = true;
var isQuitWindowAllClosed = true;
module.exports = function (socket, app) {
    // Quit when all windows are closed.
    app.on('window-all-closed', function () {
        // On macOS it is common for applications and their menu bar
        // to stay active until the user quits explicitly with Cmd + Q
        if (process.platform !== 'darwin' &&
            isQuitWindowAllClosed) {
            app.quit();
        }
    });
    socket.on('quit-app-window-all-closed-event', function (quit) {
        isQuitWindowAllClosed = quit;
    });
    socket.on('register-app-window-all-closed-event', function (id) {
        app.on('window-all-closed', function () {
            socket.emit('app-window-all-closed' + id);
        });
    });
    socket.on('register-app-before-quit-event', function (id) {
        app.on('before-quit', function (event) {
            event.preventDefault();
            socket.emit('app-before-quit' + id);
        });
    });
    socket.on('register-app-will-quit-event', function (id) {
        app.on('will-quit', function (event) {
            event.preventDefault();
            socket.emit('app-will-quit' + id);
        });
    });
    socket.on('register-app-browser-window-blur-event', function (id) {
        app.on('browser-window-blur', function () {
            socket.emit('app-browser-window-blur' + id);
        });
    });
    socket.on('register-app-browser-window-focus-event', function (id) {
        app.on('browser-window-focus', function () {
            socket.emit('app-browser-window-focus' + id);
        });
    });
    socket.on('register-app-browser-window-created-event', function (id) {
        app.on('browser-window-created', function () {
            socket.emit('app-browser-window-created' + id);
        });
    });
    socket.on('register-app-web-contents-created-event', function (id) {
        app.on('web-contents-created', function () {
            socket.emit('app-web-contents-created' + id);
        });
    });
    socket.on('register-app-accessibility-support-changed-event', function (id) {
        app.on('accessibility-support-changed', function (event, accessibilitySupportEnabled) {
            socket.emit('app-accessibility-support-changed' + id, accessibilitySupportEnabled);
        });
    });
    socket.on('appQuit', function () {
        app.quit();
    });
    socket.on('appExit', function (exitCode) {
        if (exitCode === void 0) { exitCode = 0; }
        app.exit(exitCode);
    });
    socket.on('appRelaunch', function (options) {
        app.relaunch(options);
    });
    socket.on('appFocus', function () {
        app.focus();
    });
    socket.on('appHide', function () {
        app.hide();
    });
    socket.on('appShow', function () {
        app.show();
    });
    socket.on('appGetAppPath', function () {
        var path = app.getAppPath();
        socket.emit('appGetAppPathCompleted', path);
    });
    socket.on('appGetPath', function (name) {
        var path = app.getPath(name);
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
    socket.on('appGetFileIcon', function (path, options) {
        if (options) {
            app.getFileIcon(path, options, function (error, nativeImage) {
                socket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        }
        else {
            app.getFileIcon(path, function (error, nativeImage) {
                socket.emit('appGetFileIconCompleted', [error, nativeImage]);
            });
        }
    });
    socket.on('appSetPath', function (name, path) {
        app.setPath(name, path);
    });
    socket.on('appGetVersion', function () {
        var version = app.getVersion();
        socket.emit('appGetVersionCompleted', version);
    });
    socket.on('appGetName', function () {
        var name = app.getName();
        socket.emit('appGetNameCompleted', name);
    });
    socket.on('appSetName', function (name) {
        app.setName(name);
    });
    socket.on('appGetLocale', function () {
        var locale = app.getLocale();
        socket.emit('appGetLocaleCompleted', locale);
    });
    socket.on('appAddRecentDocument', function (path) {
        app.addRecentDocument(path);
    });
    socket.on('appClearRecentDocuments', function () {
        app.clearRecentDocuments();
    });
    socket.on('appSetAsDefaultProtocolClient', function (protocol, path, args) {
        var success = app.setAsDefaultProtocolClient(protocol, path, args);
        socket.emit('appSetAsDefaultProtocolClientCompleted', success);
    });
    socket.on('appRemoveAsDefaultProtocolClient', function (protocol, path, args) {
        var success = app.removeAsDefaultProtocolClient(protocol, path, args);
        socket.emit('appRemoveAsDefaultProtocolClientCompleted', success);
    });
    socket.on('appIsDefaultProtocolClient', function (protocol, path, args) {
        var success = app.isDefaultProtocolClient(protocol, path, args);
        socket.emit('appIsDefaultProtocolClientCompleted', success);
    });
    socket.on('appSetUserTasks', function (tasks) {
        var success = app.setUserTasks(tasks);
        socket.emit('appSetUserTasksCompleted', success);
    });
    socket.on('appGetJumpListSettings', function () {
        var jumpListSettings = app.getJumpListSettings();
        socket.emit('appGetJumpListSettingsCompleted', jumpListSettings);
    });
    socket.on('appSetJumpList', function (categories) {
        app.setJumpList(categories);
    });
    socket.on('appMakeSingleInstance', function () {
        var success = app.makeSingleInstance(function (args, workingDirectory) {
            socket.emit('newInstanceOpened', [args, workingDirectory]);
        });
        socket.emit('appMakeSingleInstanceCompleted', success);
    });
    socket.on('appReleaseSingleInstance', function () {
        app.releaseSingleInstance();
    });
    socket.on('appSetUserActivity', function (type, userInfo, webpageURL) {
        app.setUserActivity(type, userInfo, webpageURL);
    });
    socket.on('appGetCurrentActivityType', function () {
        var activityType = app.getCurrentActivityType();
        socket.emit('appGetCurrentActivityTypeCompleted', activityType);
    });
    socket.on('appSetAppUserModelId', function (id) {
        app.setAppUserModelId(id);
    });
    socket.on('appImportCertificate', function (options) {
        app.importCertificate(options, function (result) {
            socket.emit('appImportCertificateCompleted', result);
        });
    });
    socket.on('appGetAppMetrics', function () {
        var processMetrics = app.getAppMetrics();
        socket.emit('appGetAppMetricsCompleted', processMetrics);
    });
    socket.on('appGetGpuFeatureStatus', function () {
        // TS Workaround - TS say getGpuFeatureStatus - but it is getGPUFeatureStatus
        var x = app;
        var gpuFeatureStatus = x.getGPUFeatureStatus();
        socket.emit('appGetGpuFeatureStatusCompleted', gpuFeatureStatus);
    });
    socket.on('appSetBadgeCount', function (count) {
        var success = app.setBadgeCount(count);
        socket.emit('appSetBadgeCountCompleted', success);
    });
    socket.on('appGetBadgeCount', function () {
        var count = app.getBadgeCount();
        socket.emit('appGetBadgeCountCompleted', count);
    });
    socket.on('appIsUnityRunning', function () {
        var isUnityRunning = app.isUnityRunning();
        socket.emit('appIsUnityRunningCompleted', isUnityRunning);
    });
    socket.on('appGetLoginItemSettings', function (options) {
        var loginItemSettings = app.getLoginItemSettings(options);
        socket.emit('appGetLoginItemSettingsCompleted', loginItemSettings);
    });
    socket.on('appSetLoginItemSettings', function (settings) {
        app.setLoginItemSettings(settings);
    });
    socket.on('appIsAccessibilitySupportEnabled', function () {
        var isAccessibilitySupportEnabled = app.isAccessibilitySupportEnabled();
        socket.emit('appIsAccessibilitySupportEnabledCompleted', isAccessibilitySupportEnabled);
    });
    socket.on('appSetAboutPanelOptions', function (options) {
        app.setAboutPanelOptions(options);
    });
    socket.on('appCommandLineAppendSwitch', function (theSwitch, value) {
        app.commandLine.appendSwitch(theSwitch, value);
    });
    socket.on('appCommandLineAppendArgument', function (value) {
        app.commandLine.appendArgument(value);
    });
    socket.on('appEnableMixedSandbox', function () {
        app.enableMixedSandbox();
    });
    socket.on('appDockBounce', function (type) {
        var id = app.dock.bounce(type);
        socket.emit('appDockBounceCompleted', id);
    });
    socket.on('appDockCancelBounce', function (id) {
        app.dock.cancelBounce(id);
    });
    socket.on('appDockDownloadFinished', function (filePath) {
        app.dock.downloadFinished(filePath);
    });
    socket.on('appDockSetBadge', function (text) {
        app.dock.setBadge(text);
    });
    socket.on('appDockGetBadge', function () {
        var text = app.dock.getBadge();
        socket.emit('appDockGetBadgeCompleted', text);
    });
    socket.on('appDockHide', function () {
        app.dock.hide();
    });
    socket.on('appDockShow', function () {
        app.dock.show();
    });
    socket.on('appDockIsVisible', function () {
        var isVisible = app.dock.isVisible();
        socket.emit('appDockIsVisibleCompleted', isVisible);
    });
    // TODO: Menü Lösung muss noch implementiert werden
    socket.on('appDockSetMenu', function (menu) {
        app.dock.setMenu(menu);
    });
    socket.on('appDockSetIcon', function (image) {
        app.dock.setIcon(image);
    });
};
//# sourceMappingURL=app.js.map