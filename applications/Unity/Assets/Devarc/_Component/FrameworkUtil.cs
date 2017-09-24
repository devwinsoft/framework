using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

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
                return value.Substring(startIndex, value.Length - startIndex);
        }

        public static int FillList<T>(string _jsonString, List<T> _list) where T : IBaseObejct, new()
        {
            if (_list == null)
                return 0;
            _list.Clear();
            JsonData value = JsonMapper.ToObject(_jsonString);
            if (value.IsArray == false)
                return 0;
            System.Collections.IList tempList = value as System.Collections.IList;
            var enumer = tempList.GetEnumerator();
            while (enumer.MoveNext())
            {
                T obj = new T();
                obj.Initialize(enumer.Current as JsonData);
                _list.Add(obj);
            }
            return _list.Count;
        }

        public static string MakeLStringKey(string _class_name, string _field_name, string _id)
        {
            return string.Format("{0}_{1}_{2}", _class_name, _field_name, _id);
        }

        public static string GetLString(string _class_name, string _field_name, string _id)
        {
            return GetLString(MakeLStringKey(_class_name, _field_name, _id));
        }

        public static string GetLString(string _lstrKey)
        {
            LString obj;
            TableManager.T_LString.TryGetAt(SQLITE.Connection, _lstrKey, out obj);
            if (obj == null)
                return string.Empty;
            return obj.Value;
        }
    }
}
