using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Devarc;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace TestServer
{
    class Stub_C2S : IServerStub, C2S.IStub
    {
        public void OnNotifyUserConnect(HostID host_hid)
        {
            using(PlayerData.LIST.WRITE_LOCK())
            {
                PlayerData obj = PlayerData.LIST.Alloc(host_hid);
                TestServer.ProxyS2C.Notify_Chat(host_hid, "");
            } // unlock
        }
        public void OnNotifyUserDisonnect(HostID host_hid)
        {
            using (PlayerData.LIST.WRITE_LOCK())
            {
                PlayerData.LIST.Free1(host_hid);
            } // unlock
        }

        public void RMI_C2S_Move(HostID remote, VECTOR3 _look, DIRECTION _move)
        {

        }

        public void RMI_C2S_Chat(HostID remote, String _msg)
        {
            using (PlayerData.LIST.READ_LOCK())
            {
                List<PlayerData>.Enumerator enumerator = PlayerData.LIST.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PlayerData obj = enumerator.Current;
                    TestServer.ProxyS2C.Notify_Chat(obj.Data.id, _msg);
                }
            } // unlock
        }


        public bool OnReceive(int rid, HostID hid, NetBuffer msg)
        {
            return C2S.Stub.OnReceive(this, rid, hid, msg);
        }
    }
}
