import { Connector } from "./connector";
import { Socket } from "socket.io";

export class HookService extends Connector {
    constructor(socket: Socket, public app: any) {
        super(socket, app);
    }

    onHostReady(): void {
        // execute your own JavaScript Host logic here
        this.on("ping", (msg, done) => {
            console.log("Received ping from C#:", msg);
            done("pong: " + msg);
        });
    }
}
