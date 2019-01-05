"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class Connector {
    constructor(socket, app) {
        this.socket = socket;
        this.app = app;
    }
    on(key, javaScriptCode) {
        this.socket.on(key, (...args) => {
            const id = args.pop();
            javaScriptCode(args, (data) => {
                this.socket.emit(`${key}Complete${id}`, data);
            });
        });
    }
}
exports.Connector = Connector;
//# sourceMappingURL=connector.js.map