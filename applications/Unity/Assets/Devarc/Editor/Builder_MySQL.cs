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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Devarc
{
    class Builder_MySQL : Builder_Base, IDisposable
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

        void Callback_Table(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;

            string filePath = Path.Combine(OutDir, GetClassName(_sheetName) + ".InnoDB.ddl");
            TextWriter sw = new StreamWriter(filePath, false);

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

                switch(_prop.GetVarType(i))
                {
                    case VAR_TYPE.BOOL:
                        sb.Append(string.Format("`{0}` VARCHAR(6) NOT NULL", _prop.GetVarName(i)));
                        break;
                    case VAR_TYPE.INT16:
                        sb.Append(string.Format("`{0}` SMALLINT(6) NOT NULL", _prop.GetVarName(i)));
                        break;
                    case VAR_TYPE.INT32:
                        sb.Append(string.Format("`{0}` INT(11) NOT NULL", _prop.GetVarName(i)));
                        break;
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.UINT32:
                        sb.Append(string.Format("`{0}` BIGINT(20) NOT NULL", _prop.GetVarName(i)));
                        break;
                    case VAR_TYPE.FLOAT:
                        sb.Append(string.Format("`{0}` FLOAT NOT NULL", _prop.GetVarName(i)));
                        break;
                    case VAR_TYPE.CLASS:
                    default:
                        int length = _prop.GetCustomLength(i);
                        if (length <= 0)
                            sb.Append(string.Format("`{0}` VARCHAR(256) NOT NULL", _prop.GetVarName(i)));
                        else
                            sb.Append(string.Format("`{0}` VARCHAR({1}) NOT NULL", _prop.GetVarName(i), length));
                        break;
                }

                if (_prop.KeyIndex == i)
                    sb.Append(" PRIMARY KEY");
            }
            sb.Append("\r\n ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
            sw.WriteLine(sb.ToString());
            sw.Close();
        }

        void Callback_Data(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;

            string tableName = GetClassName(_sheetName);
            TextWriter tw;
            if (mWriters.TryGetValue(tableName, out tw) == false)
            {
                string filePath = Path.Combine(OutDir, tableName + ".InnoDB.sql");
                tw = new StreamWriter(filePath, false);
                mWriters.Add(tableName, tw);
                tw.WriteLine("truncate {0};\r\n", tableName);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("insert into {0}", tableName));
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (");
                else
                    sb.Append(" ,");
                sb.Append(string.Format("`{0}`", _prop.GetVarName(i)));
            }
            sb.Append(")\r\n VALUES");
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (");
                else
                    sb.Append(" ,");

                switch (_prop.GetVarType(i))
                {
                    case VAR_TYPE.INT16:
                    case VAR_TYPE.INT32:
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.UINT32:
                    case VAR_TYPE.FLOAT:
                        string value = _prop.GetStr(i);
                        if (string.IsNullOrEmpty(value))
                            sb.Append(string.Format("{0}", 0));
                        else
                            sb.Append(string.Format("{0}", value));
                        break;
                    case VAR_TYPE.BOOL:
                    case VAR_TYPE.CLASS:
                    default:
                        sb.Append(string.Format("'{0}'", FrameworkUtil.InnerString(_prop.GetStr(i))));
                        break;
                }
            }
            sb.Append(");");
            tw.WriteLine(sb.ToString());
        }
    }
}
