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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Devarc
{
    public class NetClient : INetworker
    {
        public enum STATE
        {
            CONNECTED,
            CONNECTING,
            DISCONNECTING,
            DISCONNECTED,
        }

        public STATE State { get { return m_State; } }
        STATE m_State = STATE.DISCONNECTED;
        bool m_connected = false;
        float m_timeout = 0.0f;
        DISCONNECTION_REASON m_reason = DISCONNECTION_REASON.CONNECTION_FAIL;
        Socket m_Socket;
        HostID m_HostID;
        List<NetBuffer> m_SendList = new List<NetBuffer>();
        List<NetBuffer> m_RecvList = new List<NetBuffer>();
        IClientStub m_EventHandler = null;

        public void InitStub(IClientStub handler)
        {
            m_EventHandler = handler;
        }

        public bool Connect(string address, int port, float timeout)
        {
            switch (m_State)
            {
                case STATE.DISCONNECTED:
                    m_State = STATE.CONNECTING;
                    m_HostID = HostID.None;
                    m_timeout = timeout <= 0.0f ? -1.0f : timeout;
                    break;
                case STATE.CONNECTING:
                    Log.Info("Already connecting.");
                    return false;
                case STATE.CONNECTED:
                    Log.Info("Already connected.");
                    return false;
                default:
                    Log.Info("Cannot connect now.");
                    return false;
            }

            // get ip address
            IPAddress[] ip_list = Dns.GetHostAddresses(address);
            IPAddress ip = null;
            foreach (IPAddress obj in ip_list)
            {
                if (obj.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = obj;
                    break;
                }
            }
            if (ip == null)
            {
                Log.Info("Cannot connect to : " + address);
                return false;
            }

            bool success = false;
            m_reason = DISCONNECTION_REASON.CONNECTION_FAIL;
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.NoDelay = true;
            try
            {
                Log.Info("Connecting to " + ip.ToString() + ":" + port.ToString());
                m_Socket.BeginConnect(ip, port, OnConnect, m_Socket);
                success = true;
            }
            catch (SocketException e)
            {
                Log.Exception(e);
                success = false;
            }

            if (success && m_EventHandler != null)
            {
                m_EventHandler.OnNotifyConnecting();
            }
            return success;
        }

        void OnConnect(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                if (m_Socket != socket)
                {
                    if (socket.Connected)
                        socket.EndConnect(ar);
                    socket.Close();
                    return;
                }
                else if (socket.Connected == false)
                {
                    if (m_State == STATE.CONNECTING)
                    {
                        m_connected = true;
                        m_State = STATE.DISCONNECTED;
                    }
                    return;
                }
                else
                {
                    socket.EndConnect(ar);
                }

                NetDataRecv state = new NetDataRecv(1024);
                state.read_cnt = 0;
                state.socket = socket;
                socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveHostID), state);
                m_reason = DISCONNECTION_REASON.BY_SERVER;

                Log.Info("Connected.");
            }
            catch (SocketException se)
            {
                m_connected = true;
                m_State = STATE.DISCONNECTED;
                switch (se.SocketErrorCode)
                {
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionRefused:
                        Log.Info("Cannot connect.");
                        break;
                    default:
                        Log.Exception(se);
                        break;
                }
            }
            catch (Exception e)
            {
                m_State = STATE.DISCONNECTED;
                Log.Exception(e);
            }
        }

        public void Disconnect()
        {
            Disconnect(DISCONNECTION_REASON.BY_USER);
        }
        public void Disconnect(DISCONNECTION_REASON reason)
        {
            m_reason = reason;
            if (m_State == STATE.CONNECTED)
            {
                Log.Info("Disconnecting.");
                m_State = STATE.DISCONNECTING;
                m_connected = true;
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            else if (m_State == STATE.CONNECTING)
            {
                Log.Info("Disconnecting.");
                if (m_Socket.Connected)
                {
                    m_State = STATE.DISCONNECTING;
                    m_connected = true;
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                else
                {
                    m_State = STATE.DISCONNECTED;
                    m_connected = true;
                    //m_Socket.Close();
                }
            }
        }

        public bool RmiHeader(HostID host_from, HostID host_to, NetBuffer msg)
        {
            msg.Write(host_to);
            return true;
        }

        public bool RmiSend(HostID host_from, HostID host_to, NetBuffer msg)
        {
            bool success = false;

            switch (m_State)
            {
                case STATE.CONNECTING:
                case STATE.DISCONNECTING:
                case STATE.DISCONNECTED:
                    return false;
                default:
                    break;
            }

            try
            {
                lock (m_SendList)
                {
                    m_SendList.Add(msg);
                    if (m_SendList.Count == 1)
                    {
                        NetDataSend state = new NetDataSend();
                        state.buf = new byte[1024];
                        state.socket = m_Socket;
                        state.send_cnt = 0;
                        state.msg_size = msg.Length + sizeof(Int16);
                        Array.Copy(BitConverter.GetBytes((Int16)state.msg_size), state.buf, sizeof(Int16));
                        Array.Copy(msg.ToArray(), 0, state.buf, sizeof(Int16), msg.Length);
                        m_Socket.BeginSend(state.buf, 0, state.msg_size, SocketFlags.None, new AsyncCallback(OnSend), state);
                    }
                } // unlock
                success = true;
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
            return m_HostID;
        }

        void OnSend(IAsyncResult ar)
        {
            NetDataSend state = (NetDataSend)ar.AsyncState;

            switch (m_State)
            {
                case STATE.DISCONNECTING:
                case STATE.DISCONNECTED:
                    return;
                default:
                    break;
            }

            try
            {
                int send_cnt = m_Socket.EndSend(ar);
                state.send_cnt += send_cnt;

                if (send_cnt == 0)
                {
                    Disconnect();
                }
                else if (state.send_cnt > state.msg_size)
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                }
                else if (state.send_cnt < state.msg_size)
                {
                    // 메시지 전송 중...
                    m_Socket.BeginSend(state.buf, state.send_cnt, state.msg_size - state.send_cnt, SocketFlags.None, new AsyncCallback(OnSend), state);
                }
                else
                {
                    // 메시지 전송 완료
                    NetBuffer next_data = null;
                    lock (m_SendList)
                    {
                        m_SendList.RemoveAt(0);
                        if (m_SendList.Count > 0)
                        {
                            next_data = m_SendList[0];
                        }
                    } // unlock

                    if (next_data != null)
                    {
                        state.send_cnt = 0;
                        state.msg_size = next_data.Length + sizeof(Int16);
                        Array.Copy(BitConverter.GetBytes((Int16)state.msg_size), state.buf, sizeof(Int16));
                        Array.Copy(next_data.ToArray(), 0, state.buf, sizeof(Int16), next_data.Length);
                        m_Socket.BeginSend(state.buf, 0, state.msg_size, SocketFlags.None, new AsyncCallback(OnSend), state);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Disconnect(DISCONNECTION_REASON.ERROR);
            }
        }

        void OnReceiveHostID(IAsyncResult ar)
        {
            NetDataRecv state = (NetDataRecv)ar.AsyncState;
            byte[] buffer = state.buffer;
            m_State = STATE.CONNECTED;

            try
            {
                int cnt = m_Socket.EndReceive(ar);
                state.read_cnt += cnt;
                if (cnt == 0)
                {
                    Disconnect();
                    return;
                }
                else if(state.read_cnt < sizeof(Int16))
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }

                int msg_size = BitConverter.ToInt16(buffer, 0);
                if (msg_size > buffer.Length)
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }
                else if (msg_size > state.read_cnt)
                {
                    m_Socket.BeginReceive(buffer, state.read_cnt, buffer.Length - state.read_cnt, SocketFlags.None, new AsyncCallback(OnReceiveData), state);
                }
                else
                {
                    if (state.read_cnt == msg_size)
                    {
                        Process(state.buffer, 0, state.read_cnt);

                        state.read_cnt = 0;
                        m_Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), state);
                    }
                    else
                    {
                        int start_index = 0;
                        int next_index = 0;
                        do
                        {
                            start_index = next_index;
                            next_index = Process(state.buffer, start_index, state.read_cnt);
                        } while (start_index != next_index);

                        int remain = state.read_cnt - next_index;
                        byte[] temp = new byte[remain];
                        Array.Copy(buffer, next_index, temp, 0, remain);
                        Array.Copy(temp, buffer, remain);
                        state.read_cnt = remain;
                        m_Socket.BeginReceive(buffer, remain, buffer.Length - remain, SocketFlags.None, new AsyncCallback(OnReceiveData), state);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Disconnect();
            }
        }

        void OnReceiveData(IAsyncResult ar)
        {
            NetDataRecv state = (NetDataRecv)ar.AsyncState;
            byte[] buffer = state.buffer;

            switch (m_State)
            {
                case STATE.DISCONNECTING:
                case STATE.DISCONNECTED:
                    if (state.socket.Connected)
                        state.socket.EndReceive(ar);
                    return;
                default:
                    break;
            }

            try
            {
                int cnt = state.socket.Connected ? state.socket.EndReceive(ar) : 0;
                state.read_cnt += cnt;
                if (cnt == 0)
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }
                else if (state.read_cnt < sizeof(Int16))
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }

                int msg_size = BitConverter.ToInt16(buffer, 0);
                if (msg_size > buffer.Length)
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }
                else if (msg_size > state.read_cnt)
                {
                    Disconnect(DISCONNECTION_REASON.ERROR);
                    return;
                }
                else
                {
                    int start_index = 0;
                    int next_index = 0;
                    if (state.read_cnt == msg_size)
                    {
                        do
                        {
                            start_index = next_index;
                            next_index = Process(state.buffer, start_index, state.read_cnt);
                        } while (start_index != next_index);

                        state.read_cnt = 0;
                        m_Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), state);
                    }
                    else
                    {
                        do
                        {
                            start_index = next_index;
                            next_index = Process(state.buffer, start_index, state.read_cnt);
                        } while (start_index != next_index);

                        int remain = state.read_cnt - next_index;
                        byte[] temp = new byte[remain];
                        Array.Copy(buffer, next_index, temp, 0, remain);
                        Array.Copy(temp, buffer, remain);
                        state.read_cnt = remain;
                        m_Socket.BeginReceive(buffer, remain, buffer.Length - remain, SocketFlags.None, new AsyncCallback(OnReceiveData), state);
                    }
                }
            }
            catch (SocketException se)
            {
                switch (se.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        Disconnect(DISCONNECTION_REASON.BY_SERVER);
                        break;
                    case SocketError.HostDown:
                    default:
                        Disconnect(DISCONNECTION_REASON.ERROR);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Disconnect(DISCONNECTION_REASON.ERROR);
            }
        }

        //bool ProcessData(byte[] buffer, int start_index, int read_cnt, out int next_index)
        //{
        //    next_index = start_index;
        //    if (read_cnt - start_index < sizeof(Int16))
        //        return false;
        //    int msg_size = BitConverter.ToInt16(buffer, start_index);
        //    if (read_cnt - start_index < msg_size)
        //        return false;
        //    next_index = start_index + msg_size;
        //    NetBuffer msg = new NetBuffer();
        //    msg.Write(buffer, start_index + sizeof(Int16), msg_size - sizeof(Int16));
        //    lock (m_RecvList)
        //    {
        //        m_RecvList.Add(msg);
        //    } // unlock
        //    return start_index != next_index;
        //}

        public int Process(byte[] buffer, int start_index, int read_cnt)
        {
            int next_index = start_index;
            if (read_cnt - start_index < sizeof(Int16))
                return next_index;
            int msg_size = BitConverter.ToInt16(buffer, start_index);
            if (read_cnt - start_index < msg_size)
                return next_index;
            next_index = start_index + msg_size;

            if (m_HostID == HostID.None)
            {
                m_HostID = BitConverter.ToInt16(buffer, 2);
            }
            else
            {
                NetBuffer msg = new NetBuffer();
                msg.Write(buffer, start_index + sizeof(Int16), msg_size - sizeof(Int16));
                lock (m_RecvList)
                {
                    m_RecvList.Add(msg);
                } // unlock
            }
            return next_index;
        }
        public void Tick(float deltaTime)
        {
            // Connection 처리
            switch (m_State)
            {
                case STATE.CONNECTING:
                    if (m_timeout >= 0)
                    {
                        m_timeout -= deltaTime;
                        if (m_timeout <= 0.0f)
                        {
                            Disconnect(DISCONNECTION_REASON.CONNECTION_FAIL);
                        }
                    }
                    break;
                case STATE.CONNECTED:
                    if (m_connected == false)
                    {
                        m_connected = true;
                        if (m_EventHandler != null)
                        {
                            m_EventHandler.OnNotifyConnected(m_HostID);
                        }
                    }
                    break;
                case STATE.DISCONNECTING:
                    lock (m_RecvList)
                    lock (m_SendList)
                    {
                        if (m_Socket.Connected)
                        {
                            if (m_SendList.Count == 0 && m_RecvList.Count == 0)
                            {
                                m_connected = true;
                                m_State = STATE.DISCONNECTED;
                                m_Socket.Disconnect(false);
                                m_Socket.Close();
                            }
                        }
                        else
                        {
                            m_connected = true;
                            m_State = STATE.DISCONNECTED;
                            m_Socket.Close();
                        }
                    } // unlock
                    break;

                case STATE.DISCONNECTED:
                    if (m_connected)
                    {
                        m_connected = false;
                        Log.Info("Disconnected.");
                        if (m_EventHandler != null)
                        {
                            m_EventHandler.OnNotifyDisConnected(m_reason);
                        }
                        m_HostID = HostID.None;
                    }
                    break;

                default:
                    break;
            }

            while (true)
            {
                NetBuffer msg = null;
                lock (m_RecvList)
                {
                    if (m_RecvList.Count == 0)
                        break;
                    msg = m_RecvList[0];
                } // unlock

                msg.Rewind(0);
                HostID hid = msg.ReadInt16();
                Int32 rid = msg.ReadInt32();
                if (m_EventHandler == null || m_EventHandler.OnReceive(rid, hid, msg) == false)
                {
                    Disconnect();
                    // TODO: error message
                    Log.Debug("Missing RMI: " + rid.ToString());
                }

                lock (m_RecvList)
                {
                    m_RecvList.Remove(msg);
                } // unlock
            }
        } // end of Tick()
    }
}
