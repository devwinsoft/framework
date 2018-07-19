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
        ARRAY, // byte[]
        BOOL,
        BYTE,
        INT16,
        INT32,
        INT64,
        UINT32,
        FLOAT,
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
        public string VarName;
        public string TypeName;
        public VAR_TYPE VarType;
        public CLASS_TYPE ClassType;
        public bool KeyType;
        public string Data;
        public int CustomLength;
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
            if (_raw_name.StartsWith("bool") || _raw_name.StartsWith("boolean"))
            {
                return "bool";
            }
            else if (_raw_name.StartsWith("byte[]") || _raw_name.StartsWith("array"))
            {
                return "byte[]";
            }
            else if (_raw_name.StartsWith("byte"))
            {
                return "byte";
            }
            else if (_raw_name.StartsWith("int") || _raw_name.StartsWith("int32"))
            {
                return "int";
            }
            else if (_raw_name.StartsWith("uint") || _raw_name.StartsWith("uint32"))
            {
                return "uint";
            }
            else if (_raw_name.StartsWith("long") || _raw_name.StartsWith("int64"))
            {
                return "long";
            }
            else if (_raw_name.StartsWith("float"))
            {
                return "float";
            }
            else if (_raw_name.StartsWith("cstr") || _raw_name.StartsWith("cstring"))
            {
                return "string";
            }
            else if (_raw_name.StartsWith("lstr") || _raw_name.StartsWith("lstring"))
            {
                return "LString";
            }
            else if (_raw_name.StartsWith("fstr") || _raw_name.StartsWith("fstring"))
            {
                return "LString";
            }
            else if (_raw_name.StartsWith("str") || _raw_name.StartsWith("string"))
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
            string var_type = _name.ToLower().Trim();
            if (var_type.StartsWith("bool"))
            {
                return VAR_TYPE.BOOL;
            }
            else if (var_type.StartsWith("byte[]") || var_type.StartsWith("array"))
            {
                return VAR_TYPE.ARRAY;
            }
            else if (var_type.StartsWith("byte"))
            {
                return VAR_TYPE.BYTE;
            }
            else if (var_type.StartsWith("short") || var_type.StartsWith("int16"))
            {
                return VAR_TYPE.INT16;
            }
            else if (var_type.StartsWith("int") || var_type.StartsWith("int32"))
            {
                return VAR_TYPE.INT32;
            }
            else if (var_type.StartsWith("uint") || var_type.StartsWith("uint32"))
            {
                return VAR_TYPE.UINT32;
            }
            else if (var_type.StartsWith("long") || var_type.StartsWith("int64"))
            {
                return VAR_TYPE.INT64;
            }
            else if (var_type.StartsWith("float"))
            {
                return VAR_TYPE.FLOAT;
            }
            else if (var_type.StartsWith("lstr") || var_type.StartsWith("lstring"))
            {
                return VAR_TYPE.LSTRING;
            }
            else if (var_type.StartsWith("fstr") || var_type.StartsWith("fstring"))
            {
                return VAR_TYPE.FSTRING;
            }
            else if (var_type.StartsWith("str") || var_type.StartsWith("string"))
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
            mTableName = name;
        }

        public void ClearAll()
        {
            mTableName = "";
            mLength = 0;
            mItemKeyIndex = -1;
            mPropList.Clear();
            mPropTable.Clear();
        }

        public void ClearData()
        {
            var enumer = mPropList.GetEnumerator();
            while (enumer.MoveNext())
            {
                enumer.Current.Value.Data = "";
            }
        }

        public void Register(int _index, string name)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data))
            {
                return;
            }

            data = new PropData();
            data.VarName = name;
            mPropList.Add(_index, data);
            mPropTable.Add(name, data);
            mLength = Math.Max(_index + 1, mLength);
        }

        public void Set_VarType(int _index, string _typeName)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.VarType = PropTable.ToTypeID(_typeName);
            data.TypeName = PropTable.ToTypeName(_typeName);
            int s = _typeName.IndexOf('(');
            int e = _typeName.IndexOf(')');
            if (s <= 0 || e <= 0 || _typeName.Length <= s + 1)
            {
                data.CustomLength = 0;
            }
            else
            {
                int length = 0;
                int.TryParse(_typeName.Substring(s + 1, e - s - 1), out length);
                data.CustomLength = length;
            }
        }

        public void Set_ClassType(int _index, string _typeName)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
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
            if (mPropList.TryGetValue(_index, out data) == false)
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
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            mPropList[_index].KeyType = _keyType;
            if (_keyType)
            {
                mItemKeyIndex = _index;
            }
        }

        public void Set_Data(int _index, string val)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.Data = val;
        }

        public void Set_CustomLength(int _index, int val)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return;
            }
            data.CustomLength = val;
        }

        public void Attach(string _name, string _typeName, CLASS_TYPE _type, bool _keyType, string _value)
        {
            int _index = mLength++;
            Register(_index, _name);
            Set_VarType(_index, _typeName);
            Set_ClassType(_index, _type);
            Set_KeyType(_index, _keyType);
            Set_Data(_index, _value);
        }
        public void Attach_List<T>(string _name, string _typeName, VAR_TYPE val_type, List<T> _list)
        {
            int _index = mLength++;
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
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return string.Empty;
            }
            return data.VarName;
        }

        public string GetVarName(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return string.Empty;
            }
            return data.VarName;
        }

        public VAR_TYPE GetVarType(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
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
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return VAR_TYPE.NONE;
            }
            switch (this.GetClassType(_index))
            {
                case CLASS_TYPE.CLASS:
                case CLASS_TYPE.CLASS_LIST:
                    return VAR_TYPE.CLASS;
                default:
                    return mPropList[_index].VarType;
            }
        }

        public string GetTypeName(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
                return string.Empty;
            if (string.IsNullOrEmpty(data.TypeName))
                return string.Empty;

            int t = data.TypeName.IndexOf('(');
            if (t <= 0)
            {
                return data.TypeName;
            }
            else
            {
                return data.TypeName.Substring(0, t);
            }
        }

        public CLASS_TYPE GetClassType(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return CLASS_TYPE.VALUE;
            }
            return data.ClassType;
        }
        public CLASS_TYPE GetClassType(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return CLASS_TYPE.VALUE;
            }
            return data.ClassType;
        }

        public bool GetKeyType(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return false;
            }
            return data.KeyType;
        }

        public bool GetKeyType(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return false;
            }
            return data.KeyType;
        }

        public int GetCustomLength(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return 0;
            }
            return data.CustomLength;
        }

        public int GetCustomLength(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return 0;
            }
            return data.CustomLength;
        }

        public byte[] GetBytes(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return null;
            }
            return System.Convert.FromBase64String(data.Data);
        }
        public byte[] GetBytes(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
            {
                return null;
            }
            return System.Convert.FromBase64String(data.Data);
        }

        public string GetStr(string _name)
        {
            PropData data;
            if (mPropTable.TryGetValue(_name, out data) == false)
            {
                return string.Empty;
            }
            return data.Data;
        }
        public string GetStr(int _index)
        {
            PropData data;
            if (mPropList.TryGetValue(_index, out data) == false)
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


        public byte GetByte(string _name)
        {
            byte temp = 0;
            byte.TryParse(GetStr(_name), out temp);
            return temp;
        }
        public Int16 GetByte(int _index)
        {
            byte temp = 0;
            byte.TryParse(GetStr(_index), out temp);
            return temp;
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
                PropData data = mPropList[i];
                if (string.IsNullOrEmpty(mPropList[i].VarName)) continue;
                if (mPropList[i].VarName.StartsWith(pre_fix) == false)
                    continue;
                string new_name = data.VarName.Substring(pre_fix.Length);
                tb.Register(j, new_name);
                tb.Set_VarType(j, data.TypeName);
                tb.Set_ClassType(j, data.ClassType);
                tb.Set_Data(j, data.Data);
                tb.Set_CustomLength(j, data.CustomLength);
                j++;
            }
            return tb;
        }
        public PropTable GetTable(int _index)
        {
            return GetTable(GetVarName(_index));
        }

        public int GetDBSize()
        {
            int length = 0;
            for (int i = 0; i < mLength; i++)
            {
                PropData data = mPropList[i];
                switch(data.VarType)
                {
                    case VAR_TYPE.NONE:
                        break;
                    case VAR_TYPE.BOOL:
                    case VAR_TYPE.INT16:
                        length += 6;
                        break;
                    case VAR_TYPE.FLOAT:
                    case VAR_TYPE.INT32:
                    case VAR_TYPE.UINT32:
                        length += 11;
                        break;
                    case VAR_TYPE.INT64:
                        length += 20;
                        break;
                    default:
                        if (data.CustomLength == 0)
                            length += 50;
                        else
                            length += data.CustomLength;
                        break;
                }
            }
            const int blockSize = 256;
            int block = (length / blockSize) + 1;
            return blockSize * block;
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

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            for (int i = 0; i < mLength; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                string value = mPropList[i].Data != null ? mPropList[i].Data : "";
                switch (GetClassType(i))
                {
                    case CLASS_TYPE.VALUE_LIST:
                    case CLASS_TYPE.CLASS_LIST:
                        if (string.IsNullOrEmpty(value))
                            value = "[]";

                        sb.Append(string.Format("\"{0}\":{1}", mPropList[i].VarName, value));
                        break;
                    case CLASS_TYPE.CLASS:
                        if (string.IsNullOrEmpty(value))
                            value = "{}";
                        sb.Append(string.Format("\"{0}\":{1}", mPropList[i].VarName, value));
                        break;
                    default:
                        sb.Append(string.Format("\"{0}\":\"{1}\"", mPropList[i].VarName, value.Replace("\\", "\\\\").Replace("\"", "\\\"")));
                        break;
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }

        public bool IsEnum
        {
            get { return mIsEnum; }
            set { mIsEnum = value; }
        }
        public string TableName
        {
            get { return mTableName; }
            set { mTableName = value; }
        }
        public int Length { get { return mLength; } }
        public int KeyIndex { get { return mItemKeyIndex; } }
        public string KeyTypeName
        {
            get
            {
                if (mItemKeyIndex < 0)
                    return "";
                else
                    return GetTypeName(mItemKeyIndex);
            }
        }
        public string KeyVarName
        {
            get
            {
                if (mItemKeyIndex < 0)
                    return "";
                else
                    return GetVarName(mItemKeyIndex);
            }
        }

        bool mIsEnum = false;
        string mTableName = "";
        Dictionary<int, PropData> mPropList = new Dictionary<int, PropData>();
        Dictionary<string, PropData> mPropTable = new Dictionary<string, PropData>();
        int mItemKeyIndex;
        int mLength;
    }
}
