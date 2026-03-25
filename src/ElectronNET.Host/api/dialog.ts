import type { Socket } from "net";
import { BrowserWindow, dialog } from "electron";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;
  socket.on("showMessageBox", async (browserWindow, options, guid) => {
    if ("id" in browserWindow) {
      const window = BrowserWindow.fromId(browserWindow.id);

      const messageBoxReturnValue = await dialog.showMessageBox(
        window,
        options,
      );
      electronSocket.emit("showMessageBoxComplete" + guid, [
        messageBoxReturnValue.response,
        messageBoxReturnValue.checkboxChecked,
      ]);
    } else {
      const id = guid || options;
      const messageBoxReturnValue = await dialog.showMessageBox(browserWindow);

      electronSocket.emit("showMessageBoxComplete" + id, [
        messageBoxReturnValue.response,
        messageBoxReturnValue.checkboxChecked,
      ]);
    }
  });

  socket.on("showOpenDialog", async (browserWindow, options, guid) => {
    const window = BrowserWindow.fromId(browserWindow.id);
    const openDialogReturnValue = await dialog.showOpenDialog(window, options);

    electronSocket.emit(
      "showOpenDialogComplete" + guid,
      openDialogReturnValue.filePaths || [],
    );
  });

  socket.on("showSaveDialog", async (browserWindow, options, guid) => {
    const window = BrowserWindow.fromId(browserWindow.id);
    const saveDialogReturnValue = await dialog.showSaveDialog(window, options);

    electronSocket.emit(
      "showSaveDialogComplete" + guid,
      saveDialogReturnValue.filePath || "",
    );
  });

  socket.on("showErrorBox", (title, content) => {
    dialog.showErrorBox(title, content);
  });

  socket.on(
    "showCertificateTrustDialog",
    async (browserWindow, options, guid) => {
      const window = BrowserWindow.fromId(browserWindow.id);
      await dialog.showCertificateTrustDialog(window, options);

      electronSocket.emit("showCertificateTrustDialogComplete" + guid);
    },
  );
};
