"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const connector_1 = require("./connector");
class HookService extends connector_1.Connector {
    constructor(socket, app) {
        super(socket, app);
        this.app = app;
    }
    onHostReady() {
        // execute your own JavaScript Host logic here
    }
}
exports.HookService = HookService;
//# sourceMappingURL=index.js.map