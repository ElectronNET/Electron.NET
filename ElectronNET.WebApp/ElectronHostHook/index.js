"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const connector_1 = require("./connector");
const excelCreator_1 = require("./excelCreator");
class HookService extends connector_1.Connector {
    constructor(socket, app) {
        super(socket, app);
        this.app = app;
    }
    onHostReady() {
        // execute your own JavaScript Host logic here
        this.on("create-excel", (path, done) => __awaiter(this, void 0, void 0, function* () {
            const excelCreator = new excelCreator_1.ExcelCreator();
            const result = yield excelCreator.create(path);
            done(result);
        }));
    }
}
exports.HookService = HookService;
//# sourceMappingURL=index.js.map