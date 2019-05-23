"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const electron_updater_1 = require("electron-updater");
const path = require('path');
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    // Events ********
    socket.on('register-autoUpdater-error-event', (id) => {
        electron_updater_1.autoUpdater.on('error', (error) => {
            electronSocket.emit('autoUpdater-error' + id, error.message);
        });
    });
    socket.on('register-autoUpdater-checking-for-update-event', (id) => {
        electron_updater_1.autoUpdater.on('checking-for-update', () => {
            electronSocket.emit('autoUpdater-checking-for-update' + id);
        });
    });
    socket.on('register-autoUpdater-update-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-available' + id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-update-not-available-event', (id) => {
        electron_updater_1.autoUpdater.on('update-not-available', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-not-available' + id, updateInfo);
        });
    });
    socket.on('register-autoUpdater-download-progress-event', (id) => {
        electron_updater_1.autoUpdater.on('download-progress', (progressInfo) => {
            electronSocket.emit('autoUpdater-download-progress' + id, progressInfo);
        });
    });
    socket.on('register-autoUpdater-update-downloaded-event', (id) => {
        electron_updater_1.autoUpdater.on('update-downloaded', (updateInfo) => {
            electronSocket.emit('autoUpdater-update-downloaded' + id, updateInfo);
        });
    });
    // Methods ********
    socket.on('autoUpdaterCheckForUpdatesAndNotify', (guid) => __awaiter(this, void 0, void 0, function* () {
        const updateCheckResult = yield electron_updater_1.autoUpdater.checkForUpdatesAndNotify();
        electronSocket.emit('autoUpdaterCheckForUpdatesAndNotifyComplete' + guid, updateCheckResult);
    }));
    socket.on('autoUpdaterCheckForUpdates', (guid) => __awaiter(this, void 0, void 0, function* () {
        // autoUpdater.updateConfigPath = path.join(__dirname, 'dev-app-update.yml');
        const updateCheckResult = yield electron_updater_1.autoUpdater.checkForUpdates();
        electronSocket.emit('autoUpdaterCheckForUpdatesComplete' + guid, updateCheckResult);
    }));
    socket.on('autoUpdaterQuitAndInstall', (isSilent, isForceRunAfter) => __awaiter(this, void 0, void 0, function* () {
        electron_updater_1.autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
    }));
};
//# sourceMappingURL=autoUpdater.js.map