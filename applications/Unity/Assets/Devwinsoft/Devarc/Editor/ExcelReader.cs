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
    public delegate void CallbackExcelReader(string _sheetName, PropTable _property);

    public class ExcelReader : IDisposable
    {
        CallbackXmlReader callback_every_header = null;
        CallbackXmlReader callback_every_data = null;
        public Dictionary<string, CallbackXmlReader> callback_header_list = new Dictionary<string, CallbackXmlReader>();
        public Dictionary<string, CallbackXmlReader> callback_data_list = new Dictionary<string, CallbackXmlReader>();

        public void Dispose()
        {
            callback_every_header = null;
            callback_every_data = null;
            callback_header_list.Clear();
            callback_data_list.Clear();
        }

        public void RegisterCallback_EveryTable(CallbackXmlReader func)
        {
            callback_every_header = func;
        }

        public void RegisterCallback_EveryLine(CallbackXmlReader func)
        {
            callback_every_data = func;
        }

        public void RegisterCallback_Line(string sheet_name, CallbackXmlReader func)
        {
            if (callback_data_list.ContainsKey(sheet_name) == false)
            {
                callback_data_list.Add(sheet_name, func);
            }
        }

        public bool ReadFile(string _filePath)
        {
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

            PropTable rowData = new PropTable();
            for (int s = 0; s < book.NumberOfSheets; s++)
            {
                ISheet sheet = book.GetSheetAt(s);
                for (int r = 0; r < sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    for (int c = 0; c < sheet.LastRowNum; c++)
                    {
                        ICell cell = row.GetCell(c);
                        if (cell == null)
                        {
                            continue;
                        }
                        string value = "";
                        switch(cell.CellType)
                        {
                            case CellType.Boolean:
                                value = cell.BooleanCellValue.ToString();
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue.ToString();
                                break;
                            case CellType.String:
                            case CellType.Formula:
                            case CellType.Unknown:
                            default:
                                value = cell.StringCellValue;
                                break;
                        }

                        switch(r)
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
                    if (r < 3)
                    {
                        // skip
                    }
                    else if (r == 3)
                    {
                        if (callback_every_header != null)
                        {
                            callback_every_header(sheet.SheetName, rowData);
                        }
                    }
                    else
                    {
                        if (callback_every_data != null)
                        {
                            callback_every_data(sheet.SheetName, rowData);
                        }
                        OnEndOfLine(sheet.SheetName, rowData);
                        rowData.ClearData();
                    }
                }
            }
            return book.NumberOfSheets > 0;
        }

        bool OnEndOfLine(string sheet_name, PropTable tb)
        {
            if (callback_every_data != null)
            {
                callback_every_data(sheet_name, tb);
            }

            CallbackXmlReader func = null;
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
