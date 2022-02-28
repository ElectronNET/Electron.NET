// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";

export class HookService extends Connector {
    constructor(socket: SignalR.Hub.Proxy, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        // execute your own JavaScript Host logic here
    }
}
