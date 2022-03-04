"use strict";
const electron_1 = require("electron");
module.exports = (socket) => {
    socket.on('nativeTheme-shouldUseDarkColors', (guid) => {
        const shouldUseDarkColors = electron_1.nativeTheme.shouldUseDarkColors;
        socket.invoke('SendClientResponseBool', guid, shouldUseDarkColors);
    });
    socket.on('nativeTheme-shouldUseHighContrastColors', (guid) => {
        const shouldUseHighContrastColors = electron_1.nativeTheme.shouldUseHighContrastColors;
        socket.invoke('SendClientResponseBool', guid, shouldUseHighContrastColors);
    });
    socket.on('nativeTheme-shouldUseInvertedColorScheme', (guid) => {
        const shouldUseInvertedColorScheme = electron_1.nativeTheme.shouldUseInvertedColorScheme;
        socket.invoke('SendClientResponseBool', guid, shouldUseInvertedColorScheme);
    });
    socket.on('nativeTheme-themeSource-get', (guid) => {
        const themeSource = electron_1.nativeTheme.themeSource;
        socket.invoke('SendClientResponseString', guid, themeSource);
    });
    socket.on('nativeTheme-themeSource', (themeSource) => {
        electron_1.nativeTheme.themeSource = themeSource;
    });
    socket.on('register-nativeTheme-updated-event', (id) => {
        electron_1.nativeTheme.on('updated', () => {
            socket.invoke('NativeThemeOnChanged', id);
        });
    });
};
//# sourceMappingURL=nativeTheme.js.map