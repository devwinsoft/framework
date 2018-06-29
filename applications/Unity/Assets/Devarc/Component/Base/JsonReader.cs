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
using System.Text;
using System.Xml;
using LitJson;

namespace Devarc
{
    public delegate void CallbackJsonReader(string sheet_name, JsonData root);

    public class JsonReader : IDisposable
    {
        private Dictionary<string, CallbackJsonReader> m_sheet_list = new Dictionary<string, CallbackJsonReader>();

        public JsonReader()
        {
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            m_sheet_list.Clear();
        }

        public void RegisterCallback(string _sheet_name, CallbackJsonReader _callback)
        {
            m_sheet_list.Add(_sheet_name, _callback);
        }

        public bool ReadFile(string file_path)
        {
            if (System.IO.File.Exists(file_path) == false)
            {
                Log.Debug("Cannot find file: " + file_path);
                return false;
            }

            string file_data = System.IO.File.ReadAllText(file_path, new UTF8Encoding());
            return ReadData(file_data);
        }

        public bool ReadData(string _data)
        {
            JsonData root = JsonMapper.ToObject(_data);
            var enumerator = m_sheet_list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string _sheet = enumerator.Current.Key;
                CallbackJsonReader _callback = enumerator.Current.Value;
                JsonData node = root[_sheet];
                if (node == null || _callback == null)
                    continue;
                if (node.IsArray == false)
                    continue;
                for (int i = 0; i < node.Count; i++)
                {
                    _callback(_sheet, node[i]);
                }
            }
            return true;
        }

    }
}
