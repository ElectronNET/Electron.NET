import * as Excel from "exceljs";
import { Workbook, Worksheet } from "exceljs";

export class ExcelCreator {
    async create(path: string): Promise<string> {
        const workbook: Workbook = new Excel.Workbook();
        const worksheet: Worksheet = workbook.addWorksheet("My Sheet");
        // worksheet.columns = [
        //     { header: "Id", key: "id", width: 10 },
        //     { header: "Name", key: "name", width: 32 },
        //     { header: "D.O.B.", key: "DOB", width: 10, outlineLevel: 1 }
        // ];
        worksheet.addRow({ id: 1, name: "John Doe", dob: new Date(1970, 1, 1) });
        worksheet.addRow({ id: 2, name: "Jane Doe", dob: new Date(1965, 1, 7) });

        await workbook.xlsx.writeFile(path + "sample.xlsx");

        return "finish";
    }
}