//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Devarc
{
    class Builder_SQLite : Builder_Base, IDisposable
    {
        string FileName;
        string OutDir;
        Dictionary<string, TextWriter> mWriters = new Dictionary<string, TextWriter>();

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            var enumer = mWriters.GetEnumerator();
            while (enumer.MoveNext())
            {
                enumer.Current.Value.Close();
            }
            mWriters.Clear();
        }

        public void Build_ExcelFile(string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.EXCEL;
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutDir = _outDir;

            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }

            _build(_inFilePath);
        }

        public void Build_SheetFile(string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.SHEET;
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutDir = _outDir;

            if (Directory.Exists(_outDir) == false)
            {
                if (Directory.CreateDirectory(_outDir) == null)
                {
                    Log.Info("Cannot find directory: " + _outDir);
                    return;
                }
            }
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }

            _build(_inFilePath);
        }

        void _build(string _inFilePath)
        {
            using (BaseSchemaReader reader1 = _createReader())
            {
                reader1.RegisterCallback_Table(Callback_Table);
                reader1.RegisterCallback_Data(Callback_Data);
                reader1.ReadFile(_inFilePath);
            }
            Clear();
        }

        public bool Build(string _filePath)
        {
            string fileExt = Path.GetExtension(_filePath);
            switch (fileExt.ToLower())
            {
                case ".xml":
                    dataFileType = SCHEMA_TYPE.SHEET;
                    break;
                case ".xls":
                case ".xlsx":
                    dataFileType = SCHEMA_TYPE.EXCEL;
                    break;
                default:
                    return false;
            }

            using (BaseSchemaReader reader1 = _createReader())
            {
                reader1.RegisterCallback_Table(Callback_Table);
                reader1.RegisterCallback_Data(Callback_Data);
                reader1.ReadFile(_filePath);
            }
            return true;
        }

        void Callback_Table(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;

            string filePath = Path.Combine(OutDir, GetClassName(_sheetName) + ".schema.sql");
            TextWriter tw = new StreamWriter(filePath, false);

            string tableName = GetClassName(_sheetName);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("DROP TABLE IF EXISTS {0};\r\n", tableName));

            sb.Append("CREATE TABLE ");
            sb.Append(tableName);
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (\r\n\t");
                else
                    sb.Append(",\r\n\t");

                sb.Append(string.Format("`{0}` TEXT NOT NULL", _prop.GetVarName(i)));
                if (_prop.KeyIndex == i)
                    sb.Append(" PRIMARY KEY");
            }
            sb.Append("\r\n);");
            tw.WriteLine(sb.ToString());
            tw.Close();
        }

        void Callback_Data(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;

            string tableName = GetClassName(_sheetName);
            TextWriter tw;
            if (mWriters.TryGetValue(tableName, out tw) == false)
            {
                string filePath = Path.Combine(OutDir, tableName + ".data.sql");
                tw = new StreamWriter(filePath, false);
                mWriters.Add(tableName, tw);
                tw.WriteLine("delete from {0};\r\n", tableName);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(tableName);
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (");
                else
                    sb.Append(", ");
                sb.Append(_prop.GetVarName(i));
            }
            sb.Append(") VALUES");
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (");
                else
                    sb.Append(", ");

                sb.Append(string.Format("'{0}'", FrameworkUtil.InnerString_SQLite(_prop.GetStr(i))));
            }
            sb.Append(");");
            tw.WriteLine(sb.ToString());
        }
    }
}