using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestServer
{
    public class User : IContents<HostID>
    {
        public static Container_S1<User, HostID> LIST = new Container_S1<User, HostID>(100);

        public void OnAlloc(HostID k1)
        {
            hid = k1;
        }
        public void OnFree()
        {

        }
        public HostID GetKey1()
        {
            return this.hid;
        }

        public HostID hid;
    }
}
