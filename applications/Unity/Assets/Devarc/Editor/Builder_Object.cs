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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Devarc
{
    class ENUM_INFO
    {
        public string NAME;
        public int ID;
        public string DESC;

        public ENUM_INFO()
        {
            NAME = "";
            ID = 0;
            DESC = "";
        }
    }

    class ClassInfo
    {
        public string class_name { get { return _class_name; } set { _class_name = value; } }
        public string enum_name { get { return _enum_name; } set { _enum_name = value; } }
        public string container_name { get { return _container_name; } set { _container_name = value; } }
        public string key_type { get { return _key_type; } set { _key_type = value; } }
        public string key_name { get { return _key_name; } set { _key_name = value; } }
        public bool is_enum { get; set; }
        private string _class_name = "";
        private string _enum_name = "";
        private string _container_name = "";
        private string _key_type = "";
        private string _key_name = "";
    }

    class Builder_Object : Builder_Base
    {
        string FileName = "";
        string OutDir = "";
        Hashtable m_EnumList = new Hashtable();
        HashSet<string> m_ClassNames = new HashSet<string>();
        List<ClassInfo> m_ClassList = new List<ClassInfo>();

        public void Build(string _inFilePath, string _outDir)
        {
            string ext = Path.GetExtension(_inFilePath).ToLower();
            switch(ext)
            {
                case ".xml":
                    this.dataFileType = SCHEMA_TYPE.SHEET;
                    break;
                case ".xlsx":
                    this.dataFileType = SCHEMA_TYPE.EXCEL;
                    break;
                case ".schema":
                    this.dataFileType = SCHEMA_TYPE.SCHEMA;
                    break;
                default:
                    Log.Error("Cannot read file: {0}", _inFilePath);
                    return;
            }
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutDir = _outDir;
            string _outFilePath = Path.Combine(this.OutDir, "Class_" + this.FileName + ".cs");
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }
            this._build(_inFilePath, _outFilePath);
        }

        void _build(string _inFilePath, string _outFilePath)
        {
            m_EnumList.Clear();
            m_ClassNames.Clear();
            m_ClassList.Clear();

            // Get Class List
            using (BaseDataReader reader = _createReader())
            {
                reader.RegisterCallback_Table(Callback_Pass1);
                reader.ReadFile(_inFilePath);
            }

            using (TextWriter sw = new StreamWriter(_outFilePath, false))
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Text;");
                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using LitJson;");
                sw.WriteLine("namespace {0}", this.NameSpace);
                sw.WriteLine("{");
                sw.Close();
            }
            using (BaseDataReader reader = _createReader())
            {
                reader.RegisterCallback_Data(Callback_Pass2_Enum);
                reader.ReadFile(_inFilePath);
                System.Threading.Thread.Sleep(0);
            }
            using (BaseDataReader reader = _createReader())
            {
                reader.RegisterCallback_Table(Callback_Pass2_Class);
                reader.ReadFile(_inFilePath);
                System.Threading.Thread.Sleep(0);
            }

            using (TextWriter sw = new StreamWriter(_outFilePath, true))
            {
                foreach (string enum_name in m_EnumList.Keys)
                {
                    List<ENUM_INFO> enum_list = m_EnumList[enum_name] as List<ENUM_INFO>;

                    sw.WriteLine("\tpublic enum {0}", enum_name);
                    sw.WriteLine("\t{");
                    foreach (ENUM_INFO info in enum_list)
                    {
                        sw.WriteLine("\t\t{0,-20}= {1},", info.NAME, info.ID);
                    }
                    sw.WriteLine("\t}");
                    sw.WriteLine("\tpublic static partial class Marshaler");
                    sw.WriteLine("\t{");

                    sw.WriteLine("\t    public static bool Read(NetBuffer msg, ref {0} obj)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        obj = ({0})msg.ReadInt32();", enum_name);
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");

                    sw.WriteLine("\t    public static bool Write(NetBuffer msg, {0} obj)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        msg.Write((Int32)obj);");
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");

                    sw.WriteLine("\t    public static bool Read(NetBuffer msg, out {0}[] obj)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        int cnt = msg.ReadInt16();");
                    sw.WriteLine("\t        obj = new {0}[cnt];", enum_name);
                    sw.WriteLine("\t        for (int i = 0; i < cnt; i++)");
                    sw.WriteLine("\t        {");
                    sw.WriteLine("\t            obj[i] = ({0})msg.ReadInt32();", enum_name);
                    sw.WriteLine("\t        }");
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");

                    sw.WriteLine("\t    public static bool Read(NetBuffer msg, List<{0}> obj)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        int cnt = msg.ReadInt16();");
                    sw.WriteLine("\t        obj = new List<{0}>();", enum_name);
                    sw.WriteLine("\t        for (int i = 0; i < cnt; i++)");
                    sw.WriteLine("\t        {");
                    sw.WriteLine("\t            obj[i] = ({0})msg.ReadInt32();", enum_name);
                    sw.WriteLine("\t        }");
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");

                    sw.WriteLine("\t    public static bool Write(NetBuffer msg, {0}[] list)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        msg.Write((Int16)list.Length);");
                    sw.WriteLine("\t        foreach ({0} obj in list)", enum_name);
                    sw.WriteLine("\t        {");
                    sw.WriteLine("\t            msg.Write((Int32)obj);");
                    sw.WriteLine("\t        }");
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");

                    sw.WriteLine("\t    public static bool Write(NetBuffer msg, List<{0}> list)", enum_name);
                    sw.WriteLine("\t    {");
                    sw.WriteLine("\t        msg.Write((Int16)list.Count);");
                    sw.WriteLine("\t        foreach ({0} obj in list)", enum_name);
                    sw.WriteLine("\t        {");
                    sw.WriteLine("\t            msg.Write((Int32)obj);");
                    sw.WriteLine("\t        }");
                    sw.WriteLine("\t        return !msg.IsError;");
                    sw.WriteLine("\t    }");
                    sw.WriteLine("\t}");
                } // end of foreach
                sw.WriteLine("} // end of namespace");
            }
        }

        void Callback_Pass2_Class(string sheet_name, PropTable tb)
        {
            string class_name = sheet_name;
            string enum_name = sheet_name;
            string container_name = "T_" + sheet_name;
            bool is_enum = false;
            if (sheet_name.StartsWith("!"))
            {
                is_enum = true;
                class_name = "_" + sheet_name.Substring(1);
                enum_name = sheet_name.Substring(1);
                container_name = "T_" + enum_name;
            }
            string file_path = Path.Combine(this.OutDir, "Class_" + this.FileName + ".cs");
            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                sw.WriteLine("\t[System.Serializable]");
                if (tb.KeyIndex >= 0)
                    sw.WriteLine("\tpublic partial class {0} : IBaseObejct<{1}>", class_name, tb.KeyTypeName);
                else
                    sw.WriteLine("\tpublic class {0} : IBaseObejct", class_name);
                sw.WriteLine("\t{");
                if (is_enum)
                {
                    sw.WriteLine("\t\tpublic static {0} Parse(string name)", enum_name);
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\treturn FrameworkUtil.Parse<{0}>(name);", enum_name);
                    sw.WriteLine("\t\t}");
                }
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == null || var_name == "")
                    {
                        continue;
                    }
                    if (var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "bool", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "bool", var_name);
                            break;
                        case VAR_TYPE.INT16:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "Int16", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "Int16", var_name);
                            break;
                        case VAR_TYPE.INT32:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "Int32", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "Int32", var_name);
                            break;
                        case VAR_TYPE.UINT32:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "UInt32", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "UInt32", var_name);
                            break;
                        case VAR_TYPE.INT64:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "Int64", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "Int64", var_name);
                            break;
                        case VAR_TYPE.HOST_ID:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "HostID", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "HostID", var_name);
                            break;
                        case VAR_TYPE.FLOAT:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "float", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", "float", var_name);
                            break;
                        case VAR_TYPE.CSTRING:
                        case VAR_TYPE.STRING:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "string", var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1} = \"\";", "string", var_name);
                            break;
                        case VAR_TYPE.LSTRING:
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\tpublic {0,-20}{1} {{ get {{ return Table.T_LString.GetAt(_{1}.Key); }} }}", "string", var_name);
                            sw.WriteLine("\t\t{0,-20}_{1} = new {0}();", type_name, var_name);
                            break;
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", type_name, var_name);
                            else
                                sw.WriteLine("\t\tpublic {0,-20}{1};", type_name, var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                            {
                                sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", type_name, var_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\tpublic {0,-20}{1} = new {0}();", type_name, var_name);
                            }
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("");

                sw.WriteLine("\t\tpublic {0}()", class_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t\tpublic {0}({0} obj)", class_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tInitialize(obj);");
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t\tpublic {0}(PropTable obj)", class_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tInitialize(obj);");
                sw.WriteLine("\t\t}");

                if (tb.KeyIndex >= 0)
                {
                    sw.WriteLine("\t\tpublic {0} GetKey() {{ return {1}; }}", tb.KeyTypeName, tb.GetVarName(tb.KeyIndex));
                    sw.Write("\t\tpublic string GetSelectQuery({0} _key) {{ return string.Format(\"select ", tb.KeyTypeName);
                    for (int i = 0; i < tb.Length; i++)
                    {
                        if (i == 0)
                            sw.Write(tb.GetVarName(i));
                        else
                            sw.Write(", {0}", tb.GetVarName(i));
                    }
                    sw.WriteLine(" from {1} where {0}='{{0}}';\", _key); }}", tb.KeyVarName, enum_name);
                }

                sw.WriteLine("\t\tpublic bool IsDefault", class_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tget");
                sw.WriteLine("\t\t\t{");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    if (tb.GetVarType(i) == VAR_TYPE.LSTRING)
                        continue;
                    switch (tb.GetClassType(i))
                    {
                        case CLASS_TYPE.CLASS:
                            sw.WriteLine("\t\t\t\tif ({0}.IsDefault == false) return false;", var_name);
                            break;
                        case CLASS_TYPE.CLASS_LIST:
                        case CLASS_TYPE.VALUE_LIST:
                            sw.WriteLine("\t\t\t\tif ({0}.Count > 0) return false;", var_name);
                            break;
                        default:
                            switch(tb.GetVarType(i))
                            {
                                case VAR_TYPE.LSTRING:
                                case VAR_TYPE.FSTRING:
                                    break;
                                case VAR_TYPE.BOOL:
                                    sw.WriteLine("\t\t\t\tif ({0}) return false;", var_name);
                                    break;
                                case VAR_TYPE.CSTRING:
                                case VAR_TYPE.STRING:
                                    sw.WriteLine("\t\t\t\tif (string.IsNullOrEmpty({0}) == false) return false;", var_name);
                                    break;
                                case VAR_TYPE.ENUM:
                                    sw.WriteLine("\t\t\t\tif ((int){0} != 0) return false;", var_name);
                                    break;
                                default:
                                    sw.WriteLine("\t\t\t\tif ({0} != 0) return false;", var_name);
                                    break;
                            }
                            break;
                    }
                }
                sw.WriteLine("\t\t\t\treturn true;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t\tpublic void Initialize(IBaseObejct from)");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\t{0} obj = from as {0};", class_name);
                sw.WriteLine("\t\t\tif (obj == null)");
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tLog.Error(\"Cannot Initialize [name]:{0}\");", class_name);
                sw.WriteLine("\t\t\t\treturn;");
                sw.WriteLine("\t\t\t}");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetClassType(i))
                    {
                        case CLASS_TYPE.CLASS:
                            sw.WriteLine("\t\t\t{0}.Initialize(obj.{0});", var_name);
                            break;
                        case CLASS_TYPE.CLASS_LIST:
                            sw.WriteLine("\t\t\t{0}.Clear();", var_name);
                            sw.WriteLine("\t\t\tforeach({1} _obj in obj.{0}) {{ {1} _new = new {1}(_obj); {0}.Add(_new); }}", var_name, type_name);
                            break;
                        case CLASS_TYPE.VALUE_LIST:
                            sw.WriteLine("\t\t\t{0}.Clear();", var_name);
                            sw.WriteLine("\t\t\t{0}.AddRange(obj.{0});", var_name);
                            break;
                        default:
                            switch(tb.GetVarType(i))
                            {
                                case VAR_TYPE.LSTRING:
                                case VAR_TYPE.FSTRING:
                                    sw.WriteLine("\t\t\t_{0}.Initialize(obj._{0});", var_name);
                                    break;
                                default:
                                    sw.WriteLine("\t\t\t{0,-20}= obj.{0};", var_name);
                                    break;
                            }
                            break;
                    }
                }
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t\tpublic void Initialize(PropTable obj)");
                sw.WriteLine("\t\t{");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<bool>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetBool(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.INT16:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<short>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt16(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.INT32:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<int>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt32(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.UINT32:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<uint>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetUInt32(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.INT64:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<long>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt64(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.HOST_ID:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<HostID>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= (HostID)obj.GetInt16(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.FLOAT:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<float>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetFloat(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.CSTRING:
                        case VAR_TYPE.STRING:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.GetList<string>(\"{0}\", {0});", var_name);
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetStr(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.LSTRING:
                            if (tb.KeyIndex >= 0)
                            {
                                sw.WriteLine("\t\t\t_{1}.Key = FrameworkUtil.MakeLStringKey(\"{0}\", \"{1}\", {2}.ToString());", class_name, var_name, tb.KeyVarName);
                                sw.WriteLine("\t\t\t_{0}.Value = obj.GetStr(\"{0}\");", var_name);
                            }
                            break;
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\t\t_{0}.Key = obj.GetStr(\"{0}\");", var_name);
                            sw.WriteLine("\t\t\t_{0}.Value = obj.GetStr(\"{0}\");", var_name);
                            break;
                        case VAR_TYPE.ENUM:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\t{0}.Clear();", var_name);
                                sw.WriteLine("\t\t\tJsonData __{0} = JsonMapper.ToObject(obj.GetStr(\"{0}\"));", var_name);
                                sw.WriteLine("\t\t\tif (__{0} != null && __{0}.IsArray) {{ foreach (var node in __{0} as IList) {{ {0}.Add(_{1}.Parse(node.ToString())); }} }}", var_name, type_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\t{0,-20}= _{1}.Parse(obj.GetStr(\"{0}\"));", var_name, type_name);
                            }
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\t{0}.Clear();", var_name);
                                sw.WriteLine("\t\t\tJsonData __{0} = JsonMapper.ToObject(obj.GetStr(\"{0}\"));", var_name);
                                sw.WriteLine("\t\t\tif (__{0} != null && __{0}.IsArray) {{ foreach (var node in __{0} as IList) {{ {1} _v = new {1}(); _v.Initialize(node as JsonData); {0}.Add(_v); }} }}", var_name, type_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\t{0}.Initialize(obj.GetTable(\"{0}\"));", var_name);
                            }
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t\t}");



                sw.WriteLine("\t\tpublic void Initialize(JsonData obj)", class_name);
                sw.WriteLine("\t\t{");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToBoolean(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) bool.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(bool);", var_name);
                            break;
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.HOST_ID:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToInt16(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) HostID.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(short);", var_name);
                            break;
                        case VAR_TYPE.INT32:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToInt32(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) int.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(int);", var_name);
                            break;
                        case VAR_TYPE.UINT32:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToUInt32(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) uint.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(uint);", var_name);
                            break;
                        case VAR_TYPE.INT64:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToInt64(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) long.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(long);", var_name);
                            break;
                        case VAR_TYPE.FLOAT:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToSingle(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) float.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(float);", var_name);
                            break;
                        case VAR_TYPE.LSTRING:
                            sw.WriteLine("\t\t\t_{1}.Key = FrameworkUtil.MakeLStringKey(\"{0}\", \"{1}\", {2}.ToString());", class_name, var_name, tb.KeyVarName);
                            break;
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) _{0}.Key = obj[\"{0}\"].ToString(); else _{0}.Key = default(string);", var_name);
                            break;
                        case VAR_TYPE.CSTRING:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(DES.Decrypt(node.ToString())); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0} = DES.Decrypt(obj[\"{0}\"].ToString()); else {0} = default(string);", var_name);
                            break;
                        case VAR_TYPE.STRING:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(node.ToString()); }}", var_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0} = obj[\"{0}\"].ToString(); else {0} = default(string);", var_name);
                            break;
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(_{1}.Parse(node.ToString())); }}", var_name, type_name);
                            else
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0} = _{1}.Parse(obj[\"{0}\"].ToString()); else {0} = default({1});", var_name, type_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {1} _v = new {1}(); _v.Initialize(node); {0}.Add(_v); }}", var_name, type_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0}.Initialize(obj[\"{0}\"]);", var_name);
                            }
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t\t}");




                sw.WriteLine("\t\tpublic void Initialize(SQLite_Reader obj)", class_name);
                sw.WriteLine("\t\t{");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToBoolean(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetBoolean({1});", var_name, i);
                            break;
                        case VAR_TYPE.INT16:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt16(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt16({1});", var_name, i);
                            break;
                        case VAR_TYPE.INT32:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt32(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt32({1});", var_name, i);
                            break;
                        case VAR_TYPE.UINT32:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToUInt64(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= (uint)obj.GetDecimal({1});", var_name, i);
                            break;
                        case VAR_TYPE.INT64:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt64(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetInt64({1});", var_name, i);
                            break;
                        case VAR_TYPE.HOST_ID:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt16(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= (HostID)obj.GetInt16({1});", var_name, i);
                            break;
                        case VAR_TYPE.FLOAT:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToSingle(node.ToString())); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetFloat({1});", var_name, i);
                            break;
                        case VAR_TYPE.CSTRING:
                        case VAR_TYPE.STRING:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(node.ToString()); }};", var_name, i);
                            }
                            else
                                sw.WriteLine("\t\t\t{0,-20}= obj.GetString({1});", var_name, i);
                            break;
                        case VAR_TYPE.LSTRING:
                            if (tb.KeyIndex >= 0)
                            {
                                sw.WriteLine("\t\t\t_{1}.Key = FrameworkUtil.MakeLStringKey(\"{0}\", \"{1}\", {2}.ToString());", class_name, var_name, tb.KeyVarName);
                            }
                            break;
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\t\t_{0}.Key = obj.GetString({1});", var_name, i);
                            break;
                        case VAR_TYPE.ENUM:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(_{2}.Parse(node.ToString())); }};", var_name, i, type_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\t{0,-20}= _{2}.Parse(obj.GetString({1}));", var_name, i, type_name);
                            }
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                            {
                                sw.WriteLine("\t\t\tstring __{0} = obj.GetString({1}); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) FrameworkUtil.FillList<{2}>(__{0}, {0});", var_name, i, type_name);
                            }
                            else
                            {
                                sw.WriteLine("\t\t\t{0}.Initialize(JsonMapper.ToObject(obj.GetString({1})));", var_name, i);
                            }
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t\t}");



                //
                // ToString()
                //
                sw.WriteLine("\t\tpublic override string ToString()");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t    StringBuilder sb = new StringBuilder();");
                for (int i = 0, j = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    if (j == 0)
                        sw.Write("\t\t    sb.Append(\"{\");");
                    else
                        sw.Write("\t\t    sb.Append(\",\");");
                    sw.Write(" sb.Append(\" \\\"{0}\\\":\");", var_name);

                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.CSTRING:
                        case VAR_TYPE.STRING:
                        case VAR_TYPE.FSTRING:
                        case VAR_TYPE.LSTRING:
                            if (is_list)
                            {
                                sw.Write(" sb.Append(\"[\");");
                                sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(\"\\\"\"); sb.Append(_obj); sb.Append(\"\\\"\"); }}", var_name, type_name);
                                sw.WriteLine(" sb.Append(\"]\");");
                            }
                            else
                            {
                                sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append({0}); sb.Append(\"\\\"\");", var_name);
                            }
                            break;

                        case VAR_TYPE.CLASS:
                            if (is_list)
                            {
                                sw.Write(" sb.Append(\"[\");");
                                sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ sb.Append({0}[i].ToString()); }}", var_name, type_name);
                                sw.WriteLine(" sb.Append(\"]\");");
                            }
                            else
                            {
                                sw.WriteLine(" sb.Append({0} != null ? {0}.ToString() : \"{{}}\");", var_name);
                            }
                            break;
                        default:
                            if (is_list)
                            {
                                sw.Write(" sb.Append(\"[\");");
                                sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(string.Format(\"\\\"{{0}}\\\"\", _obj)); }}", var_name, type_name);
                                sw.WriteLine(" sb.Append(\"]\");");
                            }
                            else
                            {
                                sw.Write(" sb.Append(\"\\\"\");");
                                sw.Write(" sb.Append({0}.ToString());", var_name);
                                sw.WriteLine(" sb.Append(\"\\\"\");");
                            }
                            break;
                    }
                    j++;
                }
                sw.WriteLine("\t\t    sb.Append(\"}\");");
                sw.WriteLine("\t\t    return sb.ToString();");
                sw.WriteLine("\t\t}");

                //
                // ToJson
                //
                sw.WriteLine("\t\tpublic string ToJson()");
                sw.WriteLine("\t\t{");
                if (is_enum == false)
                {
                    sw.WriteLine("\t\t    if (IsDefault) { return \"{}\"; }");
                }
                sw.WriteLine("\t\t    StringBuilder sb = new StringBuilder();");
                for (int i = 0, j = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }

                    if (is_list)
                    {
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.LSTRING:
                            case VAR_TYPE.FSTRING:
                                break;
                            case VAR_TYPE.CSTRING:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(\"\\\"\"); sb.Append(DES.Encrypt(_obj)); sb.Append(\"\\\"\"); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\");");
                                }
                                else
                                {
                                    sw.Write("\t\t\tif ({0}.Count > 0) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(\"\\\"\"); sb.Append(DES.Encrypt(_obj)); sb.Append(\"\\\"\"); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\"); }");
                                }
                                break;
                            case VAR_TYPE.STRING:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(\"\\\"\"); sb.Append(_obj); sb.Append(\"\\\"\"); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\");");
                                }
                                else
                                {
                                    sw.Write("\t\t\tif ({0}.Count > 0) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(\"\\\"\"); sb.Append(_obj); sb.Append(\"\\\"\"); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\"); }");
                                }
                                break;
                            case VAR_TYPE.CLASS:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ if (i > 0) sb.Append(\",\"); sb.Append({0}[i].ToJson()); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\");");
                                }
                                else
                                {
                                    sw.Write("\t\t\tif ({0}.Count > 0) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ if (i > 0) sb.Append(\",\"); sb.Append({0}[i].ToJson()); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\"); }");
                                }
                                break;
                            default:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(string.Format(\"\\\"{{0}}\\\"\", _obj)); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\");");
                                }
                                else
                                {
                                    sw.Write("\t\t\tif ({0}.Count > 0) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"[\");");
                                    sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(string.Format(\"\\\"{{0}}\\\"\", _obj)); }}", var_name, type_name);
                                    sw.WriteLine(" sb.Append(\"]\"); }");
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.LSTRING:
                                break;
                            case VAR_TYPE.FSTRING:
                                sw.Write("\t\t\tif (string.IsNullOrEmpty(_{0}.Key) == false) {{ sb.Append(\",\");", var_name, type_name);
                                sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append(_{0}.Key); sb.Append(\"\\\"\"); }}", var_name);
                                break;
                            case VAR_TYPE.CSTRING:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append(DES.Encrypt({0})); sb.Append(\"\\\"\");", var_name);
                                }
                                else
                                {
                                    sw.Write("\t\t\tif (string.IsNullOrEmpty({0}) == false) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append(DES.Encrypt({0})); sb.Append(\"\\\"\"); }}", var_name);
                                }
                                break;
                            case VAR_TYPE.STRING:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append({0}); sb.Append(\"\\\"\");", var_name);
                                }
                                else
                                {
                                    sw.Write("\t\t\tif (string.IsNullOrEmpty({0}) == false) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append({0}); sb.Append(\"\\\"\"); }}", var_name);
                                }
                                break;
                            case VAR_TYPE.CLASS:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append({0} != null ? {0}.ToJson() : \"{{}}\");", var_name);
                                }
                                else
                                {
                                    sw.Write("\t\t    sb.Append(\",\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.WriteLine(" sb.Append({0} != null ? {0}.ToJson() : \"{{}}\");", var_name);
                                }
                                break;
                            default:
                                if (j == 0)
                                {
                                    sw.Write("\t\t    sb.Append(\"{\");");
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"\\\"\");");
                                    sw.Write(" sb.Append({0}.ToString());", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\");");
                                }
                                else
                                {
                                    sw.Write("\t\t\tif (default({1}) != {0}) {{ sb.Append(\",\");", var_name, type_name);
                                    sw.Write(" sb.Append(\"\\\"{0}\\\":\");", var_name);
                                    sw.Write(" sb.Append(\"\\\"\");");
                                    sw.Write(" sb.Append({0}.ToString());", var_name);
                                    sw.WriteLine(" sb.Append(\"\\\"\"); }");
                                }
                                break;
                        }
                    }
                    j++;
                }
                sw.WriteLine("\t\t    sb.Append(\"}\");");
                sw.WriteLine("\t\t    return sb.ToString();");
                sw.WriteLine("\t\t}");



                sw.WriteLine("\t\tpublic PropTable ToTable()");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tPropTable obj = new PropTable(\"{0}\");", class_name);
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    KEY_TYPE key_type = tb.GetKeyType(i);
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    //if (is_enum)
                    //{
                    //    sw.WriteLine("\t\t\tobj.Attach(\"Name\", \"\", CLASS_TYPE.VALUE, KEY_TYPE.NONE, {0}.ToString());", var_name, type_name, tb.KeyVarName);
                    //}
                    if (is_list)
                    {
                        sw.WriteLine("\t\t\tobj.Attach_List<{1}>(\"{0}\", \"{1}\", VAR_TYPE.{3}, {0});", var_name, type_name, tb.GetClassType(i), tb.GetVarType(i));
                    }
                    else
                    {
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.BOOL:
                            case VAR_TYPE.INT16:
                            case VAR_TYPE.INT32:
                            case VAR_TYPE.UINT32:
                            case VAR_TYPE.INT64:
                            case VAR_TYPE.HOST_ID:
                            case VAR_TYPE.FLOAT:
                                if (is_list)
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE_LIST, KEY_TYPE.{2}, {0}.ToString());", var_name, type_name, key_type);
                                else
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, {0}.ToString());", var_name, type_name, key_type);
                                break;
                            case VAR_TYPE.LSTRING:
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, _{0}.Value);", var_name, type_name, key_type);
                                break;
                            case VAR_TYPE.FSTRING:
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, _{0}.Key);", var_name, type_name, key_type);
                                break;
                            case VAR_TYPE.STRING:
                                if (is_list)
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, {0});", var_name, type_name, key_type);
                                else
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, {0});", var_name, type_name, key_type);
                                break;
                            case VAR_TYPE.ENUM:
                                if (is_enum)
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, ((int){0}).ToString());", var_name, type_name, key_type);
                                else if (is_list)
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE_LIST, KEY_TYPE.{2}, {0}.ToString());", var_name, type_name, key_type);
                                else
                                    sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, KEY_TYPE.{2}, {0}.ToString());", var_name, type_name, key_type);
                                break;
                            case VAR_TYPE.CLASS:
                                sw.WriteLine("\t\t\tobj.Attach_Class(\"{0}\", \"{1}\", {0}.ToTable());", var_name, type_name);
                                break;
                            default:
                                break;
                        }
                    }
                }
                sw.WriteLine("\t\t\treturn obj;");
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t}"); // end of class

                // mashaler start
                sw.WriteLine("\tpublic static partial class Marshaler");
                sw.WriteLine("\t{");

                sw.WriteLine("\t    public static bool Read(NetBuffer msg, {0} obj)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        bool success = true;");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.UINT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.HOST_ID:
                        case VAR_TYPE.FLOAT:
                        case VAR_TYPE.STRING:
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            else
                                sw.WriteLine("\t\t\tsuccess = success ? Marshaler.Read(msg, ref obj.{0}) : false;", var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                                sw.WriteLine("\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            else
                                sw.WriteLine("\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t        return success;");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Write(NetBuffer msg, {0} obj)", class_name);
                sw.WriteLine("\t    {");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    sw.WriteLine("\t\t\tMarshaler.Write(msg, obj.{0});", var_name);
                }
                sw.WriteLine("\t        return msg.IsError;");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Read(NetBuffer msg, List<{0}> list)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        bool success = true;");
                sw.WriteLine("\t        int cnt = msg.ReadInt16();");
                sw.WriteLine("\t        for (int i = 0; i < cnt; i++)");
                sw.WriteLine("\t        {");
                sw.WriteLine("\t\t\t\t{0} obj = new {0}();", class_name);
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    bool is_list = tb.GetClassType(i) == CLASS_TYPE.VALUE_LIST || tb.GetClassType(i) == CLASS_TYPE.CLASS_LIST;
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.UINT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.HOST_ID:
                        case VAR_TYPE.FLOAT:
                        case VAR_TYPE.STRING:
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            else
                                sw.WriteLine("\t\t\t\tsuccess = success ? Marshaler.Read(msg, ref obj.{0}) : false;", var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                                sw.WriteLine("\t\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            else
                                sw.WriteLine("\t\t\t\tsuccess = success ? Marshaler.Read(msg, obj.{0}) : false;", var_name);
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t\t\t\tlist.Add(obj);");
                sw.WriteLine("\t        }");
                sw.WriteLine("\t        return success;");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Write(NetBuffer msg, List<{0}> list)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        msg.Write((Int16)list.Count);");
                sw.WriteLine("\t        foreach ({0} obj in list)", class_name);
                sw.WriteLine("\t        {");
                for (int i = 0; i < tb.Length; i++)
                {
                    string type_name = tb.GetTypeName(i);
                    string var_name = tb.GetVarName(i);
                    if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                    {
                        continue;
                    }
                    sw.WriteLine("\t\t\t\tMarshaler.Write(msg, obj.{0});", var_name);
                }
                sw.WriteLine("\t        }");
                sw.WriteLine("\t        return msg.IsError;");
                sw.WriteLine("\t    }");
                sw.WriteLine("\t}");
                // mashaler end

            } // close file
        }


        void Callback_Pass2_Enum(string sheet_name, PropTable tb)
        {
            string enum_name = sheet_name;
            if (sheet_name.StartsWith("!"))
            {
                enum_name = sheet_name.Substring(1);
            }
            else
            {
                return;
            }

            List<ENUM_INFO> enum_list = null;
            if (m_EnumList.Contains(enum_name) == false)
            {
                enum_list = new List<ENUM_INFO>();
                m_EnumList.Add(enum_name, enum_list);
            }
            else
            {
                enum_list = m_EnumList[enum_name] as List<ENUM_INFO>;
            }
            ENUM_INFO info = new ENUM_INFO();
            info.NAME = tb.GetStr(0);
            if (tb.GetStr(1) != "")
                info.ID = Int32.Parse(tb.GetStr(1));
            else
                info.ID = 0;
            info.DESC = tb.GetStr(2);
            enum_list.Add(info);
        }


        void Callback_Pass1(string sheet_name, PropTable tb)
        {
            string class_name = sheet_name;
            bool is_enum = false;
            if (sheet_name.StartsWith("!"))
            {
                is_enum = true;
                class_name = sheet_name.Substring(1);
            }

            string item_var_name = tb.KeyVarName;
            string item_type_name = tb.KeyTypeName;

            if (m_ClassNames.Contains(class_name) == false)
            {
                m_ClassNames.Add(class_name);
                ClassInfo info = new ClassInfo();
                info.class_name = class_name;
                info.is_enum = is_enum;
                info.key_type = item_type_name;
                info.key_name = item_var_name;
                m_ClassList.Add(info);
            }

        }
    }
}
