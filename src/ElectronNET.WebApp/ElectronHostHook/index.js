"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HookService = void 0;
const connector_1 = require("./connector");
const excelCreator_1 = require("./excelCreator");
class HookService extends connector_1.Connector {
    constructor(socket, app) {
        super(socket, app);
        this.app = app;
    }
    onHostReady() {
        // execute your own JavaScript Host logic here
        this.on("create-excel-file", async (path, done) => {
            const excelCreator = new excelCreator_1.ExcelCreator();
            const result = await excelCreator.create(path);
            done(result);
        });
    }
}
exports.HookService = HookService;
//# sourceMappingURL=index.js.map