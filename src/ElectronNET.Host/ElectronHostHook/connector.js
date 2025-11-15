"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Connector = void 0;
const socket_io_1 = require("socket.io");
class Connector {
    socket;
    app;
    constructor(socket, 
    // @ts-ignore
    app) {
        this.socket = socket;
        this.app = app;
    }
    on(key, javaScriptCode) {
        this.socket.on(key, (...args) => {
            const id = args.pop();
            try {
                javaScriptCode(...args, (data) => {
                    if (data) {
                        this.socket.emit(`${key}Complete${id}`, data);
                    }
                });
            }
            catch (error) {
                this.socket.emit(`${key}Error${id}`, `Host Hook Exception`, error);
            }
        });
    }
}
exports.Connector = Connector;
//# sourceMappingURL=connector.js.map