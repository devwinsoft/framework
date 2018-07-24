using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Devarc;

namespace Devarc
{
    static class Builder_Util
    {
        static string ToTypeName(string _rawName, bool _isClass)
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
            switch (temp)
            {
                case "single":
                    return "float";
                default:
                    return _rawName;
            }
        }

        static bool IsClass(Type _type)
        {
            string temp = _type.Name.ToLower();
            return _type.IsClass && temp.StartsWith("string") == false && temp.EndsWith("[]") == false;
        }

        static string GetClassName(string _path)
        {
            int startIndex = _path[0] == '!' ? 1 : 0;
            int endIndex = _path.IndexOf('@');
            if (endIndex >= 0)
                return _path.Substring(startIndex, endIndex - startIndex);
            else
                return _path.Substring(startIndex, _path.Length - startIndex);
        }

        static string GetClassNameEx(string _path)
        {
            string value = System.IO.Path.GetFileNameWithoutExtension(_path);
            int startIndex = value[0] == '!' ? 1 : 0;
            int endIndex = value.IndexOf('@');
            if (endIndex >= 0)
                return value.Substring(startIndex, endIndex - startIndex);
            else
                return value.Substring(startIndex, value.Length - startIndex);
        }

        public static PropTable ToTable(Type tp)
        {
            PropTable prop = new PropTable(tp.Name);

            FieldInfo[] _fields = tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //FieldInfo[] _fields = tp.GetFields();
            if (tp.IsEnum == false)
            {
                int i = 0;
                foreach (FieldInfo finfo in _fields)
                {
                    bool isList = finfo.FieldType.FullName.StartsWith("System.Collections.Generic.List");
                    bool isClass = IsClass(finfo.FieldType);
                    string varTypeName = ToTypeName(finfo.FieldType.Name, isClass);
                    CLASS_TYPE classType = CLASS_TYPE.VALUE;
                    if (isList)
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
                    prop.Attach(finfo.Name, varTypeName, classType, false, "");
                    i++;
                }
            }
            else
            {
                prop.IsEnum = true;
                prop.Attach("NAME", "string", CLASS_TYPE.VALUE, false, "");
                prop.Attach("ID", tp.Name, CLASS_TYPE.VALUE, true, "");
            }

            return prop;
        }

        public static void Make_Class_Code(PropTable tb, TextWriter sw, short rmi_id = 0)
        {
            string class_name = tb.IsEnum ? "_" + tb.TableName : tb.TableName;

            sw.WriteLine("\t[System.Serializable]");
            if (rmi_id > 0)
            {
                sw.WriteLine("\t[NetProtocolAttribute(RMI_ID = {0})]", rmi_id);
            }
            if (tb.KeyIndex >= 0)
                sw.WriteLine("\tpublic partial class {0} : IBaseObejct<{1}>", class_name, tb.KeyTypeName);
            else
                sw.WriteLine("\tpublic class {0} : IBaseObejct", class_name);
            sw.WriteLine("\t{");
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
                    case VAR_TYPE.ARRAY:
                        sw.WriteLine("\t\tpublic {0,-20}{1} = null;", "byte[]", var_name);
                        break;
                    case VAR_TYPE.BOOL:
                        if (is_list)
                            sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "bool", var_name);
                        else
                            sw.WriteLine("\t\tpublic {0,-20}{1};", "bool", var_name);
                        break;
                    case VAR_TYPE.BYTE:
                        if (is_list)
                            sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "byte", var_name);
                        else
                            sw.WriteLine("\t\tpublic {0,-20}{1};", "byte", var_name);
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
                    case VAR_TYPE.FLOAT:
                        if (is_list)
                            sw.WriteLine("\t\tpublic List<{0}> {1} = new List<{0}>();", "float", var_name);
                        else
                            sw.WriteLine("\t\tpublic {0,-20}{1};", "float", var_name);
                        break;
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
                sw.WriteLine("\t\tpublic string GetQuery_Select({0} _key)", tb.KeyTypeName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\treturn string.Format(\"select * from `{2}` where {1}='{{0}}';\", _key);", tb.KeyTypeName, tb.KeyVarName, tb.TableName);
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t\tpublic string GetQuery_SelectWhere(string _where)", tb.KeyTypeName);
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\treturn string.Format(\"select * from `{1}` where {{0}};\", _where);", tb.KeyTypeName, tb.TableName);
                sw.WriteLine("\t\t}");
                sw.WriteLine("\t\tpublic string GetQuery_InsertOrUpdate()");
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\tPropTable obj = ToTable();");
                sw.Write("\t\t\treturn string.Format(\"insert into {0} (", class_name);
                for (int i = 0; i < tb.Length; i++)
                {
                    if (i == 0)
                        sw.Write("`{0}`", tb.GetVarName(i));
                    else
                        sw.Write(", `{0}`", tb.GetVarName(i));
                }
                sw.Write(") VALUES (");
                for (int i = 0; i < tb.Length; i++)
                {
                    if (i == 0)
                        sw.Write("'{{{0}}}'", i);
                    else
                        sw.Write(", '{{{0}}}'", i);
                }
                sw.Write(") on duplicate key update ");
                for (int i = 0; i < tb.Length; i++)
                {
                    if (i > 0)
                        sw.Write(", ");
                    sw.Write("`{0}`='{{{1}}}'", tb.GetVarName(i), i);
                }
                sw.Write(";\"");
                for (int i = 0; i < tb.Length; i++)
                {
                    sw.Write(", ");
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.BOOL:
                        case VAR_TYPE.ENUM:
                        case VAR_TYPE.FLOAT:
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.UINT32:
                            sw.Write("obj.GetStr(\"{0}\")", tb.GetVarName(i));
                            break;
                        default:
                            sw.Write("FrameworkUtil.InnerString(obj.GetStr(\"{0}\"))", tb.GetVarName(i));
                            break;
                    }
                }
                sw.WriteLine(");");
                sw.WriteLine("\t\t}");
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
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.LSTRING:
                            case VAR_TYPE.FSTRING:
                                break;
                            case VAR_TYPE.ARRAY:
                                sw.WriteLine("\t\t\t\tif ({0} != null && {0}.Length > 0) return false;", var_name);
                                break;
                            case VAR_TYPE.BOOL:
                                sw.WriteLine("\t\t\t\tif ({0}) return false;", var_name);
                                break;
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
                        switch (tb.GetVarType(i))
                        {
                            case VAR_TYPE.ARRAY:
                                sw.WriteLine("\t\t\t{0} = new byte[obj.{0}.Length];", var_name);
                                sw.WriteLine("\t\t\tArray.Copy(obj.{0}, {0}, {0}.Length);", var_name);
                                break;
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
                    case VAR_TYPE.ARRAY:
                        sw.WriteLine("\t\t\t{0,-20}= obj.GetBytes(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.BYTE:
                        if (is_list)
                            sw.WriteLine("\t\t\tobj.GetList<byte>(\"{0}\", {0});", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetByte(\"{0}\");", var_name);
                        break;
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
                    case VAR_TYPE.FLOAT:
                        if (is_list)
                            sw.WriteLine("\t\t\tobj.GetList<float>(\"{0}\", {0});", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetFloat(\"{0}\");", var_name);
                        break;
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
                            sw.WriteLine("\t\t\tif (__{0} != null && __{0}.IsArray) {{ foreach (var node in __{0} as IList) {{ {0}.Add(FrameworkUtil.Parse<{1}>(node.ToString())); }} }}", var_name, type_name);
                        }
                        
                        else
                        {
                            sw.WriteLine("\t\t\t{0,-20}= FrameworkUtil.Parse<{1}>(obj.GetStr(\"{0}\"));", var_name, type_name);
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
                    case VAR_TYPE.ARRAY:
                        sw.WriteLine("\t\t\t{0} = Convert.FromBase64String(obj[\"{0}\"].ToString());", var_name);
                        break;
                    case VAR_TYPE.BYTE:
                        if (is_list)
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToByte(node.ToString())); }}", var_name);
                        else
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) bool.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(bool);", var_name);
                        break;
                    case VAR_TYPE.BOOL:
                        if (is_list)
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(Convert.ToBoolean(node.ToString())); }}", var_name);
                        else
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) bool.TryParse(obj[\"{0}\"].ToString(), out {0}); else {0} = default(bool);", var_name);
                        break;
                    case VAR_TYPE.INT16:
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
                    case VAR_TYPE.STRING:
                        if (is_list)
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(node.ToString()); }}", var_name);
                        else
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0} = obj[\"{0}\"].ToString(); else {0} = default(string);", var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        if (is_list)
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) foreach (JsonData node in obj[\"{0}\"]) {{ {0}.Add(FrameworkUtil.Parse<{1}>(node.ToString())); }}", var_name, type_name);
                        else
                            sw.WriteLine("\t\t\tif (obj.Keys.Contains(\"{0}\")) {0} = FrameworkUtil.Parse<{1}>(obj[\"{0}\"].ToString()); else {0} = default({1});", var_name, type_name);
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




            sw.WriteLine("\t\tpublic void Initialize(IBaseReader obj)", class_name);
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
                    case VAR_TYPE.ARRAY:
                        sw.WriteLine("\t\t\t{0} = Convert.FromBase64String(obj.GetString(\"{0}\"));", var_name);
                        break;
                    case VAR_TYPE.BOOL:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToBoolean(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetBoolean(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.BYTE:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToBoolean(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetByte(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.INT16:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt16(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetInt16(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.INT32:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt32(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetInt32(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.UINT32:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToUInt64(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetUInt32(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.INT64:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToInt64(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetInt64(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.FLOAT:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(Convert.ToSingle(node.ToString())); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetFloat(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.STRING:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(node.ToString()); }};", var_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= obj.GetString(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.LSTRING:
                        if (tb.KeyIndex >= 0)
                            sw.WriteLine("\t\t\t_{1}.Key = FrameworkUtil.MakeLStringKey(\"{0}\", \"{1}\", {2}.ToString());", class_name, var_name, tb.KeyVarName);
                        break;
                    case VAR_TYPE.FSTRING:
                        sw.WriteLine("\t\t\t_{0}.Key = obj.GetString(\"{0}\");", var_name);
                        break;
                    case VAR_TYPE.ENUM:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) foreach (JsonData node in JsonMapper.ToObject(__{0})) {{ {0}.Add(FrameworkUtil.Parse<{1}>(node.ToString())); }};", var_name, type_name);
                        else
                            sw.WriteLine("\t\t\t{0,-20}= FrameworkUtil.Parse<{1}>(obj.GetString(\"{0}\"));", var_name, type_name);
                        break;
                    case VAR_TYPE.CLASS:
                        if (is_list)
                            sw.WriteLine("\t\t\tstring __{0} = obj.GetString(\"{0}\"); {0}.Clear(); if (!string.IsNullOrEmpty(__{0})) FrameworkUtil.FillList<{1}>(__{0}, {0});", var_name, type_name);
                        else
                            sw.WriteLine("\t\t\t{0}.Initialize(JsonMapper.ToObject(obj.GetString(\"{0}\")));", var_name);
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
                    case VAR_TYPE.ARRAY:
                        sw.Write(" sb.Append(\"\\\"\");");
                        sw.Write(" sb.Append(Convert.ToBase64String({0}));", var_name);
                        sw.WriteLine(" sb.Append(\"\\\"\");");
                        break;

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
                            sw.WriteLine(" sb.Append({0}.IsDefault == false ? {0}.ToString() : \"{{}}\");", var_name);
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
            if (tb.IsEnum == false)
            {
                sw.WriteLine("\t\t    if (IsDefault) { return \"{}\"; }");
            }
            sw.WriteLine("\t\t    StringBuilder sb = new StringBuilder();");
            sw.WriteLine("\t\t    int j = 0;");
            sw.WriteLine("\t\t\tsb.Append(\"{\");");
            for (int i = 0; i < tb.Length; i++)
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
                        case VAR_TYPE.STRING:
                            sw.WriteLine("\t\t\tif ({0}.Count > 0) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.Write(" sb.Append(\"[\");");
                            sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(string.Format(\"\\\"{{0}}\\\"\", FrameworkUtil.JsonString(_obj))); }}", var_name, type_name);
                            sw.WriteLine(" sb.Append(\"]\"); }");
                            break;
                        case VAR_TYPE.CLASS:
                            sw.WriteLine("\t\t\tif ({0}.Count > 0) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.Write(" sb.Append(\"[\");");
                            sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ if (i > 0) sb.Append(\",\"); sb.Append({0}[i].ToJson()); }}", var_name, type_name);
                            sw.WriteLine(" sb.Append(\"]\"); }");
                            break;
                        default:
                            sw.WriteLine("\t\t\tif ({0}.Count > 0) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.Write(" sb.Append(\"[\");");
                            sw.Write(" for (int i = 0; i < {0}.Count; i++) {{ {1} _obj = {0}[i]; if (i > 0) sb.Append(\",\"); sb.Append(string.Format(\"\\\"{{0}}\\\"\", _obj)); }}", var_name, type_name);
                            sw.WriteLine(" sb.Append(\"]\"); }");
                            break;
                    }
                }
                else
                {
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.ARRAY:
                            sw.WriteLine("\t\t\tif ({0} != null && {0}.Length > 0) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name, type_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.WriteLine(" sb.Append(string.Format(\"\\\"{{0}}\\\"\", Convert.ToBase64String({0}))); }}", var_name);
                            break;
                        case VAR_TYPE.LSTRING:
                            break;
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\t\tif (string.IsNullOrEmpty(_{0}.Key) == false) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name, type_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append(FrameworkUtil.JsonString(_{0}.Key)); sb.Append(\"\\\"\"); }}", var_name);
                            break;
                        case VAR_TYPE.STRING:
                            sw.WriteLine("\t\t\tif (string.IsNullOrEmpty({0}) == false) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.WriteLine(" sb.Append(\"\\\"\"); sb.Append(FrameworkUtil.JsonString({0})); sb.Append(\"\\\"\"); }}", var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            sw.WriteLine("\t\t\tif ({0}.IsDefault == false) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.WriteLine(" sb.Append({0}.ToJson()); }}", var_name);
                            break;
                        default:
                            sw.WriteLine("\t\t\tif (default({1}) != {0}) {{ if (j > 0) {{ sb.Append(\", \"); }} j++;", var_name, type_name);
                            sw.Write("\t\t\t sb.Append(\"\\\"{0}\\\":\");", var_name);
                            sw.WriteLine(" sb.Append(string.Format(\"\\\"{{0}}\\\"\", {0}.ToString())); }}", var_name);
                            break;
                    }
                }
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
                string key_type = tb.GetKeyType(i).ToString().ToLower();
                if (var_name == "" || type_name == "" || var_name.IndexOf('/') >= 0)
                {
                    continue;
                }
                if (is_list)
                {
                    sw.WriteLine("\t\t\tobj.Attach_List<{1}>(\"{0}\", \"{1}\", VAR_TYPE.{3}, {0});", var_name, type_name, tb.GetClassType(i), tb.GetVarType(i));
                }
                else
                {
                    switch (tb.GetVarType(i))
                    {
                        case VAR_TYPE.ARRAY:
                            sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, false, Convert.ToBase64String({0}));", var_name, type_name);
                            break;
                        case VAR_TYPE.BYTE:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE_LIST, {2}, {0}.ToString());", var_name, type_name, key_type);
                            else
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, {0}.ToString());", var_name, type_name, key_type);
                            break;
                        case VAR_TYPE.BOOL:
                        case VAR_TYPE.INT16:
                        case VAR_TYPE.INT32:
                        case VAR_TYPE.UINT32:
                        case VAR_TYPE.INT64:
                        case VAR_TYPE.FLOAT:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE_LIST, {2}, {0}.ToString());", var_name, type_name, key_type);
                            else
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, {0}.ToString());", var_name, type_name, key_type);
                            break;
                        case VAR_TYPE.LSTRING:
                            sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, _{0}.Value);", var_name, type_name, key_type);
                            break;
                        case VAR_TYPE.FSTRING:
                            sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, _{0}.Key);", var_name, type_name, key_type);
                            break;
                        case VAR_TYPE.STRING:
                            if (is_list)
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, {0});", var_name, type_name, key_type);
                            else
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, {0});", var_name, type_name, key_type);
                            break;
                        case VAR_TYPE.ENUM:
                            if (tb.IsEnum)
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, ((int){0}).ToString());", var_name, type_name, key_type);
                            else if (is_list)
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE_LIST, {2}, {0}.ToString());", var_name, type_name, key_type);
                            else
                                sw.WriteLine("\t\t\tobj.Attach(\"{0}\", \"{1}\", CLASS_TYPE.VALUE, {2}, {0}.ToString());", var_name, type_name, key_type);
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
        }



        public static void Make_Marshal_Code(PropTable tb, TextWriter sw)
        {
            string class_name = tb.TableName;

            if (tb.IsEnum)
            {
                sw.WriteLine("\tpublic static partial class Marshaler");
                sw.WriteLine("\t{");

                sw.WriteLine("\t    public static void Read(NetBuffer msg, ref {0} obj)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        obj = ({0})msg.ReadInt32();", class_name);
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Write(NetBuffer msg, {0} obj)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        msg.Write((Int32)obj);");
                sw.WriteLine("\t        return !msg.IsError;");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static void Read(NetBuffer msg, out {0}[] obj)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        int cnt = msg.ReadInt16();");
                sw.WriteLine("\t        obj = new {0}[cnt];", class_name);
                sw.WriteLine("\t        for (int i = 0; i < cnt; i++)");
                sw.WriteLine("\t        {");
                sw.WriteLine("\t            obj[i] = ({0})msg.ReadInt32();", class_name);
                sw.WriteLine("\t        }");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static void Read(NetBuffer msg, List<{0}> obj)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        int cnt = msg.ReadInt16();");
                sw.WriteLine("\t        obj = new List<{0}>();", class_name);
                sw.WriteLine("\t        for (int i = 0; i < cnt; i++)");
                sw.WriteLine("\t        {");
                sw.WriteLine("\t            obj[i] = ({0})msg.ReadInt32();", class_name);
                sw.WriteLine("\t        }");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Write(NetBuffer msg, {0}[] list)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        msg.Write((Int16)list.Length);");
                sw.WriteLine("\t        foreach ({0} obj in list)", class_name);
                sw.WriteLine("\t        {");
                sw.WriteLine("\t            msg.Write((Int32)obj);");
                sw.WriteLine("\t        }");
                sw.WriteLine("\t        return !msg.IsError;");
                sw.WriteLine("\t    }");

                sw.WriteLine("\t    public static bool Write(NetBuffer msg, List<{0}> list)", class_name);
                sw.WriteLine("\t    {");
                sw.WriteLine("\t        msg.Write((Int16)list.Count);");
                sw.WriteLine("\t        foreach ({0} obj in list)", class_name);
                sw.WriteLine("\t        {");
                sw.WriteLine("\t            msg.Write((Int32)obj);");
                sw.WriteLine("\t        }");
                sw.WriteLine("\t        return !msg.IsError;");
                sw.WriteLine("\t    }");
                sw.WriteLine("\t}");
            }
            else
            {
                sw.WriteLine("\tpublic static partial class Marshaler");
                sw.WriteLine("\t{");
                sw.WriteLine("\t    public static void Read(NetBuffer msg, {0} obj)", class_name);
                sw.WriteLine("\t    {");
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
                        case VAR_TYPE.FLOAT:
                        case VAR_TYPE.STRING:
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            else
                                sw.WriteLine("\t\t\tMarshaler.Read(msg, ref obj.{0});", var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                                sw.WriteLine("\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            else
                                sw.WriteLine("\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            break;
                        default:
                            break;
                    }
                }
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

                sw.WriteLine("\t    public static void Read(NetBuffer msg, List<{0}> list)", class_name);
                sw.WriteLine("\t    {");
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
                        case VAR_TYPE.FLOAT:
                        case VAR_TYPE.STRING:
                        case VAR_TYPE.ENUM:
                            if (is_list)
                                sw.WriteLine("\t\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            else
                                sw.WriteLine("\t\t\t\tMarshaler.Read(msg, ref obj.{0});", var_name);
                            break;
                        case VAR_TYPE.CLASS:
                            if (is_list)
                                sw.WriteLine("\t\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            else
                                sw.WriteLine("\t\t\t\tMarshaler.Read(msg, obj.{0});", var_name);
                            break;
                        default:
                            break;
                    }
                }
                sw.WriteLine("\t\t\t\tlist.Add(obj);");
                sw.WriteLine("\t        }");
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
            }
        }
    }

}
