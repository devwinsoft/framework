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
    class Builder_SQL : Builder_Base, IDisposable
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

            string filePath = Path.Combine(OutDir, GetClassName(_sheetName) + ".schema.sql");
            TextWriter tw = new StreamWriter(filePath, false);

            string tableName = GetClassName(_sheetName);
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("DROP TABLE IF EXISTS {0};\r\n", tableName));

            sb.Append("CREATE TABLE ");
            sb.Append(tableName);
            sb.Append(" (\r\n");
            sb.Append("\t`key` varchar(256) NOT NULL PRIMARY KEY,\r\n");
            sb.Append(string.Format("\t`data` varchar({0}) NOT NULL\r\n", _prop.GetDBSize()));
            sb.Append(" ) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");
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
                tw.WriteLine("truncate {0};\r\n", tableName);
            }

            tw.WriteLine("insert into {0} (`key`, `data`) VALUES ('{1}', '{2}');", tableName, _prop.GetStr(_prop.KeyIndex), FrameworkUtil.InnerString(_prop.ToJson()));
        }
    }
}
