using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devarc;

namespace TestClient
{
    public class TestClient
    {
        public static TestClient Instance { get { return ms_Instance; } }
        private static TestClient ms_Instance;

        public static C2S.Proxy ProxyC2Test { get { return ms_Instance != null ? ms_Instance.mProxyC2S : null; } }
        C2S.Proxy mProxyC2S = new C2S.Proxy();

        public NetClient client = new NetClient();
        Stub_S2C stub = new Stub_S2C();

        public bool IsRunning
        {
            get
            {
                return client.State != NetClient.STATE.DISCONNECTED;
            }
        }


        public TestClient()
        {
            ms_Instance = this;

            mProxyC2S.SetNetworker(client);
            client.InitStub(stub);
        }

        public bool Connect(string _address, int _port)
        {
            return client.Connect(_address, _port, 5.0f);
        }
    }
}
