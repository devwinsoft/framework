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
        public Int16 Seq { get; internal set; }

        public void IncreaseSeq()
        {
            Seq++;
            if (Seq >= short.MaxValue)
            {
                Seq = 1;
            }
        }

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

            NetBuffer msg = new NetBuffer();
            msg.Init(-2, HostID.None);
            this.Send(msg.Data);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            Log.Info("Nession is closed. Reason={0}", reason);
        }
    }
}

