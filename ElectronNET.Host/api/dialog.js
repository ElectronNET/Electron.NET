"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('showMessageBox', (browserWindow, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        if ('id' in browserWindow) {
            const window = electron_1.BrowserWindow.fromId(browserWindow.id);
            const messageBoxReturnValue = yield electron_1.dialog.showMessageBox(window, options);
            electronSocket.emit('showMessageBoxComplete' + guid, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
        else {
            const id = guid || options;
            const messageBoxReturnValue = yield electron_1.dialog.showMessageBox(browserWindow);
            electronSocket.emit('showMessageBoxComplete' + id, [messageBoxReturnValue.response, messageBoxReturnValue.checkboxChecked]);
        }
    }));
    socket.on('showOpenDialog', (browserWindow, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const openDialogReturnValue = yield electron_1.dialog.showOpenDialog(window, options);
        electronSocket.emit('showOpenDialogComplete' + guid, openDialogReturnValue.filePaths || []);
    }));
    socket.on('showSaveDialog', (browserWindow, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        const saveDialogReturnValue = yield electron_1.dialog.showSaveDialog(window, options);
        electronSocket.emit('showSaveDialogComplete' + guid, saveDialogReturnValue.filePath || '');
    }));
    socket.on('showErrorBox', (title, content) => {
        electron_1.dialog.showErrorBox(title, content);
    });
    socket.on('showCertificateTrustDialog', (browserWindow, options, guid) => __awaiter(void 0, void 0, void 0, function* () {
        const window = electron_1.BrowserWindow.fromId(browserWindow.id);
        yield electron_1.dialog.showCertificateTrustDialog(window, options);
        electronSocket.emit('showCertificateTrustDialogComplete' + guid);
    }));
};
//# sourceMappingURL=dialog.js.map