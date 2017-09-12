using System;
using System.Collections.Generic;
using System.Text;
using Devarc;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;

namespace TestServer
{
    public class TestServer : AppServer<NetSession, BinaryRequestInfo>
    {
        public static TestServer Instance { get { return msInstance; } }
        private static TestServer msInstance;

        public static S2C.Proxy ProxyS2C { get { return msInstance != null ? msInstance.mProxyS2C : null; } }
        S2C.Proxy mProxyS2C = new S2C.Proxy();
        Stub_C2S stub = new Stub_C2S();

        public TestServer()
            : base(new DefaultReceiveFilterFactory<NetReceiveFilter, BinaryRequestInfo>())
        {
            msInstance = this;
            //appServer.NewRequestReceived += new RequestHandler<AppSession, StringRequestInfo>(Stub_C2S.appServer_NewRequestReceived);
            //ProxyS2C.SetNetworker(server);
            //server.InitStub(stub);
        }

        protected override void OnNewSessionConnected(NetSession session)
        {
            base.OnNewSessionConnected(session);
        }

        protected override void OnSessionClosed(NetSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
        }

        protected override void ExecuteCommand(NetSession session, BinaryRequestInfo requestInfo)
        {
            //if (C2S.Stub.OnReceive(stub, session, requestInfo))
            //{
            //}
        }
    }
}
