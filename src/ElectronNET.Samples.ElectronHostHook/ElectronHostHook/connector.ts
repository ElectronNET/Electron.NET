import { Socket } from "socket.io";

export class Connector {
    constructor(private socket: Socket, public app: any) {
    }

    on(key: string, javaScriptCode: Function): void {
        this.socket.on(key, (...args: any[]) => {
            const id: string = args.pop();
            const done = (result: any) => {
                this.socket.emit(id, result);
            };

            args = [...args, done];
            javaScriptCode(...args);
        });
    }
}
