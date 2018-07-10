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
using System.Text;

namespace Devarc
{
    public class Builder_Data : Builder_Base
    {
        string FileName = "";
        string OutFilePath = "";
        HashSet<string> m_ClassNames = new HashSet<string>();
        List<ClassInfo> m_ClassList = new List<ClassInfo>();

        public void Build_ExcelFile(string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.EXCEL;
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutFilePath = Path.Combine(_outDir, "Table_" + this.FileName + ".cs");

            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }

            _build(_inFilePath);
        }

        public void Build_SheetFile(string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.SHEET;
            this.FileName = GetClassNameEx(_inFilePath);
            this.OutFilePath = Path.Combine(_outDir, "Table_" + this.FileName + ".cs");

            if (Directory.Exists(_outDir) == false)
            {
                if (Directory.CreateDirectory(_outDir) == null)
                {
                    Log.Info("Cannot find directory: " + _outDir);
                    return;
                }
            }
            if (File.Exists(_inFilePath) == false)
            {
                Log.Info("Cannot find file: " + _inFilePath);
                return;
            }

            _build(_inFilePath);
        }


        public void Build_SheetData(string _file_name, string _inFilePath, string _outDir)
        {
            dataFileType = SCHEMA_TYPE.SHEET;

            string tmpFileName = Path.GetFileNameWithoutExtension(_file_name);
            int tmpIndex = tmpFileName.IndexOf('@');
            if (tmpIndex >= 0)
                this.FileName = tmpFileName.Substring(0, tmpIndex);
            else
                this.FileName = tmpFileName;

            string OutDir = _outDir;
            this.OutFilePath = Path.Combine(OutDir, "Table_" + this.FileName + ".cs");
            if (Directory.Exists(OutDir) == false)
            {
                if (Directory.CreateDirectory(OutDir) == null)
                {
                    Log.Info("Cannot find directory: " + OutDir);
                    return;
                }
            }
            this._build(_inFilePath);
        }

        void _build(string _inFilePath)
        {
            m_ClassNames.Clear();
            m_ClassList.Clear();

            using (TextWriter sw = new StreamWriter(this.OutFilePath, false))
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.IO;");
                sw.WriteLine("using LitJson;");
                sw.WriteLine("namespace {0}", this.NameSpace);
                sw.WriteLine("{");
                sw.WriteLine("\tpublic partial class Table");
                sw.WriteLine("\t{");
            }

            using (BaseSchemaReader reader1 = _createReader())
            {
                reader1.RegisterCallback_Table(Callback_LoadSheet);
                reader1.ReadFile(_inFilePath);
            }

            using (TextWriter sw = new StreamWriter(this.OutFilePath, true))
            {
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t    public static Container<{0}, {1}> {2} = new Container<{0}, {1}>();", info.class_name, info.key_type, info.container_name);
                }

                sw.WriteLine("\t\tpublic static bool isLoad_{0}", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tget");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\tif (Table.{0}.Count > 0) return true;", info.container_name);
                }
                sw.WriteLine("\t\t\t\treturn false;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // UnLoad
                sw.WriteLine("\t\tpublic static void UnLoad_{0}()", this.FileName);
                sw.WriteLine("\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\tTable.{0}.Clear();", info.container_name);
                }
                sw.WriteLine("\t\t}");

                // Load EXCEL
                sw.WriteLine("\t\tpublic static bool Load_{0}_ExcelFile(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (ExcelReader reader = new ExcelReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback_Data(\"{0}\", Callback_{0}_Sheet);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadFile(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Save EXCEL
                sw.WriteLine("\t\tpublic static void Save_{0}_ExcelFile(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (ExcelWriter writer = new ExcelWriter())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t    {0} temp = new {0}();", info.class_name);
                    sw.WriteLine("\t\t\t\t    PropTable tb_header = temp.ToTable();");
                    sw.WriteLine("\t\t\t\t    writer.Write_Header(tb_header, {0});", info.is_enum.ToString().ToLower());
                    sw.WriteLine("\t\t\t\t    for (int i = 0; i < Table.{0}.Count; i++)", info.container_name);
                    sw.WriteLine("\t\t\t\t    {");
                    sw.WriteLine("\t\t\t\t        {0} obj = Table.{1}.ElementAt(i);", info.class_name, info.container_name);
                    sw.WriteLine("\t\t\t\t        PropTable tb = obj.ToTable();");
                    sw.WriteLine("\t\t\t\t        writer.Write_Contents(tb);");
                    sw.WriteLine("\t\t\t\t    }");
                    sw.WriteLine("\t\t\t\t}");
                }
                sw.WriteLine("\t\t\t    writer.Write_End(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Load XML
                sw.WriteLine("\t\tpublic static bool Load_{0}_SheetFile(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (XmlSheetReader reader = new XmlSheetReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback_Data(\"{0}\", Callback_{0}_Sheet);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadFile(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Load XML
                sw.WriteLine("\t\tpublic static bool Load_{0}_SheetData(string _data)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (XmlSheetReader reader = new XmlSheetReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback_Data(\"{0}\", Callback_{0}_Sheet);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadData(_data);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Load JSON
                sw.WriteLine("\t\tpublic static bool Load_{0}_JsonFile(string file_path)", this.FileName);
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

                // Load JSON DATA
                sw.WriteLine("\t\tpublic static bool Load_{0}_JsonData(string _data)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (JsonReader reader = new JsonReader())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\treader.RegisterCallback(\"{0}\", Callback_{0}_JSON);", info.enum_name);
                }
                sw.WriteLine("\t\t\t\treturn reader.ReadData(_data);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");


                // Save XML
                sw.WriteLine("\t\tpublic static void Save_{0}_SheetFile(string file_path)", this.FileName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tusing (XmlSheetWriter writer = new XmlSheetWriter())");
                sw.WriteLine("\t\t\t{");
                foreach (ClassInfo info in m_ClassList)
                {
                    sw.WriteLine("\t\t\t\t{");
                    sw.WriteLine("\t\t\t\t    {0} temp = new {0}();", info.class_name);
                    sw.WriteLine("\t\t\t\t    PropTable tb_header = temp.ToTable();");
                    sw.WriteLine("\t\t\t\t    System.Xml.XmlNode node = writer.Write_Header(tb_header, Table.{0}.Count, {1});", info.container_name, info.is_enum.ToString().ToLower());
                    sw.WriteLine("\t\t\t\t    for (int i = 0; i < Table.{0}.Count; i++)", info.container_name);
                    sw.WriteLine("\t\t\t\t    {");
                    sw.WriteLine("\t\t\t\t        {0} obj = Table.{1}.ElementAt(i);", info.class_name, info.container_name);
                    sw.WriteLine("\t\t\t\t        PropTable tb = obj.ToTable();");
                    sw.WriteLine("\t\t\t\t        writer.Write_Contents(node, tb);");
                    sw.WriteLine("\t\t\t\t    }");
                    sw.WriteLine("\t\t\t\t}");
                }
                sw.WriteLine("\t\t\t    writer.Write_End(file_path);");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t}");

                // Save JSON
                sw.WriteLine("\t\tpublic static void Save_{0}_JsonFile(string file_path)", this.FileName);
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
                    sw.WriteLine("\t\t\tfor (int i = 0; i < Table.{0}.Count; i++)", info.container_name);
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t    if (i > 0) sw.WriteLine(\",\");");
                    sw.WriteLine("\t\t\t    sw.Write(Table.{0}.ElementAt(i).ToJson());", info.container_name);
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t\tsw.WriteLine(\"]\");");
                }
                sw.WriteLine("\t\t\tsw.WriteLine(\"}\");");
                sw.WriteLine("\t\t\tsw.Close();");
                sw.WriteLine("\t\t}");
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
            
            string key_var_name = tb.KeyVarName;
            string key_type_name = tb.KeyTypeName;
            int key_index = tb.KeyIndex;

            if (key_index >= 0 && m_ClassNames.Contains(class_name) == false)
            {
                m_ClassNames.Add(class_name);
                ClassInfo info = new ClassInfo();
                info.class_name = class_name;
                info.enum_name = enum_name;
                info.container_name = container_name;
                info.is_enum = is_enum;
                info.key_type = key_type_name;
                info.key_name = key_var_name;
                m_ClassList.Add(info);
            }

            if (key_index < 0 || key_var_name == "" || key_type_name == "")
            {
                return;
            }

            using (TextWriter sw = new StreamWriter(this.OutFilePath, true))
            {
                sw.WriteLine("\t\tstatic void Callback_{0}_Sheet(string sheet_name, PropTable tb)", enum_name);
                sw.WriteLine("\t\t{");
                switch (tb.GetVarType(key_index))
                {
                    case VAR_TYPE.BOOL:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.ToBoolean(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT16:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.GetInt16(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT32:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.GetInt32(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.UINT32:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.GetUInt32(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.HOST_ID:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.ToInt64(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.FLOAT:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.GetFloat(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.STRING:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(tb.GetStr(\"{2}\"));", class_name, container_name, key_var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc(FrameworkUtil.Parse<{2}>(tb.GetStr(\"{3}\")));", class_name, container_name, key_type_name, key_var_name);
                        break;                    
                    default:
                        // error
                        break;
                }
                sw.WriteLine("\t\t\tif (obj == null)");
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tLog.Error(\"[Table]Cannot create '{0}'. (id={{0}})\", tb.GetStr(\"{1}\"));", enum_name, key_var_name);
                sw.WriteLine("\t\t\t\treturn;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t\tobj.Initialize(tb);");
                sw.WriteLine("\t\t}");

                sw.WriteLine("\t\tstatic void Callback_{0}_JSON(string sheet_name, JsonData node)", enum_name);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tif (node.Keys.Contains(\"{0}\") == false) return;", key_var_name);
                string keyString;
                switch (tb.GetVarType(key_index))
                {
                    case VAR_TYPE.BOOL:
                        keyString = string.Format("(bool)node[\"{1}\"]", container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT16:
                        keyString = string.Format("(short)node[\"{1}\"]", container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT32:
                        keyString = string.Format("(int)node[\"{1}\"]", container_name, key_var_name);
                        break;
                    case VAR_TYPE.UINT32:
                        keyString = string.Format("(uint)node[\"{1}\"]", container_name, key_var_name);
                        break;
                    case VAR_TYPE.INT64:
                    case VAR_TYPE.HOST_ID:
                        keyString = string.Format("(long)node[\"{1}\"])", container_name, key_var_name);
                        break;
                    case VAR_TYPE.FLOAT:
                        keyString = string.Format("(float)node[\"{1}\"]", container_name, key_var_name);
                        break;
                    case VAR_TYPE.STRING:
                        keyString = string.Format("node[\"{1}\"].ToString()", container_name, key_var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        keyString = string.Format("FrameworkUtil.Parse<{1}>(node[\"{2}\"].ToString())", container_name, key_type_name, key_var_name);
                        break;
                    default:
                        // error
                        keyString = "0";
                        break;
                }
                sw.WriteLine("\t\t\t{0} obj = Table.{1}.Alloc({2});", class_name, container_name, keyString);
                sw.WriteLine("\t\t\tif (obj == null)");
                sw.WriteLine("\t\t\t{");
                sw.WriteLine("\t\t\t\tLog.Error(\"[Table]Cannot create '{0}'. (id={{0}})\", {1});", enum_name, keyString);
                sw.WriteLine("\t\t\t\treturn;");
                sw.WriteLine("\t\t\t}");
                sw.WriteLine("\t\t\tobj.Initialize(node);");
                sw.WriteLine("\t\t}");

            }
        }


        void MakeDataCode(TextWriter sw, PropTable tb, string pre_fix)
        {
            for (int i = 0; i < tb.Length; i++)
            {
                string var_name = tb.GetVarName(i);
                string type_name = tb.GetTypeName(i);

                if (type_name.Length == 0 || var_name.Length == 0)
                    continue;
                if (var_name.IndexOf('/') < 0)
                {
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                            {
                                bool temp_value;
                                if (bool.TryParse(tb.GetStr(i), out temp_value))
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2};", pre_fix.Replace('/', '.'), var_name, temp_value);
                                else
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = false;", pre_fix.Replace('/', '.'), var_name);
                            }
                            break;
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.UINT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.HOST_ID:
                            {
                                string temp = tb.GetStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2};", pre_fix.Replace('/', '.'), var_name, tb.GetStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.FLOAT:
                            {
                                string temp = tb.GetStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3}f;", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2}f;", pre_fix.Replace('/', '.'), var_name, tb.GetStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.STRING:
                            sw.WriteLine("\t\t\t\tobj.{0}.{1} = \"{2}\";", pre_fix.Replace('/', '.'), var_name, tb.GetStr(i).Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n\r", "\\n").Replace("\n", "\\n").Replace("\r", "\\n"));
                            break;

                        case VAR_TYPE.ENUM:
                            {
                                string temp = tb.GetStr(i);
                                if (temp.Length == 0)
                                {
                                    sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, 0);
                                }
                                else
                                {
                                    int temp_value;
                                    if (int.TryParse(tb.GetStr(i), out temp_value))
                                        sw.WriteLine("\t\t\t\tobj.{0}.{1} = ({2}){3};", pre_fix.Replace('/', '.'), var_name, type_name, temp_value);
                                    else
                                        sw.WriteLine("\t\t\t\tobj.{0}.{1} = {2}.{3};", pre_fix.Replace('/', '.'), var_name, type_name, tb.GetStr(i));
                                }
                            }
                            break;
                        case VAR_TYPE.CLASS:
                            MakeDataCode(sw, tb.GetTable(var_name), pre_fix + "/" + var_name);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    } // end of class
}
