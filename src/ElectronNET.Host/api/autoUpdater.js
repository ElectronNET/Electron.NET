"use strict";
const electron_updater_1 = require("electron-updater");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on("register-autoUpdater-error", (id) => {
        electron_updater_1.autoUpdater.on("error", (error) => {
            electronSocket.emit("autoUpdater-error" + id, error.message);
        });
    });
    socket.on("register-autoUpdater-checking-for-update", (id) => {
        electron_updater_1.autoUpdater.on("checking-for-update", () => {
            electronSocket.emit("autoUpdater-checking-for-update" + id);
        });
    });
    socket.on("register-autoUpdater-update-available", (id) => {
        electron_updater_1.autoUpdater.on("update-available", (updateInfo) => {
            electronSocket.emit("autoUpdater-update-available" + id, updateInfo);
        });
    });
    socket.on("register-autoUpdater-update-not-available", (id) => {
        electron_updater_1.autoUpdater.on("update-not-available", (updateInfo) => {
            electronSocket.emit("autoUpdater-update-not-available" + id, updateInfo);
        });
    });
    socket.on("register-autoUpdater-download-progress", (id) => {
        electron_updater_1.autoUpdater.on("download-progress", (progressInfo) => {
            electronSocket.emit("autoUpdater-download-progress" + id, progressInfo);
        });
    });
    socket.on("register-autoUpdater-update-downloaded", (id) => {
        electron_updater_1.autoUpdater.on("update-downloaded", (updateInfo) => {
            electronSocket.emit("autoUpdater-update-downloaded" + id, updateInfo);
        });
    });
    // Properties *****
    socket.on("autoUpdater-autoDownload", () => {
        electronSocket.emit("autoUpdater-autoDownload-completed", electron_updater_1.autoUpdater.autoDownload);
    });
    socket.on("autoUpdater-autoDownload-set", (value) => {
        electron_updater_1.autoUpdater.autoDownload = value;
    });
    socket.on("autoUpdater-autoInstallOnAppQuit", () => {
        electronSocket.emit("autoUpdater-autoInstallOnAppQuit-completed", electron_updater_1.autoUpdater.autoInstallOnAppQuit);
    });
    socket.on("autoUpdater-autoInstallOnAppQuit-set", (value) => {
        electron_updater_1.autoUpdater.autoInstallOnAppQuit = value;
    });
    socket.on("autoUpdater-allowPrerelease", () => {
        electronSocket.emit("autoUpdater-allowPrerelease-completed", electron_updater_1.autoUpdater.allowPrerelease);
    });
    socket.on("autoUpdater-allowPrerelease-set", (value) => {
        electron_updater_1.autoUpdater.allowPrerelease = value;
    });
    socket.on("autoUpdater-fullChangelog", () => {
        electronSocket.emit("autoUpdater-fullChangelog-completed", electron_updater_1.autoUpdater.fullChangelog);
    });
    socket.on("autoUpdater-fullChangelog-set", (value) => {
        electron_updater_1.autoUpdater.fullChangelog = value;
    });
    socket.on("autoUpdater-allowDowngrade", () => {
        electronSocket.emit("autoUpdater-allowDowngrade-completed", electron_updater_1.autoUpdater.allowDowngrade);
    });
    socket.on("autoUpdater-allowDowngrade-set", (value) => {
        electron_updater_1.autoUpdater.allowDowngrade = value;
    });
    socket.on("autoUpdater-updateConfigPath", () => {
        electronSocket.emit("autoUpdater-updateConfigPath-completed", electron_updater_1.autoUpdater.updateConfigPath || "");
    });
    socket.on("autoUpdater-updateConfigPath-set", (value) => {
        electron_updater_1.autoUpdater.updateConfigPath = value;
    });
    socket.on("autoUpdater-currentVersion", () => {
        electronSocket.emit("autoUpdater-currentVersion-completed", electron_updater_1.autoUpdater.currentVersion);
    });
    socket.on("autoUpdater-channel", () => {
        electronSocket.emit("autoUpdater-channel-completed", electron_updater_1.autoUpdater.channel || "");
    });
    socket.on("autoUpdater-channel-set", (value) => {
        electron_updater_1.autoUpdater.channel = value;
    });
    socket.on("autoUpdater-requestHeaders", () => {
        electronSocket.emit("autoUpdater-requestHeaders-completed", electron_updater_1.autoUpdater.requestHeaders);
    });
    socket.on("autoUpdater-requestHeaders-set", (value) => {
        electron_updater_1.autoUpdater.requestHeaders = value;
    });
    socket.on("autoUpdater-checkForUpdatesAndNotify", async (guid) => {
        electron_updater_1.autoUpdater
            .checkForUpdatesAndNotify()
            .then((updateCheckResult) => {
            electronSocket.emit("autoUpdater-checkForUpdatesAndNotify-completed" + guid, updateCheckResult);
        })
            .catch((error) => {
            electronSocket.emit("autoUpdater-checkForUpdatesAndNotifyError" + guid, error);
        });
    });
    socket.on("autoUpdater-checkForUpdates", async (guid) => {
        electron_updater_1.autoUpdater
            .checkForUpdates()
            .then((updateCheckResult) => {
            electronSocket.emit("autoUpdater-checkForUpdates-completed" + guid, updateCheckResult);
        })
            .catch((error) => {
            electronSocket.emit("autoUpdater-checkForUpdatesError" + guid, error);
        });
    });
    socket.on("autoUpdater-quitAndInstall", async (isSilent, isForceRunAfter) => {
        electron_updater_1.autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
    });
    socket.on("autoUpdater-downloadUpdate", async (guid) => {
        const downloadedPath = await electron_updater_1.autoUpdater.downloadUpdate();
        electronSocket.emit("autoUpdater-downloadUpdate-completed" + guid, downloadedPath);
    });
    socket.on("autoUpdater-getFeedURL", async (guid) => {
        const feedUrl = await electron_updater_1.autoUpdater.getFeedURL();
        electronSocket.emit("autoUpdater-getFeedURL-completed" + guid, feedUrl || "");
    });
};
//# sourceMappingURL=autoUpdater.js.map