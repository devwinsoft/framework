﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Devarc;
using Devarc.C2S;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace TestServer
{
    class Stub_C2S : StubBase, IStub
    {
        public void RMI_C2S_Request_Move(HostID remote, Request_Move msg)
        {
        }

        public void RMI_C2S_Request_Chat(HostID remote, Request_Chat msg)
        {
            Log.Info(msg.msg);
            Log.Info(System.Convert.ToBase64String(msg.data));
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


        public override bool OnReceive(object sender, NetBuffer msg)
        {
            switch (Stub.OnReceive(this, msg))
            {
                case RECEIVE_RESULT.SUCCESS:
                    return true;
                case RECEIVE_RESULT.INVALID_PACKET_DOWNFLOW:
                case RECEIVE_RESULT.INVALID_PACKET_OVERFLOW:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}
