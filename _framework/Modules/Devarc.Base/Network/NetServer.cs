//
// Copyright (c) 2012 Hyoung Joon, Kim
// http://www.devwinsoft.com/
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

//
// @author Hyoung Joon, Kim (maoshy@nate.com)
// @version $Rev: 1, $Date: 2012-02-20
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Devarc
{
    public class NetServer : INetworker
    {
        internal Dictionary<HostID, NetMessage> WORKING = new Dictionary<HostID, NetMessage>();
        internal List<NetMessage> PENDING = new List<NetMessage>();
        internal NetHostList HostList = new NetHostList(10000);

        private List<HostID> m_DisconnList = new List<HostID>();
        public bool IsRunning { get { return m_running; } }
        private bool m_running = false;

        private NetThreadList m_ThreadList = new NetThreadList();
        internal int m_ThreadCnt = 0;
        private Socket listen_socket;
        private IServerStub m_EventHandler;

        public void InitStub(IServerStub handler)
        {
            m_EventHandler = handler;
        }

        public bool Start(int port, int thread_cnt)
        {
            if (m_running)
            {
                Log.Message(LOG_TYPE.DEBUG, "Server is aleady running");
                return false;
            }

            try
            {
                m_ThreadCnt = thread_cnt;
                m_ThreadList.Init(thread_cnt, this, m_EventHandler);
                m_ThreadList.StartThread();

                listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listen_socket.NoDelay = true;
                listen_socket.Bind(new IPEndPoint(IPAddress.Any, port));
                listen_socket.Listen(500);
                listen_socket.Blocking = false;
                Log.Message(LOG_TYPE.INFO, "Network thread is started: " + thread_cnt.ToString());

                listen_socket.BeginAccept(new AsyncCallback(OnAccept), null);
                m_running = true;
            }
            catch (SocketException se)
            {
                Log.Message(se);
                m_running = false;
            }
            return m_running;
        }

        public void Disconnect(HostID user_hid)
        {
            using (this.HostList.READ_LOCK())
            {
                NetHost host_info = this.HostList.GetAt1(user_hid);
                if (host_info != null)
                {
                    switch (host_info.state)
                    {
                        case NetHost.STATE.CONNECTING:
                        case NetHost.STATE.CONNECTED:
                            host_info.state = NetHost.STATE.DISCONNECTING;
                            lock (m_DisconnList)
                            {
                                if (m_DisconnList.Contains(user_hid) == false)
                                    m_DisconnList.Add(user_hid);
                            } // unlock
                            break;
                        default:
                            break;
                    }
                }
            } // unlock
        }

        public void Stop()
        {
            if (m_running)
            {
                m_running = false;
                listen_socket.Close();

                HostID[] list = null;
                using (this.HostList.WRITE_LOCK())
                {
                    list = new HostID[this.HostList.Count];
                    int i = 0;
                    List<NetHost>.Enumerator enumerator = HostList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        list[i] = enumerator.Current.host_id;
                        i++;
                    }
                } // unlock
                foreach (HostID user_hid in list)
                {
                    Disconnect(user_hid);
                }
                m_ThreadList.StopThread(true);
                Log.Message(LOG_TYPE.INFO, "Network thread is stopped...");
            }
        }

        public void Tick()
        {
            // Disconnect 처리
            List<HostID> disconnected_list = new List<HostID>();
            using (this.HostList.WRITE_LOCK())
            {
                lock (m_DisconnList)
                {
                    if (m_DisconnList.Count > 0)
                    {
                        foreach (HostID user_hid in m_DisconnList.ToArray())
                        {
                            NetHost host = this.HostList.GetAt1(user_hid);
                            if (host == null || host.send.Count > 0)
                            {
                                continue;
                            }
                            if (host.socket.Connected)
                            {
                                host.socket.Shutdown(SocketShutdown.Send);
                                host.socket.Close();
                            }
                            this.HostList.Free1(user_hid);
                            m_DisconnList.Remove(user_hid);
                            disconnected_list.Add(user_hid);
                        }
                        m_DisconnList.Clear();
                    }
                } // unlock
            } // unlock

            try
            {
                foreach (HostID user_hid in disconnected_list)
                {
                    Log.Message(LOG_TYPE.DEBUG, "Disconnected. host:" + user_hid.ToString());
                    if (m_EventHandler != null)
                    {
                        m_EventHandler.OnNotifyUserDisonnect(user_hid);
                    }
                }
            }
            finally
            {
                disconnected_list.Clear();
                disconnected_list = null;
            }
        }

        public bool RmiHeader(HostID host_from, HostID host_to, NetBuffer msg)
        {
            msg.Write(host_from);
            return true;
        }
        public bool RmiSend(HostID host_from, HostID host_to, NetBuffer msg)
        {
            bool success = false;
            try
            {
                NetDataSend state = null;

                using (this.HostList.WRITE_LOCK())
                {
                    NetHost host = this.HostList.GetAt1(host_to);
                    if (host != null && host.state == NetHost.STATE.CONNECTED)
                    {
                        success = true;
                        host.send.Add(msg);
                        if (host.send.Count == 1)
                        {
                            state = new NetDataSend();
                            state.buf = new byte[1024];
                            state.msg_size = msg.Length + sizeof(Int16);
                            Array.Copy(BitConverter.GetBytes((Int16)state.msg_size), state.buf, sizeof(Int16));
                            Array.Copy(msg.ToArray(), 0, state.buf, sizeof(Int16), msg.Length);
                            state.socket = host.socket;
                            state.host_id = host.GetKey1();
                            state.send_cnt = 0;
                        }
                    }
                    else
                    {
                        success = false;
                    }
                } // unlock

                if (state != null)
                {
                    state.socket.BeginSend(state.buf, 0, state.msg_size, SocketFlags.None, new AsyncCallback(OnSend), state);
                }
            }
            catch (SocketException se)
            {
                Log.Message(se);
                success = false;
            }
            finally
            {
                if (success == false)
                    msg.Clear();
            }
            return success;
        }

        public HostID GetMyHostID()
        {
            return HostID.Server;
        }

        void OnSend(IAsyncResult ar)
        {
            NetDataSend state = (NetDataSend)ar.AsyncState;
            Socket socket = state.socket;

            using (this.HostList.READ_LOCK())
            {
                NetHost host_info = this.HostList.GetAt1(state.host_id);
                if (host_info == null)
                {
                    return;
                }
                else if (host_info.send.Count > 0)
                {
                    host_info.send.RemoveAt(0);
                }
            } // unlock

            try
            {
                int cnt = socket.EndSend(ar);
                state.send_cnt += cnt;

                if (cnt == 0)
                {
                    // disconnected
                    Disconnect(state.host_id);
                }
                else if(state.msg_size < state.send_cnt)
                {
                    // error
                    Disconnect(state.host_id);
                }
                else if (state.msg_size > state.send_cnt)
                {
                    socket.BeginSend(state.buf, state.send_cnt, state.msg_size - state.send_cnt, SocketFlags.None, new AsyncCallback(OnSend), state);
                }
                else
                {
                    NetBuffer next_data = null;

                    // get next data
                    using (this.HostList.WRITE_LOCK())
                    {
                        NetHost host_info = this.HostList.GetAt1(state.host_id);
                        if (host_info != null)
                        {
                            next_data = host_info.send.Count > 0 ? host_info.send[0] : null;
                        }
                    } // unlock
                    if (next_data != null)
                    {
                        state.msg_size = next_data.Length + sizeof(Int16);
                        Array.Copy(BitConverter.GetBytes((Int16)state.msg_size), state.buf, sizeof(Int16));
                        Array.Copy(next_data.ToArray(), 0, state.buf, sizeof(Int16), next_data.Length);
                        state.send_cnt = 0;
                        socket.BeginSend(state.buf, 0, state.msg_size, SocketFlags.None, new AsyncCallback(OnSend), state);
                    }
                }
            }
            catch (SocketException se)
            {
                Log.Message(se);
                Disconnect(state.host_id);
            }
            catch (Exception e)
            {
                Log.Message(e);
                Disconnect(state.host_id);
            }
        }

        void OnAccept(IAsyncResult ar)
        {
            Socket socket = null;

            if (IsRunning == false)
            {
                return;
            }

            try
            {
                socket = listen_socket.EndAccept(ar);

                // Create TcpHost
                NetHost host_info = null;
                using (this.HostList.WRITE_LOCK())
                {
                    host_info = this.HostList.Create(socket);
                } // unlock
                Log.Message(LOG_TYPE.DEBUG, "Accepted host : " + host_info.host_id.ToString());

                // Notify HostID
                {
                    NetBuffer msg = new NetBuffer();
                    msg.Write(host_info.host_id);
                    RmiSend(HostID.Server, host_info.host_id, msg);
                }

                // Begin Receive
                {
                    NetDataRecv state = new NetDataRecv(1024);
                    state.host_id = host_info.GetKey1();
                    state.socket = socket;
                    state.read_cnt = 0;
                    socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), state);
                }
                if (m_EventHandler != null)
                {
                    m_EventHandler.OnNotifyUserConnect(host_info.GetKey1());
                }
            }
            catch (Exception e)
            {
                Log.Message(e);
                if (socket != null)
                {
                    socket.Close();
                }
            }
            finally
            {
                listen_socket.BeginAccept(new AsyncCallback(OnAccept), null);
            }
        }

        void OnReceive(IAsyncResult ar)
        {
            NetDataRecv state = (NetDataRecv)ar.AsyncState;
            Socket socket = state.socket;
            byte[] buffer = state.buffer;

            try
            {
                using (this.HostList.READ_LOCK())
                {
                    NetHost host_info = this.HostList.GetAt1(state.host_id);
                    if (host_info == null)
                    {
                        // 서버가 접속을 끊은 경우...
                        if (socket.Connected)
                            socket.EndReceive(ar);
                        socket.Close();
                        return;
                    }
                } // unlock

                int cnt = socket.EndReceive(ar);
                state.read_cnt += cnt;
                if (cnt == 0 || state.read_cnt < sizeof(Int16))
                {
                    Disconnect(state.host_id);
                    return;
                }

                int msg_size = BitConverter.ToInt16(buffer, 0);
                if (msg_size > buffer.Length)
                {
                    Disconnect(state.host_id);
                    return;
                }
                else if (msg_size > state.read_cnt)
                {
                    Disconnect(state.host_id);
                    return;
                }
                else
                {
                    int start_index = 0;
                    int next_index;
                    while (ProcessData(state.host_id, buffer, start_index, state.read_cnt, out next_index))
                    {
                        start_index = next_index;
                    }
                    int remain = state.read_cnt - next_index;
                    byte[] temp = new byte[remain];
                    Array.Copy(buffer, next_index, temp, 0, remain);
                    Array.Copy(temp, buffer, remain);
                    state.read_cnt = remain;
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), state);
                }
            }
            catch (SocketException se)
            {
                switch (se.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        Disconnect(state.host_id);
                        break;
                    default:
                        Log.Message(se);
                        Disconnect(state.host_id);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Message(e);
                Disconnect(state.host_id);
            }
        }

        bool ProcessData(HostID hid, byte[] buffer, int start_index, int read_cnt, out int next_index)
        {
            next_index = start_index;
            if (read_cnt - start_index < sizeof(Int16))
                return false;
            int msg_size = BitConverter.ToInt16(buffer, start_index);
            if (read_cnt - start_index < msg_size)
                return false;
            next_index = start_index + msg_size;
            NetBuffer msg = new NetBuffer();
            msg.Write(buffer, start_index + sizeof(Int16), msg_size - sizeof(Int16));
            this.OnReceived(hid, msg);
            return start_index != next_index;
        }

        void OnReceived(HostID hid, NetBuffer msg)
        {
            NetMessage net_msg = new NetMessage();
            net_msg.busy = false;
            net_msg.hid = hid;
            net_msg.msg = msg;

            lock (PENDING)
            lock (WORKING)
            {
                if (WORKING.Count < this.m_ThreadCnt && WORKING.ContainsKey(hid) == false)
                {
                    WORKING.Add(hid, net_msg);
                }
                else
                {
                    PENDING.Add(net_msg);
                }
            }
        }

    } // end class
} // end of namespace
