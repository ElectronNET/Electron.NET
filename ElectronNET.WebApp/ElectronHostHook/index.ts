// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";
import { ExcelCreator } from "./excelCreator";

export class HookService extends Connector {
    constructor(socket: SignalR.Hub.Proxy, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        // execute your own JavaScript Host logic here
        this.on("create-excel-file", async (path, done) => {
            const excelCreator: ExcelCreator = new ExcelCreator();
            const result: string = await excelCreator.create(path);

            done(result);
        });
    }
}

