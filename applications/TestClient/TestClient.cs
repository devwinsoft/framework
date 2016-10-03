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

        public static C2Test.Proxy C2Test { get { return ms_Instance != null ? ms_Instance.net_C2Test : null; } }
        C2Test.Proxy net_C2Test = new C2Test.Proxy();

        public NetClient client = new NetClient();
        Stub_Test2C stub = new Stub_Test2C();

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

            net_C2Test.SetNetworker(client);
            client.InitStub(stub);
        }

        public bool Connect(string _address, int _port)
        {
            return client.Connect(_address, _port, 5.0f);
        }
    }
}
