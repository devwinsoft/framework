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
            using (BaseSchemaReader reader = _createReader())
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
            using (BaseSchemaReader reader = _createReader())
            {
                reader.RegisterCallback_Data(Callback_Pass2_Enum);
                reader.ReadFile(_inFilePath);
                System.Threading.Thread.Sleep(0);
            }
            using (BaseSchemaReader reader = _createReader())
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
                } // end of foreach
                sw.WriteLine("} // end of namespace");
            }
        }

        void Callback_Pass2_Class(string sheet_name, PropTable tb)
        {
            string file_path = Path.Combine(this.OutDir, "Class_" + this.FileName + ".cs");
            using (TextWriter sw = new StreamWriter(file_path, true))
            {
                Builder_Util.Make_Class_Code(tb, sw);
                Builder_Util.Make_Marshal_Code(tb, sw);
            }
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
