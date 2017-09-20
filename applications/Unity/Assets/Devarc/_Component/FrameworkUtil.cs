using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devarc
{
    public static class FrameworkUtil
    {
        public static string GetClassName(string _path)
        {
            int startIndex = _path[0] == '!' ? 1 : 0;
            int endIndex = _path.IndexOf('@');
            if (endIndex >= 0)
                return _path.Substring(startIndex, endIndex - startIndex);
            else
                return _path.Substring(startIndex, _path.Length - startIndex);
        }
        public static string GetClassNameEx(string _path)
        {
            string value = System.IO.Path.GetFileNameWithoutExtension(_path);
            int startIndex = value[0] == '!' ? 1 : 0;
            int endIndex = value.IndexOf('@');
            if (endIndex >= 0)
                return value.Substring(startIndex, endIndex - startIndex);
            else
                return value.Substring(startIndex, _path.Length - startIndex);
        }

        public static string MakeLStringKey(string _class_name, string _field_name, string _id)
        {
            return string.Format("{0}_{1}_{2}", _class_name, _field_name, _id);
        }

        public static string GetLString(string _class_name, string _field_name, string _id)
        {
            string key = MakeLStringKey(_class_name, _field_name, _id);
            LString obj = T_LString.MAP.GetAt(key);
            if (obj == null)
                return string.Empty;
            return obj.Value;
        }
    }
}
