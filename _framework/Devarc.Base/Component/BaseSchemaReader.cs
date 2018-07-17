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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using LitJson;

namespace Devarc
{
    public enum SCHEMA_TYPE
    {
        SCHEMA,
        EXCEL,
        SHEET,
        JSON,
    }

    public delegate void CallbackDataReader(string sheet_name, PropTable tb);

    public abstract class BaseSchemaReader : IDisposable
    {
        protected CallbackDataReader callback_header = null;
        protected CallbackDataReader callback_data = null;
        protected Dictionary<string, CallbackDataReader> callback_data_list = new Dictionary<string, CallbackDataReader>();
        protected string m_SheetName = "";

        public virtual bool ReadFile(string _filePath) { return false; }
        public virtual bool ReadData(string _data) { return false; }

        public void Clear()
        {
            callback_header = null;
            callback_data = null;
            callback_data_list.Clear();
            m_SheetName = "";
        }

        public void Dispose()
        {
            Clear();
        }

        public void RegisterCallback_Table(CallbackDataReader func)
        {
            callback_header = func;
        }

        public void RegisterCallback_Data(CallbackDataReader func)
        {
            callback_data = func;
        }

        public void RegisterCallback_Data(string sheet_name, CallbackDataReader func)
        {
            if (callback_data_list.ContainsKey(sheet_name) == false)
            {
                callback_data_list.Add(sheet_name, func);
            }
        }

        protected bool invoke_callback_data(string sheet_name, PropTable tb)
        {
            if (tb.KeyIndex < 0)
                return false;
            if (string.IsNullOrEmpty(tb.GetStr(tb.KeyIndex)))
                return false;

            if (callback_data != null)
            {
                callback_data(sheet_name, tb);
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

        public string GetClassName(string _path)
        {
            int startIndex = _path[0] == '!' ? 1 : 0;
            int endIndex = _path.IndexOf('@');
            if (endIndex >= 0)
                return _path.Substring(startIndex, endIndex - startIndex);
            else
                return _path.Substring(startIndex, _path.Length - startIndex);
        }
    }
}

