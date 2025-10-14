"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ExcelCreator = void 0;
const exceljs_1 = require("exceljs");
class ExcelCreator {
    async create(path) {
        const workbook = new exceljs_1.Workbook();
        const worksheet = workbook.addWorksheet("My Sheet");
        worksheet.columns = [
            { header: "Id", key: "id", width: 10 },
            { header: "Name", key: "name", width: 32 },
            { header: "Birthday", key: "birthday", width: 10, outlineLevel: 1 }
        ];
        worksheet.addRow({ id: 1, name: "John Doe", birthday: new Date(1970, 1, 1) });
        worksheet.addRow({ id: 2, name: "Jane Doe", birthday: new Date(1965, 1, 7) });
        await workbook.xlsx.writeFile(path + "\\sample.xlsx");
        return "Excel file created YAY YAY!";
    }
}
exports.ExcelCreator = ExcelCreator;
//# sourceMappingURL=excelCreator.js.map