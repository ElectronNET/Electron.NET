import { Connector } from "./connector";
import { Socket } from "socket.io";
import { ExcelCreator } from './excelCreator';

export class HookService extends Connector {
    constructor(socket: Socket, public app: any) {
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

