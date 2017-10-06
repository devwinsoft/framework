using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;
using Devarc;

namespace TestClient
{
    public class TestClient
    {
        public static TestClient Instance { get { return ms_Instance; } }
        private static TestClient ms_Instance;

        public C2S.Proxy ProxyC2Test { get { return mProxyC2S; } }
        C2S.Proxy mProxyC2S = new C2S.Proxy();

        public NetClient client = new NetClient();
        Stub_S2C stub = new Stub_S2C();

        public TestClient()
        {
            ms_Instance = this;

            mProxyC2S.Init(client);
            client.OnReceiveData += stub.OnReceive;
        }

        public bool Connect(string _address, int _port)
        {
            return client.Connect(_address, _port);
        }
    }
}
