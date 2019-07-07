using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Devarc
{
    public partial class LString
    {
        public static string[] Files = new string[]
        { "LString_Item"
        , "LString_TestSchema"
        };

        public static implicit operator string(LString obj)
        {
            return obj != null ? obj.Value : string.Empty;
        }

        public LString(string _key)
        {
            Key = _key;
        }
    }
}
