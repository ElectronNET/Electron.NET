import { App } from "electron";
import { Socket } from "socket.io";

let electronSocket: Socket;

export default function connectApi(socket: Socket, app: App) {
  electronSocket = socket;

  socket.on(
    "appCommandLineAppendSwitch",
    (the_switch: string, value: string) => {
      app.commandLine.appendSwitch(the_switch, value);
    }
  );

  socket.on("appCommandLineAppendArgument", (value: string) => {
    app.commandLine.appendArgument(value);
  });

  socket.on("appCommandLineHasSwitch", (value: string) => {
    const hasSwitch = app.commandLine.hasSwitch(value);
    electronSocket.emit("appCommandLineHasSwitchCompleted", hasSwitch);
  });

  socket.on("appCommandLineGetSwitchValue", (the_switch: string) => {
    const value = app.commandLine.getSwitchValue(the_switch);
    electronSocket.emit("appCommandLineGetSwitchValueCompleted", value);
  });
}
