using System;
using System.Collections.Generic;
using System.Text;
using Devarc;

namespace TestServer
{
    public class TestServer
    {
        public static TestServer Instance { get { return msInstance; } }
        private static TestServer msInstance;

        public static S2C.Proxy ProxyS2C { get { return msInstance != null ? msInstance.mProxyS2C : null; } }
        S2C.Proxy mProxyS2C = new S2C.Proxy();

        public NetServer server = new NetServer();
        Stub_C2Test stub = new Stub_C2Test();
        bool m_run = false;

        public bool IsRunning
        {
            get
            {
                return m_run || server.IsRunning;
            }
        }

        public TestServer()
        {
            msInstance = this;

            ProxyS2C.SetNetworker(server);
            server.InitStub(stub);
        }

        public bool Run(int service_port)
        {
            m_run = true;
            server.Start(service_port, 8);

            return false;
        }

        public void Stop()
        {
            m_run = false;
            server.Stop();
        }

        public void Tick(float deltaTime)
        {
            server.Tick();
        }
    }
}
