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
        Dictionary<string, object> mDataList = new Dictionary<string, object>();

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
                object cValue;
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            cValue = SQLiteMethods.sqlite3_column_int(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            cValue = SQLiteMethods.sqlite3_column_double(mStmt, i);
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
                object cValue;
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            cValue = SQLiteMethods.sqlite3_column_int(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            cValue = SQLiteMethods.sqlite3_column_double(mStmt, i);
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

        T get<T>(string _name)
        {
            try
            {
                object value;
                if (mDataList.TryGetValue(_name, out value))
                    return (T)value;
                return default(T);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return default(T);
            }
        }

        public bool GetBoolean(string _name)
        {
            string value = get<string>(_name);
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

        public short GetInt16(string _name)
        {
            short value = get<short>(_name);
            return value;
        }

        public int GetInt32(string _name)
        {
            int value = get<int>(_name);
            return value;
        }

        public long GetInt64(string _name)
        {
            long value = get<long>(_name);
            return value;
        }

        public uint GetUInt32(string _name)
        {
            uint value = get<uint>(_name);
            return value;
        }

        public float GetFloat(string _name)
        {
            float value = get<float>(_name);
            return value;
        }

        public string GetString(string _name)
        {
            string value = get<string>(_name);
            return value;
        }
    }

}
