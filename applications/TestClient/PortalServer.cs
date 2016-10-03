using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Devarc;
namespace Portal
{
    public class PortalServer
    {
        public static PortalServer Instance { get { return ms_Instance; } }
        private static PortalServer ms_Instance;

        public NetServer nets_client { get { return m_nets_client; } }
        NetServer m_nets_client = new NetServer();
        Stub_Client2Portal m_stub_client = new Stub_Client2Portal();

        public NetClient netc_auth { get { return m_netc_auth; } }
        NetClient m_netc_auth = new NetClient();
        Stub_Auth2Portal m_stub_auth = new Stub_Auth2Portal();

        public NetClient netc_lobby { get { return m_netc_lobby; } }
        NetClient m_netc_lobby = new NetClient();
        Stub_Lobby2Portal m_stub_lobby = new Stub_Lobby2Portal();

        public NetServer nets_viking { get { return m_nets_viking; } }
        NetServer m_nets_viking = new NetServer();
        Stub_Viking2Portal m_stub_viking = new Stub_Viking2Portal();


        bool m_run = false;
        string m_auth_ip;
        int m_auth_port;
        string m_lobby_ip;
        int m_lobby_port;
        int m_internal_port;
        int m_external_port;
        float m_elapsedTime = 0.0f;

        internal DateTime reconnect_time { get; set; }
        internal HostID battle_hid { get; set; }

        public bool IsRunning
        {
            get
            {
                return m_run
                    || m_nets_client.IsRunning
                    || m_netc_auth.State != NetClient.STATE.DISCONNECTED
                    || m_netc_lobby.State != NetClient.STATE.DISCONNECTED
                    || m_nets_viking.IsRunning;
            }
        }

        public PortalServer()
        {
            ms_Instance = this;

            Viking2C.Proxy.SetNetworker(m_nets_client);
            Portal2C.Proxy.SetNetworker(m_nets_client);
            Lobby2C.Proxy.SetNetworker(m_nets_client);
            m_nets_client.InitStub(m_stub_client);

            P2Auth.Proxy.SetNetworker(m_netc_auth);
            S2Auth.Proxy.SetNetworker(m_netc_auth);
            m_netc_auth.InitStub(m_stub_auth);

            P2Lobby.Proxy.SetNetworker(m_netc_lobby);
            Auth2S.Proxy.SetNetworker(m_netc_lobby);
            C2Lobby.Proxy.SetNetworker(m_netc_lobby);
            m_netc_lobby.InitStub(m_stub_lobby);

            P2Battle.Proxy.SetNetworker(m_nets_viking);
            C2Viking.Proxy.SetNetworker(m_nets_viking);
            m_nets_viking.InitStub(m_stub_viking);
        }

        public bool Run(string auth_ip, int auth_port, string lobby_ip, int lobby_port, int internal_port, int external_port)
        {
            if (IsRunning == true)
            {
                return false;
            }

            m_run = true;
            m_auth_ip = auth_ip;
            m_auth_port = auth_port;
            m_lobby_ip = lobby_ip;
            m_lobby_port = lobby_port;
            m_internal_port = internal_port;
            m_external_port = external_port;
            m_nets_viking.Start(m_internal_port, 8);

            return true;
        }

        public void Stop()
        {
            m_run = false;
            m_netc_auth.Disconnect();
            m_netc_lobby.Disconnect();
            m_nets_viking.Stop();
            m_nets_client.Stop();
        }

        public void Tick(float deltaTime)
        {
            m_nets_client.Tick();
            m_netc_auth.Tick(deltaTime);
            m_netc_lobby.Tick(deltaTime);
            m_nets_viking.Tick();

            if (IsRunning)
            {
                if (m_netc_auth.State == NetClient.STATE.DISCONNECTED
                    || m_netc_lobby.State == NetClient.STATE.DISCONNECTED)
                {
                    TimeSpan diff_time = DateTime.Now - reconnect_time;
                    if (diff_time > TimeSpan.FromSeconds(10.0))
                    {
                        reconnect_time = DateTime.Now;
                        if (m_netc_auth.State == NetClient.STATE.DISCONNECTED)
                            m_netc_auth.Connect(m_auth_ip, m_auth_port, 5.0f);
                        if (m_netc_lobby.State == NetClient.STATE.DISCONNECTED)
                            m_netc_lobby.Connect(m_lobby_ip, m_lobby_port, 5.0f);
                    }
                }
                if (m_netc_auth.State == NetClient.STATE.CONNECTED
                    && m_netc_lobby.State == NetClient.STATE.CONNECTED
                    && m_nets_viking.IsRunning == true
                    && m_nets_client.IsRunning == false)
                {
                    m_nets_client.Start(m_external_port, 8);
                }

                m_elapsedTime += deltaTime;
                if (m_elapsedTime > 30.0f)
                {
                    m_elapsedTime = 0.0f;
                    using (PortalUser.LIST.READ_LOCK())
                    {
                        foreach (PortalUser user_info in PortalUser.LIST.ToArray())
                        {
                            Portal2C.Proxy.Ping(user_info.user_hid);
                        }
                    } // unlock
                }
            }
        }
    } // end of class
} // end of namespace
