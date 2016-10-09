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
using System.Text;

namespace Devarc
{
    class Builder_Data
    {
        string NameSpace = "Devarc";
        string FileName = "";
        string OutDir = "";
        HashSet<string> m_ClassNames = new HashSet<string>();
        List<ClassInfo> m_ClassList = new List<ClassInfo>();


        public void BuildFromFile(string _input_file, string _out_dir)
        {
            this.FileName = Path.GetFileNameWithoutExtension(_input_file);
            this.OutDir = _out_dir; 
            string file_path = this.OutDir + "\\TableData_" + this.FileName + ".cs";

            if (File.Exists(_input_file) == false)
            {
                Log.Message(LOG_TYPE.INFO, "Cannot find file: " + _input_file);
                return;
            }

            this._build(File.ReadAllText(_input_file), file_path);
        }

        public void BuildFromData(string _file_name, string _input_data, string _out_dir)
        {
            this.FileName = _file_name;
            this.OutDir = _out_dir;
            string file_path = this.OutDir + "\\TableData_" + this.FileName + ".cs";

            this._build(_input_data, file_path);
        }

        void _build(string _data, string file_path)
        {
            using (TextWriter sw = new StreamWriter(file_path, false))
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.IO;");
                sw.WriteLine("using LitJson;");
                sw.WriteLine("namespace {0}", this.NameSpace);
                sw.WriteLine("{");
                sw.WriteLine("\tpublic partial class TableData");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tpublic static bool isLoad_{0} {{ get {{ return m_isLoad_{0};}} set {{ m_isLoad_{0} = value; }} }}", this.FileName);
                sw.WriteLine("\t\tprivate static bool m_isLoad_{0} = false;", this.FileName);
            }

            using (XmlReader reader1 = new XmlReader())
            {
                reader1.RegisterCallback_EveryTable(Callback_LoadSheet);
                reader1.ReadData(_data);
            }

            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                // UnLoad
                sw.WriteLine("\t\tpublic static void UnLoad_{0}()", this.FileName);
                sw.WriteLine("\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t{0}.LIST.Clear();", info.container_name);
                }
                sw.WriteLine("\t\t}");

                // Load XML
                sw.WriteLine("\t\tpublic static bool Load_{0}_XML(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (XmlReader reader = new XmlReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback_Line(\"{0}\", Callback_{0}_XML);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadFile(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Load JSON
                sw.WriteLine("\t\tpublic static bool Load_{0}_JSON(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (JsonReader reader = new JsonReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback(\"{0}\", Callback_{0}_JSON);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadFile(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");


                // Save XML
                sw.WriteLine("\t\tpublic static void Save_{0}_XML(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (XmlWriter writer = new XmlWriter())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t    {0} temp = new {0}();", info.class_name);
                    sw.WriteLine("\t\t\t\t    PropTable tb_header = temp.ToTable();");
                    sw.WriteLine("\t\t\t\t    System.Xml.XmlNode node = writer.Write_Header(tb_header, {0}.LIST.Count, {1});", info.container_name, info.is_enum.ToString().ToLower());
                    sw.WriteLine("\t\t\t\t    foreach ({0} obj in {0}.LIST.ToArray())", info.container_name);
                    sw.WriteLine("\t\t\t\t    {");
                    sw.WriteLine("\t\t\t\t        PropTable tb = obj.ToTable();");
                    sw.WriteLine("\t\t\t\t        writer.Write_Contents(node, tb);");
                    sw.WriteLine("\t\t\t\t    }");
                    sw.WriteLine("\t\t\t\t}");
                }
                sw.WriteLine("\t\t\t    writer.Write_End(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Save JSON
                sw.WriteLine("\t\tpublic static void Save_{0}_JSON(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tTextWriter sw = new StreamWriter(file_path, false);");
                sw.WriteLine("\t\t\tsw.WriteLine(\"{\");");
                for (int i = 0; i < m_ClassList.Count; i++)
                {
                    ClassInfo info = m_ClassList[i];
                    if (i == 0)
                        sw.WriteLine("\t\t\tsw.WriteLine(\"\\\"{0}\\\":[\");", info.enum_name);
                    else
                        sw.WriteLine("\t\t\tsw.WriteLine(\",\\\"{0}\\\":[\");", info.enum_name);
                    sw.WriteLine("\t\t\tfor (int i = 0; i < {0}.LIST.Count; i++)", info.container_name);
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t    if (i > 0) sw.WriteLine(\",\");");
                    sw.WriteLine("\t\t\t    sw.Write({0}.LIST.ElementAt(i).ToJson());", info.container_name);
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tsw.WriteLine(\"]\");");
                }
                sw.WriteLine("\t\t\tsw.WriteLine(\"}\");");
                sw.WriteLine("\t\t\tsw.Close();");
                sw.WriteLine("\t\t}");
            }

            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                //sw.WriteLine("\t\t}");
                sw.WriteLine("\t}");
                sw.WriteLine("} // end of namespace");
            }
        }

        void Callback_LoadSheet(string sheet_name, PropTable tb)
        {
            string class_name = sheet_name;
            string enum_name = sheet_name;
            string container_name = "T_" + sheet_name;
            bool is_enum = false;
            if (sheet_name.StartsWith("!"))
            {
                is_enum = true;
                enum_name = sheet_name.Substring(1);
                class_name = "_" + enum_name;
                container_name = "T_" + enum_name;
            }
            
            string item_var_name = tb.KeyVarName;
            string item_type_name = tb.KeyTypeName;
            int item_key_index = tb.KeyIndex;

            if (item_key_index >= 0 && m_ClassNames.Contains(class_name) == false)
            {
                m_ClassNames.Add(class_name);
                ClassInfo info = new ClassInfo();
                info.class_name = class_name;
                info.enum_name = enum_name;
                info.container_name = container_name;
                info.is_enum = is_enum;
                info.key_type = item_type_name;
                info.key_name = item_var_name;
                m_ClassList.Add(info);
            }

            if (item_key_index < 0 || item_var_name == "" || item_type_name == "")
            {
                return;
            }

            string file_path = this.OutDir + "\\TableData_" + this.FileName + ".cs";
            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                sw.WriteLine("\t\tstatic void Callback_{0}_XML(string sheet_name, PropTable tb)", enum_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\t m_isLoad_{0} = true;", this.FileName);
                switch (tb.GetVarType(item_key_index))
                {
                    case VAR_TYPE.BOOL:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToBoolean(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT16:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToInt16(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT32:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToInt32(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.HOST_ID:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToInt64(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.FLOAT:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToFlt(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.STRING:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(tb.ToStr(\"{1}\")))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(_{1}.Parse(tb.ToStr(\"{2}\"))))", container_name, item_type_name, item_var_name);
                        break;
                    default:
                        // error
                        break;
                }
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tobj.Initialize(tb);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");


                sw.WriteLine("\t\tstatic void Callback_{0}_JSON(string sheet_name, JsonData node)", enum_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tif (node.Keys.Contains(\"unit_type\") == false) return;");
                sw.WriteLine("\t\t\tm_isLoad_{0} = true;", this.FileName);
                switch (tb.GetVarType(item_key_index))
                {
                    case VAR_TYPE.BOOL:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc((bool)node[\"{1}\"]))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT16:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc((short)node[\"{1}\"]))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT32:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc((int)node[\"{1}\"]))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.HOST_ID:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc((long)node[\"{1}\"]))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.FLOAT:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc((float)node[\"{1}\"]))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.STRING:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(node[\"{1}\"].ToString()))", container_name, item_var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        sw.WriteLine("\t\t\tusing({0} obj = {0}.LIST.Alloc(_{1}.Parse(node[\"{2}\"].ToString())))", container_name, item_type_name, item_var_name);
                        break;
                    default:
                        // error
                        break;
                }
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tobj.Initialize(node);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

            }
        }


        void Callback_DataSheet(string sheet_name, PropTable tb)
        {
            string class_name = sheet_name;
            if (sheet_name.StartsWith("!"))
                class_name = sheet_name.Substring(1);

            string item_var_name = tb.KeyTypeName;
            string item_type_name = tb.KeyTypeName;
            int item_key_index = tb.KeyIndex;
            if (item_key_index < 0 || item_var_name == "" || item_type_name == "")
            {
                return;
            }

            string file_path = this.OutDir + "\\TableData_" + this.FileName + ".cs";
            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                if (tb.GetVarType(item_key_index) == VAR_TYPE.STRING)
                {
                    sw.WriteLine("\t\t\tusing(T_{0} obj = T_{0}.LIST.Alloc(\"{1}\"))", class_name, tb.ToStr(item_key_index));
                }
                else if (tb.GetVarType(item_key_index) == VAR_TYPE.ENUM)
                {
                    int temp;
                    if (int.TryParse(tb.ToStr(item_key_index), out temp))
                        sw.WriteLine("\t\t\tusing(T_{0} obj = T_{0}.LIST.Alloc(({1}){2}))", class_name, item_type_name, tb.ToStr(item_key_index));
                    else
                        sw.WriteLine("\t\t\tusing(T_{0} obj = T_{0}.LIST.Alloc({1}.{2}))", class_name, item_type_name, tb.ToStr(item_key_index));
                }
                else
                {
                    sw.WriteLine("\t\t\tusing(T_{0} obj = T_{0}.LIST.Alloc(({1}){2}))", class_name, item_type_name, tb.ToStr(item_key_index));
                }
                sw.WriteLine("\t\t\t{");
                for (int i = 0; i < tb.Length; i++)
                {
                    string var_name = tb.GetVarName(i);
                    string type_name = tb.GetTypeName(i);

                    if (type_name.Length == 0 || var_name.Length == 0)
                        continue;
                    if (var_name.Contains('/') == false)
                    {
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.BOOL:
                                {
                                    int val = tb.ToInt32(i);
                                    if(val != 0)
                                        sw.WriteLine("\t\t\t\tobj.{0} = true;", var_name);
                                    else
                                        sw.WriteLine("\t\t\t\tobj.{0} = false;", var_name);
                                }
                                break;
                            case VAR_TYPE.INT16:
                            case VAR_TYPE.INT32:
                            case VAR_TYPE.INT64:
                            case VAR_TYPE.HOST_ID:
                                {
                                    string temp = tb.ToStr(i);
                                    if (temp.Length == 0)
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = ({1}){2};", var_name, type_name, 0);
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = {1};", var_name, temp);
                                    }
                                }
                                break;
                            case VAR_TYPE.FLOAT:
                                {
                                    string temp = tb.ToStr(i);
                                    if (temp.Length == 0)
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = ({1}){2}f;", var_name, type_name, 0);
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = {1}f;", var_name, temp);
                                    }
                                }
                                break;
                            case VAR_TYPE.STRING:
                                sw.WriteLine("\t\t\t\tobj.{0} = \"{1}\";", var_name, tb.ToStr(i).Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n\r", "\\n").Replace("\n", "\\n").Replace("\r", "\\n"));
                                break;

                            case VAR_TYPE.ENUM:
                                {
                                    string temp = tb.ToStr(i);
                                    int number = 0;
                                    if (temp.Length == 0)
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = ({1}){2};", var_name, type_name, 0);
                                    }
                                    else if (Int32.TryParse(temp, out number))
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = ({1}){2};", var_name, type_name, temp);
                                    }
                                    else
                                    {
                                        sw.WriteLine("\t\t\t\tobj.{0} = {1}.{2};", var_name, type_name, temp);
                                    }
                                }
                                break;
                            case VAR_TYPE.CLASS:
                                MakeDataCode(sw, tb.ToTable(var_name), var_name);
                                break;
                            default:
                                break;
                        }
                    }
                }
                sw.WriteLine("\t\t\t}");
            } // close
        }

        void MakeDataCode(TextWriter sw, PropTable tb, string pre_fix)
        {
            for (int i = 0; i < tb.Length; i++)
            {
                string var_name = tb.GetVarName(i);
                string type_name = tb.GetTypeName(i);

                if (type_name.Length == 0 || var_name.Length == 0)
                    continue;
                if (var_name.Contains('/') == false)
                {
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            {
                                bool temp_value;
                                if (bool.TryParse(tb.ToStr(i), out temp_value))
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2};", pre_fix.Replace('/', '.'), var_name, temp_value);
                                else
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = false;", pre_fix.Replace('/', '.'), var_name);
                            }
                            break;
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.HOST_ID:
                            {
                                string temp = tb.ToStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2};", pre_fix.Replace('/', '.'), var_name, tb.ToStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.FLOAT:
                            {
                                string temp = tb.ToStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3}f;", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2}f;", pre_fix.Replace('/', '.'), var_name, tb.ToStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.STRING:
                            sw.WriteLine("\t\t\t\tobj.{0}.{1} = \"{2}\";", pre_fix.Replace('/', '.'), var_name, tb.ToStr(i).Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n\r", "\\n").Replace("\n", "\\n").Replace("\r", "\\n"));
                            break;

                        case VAR_TYPE.ENUM:
                            {
                                string temp = tb.ToStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    int temp_value;
                                    if (int.TryParse(tb.ToStr(i), out temp_value))
                                        sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, temp_value);
                                    else
                                        sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2}.{3};", pre_fix.Replace('/', '.'), var_name, type_name, tb.ToStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.CLASS:
                            MakeDataCode(sw, tb.ToTable(var_name), pre_fix + "/" + var_name);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    } // end of class
}
