using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace Devarc
{
    public static class FrameworkUtil
    {
        public static string ToBase64String(byte[] _data)
        {
            if (_data == null)
            {
                return string.Empty;
            }
            return Convert.ToBase64String(_data);
        }
        public static string ToBase64String(string _value)
        {
            Encoding encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(_value);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64String(string _value)
        {
            Encoding encoding = Encoding.UTF8;
            try
            {
                return encoding.GetString(Convert.FromBase64String(_value));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return string.Empty;
            }
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

        public static HostID ToHostID(string _name)
        {
            Int32 result;
            if (string.IsNullOrEmpty(_name))
                return HostID.None;
            if (Int32.TryParse(_name, out result))
            {
                return (HostID)result;
            }
            if (string.Equals("server", _name.ToLower()))
            {
                return HostID.Server;
            }
            return HostID.None;
        }

        public static T Parse<T>(string _name) where T : struct
        {
            Int32 result;
            if (string.IsNullOrEmpty(_name))
                return default(T);
            if (Int32.TryParse(_name, out result))
            {
                return (T)Enum.ToObject(typeof(T), result);
            }
            try
            {
                return (T)System.Enum.Parse(typeof(T), _name);
            }
            catch
            {
                return default(T);
            }
        }

        public static string MakeLStringKey(string _class_name, string _field_name, string _id)
        {
            return string.Format("{0}_{1}_{2}", _class_name, _field_name, _id); ;
        }

        public static string InnerString<T>(List<T> _list) where T : IBaseObejct
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            bool isFirst = true;
            var enumer = _list.GetEnumerator();
            while (enumer.MoveNext())
            {
                T obj = enumer.Current;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(obj.ToJson());
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string InnerString_SQLite(string _value)
        {
            if (string.IsNullOrEmpty(_value))
                return string.Empty;
            return _value.Replace("'", "''");
        }

        public static string JsonString(string _value)
        {
            if (string.IsNullOrEmpty(_value))
                return string.Empty;
            return _value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string InnerString(string _value)
        {
            if (string.IsNullOrEmpty(_value))
                return string.Empty;
            return _value.Replace("'", "''").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string InnerString(List<string> _list)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            bool isFirst = true;
            var enumer = _list.GetEnumerator();
            while (enumer.MoveNext())
            {
                string obj = enumer.Current;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(string.Format("\"{0}\"", InnerString(obj)));
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
