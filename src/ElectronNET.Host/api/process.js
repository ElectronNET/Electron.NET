"use strict";
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('process-execPath', () => {
        const value = process.execPath;
        electronSocket.emit('process-execPathCompleted', value);
    });
    socket.on('process-argv', () => {
        const value = process.argv;
        electronSocket.emit('process-argvCompleted', value);
    });
    socket.on('process-type', () => {
        const value = process.type;
        electronSocket.emit('process-typeCompleted', value);
    });
};
//# sourceMappingURL=process.js.map