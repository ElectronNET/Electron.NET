"use strict";
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on("process-execPath", () => {
        const value = process.execPath;
        electronSocket.emit("process-execPath-completed", value);
    });
    socket.on("process-argv", () => {
        const value = process.argv;
        electronSocket.emit("process-argv-completed", value);
    });
    socket.on("process-type", () => {
        const value = process.type;
        electronSocket.emit("process-type-completed", value);
    });
    socket.on("process-versions", () => {
        const value = process.versions;
        electronSocket.emit("process-versions-completed", value);
    });
    socket.on("process-defaultApp", () => {
        if (process.defaultApp === undefined) {
            electronSocket.emit("process-defaultApp-completed", false);
            return;
        }
        electronSocket.emit("process-defaultApp-completed", process.defaultApp);
    });
    socket.on("process-isMainFrame", () => {
        if (process.isMainFrame === undefined) {
            electronSocket.emit("process-isMainFrame-completed", false);
            return;
        }
        electronSocket.emit("process-isMainFrame-completed", process.isMainFrame);
    });
    socket.on("process-resourcesPath", () => {
        const value = process.resourcesPath;
        electronSocket.emit("process-resourcesPath-completed", value);
    });
    socket.on("process-upTime", () => {
        let value = process.uptime();
        if (value === undefined) {
            value = -1;
        }
        electronSocket.emit("process-upTime-completed", value);
    });
    socket.on("process-pid", () => {
        if (process.pid === undefined) {
            electronSocket.emit("process-pid-completed", -1);
            return;
        }
        electronSocket.emit("process-pid-completed", process.pid);
    });
    socket.on("process-arch", () => {
        const value = process.arch;
        electronSocket.emit("process-arch-completed", value);
    });
    socket.on("process-platform", () => {
        const value = process.platform;
        electronSocket.emit("process-platform-completed", value);
    });
};
//# sourceMappingURL=process.js.map