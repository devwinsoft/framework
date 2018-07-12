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
    public class TestServer : NetServer
    {
        public static TestServer Instance { get { return msInstance; } }
        private static TestServer msInstance;

        public S2C.Proxy Proxy { get { return proxy; } }
        S2C.Proxy proxy = new S2C.Proxy();
        Stub_C2S stub = new Stub_C2S();

        public TestServer()
        {
            msInstance = this;
            ServerConfig serverConfig = new ServerConfig
            {
                Ip = "127.0.0.1",
                Port = 5000,
                Mode = SocketMode.Tcp,
            };
            Setup(serverConfig);

            Init(10);
            InitStub(stub);
            proxy.Init(this);
        }


        protected override void OnStarted()
        {
            base.OnStarted();

            Log.Debug("OnStarted");
        }

        protected override void OnStopped()
        {
            Log.Debug("OnStopped");

            base.OnStopped();
        }

        protected override void OnNewSessionConnected(NetSession session)
        {
            base.OnNewSessionConnected(session);

            using (PlayerData.LIST.WRITE_LOCK())
            {
                PlayerData obj = PlayerData.LIST.Alloc(session.Hid);
            }

            Log.Debug("OnNewSessionConnected");
        }

        protected override void OnSessionClosed(NetSession session, CloseReason reason)
        {
            Log.Debug("OnSessionClosed: {0}", reason);

            using (PlayerData.LIST.WRITE_LOCK())
            {
                PlayerData.LIST.Free1(session.Hid);
            }

            base.OnSessionClosed(session, reason);
        }
    }
}

