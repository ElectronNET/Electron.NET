import * as Electron from "electron";
import { Socket } from "socket.io";
import { Connector } from "./connector";

export class HookService extends Connector {
    constructor(socket: Socket, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady() {
        // execute your own JavaScript Host logic here
    }
}
