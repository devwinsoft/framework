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
using LitJson;

namespace Devarc
{
    public enum VAR_TYPE
    {
        NONE,
        BOOL,
        INT16,
        INT32,
        INT64,
        UINT32,
        HOST_ID,
        FLOAT,
        CSTRING,
        STRING,
        LSTRING,
        FSTRING,
        ENUM,
        CLASS,
    }

    public enum CLASS_TYPE
    {
        VALUE,
        CLASS,
        VALUE_LIST,
        CLASS_LIST,
    }

    enum ROW_TYPE
    {
        VAR_NAME = 1,
        TYPE_NAME,
        CLASS_TYPE, // value, class, list
        KEY_TYPE,
        DATA_FIELD,
    }


    public class PropData
    {
        public string VarName { get; set; }
        public string TypeName { get; set; }
        public VAR_TYPE VarType { get; set; }
        public CLASS_TYPE ClassType { get; set; }
        public bool KeyType { get; set; }
        public string Data { get; set; }
    }

    public class PropTable
    {
        public static string ToTypeName(string _name)
        {
            string _raw_name = _name.ToLower();
            if (string.IsNullOrEmpty(_raw_name))
            {
                return "";
            }
            if (_raw_name.Equals("int") || _raw_name.Equals("int32"))
            {
                return "int";
            }
            else if (_raw_name.Equals("uint") || _raw_name.Equals("uint32"))
            {
                return "uint";
            }
            else if (_raw_name.Equals("long") || _raw_name.Equals("int64"))
            {
                return "long";
            }
            else if (_raw_name.Equals("hid") || _raw_name.Equals("hostid"))
            {
                return "HostID";
            }
            else if (_raw_name.Equals("float"))
            {
                return "float";
            }
            else if (_raw_name.Equals("cstr") || _raw_name.Equals("cstring"))
            {
                return "string";
            }
            else if (_raw_name.Equals("lstr") || _raw_name.Equals("lstring"))
            {
                return "LString";
            }
            else if (_raw_name.Equals("fstr") || _raw_name.Equals("fstring"))
            {
                return "LString";
            }
            else if (_raw_name.Equals("str") || _raw_name.Equals("string"))
            {
                return "string";
            }
            else
            {
                return _name;
            }
        }

        public static VAR_TYPE ToTypeID(string _name)
        {
            string var_type = _name.ToLower().Replace("[]","");
            if (var_type.Equals("bool"))
            {
                return VAR_TYPE.BOOL;
            }
            else if (var_type.Equals("short") || var_type.Equals("int16"))
            {
                return VAR_TYPE.INT16;
            }
            else if (var_type.Equals("int") || var_type.Equals("int32"))
            {
                return VAR_TYPE.INT32;
            }
            else if (var_type.Equals("uint") || var_type.Equals("uint32"))
            {
                return VAR_TYPE.UINT32;
            }
            else if (var_type.Equals("long") || var_type.Equals("int64"))
            {
                return VAR_TYPE.INT64;
            }
            else if (var_type.Equals("hid") || var_type.Equals("hostid"))
            {
                return VAR_TYPE.HOST_ID;
            }
            else if (var_type.Equals("float"))
            {
                return VAR_TYPE.FLOAT;
            }
            else if (var_type.Equals("cstr") || var_type.Equals("cstring"))
            {
                return VAR_TYPE.CSTRING;
            }
            else if (var_type.Equals("lstr") || var_type.Equals("lstring"))
            {
                return VAR_TYPE.LSTRING;
            }
            else if (var_type.Equals("fstr") || var_type.Equals("fstring"))
            {
                return VAR_TYPE.FSTRING;
            }
            else if (var_type.Equals("str") || var_type.Equals("string"))
            {
                return VAR_TYPE.STRING;
            }
            else if (var_type.Length > 0)
            {
                return VAR_TYPE.ENUM;
            }
            else
            {
                return VAR_TYPE.NONE;
            }
        }

        public static CLASS_TYPE ToClassType(string _name)
        {
            string _typeName = _name.ToLower();
            if (_typeName.ToLower().Equals("class_list"))
            {
                return CLASS_TYPE.CLASS_LIST;
            }
            else if (_typeName.ToLower().Equals("class"))
            {
                return CLASS_TYPE.CLASS;
            }
            else if (_typeName.ToLower().Equals("list"))
            {
                return CLASS_TYPE.VALUE_LIST;
            }
            else
            {
                return CLASS_TYPE.VALUE;
            }
        }

        public static bool ToKeyType(string _name)
        {
            if (string.IsNullOrEmpty(_name))
                return false;
            switch(_name.ToUpper())
            {
                case "1":
                case "TRUE":
                case "MAP":
                    return true;
                default:
                    return false;
            }
        }


        public PropTable()
        {
            ClearAll();
        }

        public PropTable(string name)
        {
            ClearAll();
            m_TableName = name;
        }

        public void ClearAll()
        {
            m_TableName = "";
            m_Length = 0;
            m_ItemKeyIndex = -1;
            m_PropList.Clear();
            m_PropTable.Clear();
        }

        public void ClearData()
        {
            var enumer = m_PropList.GetEnumerator();
            while (enumer.MoveNext())
            {
                enumer.Current.Value.Data = "";
            }
        }

        public void Register(int _index, string name)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data))
            {
                return;
            }

            data = new PropData();
            data.VarName = name;
            m_PropList.Add(_index, data);
            m_PropTable.Add(name, data);
            m_Length = Math.Max(_index + 1, m_Length);
        }

        public void Set_VarType(int _index, string _typeName)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.VarType = PropTable.ToTypeID(_typeName);
            data.TypeName = PropTable.ToTypeName(_typeName);
        }

        public void Set_ClassType(int _index, string _typeName)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            if (string.IsNullOrEmpty(_typeName))
            {
                data.ClassType = CLASS_TYPE.VALUE;
                return;
            }
            switch (_typeName.ToUpper())
            {
                case "CLASS":
                    data.ClassType = CLASS_TYPE.CLASS;
                    break;
                case "CLASS_LIST":
                    data.ClassType = CLASS_TYPE.CLASS_LIST;
                    break;
                case "LIST":
                    data.ClassType = CLASS_TYPE.VALUE_LIST;
                    break;
                default:
                    data.ClassType = CLASS_TYPE.VALUE;
                    break;
            }
        }
        public void Set_ClassType(int _index, CLASS_TYPE _type)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.ClassType = _type;
        }
        public void Set_KeyType(int _index, string _keyType)
        {
            Set_KeyType(_index, ToKeyType(_keyType));
        }
        public void Set_KeyType(int _index, bool _keyType)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            m_PropList[_index].KeyType = _keyType;
            if (_keyType)
            {
                m_ItemKeyIndex = _index;
            }
        }

        public void Set_Data(int _index, string val)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.Data = val;
        }

        public void Attach(string _name, string _typeName, CLASS_TYPE _type, bool _keyType, string _value)
        {
            int _index = m_Length++;
            Register(_index, _name);
            Set_VarType(_index, _typeName);
            Set_ClassType(_index, _type);
            Set_KeyType(_index, _keyType);
            Set_Data(_index, _value);
        }
        public void Attach_List<T>(string _name, string _typeName, VAR_TYPE val_type, List<T> _list)
        {
            int _index = m_Length++;
            Register(_index, _name);
            Set_VarType(_index, _typeName);
            switch (val_type)
            {
                case VAR_TYPE.CLASS:
                    Set_ClassType(_index, CLASS_TYPE.CLASS_LIST);
                    break;
                default:
                    Set_ClassType(_index, CLASS_TYPE.VALUE_LIST);
                    break;
            }
            Set_KeyType(_index, false);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0;i<_list.Count;i++)
            {
                T tmp = _list[i];
                if (i > 0) sb.Append(",");
                switch(val_type)
                {
                    case VAR_TYPE.ENUM:
                    case VAR_TYPE.CSTRING:
                    case VAR_TYPE.STRING:
                        sb.Append("\"");
                        sb.Append(tmp.ToString());
                        sb.Append("\"");
                        break;
                    default:
                        sb.Append(tmp.ToString());
                        break;
                }
            }
            sb.Append("]");
            Set_Data(_index, sb.ToString());
        }
        public void Attach_Class(string _name, string _raw_type, PropTable obj)
        {
            Attach(_name, _raw_type, CLASS_TYPE.CLASS, false, "");
            string temp_full_name;
            string temp_var_name;
            string temp_type_name;
            for (int j = 0; j < obj.Length; j++)
            {
                temp_var_name = obj.GetVarName(j);
                temp_type_name = obj.GetTypeName(j);
                temp_full_name = _name + "/" + obj.GetVarName(j);
                if (temp_var_name.Length == 0 || temp_type_name.Length == 0)
                {
                    continue;
                }
                Attach(temp_full_name, obj.GetTypeName(j), obj.GetClassType(j), obj.GetKeyType(j), obj.GetStr(j));
            }
        }

        public string GetVarName(string _name)
        {
            PropData data;
            if (m_PropTable.TryGetValue(_name, out data) == false)
            {
                return string.Empty;
            }
            return data.VarName;
        }

        public string GetVarName(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return string.Empty;
            }
            return data.VarName;
        }

        public VAR_TYPE GetVarType(string _name)
        {
            PropData data;
            if (m_PropTable.TryGetValue(_name, out data) == false)
            {
                return VAR_TYPE.NONE;
            }
            switch (this.GetClassType(_name))
            {
                case CLASS_TYPE.CLASS:
                case CLASS_TYPE.CLASS_LIST:
                    return VAR_TYPE.CLASS;
                default:
                    return data.VarType;
            }
        }
        public VAR_TYPE GetVarType(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return VAR_TYPE.NONE;
            }
            switch (this.GetClassType(_index))
            {
                case CLASS_TYPE.CLASS:
                case CLASS_TYPE.CLASS_LIST:
                    return VAR_TYPE.CLASS;
                default:
                    return m_PropList[_index].VarType;
            }
        }

        public string GetTypeName(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return string.Empty;
            }
            return data.TypeName;
        }

        public CLASS_TYPE GetClassType(string _name)
        {
            PropData data;
            if (m_PropTable.TryGetValue(_name, out data) == false)
            {
                return CLASS_TYPE.VALUE;
            }
            return data.ClassType;
        }
        public CLASS_TYPE GetClassType(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return CLASS_TYPE.VALUE;
            }
            return data.ClassType;
        }

        public bool GetKeyType(string _name)
        {
            PropData data;
            if (m_PropTable.TryGetValue(_name, out data) == false)
            {
                return false;
            }
            return data.KeyType;
        }
        public bool GetKeyType(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return false;
            }
            return data.KeyType;
        }

        public string GetStr(string _name)
        {
            PropData data;
            if (m_PropTable.TryGetValue(_name, out data) == false)
            {
                return string.Empty;
            }
            return data.Data;
        }
        public string GetStr(int _index)
        {
            PropData data;
            if (m_PropList.TryGetValue(_index, out data) == false)
            {
                return string.Empty;
            }
            return data.Data;
        }

        public bool GetBool(string _name)
        {
            string temp = GetStr(_name).ToLower();
            if (temp == "true")
                return true;
            else if (temp == "false")
                return false;
            return GetInt32(_name) != 0;
        }
        public bool GetBool(int _index)
        {
            string temp = GetStr(_index).ToLower();
            if (temp == "true")
                return true;
            else if (temp == "false")
                return false;
            return GetInt32(_index) != 0;
        }

        public Int16 GetInt16(string _name)
        {
            Int16 temp = 0;
            Int16.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public Int16 GetInt16(int _index)
        {
            Int16 temp = 0;
            Int16.TryParse(GetStr(_index), out temp);
            return temp;
        }

        public Int32 GetInt32(string _name)
        {
            Int32 temp = 0;
            Int32.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public Int32 GetInt32(int _index)
        {
            Int32 temp = 0;
            Int32.TryParse(GetStr(_index), out temp);
            return temp;
        }

        public Int64 GetInt64(string _name)
        {
            Int64 temp = 0;
            Int64.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public Int64 GetInt64(int _index)
        {
            Int64 temp = 0;
            Int64.TryParse(GetStr(_index), out temp);
            return temp;
        }

        public UInt32 GetUInt32(string _name)
        {
            UInt32 temp = 0;
            UInt32.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public UInt32 GetUInt32(int _index)
        {
            UInt32 temp = 0;
            UInt32.TryParse(GetStr(_index), out temp);
            return temp;
        }

        public float GetFloat(string _name)
        {
            float temp = 0;
            float.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public float GetFloat(int _index)
        {
            float temp = 0;
            float.TryParse(GetStr(_index), out temp);
            return temp;
        }

        public PropTable GetTable(string _name)
        {
            if (string.IsNullOrEmpty(_name))
            {
                return null;
            }

            PropTable tb = new PropTable();
            string pre_fix = _name + "/";
            int i, j;
            for (i = j = 0; i < Length; i++)
            {
                PropData data = m_PropList[i];
                if (string.IsNullOrEmpty(m_PropList[i].VarName)) continue;
                if (m_PropList[i].VarName.StartsWith(pre_fix) == false)
                    continue;
                string new_name = data.VarName.Substring(pre_fix.Length);
                tb.Register(j, new_name);
                tb.Set_VarType(j, data.TypeName);
                tb.Set_ClassType(j, data.ClassType);
                tb.Set_Data(j, data.Data);
                j++;
            }
            return tb;
        }
        public PropTable GetTable(int _index)
        {
            return GetTable(GetVarName(_index));
        }

        public bool GetClassList<T>(string _name, List<T> _list) where T : IBaseObejct, new()
        {
            _list.Clear();
            JsonData node = JsonMapper.ToObject(GetStr(_name));
            for (int i = 0; i < node.Count; i++)
            {
                T obj = new T();
                obj.Initialize(node[i]);
                _list.Add(obj);
            }
            return true;
        }
        public bool GetClassList<T>(int _index, List<T> _list) where T : IBaseObejct, new()
        {
            _list.Clear();
            JsonData node = JsonMapper.ToObject(GetStr(_index));
            for (int i = 0; i < node.Count; i++)
            {
                T obj = new T();
                obj.Initialize(node[i]);
                _list.Add(obj);
            }
            return true;
        }

        public bool GetList<T>(string _name, List<T> _list)
        {
            VAR_TYPE type = this.GetVarType(_name);
            string json = GetStr(_name);
            return getList<T>(type, json, _list);
        }
        public bool GetList<T>(int _index, List<T> list)
        {
            VAR_TYPE type = this.GetVarType(_index);
            string json = GetStr(_index);
            return getList<T>(type, json, list);
        }

        bool getList<T>(VAR_TYPE _type, string _json, List<T> _list)
        {
            _list.Clear();
            if (string.IsNullOrEmpty(_json))
            {
                return true;
            }
            JsonData node = JsonMapper.ToObject(_json);
            switch (_type)
            {
                case VAR_TYPE.BOOL:
                    {
                        List<bool> temp_list = _list as List<bool>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToBoolean(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.INT16:
                    {
                        List<Int16> temp_list = _list as List<Int16>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToInt16(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.INT32:
                    {
                        List<Int32> temp_list = _list as List<Int32>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToInt32(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.INT64:
                    {
                        List<Int64> temp_list = _list as List<Int64>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToInt64(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.UINT32:
                    {
                        List<UInt32> temp_list = _list as List<UInt32>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToUInt32(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.FLOAT:
                    {
                        List<Single> temp_list = _list as List<Single>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(Convert.ToSingle(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.CSTRING:
                    {
                        List<string> temp_list = _list as List<string>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(DES.Encrypt(node[i].ToString()));
                            }
                        }
                    }
                    break;
                case VAR_TYPE.STRING:
                    {
                        List<string> temp_list = _list as List<string>;
                        if (temp_list != null)
                        {
                            for (int i = 0; i < node.Count; i++)
                            {
                                temp_list.Add(node[i].ToString());
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }


        public int Length { get { return m_Length; } }
        public string TableName { get { return m_TableName; } }
        public int KeyIndex { get { return m_ItemKeyIndex; } }
        public string KeyTypeName
        {
            get
            {
                if (m_ItemKeyIndex < 0)
                    return "";
                else
                    return GetTypeName(m_ItemKeyIndex);
            }
        }
        public string KeyVarName
        {
            get
            {
                if (m_ItemKeyIndex < 0)
                    return "";
                else
                    return GetVarName(m_ItemKeyIndex);
            }
        }

        string m_TableName = "";
        Dictionary<int, PropData> m_PropList = new Dictionary<int, PropData>();
        Dictionary<string, PropData> m_PropTable = new Dictionary<string, PropData>();
        int m_ItemKeyIndex;
        int m_Length;
    }
}
