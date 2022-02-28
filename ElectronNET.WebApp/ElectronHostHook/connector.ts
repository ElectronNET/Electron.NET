export class Connector {
    constructor(private socket: SignalR.Hub.Proxy,
        // @ts-ignore
        public app: Electron.App) { }

    on(key: string, javaScriptCode: Function): void {
        this.socket.on(key, (...args: any[]) => {
            const id: string = args.pop();

            try {
                javaScriptCode(...args, (data) => {
                    if (data) {
                	this.socket.invoke(`${key}Complete${id}`, data);
                    }
                });
            } catch (error) {
                this.socket.invoke(`${key}Error${id}`, `Host Hook Exception`, error);
            }
        });
    }
}
