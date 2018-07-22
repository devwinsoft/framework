using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NetProtocolAttribute : System.Attribute
    {
        public short RMI_ID = 0;
    }
}
