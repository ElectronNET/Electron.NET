export class Connector {
    constructor(private socket: SocketIO.Socket, public app: Electron.App) { }

    on(key: string, javaScriptCode: Function): void {
        this.socket.on(key, (...args: any[]) => {
            const id: string = args.pop();

            javaScriptCode(args, (data) => {
                this.socket.emit(`${key}Complete${id}`, data);
            });
        });
    }
}
