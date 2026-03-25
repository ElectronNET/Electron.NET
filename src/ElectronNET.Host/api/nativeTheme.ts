import type { Socket } from "net";
import { nativeTheme } from "electron";

let electronSocket: Socket;

export = (socket: Socket) => {
  electronSocket = socket;

  socket.on("nativeTheme-shouldUseDarkColors", () => {
    const shouldUseDarkColors = nativeTheme.shouldUseDarkColors;

    electronSocket.emit(
      "nativeTheme-shouldUseDarkColors-completed",
      shouldUseDarkColors,
    );
  });

  socket.on("nativeTheme-shouldUseHighContrastColors", () => {
    const shouldUseHighContrastColors = nativeTheme.shouldUseHighContrastColors;

    electronSocket.emit(
      "nativeTheme-shouldUseHighContrastColors-completed",
      shouldUseHighContrastColors,
    );
  });

  socket.on("nativeTheme-shouldUseInvertedColorScheme", () => {
    const shouldUseInvertedColorScheme =
      nativeTheme.shouldUseInvertedColorScheme;

    electronSocket.emit(
      "nativeTheme-shouldUseInvertedColorScheme-completed",
      shouldUseInvertedColorScheme,
    );
  });

  socket.on("nativeTheme-getThemeSource", () => {
    const themeSource = nativeTheme.themeSource;

    electronSocket.emit("nativeTheme-getThemeSource-completed", themeSource);
  });

  socket.on("nativeTheme-themeSource", (themeSource) => {
    nativeTheme.themeSource = themeSource;
  });

  socket.on("register-nativeTheme-updated", (id) => {
    nativeTheme.on("updated", () => {
      electronSocket.emit("nativeTheme-updated" + id);
    });
  });
};
