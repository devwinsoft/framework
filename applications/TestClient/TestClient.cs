﻿using System;
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

        public Devarc.C2S.Proxy Proxy { get { return proxy; } }
        Devarc.C2S.Proxy proxy = new Devarc.C2S.Proxy();
        Stub_S2C stub = new Stub_S2C();

        public TestClient()
        {
            ms_Instance = this;
            proxy.Init(this);
            stub.Init(this);
        }

        protected override void OnConnected()
        {
            Log.Debug("TestClient::OnConnected");
        }

        protected override void OnDisConnected()
        {
            Log.Debug("TestClient::OnDisConnected");
        }
    }
}
