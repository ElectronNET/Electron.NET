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
const Excel = require("exceljs");
class ExcelCreator {
    create(path) {
        return __awaiter(this, void 0, void 0, function* () {
            const workbook = new Excel.Workbook();
            const worksheet = workbook.addWorksheet("My Sheet");
            // worksheet.columns = [
            //     { header: "Id", key: "id", width: 10 },
            //     { header: "Name", key: "name", width: 32 },
            //     { header: "D.O.B.", key: "DOB", width: 10, outlineLevel: 1 }
            // ];
            worksheet.addRow({ id: 1, name: "John Doe", dob: new Date(1970, 1, 1) });
            worksheet.addRow({ id: 2, name: "Jane Doe", dob: new Date(1965, 1, 7) });
            yield workbook.xlsx.writeFile(path + "sample.xlsx");
            return "finish";
        });
    }
}
exports.ExcelCreator = ExcelCreator;
//# sourceMappingURL=excelCreator.js.map