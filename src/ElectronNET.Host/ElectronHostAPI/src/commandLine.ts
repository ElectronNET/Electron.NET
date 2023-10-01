import { Socket } from 'node:net';

let electronSocket;

export function commandLineApi(socket: Socket, app: Electron.App) {
    electronSocket = socket;

    socket.on('appCommandLineAppendSwitch', (the_switch: string, value: string) => {
        app.commandLine.appendSwitch(the_switch, value);
    });

    socket.on('appCommandLineAppendArgument', (value: string) => {
        app.commandLine.appendArgument(value);
    });

    socket.on('appCommandLineHasSwitch', (value: string) => {
        const hasSwitch = app.commandLine.hasSwitch(value);
        electronSocket.emit('appCommandLineHasSwitchCompleted', hasSwitch);
    });

    socket.on('appCommandLineGetSwitchValue', (the_switch: string) => {
        const value = app.commandLine.getSwitchValue(the_switch);
        electronSocket.emit('appCommandLineGetSwitchValueCompleted', value);
    });
}
