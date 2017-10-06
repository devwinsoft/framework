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
    public class TestServer
    {
        public static TestServer Instance { get { return msInstance; } }
        private static TestServer msInstance;

        public NetServer server = new NetServer();

        public S2C.Proxy ProxyS2C { get { return mProxyS2C; } }
        S2C.Proxy mProxyS2C = new S2C.Proxy();
        Stub_C2S stub = new Stub_C2S();

        public TestServer()
        {
            msInstance = this;
            mProxyS2C.Init(server);
            server.OnReceiveData += stub.OnReceive;
        }
    }
}

