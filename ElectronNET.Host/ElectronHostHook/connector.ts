export class Connector {
    constructor(private socket: SocketIO.Socket, public app: Electron.App) { }

    on(key: string, javaScriptCode: Function): void {
        this.socket.on(key, (...args: any[]) => {
            const id: string = args.pop();

            try {
                javaScriptCode(...args, (data) => {
                    if (isNaN(data)) {
                        throw new Error('Result is NaN');
                    } else {
                        this.socket.emit(`${key}Complete${id}`, data);
                    }
                });
            } catch (error) {
                this.socket.emit(`${key}Error${id}`, 'Host Hook Exception', error);
            }
        });
    }
}
