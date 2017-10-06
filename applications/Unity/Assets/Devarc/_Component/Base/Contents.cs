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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
#if UNITY_5
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif
using LitJson;

namespace Devarc
{
    public enum DATA_FILE_TYPE
    {
        SHEET,
        EXCEL,
        JSON,
    }

    public delegate void CallbackDataReader(string sheet_name, PropTable tb);

    public abstract class BaseDataReader : IDisposable
    {
        protected CallbackDataReader callback_every_header = null;
        protected CallbackDataReader callback_every_data = null;
        protected Dictionary<string, CallbackDataReader> callback_data_list = new Dictionary<string, CallbackDataReader>();
        protected string m_SheetName = "";

        public virtual bool ReadFile(string _filePath) { return false; }
        public virtual bool ReadData(string _data) { return false; }

        public void Clear()
        {
            callback_every_header = null;
            callback_every_data = null;
            callback_data_list.Clear();
            m_SheetName = "";
        }

        public void Dispose()
        {
            Clear();
        }

        public void RegisterCallback_EveryTable(CallbackDataReader func)
        {
            callback_every_header = func;
        }

        public void RegisterCallback_EveryLine(CallbackDataReader func)
        {
            callback_every_data = func;
        }

        public void RegisterCallback_DataLine(string sheet_name, CallbackDataReader func)
        {
            if (callback_data_list.ContainsKey(sheet_name) == false)
            {
                callback_data_list.Add(sheet_name, func);
            }
        }

        protected bool invoke_callback_line(string sheet_name, PropTable tb)
        {
            if (tb.KeyIndex < 0)
                return false;
            if (string.IsNullOrEmpty(tb.GetStr(tb.KeyIndex)))
                return false;

            if (callback_every_data != null)
            {
                callback_every_data(sheet_name, tb);
            }

            CallbackDataReader func = null;
            string class_name = GetClassName(sheet_name);
            if (callback_data_list.TryGetValue(class_name, out func))
            {
                func(sheet_name, tb);
                return true;
            }
            return false;
        }

        string GetClassName(string _path)
        {
            int startIndex = _path[0] == '!' ? 1 : 0;
            int endIndex = _path.IndexOf('@');
            if (endIndex >= 0)
                return _path.Substring(startIndex, endIndex - startIndex);
            else
                return _path.Substring(startIndex, _path.Length - startIndex);
        }
    }


    public interface IBaseObejct
    {
        void Initialize(IBaseObejct obj);
        void Initialize(PropTable obj);
        void Initialize(SqliteDataReader obj);
        void Initialize(LitJson.JsonData obj);
    }
    public interface IBaseObejct<T> : IBaseObejct
    {
        string GetSelectQuery(T _key);
    }

    public interface IContents<KEY1>
    {
        void OnAlloc(KEY1 k1);
        void OnFree();
        KEY1 GetKey1();
    }

    public interface IContents<KEY1, KEY2>
    {
        void OnAlloc(KEY1 k1, KEY2 k2);
        void OnFree();
        KEY1 GetKey1();
        KEY2 GetKey2();
    }

}
