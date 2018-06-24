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
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
#if UNITY_5 || UNITY_2017
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif

namespace Devarc
{
    public class Builder_SQLite : IDisposable
    {
        SQLite_Session mSession = new SQLite_Session();
        DATA_FILE_TYPE dataFileType = DATA_FILE_TYPE.SHEET;

        private Builder_SQLite()
        {
        }

        public Builder_SQLite(string _databasePath)
        {
            mSession.Open(_databasePath);
        }

        public void Commit()
        {
            mSession.Commit();
            mSession.Close();
        }

        public void Dispose()
        {
            Commit();
        }

        public bool Build(string _filePath)
        {
            string fileExt = Path.GetExtension(_filePath);
            switch (fileExt.ToLower())
            {
                case ".xml":
                    dataFileType = DATA_FILE_TYPE.SHEET;
                    break;
                case ".xls":
                case ".xlsx":
                    dataFileType = DATA_FILE_TYPE.EXCEL;
                    break;
                default:
                    return false;
            }

            using (BaseDataReader reader1 = _createReader())
            {
                reader1.RegisterCallback_EveryTable(Callback_Header);
                reader1.RegisterCallback_EveryLine(Callback_Data);
                reader1.ReadFile(_filePath);
            }
            return true;
        }

        BaseDataReader _createReader()
        {
            switch (dataFileType)
            {
                case DATA_FILE_TYPE.EXCEL:
                    return new ExcelReader();
                case DATA_FILE_TYPE.SHEET:
                default:
                    return new XmlSheetReader();
            }
        }

        void Callback_Header(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;
            string tableName = FrameworkUtil.GetClassName(_sheetName);
            mSession.ExecuteNonQuery(string.Format("DROP TABLE IF EXISTS {0};", tableName));

            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(tableName);
            for (int i = 0; i < _prop.Length; i++)
            {
                if (i == 0)
                    sb.Append(" (\n\t");
                else
                    sb.Append(",\n\t");
                sb.Append(string.Format("{0} TEXT NOT NULL", _prop.GetVarName(i)));
                if (_prop.KeyIndex == i)
                    sb.Append(" PRIMARY KEY");
            }
            sb.Append(");");

            mSession.ExecuteNonQuery(sb.ToString());
        }

        void Callback_Data(string _sheetName, PropTable _prop)
        {
            if (_prop.KeyIndex < 0)
                return;
            string tableName = FrameworkUtil.GetClassName(_sheetName);
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
                switch (_prop.GetVarType(i))
                {
                    case VAR_TYPE.LSTRING:
                        sb.Append("''");
                        break;
                    default:
                        string tempStr = _prop.GetStr(i).Replace("\'", "\'\'");
                        sb.Append(string.Format("'{0}'", tempStr));
                        break;
                }
            }
            sb.Append(");");
            mSession.ExecuteNonQuery(sb.ToString());
        }
    }
}