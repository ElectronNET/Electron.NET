"use strict";
module.exports = (socket, app) => {
    socket.on('appCommandLineAppendSwitch', (the_switch, value) => {
        app.commandLine.appendSwitch(the_switch, value);
    });
    socket.on('appCommandLineAppendArgument', (value) => {
        app.commandLine.appendArgument(value);
    });
    socket.on('appCommandLineHasSwitch', (guid, value) => {
        const hasSwitch = app.commandLine.hasSwitch(value);
        socket.invoke('SendClientResponseBool', guid, hasSwitch);
    });
    socket.on('appCommandLineGetSwitchValue', (guid, the_switch) => {
        const value = app.commandLine.getSwitchValue(the_switch);
        socket.invoke('SendClientResponseString', guid, value);
    });
};
//# sourceMappingURL=commandLine.js.map