using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;

namespace Devarc
{
    public class ExcelReader : BaseDataReader
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
                                    rowData.initVarName(c, value);
                                    break;
                                case 1:
                                    rowData.initVarType(c, value);
                                    break;
                                case 2:
                                    rowData.initClassType(c, PropTable.ToClassType(value));
                                    break;
                                case 3:
                                    rowData.initKeyType(c, PropTable.ToKeyType(value));
                                    break;
                                default:
                                    rowData.SetData(c, value);
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
                        if (funcCalled == false && callback_every_header != null)
                        {
                            funcCalled = true;
                            callback_every_header(sheet.SheetName, rowData);
                        }
                    }
                    else
                    {
                        OnEndOfLine(sheet.SheetName, rowData);
                        rowData.ClearData();
                    }
                }
                if (funcCalled == false && callback_every_header != null)
                {
                    funcCalled = true;
                    callback_every_header(sheet.SheetName, rowData);
                }
            }
            return book.NumberOfSheets > 0;
        }

        public override bool ReadData(string _data)
        {
            Log.Error("[ExcelReader] Cannot read string data.");
            return false;
        }

        bool OnEndOfLine(string sheet_name, PropTable tb)
        {
            if (callback_every_data != null)
            {
                callback_every_data(sheet_name, tb);
            }

            CallbackDataReader func = null;
            string class_name = sheet_name.Replace("!", "");
            if (callback_data_list.TryGetValue(class_name, out func))
            {
                if (func != null)
                {
                    func(sheet_name, tb);
                    return true;
                }
            }
            return false;
        }
    }

}
