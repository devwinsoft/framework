using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;

namespace Devarc
{
    public class ExcelReader : BaseSchemaReader
    {
        public override bool ReadFile(string _filePath)
        {
            if (File.Exists(_filePath) == false)
            {
                Log.Debug("Cannot read file: {0}", _filePath);
                return false;
            }
            IWorkbook book = null;
            using (FileStream stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (Path.GetExtension(_filePath) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }
            }
            for (int s = 0; s < book.NumberOfSheets; s++)
            {
                PropTable rowData = new PropTable();
                ISheet sheet = book.GetSheetAt(s);
                bool funcCalled = false;
                rowData.TableName = GetClassName(sheet.SheetName);
                rowData.IsEnum = sheet.SheetName.StartsWith("!");
                for (int r = 0; r <= sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    if (row != null)
                    {
                        for (int c = 0; c < row.LastCellNum; c++)
                        {
                            ICell cell = row.GetCell(c);
                            if (cell == null)
                            {
                                continue;
                            }
                            cell.SetCellType(CellType.String);
                            string value = cell.StringCellValue;
                            switch (r)
                            {
                                case 0:
                                    rowData.Register(c, value);
                                    break;
                                case 1:
                                    rowData.Set_VarType(c, value);
                                    break;
                                case 2:
                                    rowData.Set_ClassType(c, PropTable.ToClassType(value));
                                    break;
                                case 3:
                                    rowData.Set_KeyType(c, PropTable.ToKeyType(value));
                                    break;
                                default:
                                    rowData.Set_Data(c, value);
                                    break;
                            }
                        }
                    }
                    if (r < 3)
                    {
                        // skip
                    }
                    else if (r == 3)
                    {
                        if (funcCalled == false)
                        {
                            funcCalled = true;
                            callback_header?.Invoke(sheet.SheetName, rowData);
                        }
                    }
                    else
                    {
                        invoke_callback_data(sheet.SheetName, rowData);
                        rowData.ClearData();
                    }
                }
                if (funcCalled == false)
                {
                    funcCalled = true;
                    callback_header?.Invoke(sheet.SheetName, rowData);
                }
                callback_complete?.Invoke(sheet.SheetName, rowData);
            }
            return book.NumberOfSheets > 0;
        }

        public override bool ReadData(string _data)
        {
            Log.Error("[ExcelReader] Cannot read string data.");
            return false;
        }
    }

}
