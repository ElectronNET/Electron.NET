import { App } from "electron";
import { Socket } from "socket.io";
import { Connector } from "./connector";

export class HookService extends Connector {
  constructor(socket: Socket, public app: App) {
    super(socket, app);
  }

  onHostReady(): void {
    // execute your own JavaScript Host logic here
  }
}
