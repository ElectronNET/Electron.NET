"use strict";
let electronSocket;
module.exports = (socket, app) => {
    electronSocket = socket;
    socket.on('appCommandLineAppendSwitch', (the_switch, value) => {
        app.commandLine.appendSwitch(the_switch, value);
    });
    socket.on('appCommandLineAppendArgument', (value) => {
        app.commandLine.appendArgument(value);
    });
    socket.on('appCommandLineHasSwitch', (value) => {
        const hasSwitch = app.commandLine.hasSwitch(value);
        electronSocket.emit('appCommandLineHasSwitchCompleted', hasSwitch);
    });
    socket.on('appCommandLineGetSwitchValue', (the_switch) => {
        const value = app.commandLine.getSwitchValue(the_switch);
        electronSocket.emit('appCommandLineGetSwitchValueCompleted', value);
    });
};
//# sourceMappingURL=commandLine.js.map