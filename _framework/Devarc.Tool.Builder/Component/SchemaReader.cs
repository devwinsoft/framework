using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Devarc;

namespace Devarc
{
    public class SchemaReader : BaseDataReader
    {
        public override bool ReadFile(string _filePath)
        {
            if (System.IO.File.Exists(_filePath) == false)
            {
                Log.Debug("Cannot find file: " + _filePath);
                return false;
            }

            string data = File.ReadAllText(_filePath);
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }

            return ReadData(data);
        }

        public override bool ReadData(string _data)
        {
            try
            {
                var cp = new System.CodeDom.Compiler.CompilerParameters();
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;

                CSharpCodeProvider provider = new CSharpCodeProvider();
                var cr = provider.CompileAssemblyFromSource(cp, _data);

                Type[] types = cr.CompiledAssembly.GetTypes();
                foreach (Type tp in types)
                {
                    PropTable prop = new PropTable(tp.Name);

                    FieldInfo[] _fields = tp.GetFields();
                    if (tp.IsEnum == false)
                    {
                        int i = 0;
                        foreach (FieldInfo finfo in _fields)
                        {
                            bool isArray = finfo.FieldType.Name.EndsWith("[]");
                            bool isClass = IsClass(finfo.FieldType);
                            //var attributes = finfo.GetCustomAttributes();
                            string varTypeName = ToTypeName(finfo.FieldType.Name, isClass);
                            CLASS_TYPE classType = CLASS_TYPE.VALUE;
                            if (isArray)
                            {
                                if (isClass)
                                    classType = CLASS_TYPE.CLASS_LIST;
                                else
                                    classType = CLASS_TYPE.VALUE_LIST;
                            }
                            else
                            {
                                if (isClass)
                                    classType = CLASS_TYPE.CLASS;
                                else
                                    classType = CLASS_TYPE.VALUE;
                            }
                            prop.Attach(finfo.Name, varTypeName, classType, KEY_TYPE.NONE, "");
                            i++;
                        }
                    }

                    if (tp.IsEnum)
                        m_SheetName = "!" + tp.Name;
                    else
                        m_SheetName = tp.Name;

                    if (callback_header != null)
                    {
                        if (tp.IsEnum)
                        {
                            prop.Attach("NAME", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, "");
                            prop.Attach("ID", tp.Name, CLASS_TYPE.VALUE, KEY_TYPE.MAP, "");
                        }
                        callback_header(m_SheetName, prop);
                    }

                    if (tp.IsEnum)
                    {
                        string[] enumNames = Enum.GetNames(tp);
                        for (int i = 0; i < enumNames.Length; i++)
                        {
                            PropTable temp = new PropTable(tp.Name);
                            object enumValue = Enum.Parse(tp, enumNames[i]);
                            temp.Attach("NAME", "string", CLASS_TYPE.VALUE, KEY_TYPE.NONE, enumNames[i]);
                            temp.Attach("ID", tp.Name, CLASS_TYPE.VALUE, KEY_TYPE.MAP, ((int)enumValue).ToString());
                            invoke_callback_data(m_SheetName, temp);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        bool IsClass(Type _type)
        {
            string temp = _type.Name.ToLower();
            return _type.IsClass && temp.StartsWith("string") == false;
        }

        string ToTypeName(string _rawName, bool _isClass)
        {
            if (_isClass)
            {
                return _rawName;
            }

            string temp;
            if (_rawName.EndsWith("[]"))
                temp = _rawName.Substring(0, _rawName.Length - 2).ToLower();
            else
                temp = _rawName.ToLower();
            switch(temp)
            {
                case "single":
                    return "float";
                default:
                    return temp;
            }
        }

    }
}
