using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using Devarc;

public class ExcelWriter : IDisposable
{
    XSSFWorkbook book = null;
    ISheet sheet = null;
    int lastRow = 0;

    public ExcelWriter()
    {
        book = new XSSFWorkbook();
    }

    public void Dispose()
    {
    }

    public void Write_Header(PropTable _prop, bool _isEnum)
    {
        lastRow = 0;
        if (_isEnum)
            sheet = book.CreateSheet("!" + _prop.TableName);
        else
            sheet = book.CreateSheet(_prop.TableName);

        IRow row = null;
        // write variable name
        {
            row = sheet.CreateRow(lastRow++);
            for (int i = 0; i < _prop.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(_prop.GetVarName(i));
            }
        }

        // write variable type
        {
            row = sheet.CreateRow(lastRow++);
            for (int i = 0; i < _prop.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(_prop.GetVarType(i).ToString());
            }
        }

        // write class type
        {
            row = sheet.CreateRow(lastRow++);
            for (int i = 0; i < _prop.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(_prop.GetClassType(i).ToString());
            }
        }

        // write key type
        {
            row = sheet.CreateRow(lastRow++);
            for (int i = 0; i < _prop.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellType(CellType.String);
                cell.SetCellValue(_prop.GetKeyType(i).ToString());
            }
        }
    }

    public void Write_Contents(PropTable _prop)
    {
        if (sheet == null || _prop.KeyIndex < 0 || string.IsNullOrEmpty(_prop.GetStr(_prop.KeyIndex)))
        {
            return;
        }
        IRow row = sheet.CreateRow(lastRow++);
        for (int i = 0; i < _prop.Length; i++)
        {
            ICell cell = row.CreateCell(i);
            cell.SetCellType(CellType.String);
            cell.SetCellValue(_prop.GetStr(i));
        }
    }

    public void Write_End(string file_path)
    {
        FileStream fstream = File.Open(file_path, FileMode.Create, FileAccess.Write);
        if (book != null)
        {
            book.Write(fstream);
        }
        fstream.Close();
        fstream = null;
    }
}
