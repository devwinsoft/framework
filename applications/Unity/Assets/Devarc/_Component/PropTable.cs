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
using Mono.Data.Sqlite;
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

    public enum KEY_TYPE
    {
        NONE,
        LIST,
        MAP,
    }

    enum ROW_TYPE
    {
        VAR_NAME = 1,
        TYPE_NAME,
        CLASS_TYPE, // value, class, list
        KEY_TYPE,
        DATA_FIELD,
    }


    public struct PropData
    {
        public string VarName { get; set; }
        public string TypeName { get; set; }
        public CLASS_TYPE ClassType { get; set; }
        public KEY_TYPE KeyType { get; set; }
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
            else if (_raw_name.Equals("str") || _raw_name.Equals("string"))
            {
                return "string";
            }
            else
            {
                return _name;
            }
        }

        public static VAR_TYPE ToVarType(string _name)
        {
            string var_type = _name.ToLower();
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

        public static KEY_TYPE ToKeyType(string _name)
        {
            if (string.IsNullOrEmpty(_name))
                return KEY_TYPE.NONE;
            switch(_name.ToUpper())
            {
                case "1":
                case "TRUE":
                case "MAP":
                    return KEY_TYPE.MAP;
                case "LIST":
                    return KEY_TYPE.LIST;
                default:
                    return KEY_TYPE.NONE;
            }
        }

        public PropTable()
        {
            for (int i = 0; i < m_PropList.Length; i++)
            {
                m_PropList[i] = new PropData();
            }
            ClearAll();
        }

        public PropTable(string name)
        {
            for (int i = 0; i < m_PropList.Length; i++)
            {
                m_PropList[i] = new PropData();
            }
            ClearAll();
            m_TableName = name;
        }

        public void ClearAll()
        {
            m_TableName = "";
            m_Length = 0;
            m_ItemKeyIndex = -1;
            m_PropTable.Clear();
            for (int i = 0; i < m_PropList.Length; i++)
            {
                PropData prop = m_PropList[i];
                prop.TypeName = "";
                prop.VarName = "";
                prop.ClassType = CLASS_TYPE.VALUE;
                prop.Data = "";
            }
        }

        public void ClearData()
        {
            for (int i = 0; i < m_PropList.Length; i++)
            {
                m_PropList[i].Data = "";
            }
        }

        public void initVarName(int index, string name)
        {
            m_Length = Math.Max(index+1, m_Length);
            m_PropList[index].VarName = name;
            m_PropList[index].TypeName = "";
            m_PropTable.Add(name, index);
        }

        public void initVarType(int index, string _typeName)
        {
            m_PropList[index].TypeName = PropTable.ToTypeName(_typeName);
        }

        public void initClassType(int index, string _typeName)
        {
            if (string.IsNullOrEmpty(_typeName))
                m_PropList[index].ClassType = CLASS_TYPE.VALUE;
            switch (_typeName.ToUpper())
            {
                case "CLASS":
                    m_PropList[index].ClassType = CLASS_TYPE.CLASS;
                    break;
                case "CLASS_LIST":
                    m_PropList[index].ClassType = CLASS_TYPE.CLASS_LIST;
                    break;
                case "LIST":
                    m_PropList[index].ClassType = CLASS_TYPE.VALUE_LIST;
                    break;
                default:
                    m_PropList[index].ClassType = CLASS_TYPE.VALUE;
                    break;
            }
        }
        public void initClassType(int index, CLASS_TYPE _type)
        {
            m_PropList[index].ClassType = _type;
        }
        public void initKeyType(int index, string _keyType)
        {
            if (string.IsNullOrEmpty(_keyType))
                initKeyType(index, KEY_TYPE.NONE);
            switch (_keyType.ToUpper())
            {
                case "TRUE":
                case "MAP":
                    initKeyType(index, KEY_TYPE.MAP);
                    break;
                case "LIST":
                    initKeyType(index, KEY_TYPE.LIST);
                    break;
                default:
                    initKeyType(index, KEY_TYPE.NONE);
                    break;
            }
        }
        public void initKeyType(int index, KEY_TYPE _keyType)
        {
            m_PropList[index].KeyType = _keyType;
            switch (_keyType)
            {
                case KEY_TYPE.MAP:
                    m_ItemKeyIndex = index;
                    break;
                case KEY_TYPE.LIST:
                    break;
                default:
                    break;
            }
        }

        public void SetData(int index, string val)
        {
            m_PropList[index].Data = val;
        }

        public void Attach(string var_name, string _typeName, CLASS_TYPE _type, KEY_TYPE _keyType, string _value)
        {
            int index = m_Length;
            initVarName(index, var_name);
            initVarType(index, _typeName);
            initClassType(index, _type);
            initKeyType(index, _keyType);
            SetData(index, _value);
        }
        public void Attach_List<T>(string var_name, string _typeName, VAR_TYPE val_type, List<T> _list)
        {
            int index = m_Length;
            initVarName(index, var_name);
            initVarType(index, _typeName);
            switch (val_type)
            {
                case VAR_TYPE.CLASS:
                    initClassType(index, CLASS_TYPE.CLASS_LIST);
                    break;
                default:
                    initClassType(index, CLASS_TYPE.VALUE_LIST);
                    break;
            }
            initKeyType(index, KEY_TYPE.NONE);
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
            SetData(index, sb.ToString());
        }
        public void Attach_Class(string var_name, string _raw_type, PropTable obj)
        {
            Attach(var_name, _raw_type, CLASS_TYPE.CLASS, KEY_TYPE.NONE, "");
            string temp_full_name;
            string temp_var_name;
            string temp_type_name;
            for (int j = 0; j < obj.Length; j++)
            {
                temp_var_name = obj.GetVarName(j);
                temp_type_name = obj.GetTypeName(j);
                temp_full_name = var_name + "/" + obj.GetVarName(j);
                if (temp_var_name.Length == 0 || temp_type_name.Length == 0)
                {
                    continue;
                }
                Attach(temp_full_name, obj.GetTypeName(j), obj.GetClassType(j), obj.GetKeyType(j), obj.GetStr(j));
            }
        }

        public string GetVarName(string var_name)
        {
            return GetVarName(GetIndexOfName(var_name));
        }
        public string GetVarName(int index)
        {
            if (index >= 0 && index < m_PropList.Length && m_PropList[index].VarName != null)
                return m_PropList[index].VarName;
            else
                return "";
        }

        public VAR_TYPE GetVarType(string var_name)
        {
            return GetVarType(GetIndexOfName(var_name));
        }
        public VAR_TYPE GetVarType(int index)
        {
            switch (this.GetClassType(index))
            {
                case CLASS_TYPE.CLASS:
                case CLASS_TYPE.CLASS_LIST:
                    return VAR_TYPE.CLASS;
                default:
                    return PropTable.ToVarType(m_PropList[index].TypeName);
            }
        }

        public string GetTypeName(int index)
        {
            return m_PropList[index].TypeName;
        }


        public CLASS_TYPE GetClassType(int index)
        {
            if (index >= 0 && index < m_PropList.Length)
                return m_PropList[index].ClassType;
            else
                return CLASS_TYPE.VALUE;
        }

        public KEY_TYPE GetKeyType(string var_name)
        {
            return GetKeyType(GetIndexOfName(var_name));
        }
        public KEY_TYPE GetKeyType(int index)
        {
            if (index >= 0 && index < m_PropList.Length)
                return m_PropList[index].KeyType;
            else
                return KEY_TYPE.NONE;
        }

        public string GetStr(string var_name)
        {
            return GetStr(GetIndexOfName(var_name));
        }
        public string GetStr(int index)
        {
            if (index >= 0 && index < m_PropList.Length && m_PropList[index].Data != null)
                return m_PropList[index].Data;
            else
                return "";
        }

        public bool GetBool(string var_name)
        {
            return GetBool(GetIndexOfName(var_name));
        }
        public bool GetBool(int index)
        {
            string temp = GetStr(index).ToLower();
            if (temp == "true")
                return true;
            else if (temp == "false")
                return false;
            return GetInt32(index) != 0;
        }

        public Int16 GetInt16(string var_name)
        {
            return GetInt16(GetIndexOfName(var_name));
        }
        public Int16 GetInt16(int index)
        {
            string data = GetStr(index);
            Int16 temp = 0;
            Int16.TryParse(data, out temp);
            return temp;
        }

        public Int32 GetInt32(string var_name)
        {
            return GetInt32(GetIndexOfName(var_name));
        }
        public Int32 GetInt32(int index)
        {
            string data = GetStr(index);
            Int32 temp = 0;
            Int32.TryParse(data, out temp);
            return temp;
        }

        public Int64 GetInt64(string var_name)
        {
            return GetInt64(GetIndexOfName(var_name));
        }
        public Int64 GetInt64(int index)
        {
            string data = GetStr(index);
            Int64 temp = 0;
            Int64.TryParse(data, out temp);
            return temp;
        }

        public UInt32 GetUInt32(string var_name)
        {
            return GetUInt32(GetIndexOfName(var_name));
        }
        public UInt32 GetUInt32(int index)
        {
            string data = GetStr(index);
            UInt32 temp = 0;
            UInt32.TryParse(data, out temp);
            return temp;
        }

        public float GetFloat(string var_name)
        {
            return GetFloat(GetIndexOfName(var_name));
        }
        public float GetFloat(int index)
        {
            string data = GetStr(index);
            float temp = 0.0f;
            float.TryParse(data, out temp);
            return temp;
        }

        public PropTable GetTable(string var_name)
        {
            PropTable tb = new PropTable();
            string pre_fix = var_name + "/";
            int i, j;
            for (i = j = 0; i < Length; i++)
            {
                PropData data = m_PropList[i];
                if (string.IsNullOrEmpty(m_PropList[i].VarName)) continue;
                if (m_PropList[i].VarName.StartsWith(pre_fix) == false)
                    continue;
                string new_name = data.VarName.Substring(pre_fix.Length);
                tb.initVarName(j, new_name);
                tb.initVarType(j, data.TypeName);
                tb.initClassType(j, data.ClassType);
                tb.SetData(j, data.Data);
                j++;
            }
            return tb;
        }
        public PropTable GetTable(int index)
        {
            return GetTable(GetVarName(index));
        }

        public bool GetClassList<T>(string var_name, List<T> list) where T : IBaseObejct, new()
        {
            return GetClassList<T>(GetIndexOfName(var_name), list);
        }
        public bool GetClassList<T>(int index, List<T> list) where T : IBaseObejct, new()
        {
            list.Clear();
            if (index < 0 || index > this.Length)
                return false;
            JsonData node = JsonMapper.ToObject(GetStr(index));
            for (int i = 0; i < node.Count; i++)
            {
                T obj = new T();
                obj.Initialize(node[i]);
                list.Add(obj);
            }
            return true;
        }

        public bool GetList<T>(string var_name, List<T> list)
        {
            return GetList<T>(GetIndexOfName(var_name), list);
        }
        public bool GetList<T>(int index, List<T> list)
        {
            list.Clear();
            if (index < 0 || index > this.Length)
                return false;
            VAR_TYPE var_type = this.GetVarType(index);
            string json = GetStr(index);
            if (string.IsNullOrEmpty(json))
            {
                return true;
            }
            JsonData node = JsonMapper.ToObject(json);
            switch (var_type)
            {
                case VAR_TYPE.BOOL:
                    {
                        List<bool> temp_list = list as List<bool>;
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
                        List<Int16> temp_list = list as List<Int16>;
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
                        List<Int32> temp_list = list as List<Int32>;
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
                        List<Int64> temp_list = list as List<Int64>;
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
                        List<UInt32> temp_list = list as List<UInt32>;
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
                        List<Single> temp_list = list as List<Single>;
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
                        List<string> temp_list = list as List<string>;
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
                        List<string> temp_list = list as List<string>;
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

        public int GetIndexOfName(string var_name)
        {
            if (m_PropTable.Contains(var_name))
            {
                return (int)m_PropTable[var_name];
            }
            return -1;
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

        private string m_TableName = "";
        private PropData[] m_PropList = new PropData[256];
        private Hashtable m_PropTable = new Hashtable();
        private int m_Length;
        private int m_ItemKeyIndex;
    }
}
