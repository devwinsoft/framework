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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SQLiteWrapper;
using Devarc;

namespace Devarc
{
    public class SQLite_Reader : IBaseReader, IDisposable
    {
        IntPtr mStmt = IntPtr.Zero;
        int mReadCount = 0;
        int mColumnCount;
        Dictionary<string, string> mDataList = new Dictionary<string, string>();

        public SQLite_Reader(IntPtr _stmt)
        {
            mStmt = _stmt;
            mColumnCount = 0;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (mStmt != IntPtr.Zero)
            {
                SQLiteMethods.sqlite3_finalize(mStmt);
                mStmt = IntPtr.Zero;
            }
        }

        public bool Read()
        {
            if (mStmt == IntPtr.Zero)
            {
                return false;
            }

            ResultCode result;
            if (mReadCount == 0)
            {
                result = readFirst();
            }
            else
            {
                result = readNext();
            }

            if (result == ResultCode.SQLITE_ROW)
            {
                mReadCount++;
                return true;
            }
            else
            {
                SQLiteMethods.sqlite3_finalize(mStmt);
                mStmt = IntPtr.Zero;
                return false;
            }
        }

        private ResultCode readFirst()
        {
            ResultCode resultType = (ResultCode)SQLiteMethods.sqlite3_step(mStmt);
            if (resultType != ResultCode.SQLITE_ROW)
            {
                return resultType;
            }
            mColumnCount = SQLiteMethods.sqlite3_column_count(mStmt);
            for (int i = 0; i < mColumnCount; i++)
            {
                string cName = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_name(mStmt, i));
                string cValue;
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            cValue = SQLiteMethods.sqlite3_column_int(mStmt, i).ToString();
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            cValue = SQLiteMethods.sqlite3_column_double(mStmt, i).ToString();
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_TEXT:
                        {
                            cValue = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_text(mStmt, i));
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_BLOB:
                        {
                            cValue = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_blob(mStmt, i));
                            break;
                        }
                    default:
                        {
                            cValue = "";
                            break;
                        }
                }
                mDataList.Add(cName, cValue);
            }
            return resultType;
        }


        ResultCode readNext()
        {
            ResultCode resultType = (ResultCode)SQLiteMethods.sqlite3_step(mStmt);
            if (resultType != ResultCode.SQLITE_ROW)
            {
                return resultType;
            }

            int columnCount = SQLiteMethods.sqlite3_column_count(mStmt);
            for (int i = 0; i < columnCount; i++)
            {
                string cName = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_name(mStmt, i));
                string cValue;
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            cValue = SQLiteMethods.sqlite3_column_int(mStmt, i).ToString();
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            cValue = SQLiteMethods.sqlite3_column_double(mStmt, i).ToString();
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_TEXT:
                        {
                            cValue = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_text(mStmt, i));
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_BLOB:
                        {
                            cValue = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_blob(mStmt, i));
                            break;
                        }
                    default:
                        {
                            cValue = "";
                            break;
                        }
                }
                mDataList.Add(cName, cValue);
            }
            return resultType;
        }

        string get(string _name)
        {
            string value;
            if (mDataList.TryGetValue(_name, out value))
                return value;
            return string.Empty;
        }

        public bool GetBoolean(string _name)
        {
            try
            {
                string value = get(_name);
                switch (value.ToLower())
                {
                    case "0":
                    case "false":
                    case "f":
                    case "no":
                    case "n":
                        return false;
                    //case "true":
                    //case "t":
                    //case "yes":
                    //case "y":
                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public short GetInt16(string _name)
        {
            try
            {
                string value = get(_name);
                return short.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public int GetInt32(string _name)
        {
            try
            {
                string value = get(_name);
                return int.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public long GetInt64(string _name)
        {
            try
            {
                string value = get(_name);
                return long.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public uint GetUInt32(string _name)
        {
            try
            {
                string value = get(_name);
                return uint.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public float GetFloat(string _name)
        {
            try
            {
                string value = get(_name);
                return float.Parse(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public string GetString(string _name)
        {
            return get(_name);
        }
    }

}
