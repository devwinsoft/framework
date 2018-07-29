using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Devarc
{
    public partial class LString
    {
        public static implicit operator string(LString obj)
        {
            return obj.Value;
        }

        public LString(string _key)
        {
            Key = _key;
        }
    }
}
