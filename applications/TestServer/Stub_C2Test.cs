using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Devarc;

namespace TestServer
{
    class Stub_C2Test : IServerStub, C2S.IStub
    {
        public void OnNotifyUserConnect(HostID host_hid)
        {
            using(User.LIST.WRITE_LOCK())
            {
                User.LIST.Alloc(host_hid);
            } // unlock
        }
        public void OnNotifyUserDisonnect(HostID host_hid)
        {
            using (User.LIST.WRITE_LOCK())
            {
                User.LIST.Free1(host_hid);
            } // unlock
        }

        public void RMI_C2S_Chat(HostID remote, String _msg)
        {
            using (User.LIST.READ_LOCK())
            {
                List<User>.Enumerator enumerator = User.LIST.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    User obj = enumerator.Current;
                    TestServer.ProxyS2C.Notify_Chat(obj.hid, _msg);
                }
            } // unlock
        }


        public bool OnReceive(int rid, HostID hid, NetBuffer msg)
        {
            return C2S.Stub.OnReceive(this, rid, hid, msg);
        }
    }
}
