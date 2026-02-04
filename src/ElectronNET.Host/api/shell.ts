import type { Socket } from "net";
import { shell } from "electron";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;
  socket.on("shell-showItemInFolder", (fullPath) => {
    shell.showItemInFolder(fullPath);

    electronSocket.emit("shell-showItemInFolderCompleted");
  });

  socket.on("shell-openPath", async (path) => {
    const errorMessage = await shell.openPath(path);

    electronSocket.emit("shell-openPathCompleted", errorMessage);
  });

  socket.on("shell-openExternal", async (url, options) => {
    let result = "";

    if (options) {
      await shell.openExternal(url, options).catch((e) => {
        result = e.message;
      });
    } else {
      await shell.openExternal(url).catch((e) => {
        result = e.message;
      });
    }

    electronSocket.emit("shell-openExternalCompleted", result);
  });

  socket.on("shell-trashItem", async (fullPath, deleteOnFail) => {
    let success = false;

    try {
      await shell.trashItem(fullPath);
      success = true;
    } catch (error) {
      success = false;
    }

    electronSocket.emit("shell-trashItem-completed", success);
  });

  socket.on("shell-beep", () => {
    shell.beep();
  });

  socket.on("shell-writeShortcutLink", (shortcutPath, operation, options) => {
    const success = shell.writeShortcutLink(shortcutPath, operation, options);

    electronSocket.emit("shell-writeShortcutLinkCompleted", success);
  });

  socket.on("shell-readShortcutLink", (shortcutPath) => {
    const shortcutDetails = shell.readShortcutLink(shortcutPath);

    electronSocket.emit("shell-readShortcutLinkCompleted", shortcutDetails);
  });
};
