import { Socket } from "socket.io";

export class Connector {
    constructor(private socket: Socket,
        public app: Electron.App) { }

    on(key: string, javaScriptCode: (...args) => void): void {
        this.socket.on(key, (...args: any[]) => {
            const id: string = args.pop();

            try {
                javaScriptCode(...args, (data) => {
                    if (data) {
                        this.socket.emit(`${key}Complete${id}`, data);
                    }
                });
            } catch (error) {
                this.socket.emit(`${key}Error${id}`, `Host Hook Exception`, error);
            }
        });
    }
}
