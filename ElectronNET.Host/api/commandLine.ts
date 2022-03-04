import { HubConnection  } from "@microsoft/signalr";

export = (socket: HubConnection, app: Electron.App) => {

    socket.on('appCommandLineAppendSwitch', (the_switch: string, value: string) => {
        app.commandLine.appendSwitch(the_switch, value);
    });

    socket.on('appCommandLineAppendArgument', (value: string) => {
        app.commandLine.appendArgument(value);
    });

    socket.on('appCommandLineHasSwitch', (guid, value: string) => {
        const hasSwitch = app.commandLine.hasSwitch(value);
        socket.invoke('SendClientResponseBool', guid, hasSwitch);
    });

    socket.on('appCommandLineGetSwitchValue', (guid, the_switch: string) => {
        const value = app.commandLine.getSwitchValue(the_switch);
        socket.invoke('SendClientResponseString', guid, value);
    });
};
