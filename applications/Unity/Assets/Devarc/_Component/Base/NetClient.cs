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
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;

namespace Devarc
{
    public class NetClient
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

        public HostID GetMyHostID()
        {
            return m_HostID;
        }
        HostID m_HostID;

        IClientStub m_EventHandler = null;

        AsyncTcpSession m_Session;

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

            IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse(address), port);
            EndPoint epTemp = ipServer as EndPoint;

            m_Session = new AsyncTcpSession();

            m_Session.Connected += m_Session_Connected;
            m_Session.Closed += m_Session_Closed;
            m_Session.DataReceived += m_Session_DataReceived;
            m_Session.Error += new EventHandler<ErrorEventArgs>(m_Session_Error);

            m_Session.Connect(epTemp);

            //// get ip address
            //IPAddress[] ip_list = Dns.GetHostAddresses(address);
            //IPAddress ip = null;
            //foreach (IPAddress obj in ip_list)
            //{
            //    if (obj.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        ip = obj;
            //        break;
            //    }
            //}
            //if (ip == null)
            //{
            //    Log.Info("Cannot connect to : " + address);
            //    return false;
            //}
            //bool success = false;
            //m_reason = DISCONNECTION_REASON.CONNECTION_FAIL;
            //m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //m_Socket.NoDelay = true;
            //try
            //{
            //    Log.Info("Connecting to " + ip.ToString() + ":" + port.ToString());
            //    m_Socket.BeginConnect(ip, port, OnConnect, m_Socket);
            //    success = true;
            //}
            //catch (SocketException e)
            //{
            //    Log.Exception(e);
            //    success = false;
            //}
            //if (success && m_EventHandler != null)
            //{
            //    m_EventHandler.OnNotifyConnecting();
            //}
            return true;
        }

        void m_Session_Connected(object sender, EventArgs e)
        {
            if (true == m_Session.IsConnected)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes("ABC");
                m_Session.Send(data, 0, data.Length);
            }
            else
            {
                Disconnect(DISCONNECTION_REASON.CONNECTION_FAIL);
            }
        }

        void m_Session_Closed(object sender, EventArgs e)
        {
            Disconnect(DISCONNECTION_REASON.BY_SERVER);
        }

        void m_Session_DataReceived<DataEventArgs>(object sender, DataEventArgs e)
        {
            SuperSocket.ClientEngine.DataEventArgs dataArgs = e as SuperSocket.ClientEngine.DataEventArgs;
            if (true == m_Session.IsConnected)
            {
                Log.Debug("m_Session_DataReceived");
            }
            else
            {
                Disconnect();
            }
        }

        void m_Session_Error<DataEventArgs>(object sender, DataEventArgs e)
        {
            ErrorEventArgs ex = e as ErrorEventArgs;
            Log.Exception(ex.Exception);
			throw new NotImplementedException();
        }


        //void OnConnect(IAsyncResult ar)
        //{
        //    Socket socket = (Socket)ar.AsyncState;
        //    try
        //    {
        //        if (m_Socket != socket)
        //        {
        //            if (socket.Connected)
        //                socket.EndConnect(ar);
        //            socket.Close();
        //            return;
        //        }
        //        else if (socket.Connected == false)
        //        {
        //            if (m_State == STATE.CONNECTING)
        //            {
        //                m_connected = true;
        //                m_State = STATE.DISCONNECTED;
        //            }
        //            return;
        //        }
        //        else
        //        {
        //            socket.EndConnect(ar);
        //        }
        //        NetDataRecv state = new NetDataRecv(1024);
        //        state.read_cnt = 0;
        //        state.socket = socket;
        //        socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveHostID), state);
        //        m_reason = DISCONNECTION_REASON.BY_SERVER;
        //        Log.Info("Connected.");
        //    }
        //    catch (SocketException se)
        //    {
        //        m_connected = true;
        //        m_State = STATE.DISCONNECTED;
        //        switch (se.SocketErrorCode)
        //        {
        //            case SocketError.ConnectionAborted:
        //            case SocketError.ConnectionRefused:
        //                Log.Info("Cannot connect.");
        //                break;
        //            default:
        //                Log.Exception(se);
        //                break;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        m_State = STATE.DISCONNECTED;
        //        Log.Exception(e);
        //    }
        //}

        public void Disconnect()
        {
            Disconnect(DISCONNECTION_REASON.BY_USER);
        }
        public void Disconnect(DISCONNECTION_REASON reason)
        {
            m_Session.Close();

            //if (m_State == STATE.CONNECTED)
            //{
            //    Log.Info("Disconnecting.");
            //    m_State = STATE.DISCONNECTING;
            //    m_connected = true;
            //    m_Socket.Shutdown(SocketShutdown.Both);
            //}
            //else if (m_State == STATE.CONNECTING)
            //{
            //    Log.Info("Disconnecting.");
            //    if (m_Socket.Connected)
            //    {
            //        m_State = STATE.DISCONNECTING;
            //        m_connected = true;
            //        m_Socket.Shutdown(SocketShutdown.Both);
            //    }
            //    else
            //    {
            //        m_State = STATE.DISCONNECTED;
            //        m_connected = true;
            //        //m_Socket.Close();
            //    }
            //}
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

            //try
            //{
            //    lock (m_SendList)
            //    {
            //        m_SendList.Add(msg);
            //        if (m_SendList.Count == 1)
            //        {
            //            NetDataSend state = new NetDataSend();
            //            state.buf = new byte[1024];
            //            state.socket = m_Socket;
            //            state.send_cnt = 0;
            //            state.msg_size = msg.Length + sizeof(Int16);
            //            Array.Copy(BitConverter.GetBytes((Int16)state.msg_size), state.buf, sizeof(Int16));
            //            Array.Copy(msg.ToArray(), 0, state.buf, sizeof(Int16), msg.Length);
            //            m_Socket.BeginSend(state.buf, 0, state.msg_size, SocketFlags.None, new AsyncCallback(OnSend), state);
            //        }
            //    } // unlock
            //    success = true;
            //}
            //finally
            //{
            //    if (success == false)
            //        msg.Clear();
            //}
            return success;
        }





    }
}
