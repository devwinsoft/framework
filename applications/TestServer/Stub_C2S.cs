﻿using System;
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
    class Stub_C2S : IStubBase, C2S.IStub
    {
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
                    using (obj.READ_LOCK())
                    {
                        //TestServer.Instance.Proxy.Notify_Chat(obj.Data.id, _msg);
                    }
                }
            } // unlock
        }


        public bool OnReceiveData(object sender, NetBuffer msg)
        {
            switch (C2S.Stub.OnReceive(this, msg))
            {
                case RECEIVE_RESULT.INVALID_PACKET:
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
