import { nativeTheme } from 'electron';

export = (socket: SignalR.Hub.Proxy) => {

    socket.on('nativeTheme-shouldUseDarkColors', (guid) => {
        const shouldUseDarkColors = nativeTheme.shouldUseDarkColors;

        socket.invoke('SendClientResponseBool', guid, shouldUseDarkColors);
    });

    socket.on('nativeTheme-shouldUseHighContrastColors', (guid) => {
        const shouldUseHighContrastColors = nativeTheme.shouldUseHighContrastColors;

        socket.invoke('SendClientResponseBool', guid, shouldUseHighContrastColors);
    });

    socket.on('nativeTheme-shouldUseInvertedColorScheme', (guid) => {
        const shouldUseInvertedColorScheme = nativeTheme.shouldUseInvertedColorScheme;

        socket.invoke('SendClientResponseBool', guid, shouldUseInvertedColorScheme);
    });

    socket.on('nativeTheme-themeSource-get', (guid) => {
        const themeSource = nativeTheme.themeSource;

        socket.invoke('SendClientResponseString', guid, themeSource);
    });

    socket.on('nativeTheme-themeSource', (themeSource) => {
        nativeTheme.themeSource = themeSource;
    });

    socket.on('register-nativeTheme-updated-event', (id) => {
        nativeTheme.on('updated', () => {
            socket.invoke('NativeThemeOnChanged', id);
        });
    });
};
