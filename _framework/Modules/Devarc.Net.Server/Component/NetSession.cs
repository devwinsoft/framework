using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devarc;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace Devarc
{
    public class NetSession : AppSession<NetSession, NetRequestInfo>
    {
        public HostID Hid { get; internal set; }

        protected override void OnSessionStarted()
        {
            //base.OnSessionStarted();
            //this.Send("Welcome to SuperSocket Telnet Server");
        }

        protected override void HandleException(Exception e)
        {
            Log.Info("Error : {0}", e.Message);

            this.Send("Error : {0}", e.Message);
        }

        protected override void HandleUnknownRequest(NetRequestInfo requestInfo)
        {
            Log.Info("Unknow request");

            PacketData msg = new PacketData();
            msg.Init(1, "Unknow request");
            this.Send(msg.Data);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            Log.Info("Nession is closed. Reason={0}", reason);
        }
    }
}

