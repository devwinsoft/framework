using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devarc
{
    public static class FrameworkUtil
    {
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
