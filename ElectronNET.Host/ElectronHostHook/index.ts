// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";

export class HookService extends Connector {
    constructor(socket: SocketIO.Socket, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        // execute your own JavaScript Host logic here
    }
}

