"use strict";
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('process-execPath', () => {
        const value = process.execPath;
        electronSocket.emit('process-execPathCompleted', value);
    });
};
//# sourceMappingURL=process.js.map