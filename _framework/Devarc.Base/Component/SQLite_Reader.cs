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
    public class SQLite_Reader : IDisposable
    {
        IntPtr mStmt = IntPtr.Zero;
        int mReadCount = 0;
        int mColumnCount;
        Type[] mColumnTypes;
        string[] mColumnNames;
        object[] mColumnValues;

        public SQLite_Reader(IntPtr _stmt)
        {
            mStmt = _stmt;
            mColumnCount = 0;
        }

        public void Dispose()
        {
            if (mStmt != IntPtr.Zero)
            {
                SQLiteMethods.sqlite3_finalize(mStmt);
                mStmt = IntPtr.Zero;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public short GetInt16(int i)
        {
            if (mColumnCount <= i)
            {
                return 0;
            }
            short value;
            short.TryParse(mColumnValues[i].ToString(), out value);
            return value;
        }

        public int GetInt32(int i)
        {
            if (mColumnCount <= i)
            {
                return 0;
            }
            int value;
            int.TryParse(mColumnValues[i].ToString(), out value);
            return value;
        }

        public long GetInt64(int i)
        {
            if (mColumnCount <= i)
            {
                return 0;
            }
            long value;
            long.TryParse(mColumnValues[i].ToString(), out value);
            return value;
        }

        public uint GetDecimal(int i)
        {
            if (mColumnCount <= i)
            {
                return 0;
            }
            uint value;
            uint.TryParse(mColumnValues[i].ToString(), out value);
            return value;
        }

        public float GetFloat(int i)
        {
            if (mColumnCount <= i)
            {
                return 0f;
            }
            float value;
            float.TryParse(mColumnValues[i].ToString(), out value);
            return value;
        }

        public string GetString(int i)
        {
            if (mColumnCount <= i)
            {
                return string.Empty;
            }
            return mColumnValues[i].ToString();
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
            this.mColumnCount = SQLiteMethods.sqlite3_column_count(mStmt);
            this.mColumnTypes = new Type[mColumnCount];
            this.mColumnNames = new string[mColumnCount];
            this.mColumnValues = new object[mColumnCount];

            for (int i = 0; i < mColumnCount; i++)
            {
                mColumnNames[i] = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_name(mStmt, i));
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            mColumnTypes[i] = typeof(Int32);
                            mColumnValues[i] = SQLiteMethods.sqlite3_column_int(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            mColumnTypes[i] = typeof(Single);
                            mColumnValues[i] = SQLiteMethods.sqlite3_column_double(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_TEXT:
                        {
                            mColumnTypes[i] = typeof(string);
                            mColumnValues[i] = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_text(mStmt, i));
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_BLOB:
                        {
                            mColumnTypes[i] = typeof(string);
                            mColumnValues[i] = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_blob(mStmt, i));
                            break;
                        }
                    default:
                        {
                            mColumnTypes[i] = typeof(string);
                            mColumnValues[i] = "";
                            break;
                        }
                }
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
                int columnType = SQLiteMethods.sqlite3_column_type(mStmt, i);
                switch (columnType)
                {
                    case (int)FundamentalDatatypes.SQLITE_INTEGER:
                        {
                            this.mColumnValues[i] = SQLiteMethods.sqlite3_column_int(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_FLOAT:
                        {
                            this.mColumnValues[i] = SQLiteMethods.sqlite3_column_double(mStmt, i);
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_TEXT:
                        {
                            this.mColumnValues[i] = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_text(mStmt, i));
                            break;
                        }
                    case (int)FundamentalDatatypes.SQLITE_BLOB:
                        {
                            this.mColumnValues[i] = SQLiteMethods.PtrToString(SQLiteMethods.sqlite3_column_blob(mStmt, i));
                            break;
                        }
                    default:
                        {
                            this.mColumnValues[i] = "";
                            break;
                        }
                }
            }
            return resultType;
        }
    }

}
