"use strict";
const electron_1 = require("electron");
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('nativeTheme-shouldUseDarkColors', () => {
        const shouldUseDarkColors = electron_1.nativeTheme.shouldUseDarkColors;
        electronSocket.emit('nativeTheme-shouldUseDarkColors-completed', shouldUseDarkColors);
    });
    socket.on('nativeTheme-shouldUseHighContrastColors', () => {
        const shouldUseHighContrastColors = electron_1.nativeTheme.shouldUseHighContrastColors;
        electronSocket.emit('nativeTheme-shouldUseHighContrastColors-completed', shouldUseHighContrastColors);
    });
    socket.on('nativeTheme-shouldUseInvertedColorScheme', () => {
        const shouldUseInvertedColorScheme = electron_1.nativeTheme.shouldUseInvertedColorScheme;
        electronSocket.emit('nativeTheme-shouldUseInvertedColorScheme-completed', shouldUseInvertedColorScheme);
    });
    socket.on('nativeTheme-themeSource-get', () => {
        const themeSource = electron_1.nativeTheme.themeSource;
        electronSocket.emit('nativeTheme-themeSource-getCompleted', themeSource);
    });
    socket.on('nativeTheme-themeSource', (themeSource) => {
        electron_1.nativeTheme.themeSource = themeSource;
    });
    socket.on('register-nativeTheme-updated-event', (id) => {
        electron_1.nativeTheme.on('updated', () => {
            electronSocket.emit('nativeTheme-updated' + id);
        });
    });
};
//# sourceMappingURL=nativeTheme.js.map