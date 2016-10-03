using System;
using System.Collections.Generic;
using System.Text;
using Devarc;

namespace TestServer
{
    public class TestServer
    {
        public static TestServer Instance { get { return ms_Instance; } }
        private static TestServer ms_Instance;

        public static Test2C.Proxy Test2C { get { return ms_Instance != null ? ms_Instance.net_Test2C : null; } }
        Test2C.Proxy net_Test2C = new Test2C.Proxy();

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
            ms_Instance = this;

            net_Test2C.SetNetworker(server);
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
