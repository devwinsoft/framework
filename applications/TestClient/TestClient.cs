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
    public class TestClient : NetClient
    {
        public static TestClient Instance { get { return ms_Instance; } }
        private static TestClient ms_Instance;

        public C2S.Proxy Proxy { get { return proxy; } }
        C2S.Proxy proxy = new C2S.Proxy();
        Stub_S2C stub = new Stub_S2C();

        public TestClient()
        {
            ms_Instance = this;

            this.Init(proxy, stub);
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisConnected()
        {
        }
    }
}
