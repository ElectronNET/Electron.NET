import type { Socket } from "net";
import { autoUpdater } from "electron-updater";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;

  socket.on("register-autoUpdater-error", (id) => {
    autoUpdater.on("error", (error) => {
      electronSocket.emit("autoUpdater-error" + id, error.message);
    });
  });

  socket.on("register-autoUpdater-checking-for-update", (id) => {
    autoUpdater.on("checking-for-update", () => {
      electronSocket.emit("autoUpdater-checking-for-update" + id);
    });
  });

  socket.on("register-autoUpdater-update-available", (id) => {
    autoUpdater.on("update-available", (updateInfo) => {
      electronSocket.emit("autoUpdater-update-available" + id, updateInfo);
    });
  });

  socket.on("register-autoUpdater-update-not-available", (id) => {
    autoUpdater.on("update-not-available", (updateInfo) => {
      electronSocket.emit("autoUpdater-update-not-available" + id, updateInfo);
    });
  });

  socket.on("register-autoUpdater-download-progress", (id) => {
    autoUpdater.on("download-progress", (progressInfo) => {
      electronSocket.emit("autoUpdater-download-progress" + id, progressInfo);
    });
  });

  socket.on("register-autoUpdater-update-downloaded", (id) => {
    autoUpdater.on("update-downloaded", (updateInfo) => {
      electronSocket.emit("autoUpdater-update-downloaded" + id, updateInfo);
    });
  });

  // Properties *****

  socket.on("autoUpdater-autoDownload", () => {
    electronSocket.emit(
      "autoUpdater-autoDownload-completed",
      autoUpdater.autoDownload,
    );
  });

  socket.on("autoUpdater-autoDownload-set", (value) => {
    autoUpdater.autoDownload = value;
  });

  socket.on("autoUpdater-autoInstallOnAppQuit", () => {
    electronSocket.emit(
      "autoUpdater-autoInstallOnAppQuit-completed",
      autoUpdater.autoInstallOnAppQuit,
    );
  });

  socket.on("autoUpdater-autoInstallOnAppQuit-set", (value) => {
    autoUpdater.autoInstallOnAppQuit = value;
  });

  socket.on("autoUpdater-allowPrerelease", () => {
    electronSocket.emit(
      "autoUpdater-allowPrerelease-completed",
      autoUpdater.allowPrerelease,
    );
  });

  socket.on("autoUpdater-allowPrerelease-set", (value) => {
    autoUpdater.allowPrerelease = value;
  });

  socket.on("autoUpdater-fullChangelog", () => {
    electronSocket.emit(
      "autoUpdater-fullChangelog-completed",
      autoUpdater.fullChangelog,
    );
  });

  socket.on("autoUpdater-fullChangelog-set", (value) => {
    autoUpdater.fullChangelog = value;
  });

  socket.on("autoUpdater-allowDowngrade", () => {
    electronSocket.emit(
      "autoUpdater-allowDowngrade-completed",
      autoUpdater.allowDowngrade,
    );
  });

  socket.on("autoUpdater-allowDowngrade-set", (value) => {
    autoUpdater.allowDowngrade = value;
  });

  socket.on("autoUpdater-updateConfigPath", () => {
    electronSocket.emit(
      "autoUpdater-updateConfigPath-completed",
      autoUpdater.updateConfigPath || "",
    );
  });

  socket.on("autoUpdater-updateConfigPath-set", (value) => {
    autoUpdater.updateConfigPath = value;
  });

  socket.on("autoUpdater-currentVersion", () => {
    electronSocket.emit(
      "autoUpdater-currentVersion-completed",
      autoUpdater.currentVersion,
    );
  });

  socket.on("autoUpdater-channel", () => {
    electronSocket.emit(
      "autoUpdater-channel-completed",
      autoUpdater.channel || "",
    );
  });

  socket.on("autoUpdater-channel-set", (value) => {
    autoUpdater.channel = value;
  });

  socket.on("autoUpdater-requestHeaders", () => {
    electronSocket.emit(
      "autoUpdater-requestHeaders-completed",
      autoUpdater.requestHeaders,
    );
  });

  socket.on("autoUpdater-requestHeaders-set", (value) => {
    autoUpdater.requestHeaders = value;
  });

  socket.on("autoUpdater-checkForUpdatesAndNotify", async (guid) => {
    autoUpdater
      .checkForUpdatesAndNotify()
      .then((updateCheckResult) => {
        electronSocket.emit(
          "autoUpdater-checkForUpdatesAndNotify-completed" + guid,
          updateCheckResult,
        );
      })
      .catch((error) => {
        electronSocket.emit(
          "autoUpdater-checkForUpdatesAndNotifyError" + guid,
          error,
        );
      });
  });

  socket.on("autoUpdater-checkForUpdates", async (guid) => {
    autoUpdater
      .checkForUpdates()
      .then((updateCheckResult) => {
        electronSocket.emit(
          "autoUpdater-checkForUpdates-completed" + guid,
          updateCheckResult,
        );
      })
      .catch((error) => {
        electronSocket.emit("autoUpdater-checkForUpdatesError" + guid, error);
      });
  });

  socket.on("autoUpdater-quitAndInstall", async (isSilent, isForceRunAfter) => {
    autoUpdater.quitAndInstall(isSilent, isForceRunAfter);
  });

  socket.on("autoUpdater-downloadUpdate", async (guid) => {
    const downloadedPath = await autoUpdater.downloadUpdate();
    electronSocket.emit(
      "autoUpdater-downloadUpdate-completed" + guid,
      downloadedPath,
    );
  });

  socket.on("autoUpdater-getFeedURL", async (guid) => {
    const feedUrl = await autoUpdater.getFeedURL();
    electronSocket.emit(
      "autoUpdater-getFeedURL-completed" + guid,
      feedUrl || "",
    );
  });
};
