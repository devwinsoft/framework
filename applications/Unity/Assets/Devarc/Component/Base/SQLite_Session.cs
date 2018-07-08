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
    public class SQLite_Session
    {
        bool mIsOpened = false;
        IntPtr mSQLiteID = IntPtr.Zero;

        IntPtr StringToPtr(string _value)
        {
            if (string.IsNullOrEmpty(_value))
            {
                return IntPtr.Zero;
            }
            return Marshal.StringToHGlobalAnsi(_value);
        }

        void OnError(IntPtr _err)
        {
            string msg = SQLiteUnsafeNativeMethods.PtrToString(_err);
            SQLiteUnsafeNativeMethods.sqlite3_free(_err);
            Log.Error(msg);
        }

        public bool Open(string _databasePath)
        {
            int result = 0;
            try
            {
                if (mIsOpened == false)
                {
                    IntPtr filename = StringToPtr(_databasePath);
                    result = SQLiteUnsafeNativeMethods.sqlite3_open(filename, out mSQLiteID);
                    mIsOpened = result == 0;
                    return result == 0;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            int result = 0;
            try
            {
                if (mIsOpened == false)
                {
                    return false;
                }
                else
                {
                    result = SQLiteUnsafeNativeMethods.sqlite3_close(mSQLiteID);
                    mIsOpened = false;
                    mSQLiteID = IntPtr.Zero;
                    return result == 0;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Begin_Transaction()
        {
            if (mIsOpened == false)
            {
                return false;
            }
            IntPtr err = IntPtr.Zero;
            IntPtr cmd = StringToPtr("BEGIN TRANSACTION");
            try
            {
                SQLiteUnsafeNativeMethods.sqlite3_exec(mSQLiteID, cmd, null, IntPtr.Zero, out err);
                SQLiteUnsafeNativeMethods.sqlite3_free(err);
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool End_Transaction()
        {
            if (mIsOpened == false)
            {
                return false;
            }
            IntPtr err = IntPtr.Zero;
            IntPtr cmd = StringToPtr("END TRANSACTION");
            try
            {
                SQLiteUnsafeNativeMethods.sqlite3_exec(mSQLiteID, cmd, null, IntPtr.Zero, out err);
                SQLiteUnsafeNativeMethods.sqlite3_free(err);
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Commit()
        {
            if (mIsOpened == false)
            {
                return false;
            }
            IntPtr err = IntPtr.Zero;
            IntPtr cmd = StringToPtr("COMMIT");
            try
            {
                SQLiteUnsafeNativeMethods.sqlite3_exec(mSQLiteID, cmd, null, IntPtr.Zero, out err);
                if (err != IntPtr.Zero)
                {
                    OnError(err);
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Execute_NonQuery(string _query)
        {
            if (mIsOpened == false)
            {
                return false;
            }

            IntPtr err = IntPtr.Zero;
            IntPtr query = StringToPtr(_query);
            try
            {
                SQLiteUnsafeNativeMethods.sqlite3_exec(mSQLiteID, query, null, IntPtr.Zero, out err);
                if (err != IntPtr.Zero)
                {
                    OnError(err);
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public SQLite_Reader Execute_Reader(string _query)
        {
            if (mIsOpened == false)
            {
                return null;
            }

            IntPtr err = IntPtr.Zero;
            IntPtr query = StringToPtr(_query);
            IntPtr stmt = IntPtr.Zero;
            IntPtr excess = IntPtr.Zero;
            try
            {
                SQLiteUnsafeNativeMethods.sqlite3_prepare_v2(mSQLiteID, query, SQLiteUnsafeNativeMethods.lstrlen(query), out stmt, out excess);
                if (err != IntPtr.Zero)
                {
                    OnError(err);
                    return null;
                }
                SQLite_Reader reader = new SQLite_Reader(stmt);
                return reader;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
    }
}
