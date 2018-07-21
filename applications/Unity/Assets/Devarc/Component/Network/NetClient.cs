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
using System.Text;
using System.Net;
//using System.Net.Sockets;
using SuperSocket.ProtoBase;
using SuperSocket.ClientEngine;


namespace Devarc
{
    public class NetClient : INetworker
    {
        public event NET_RECEIVER OnReceiveData;

        public EasyClient Client
        {
            get { return client; }
        }
        EasyClient client = new EasyClient();

        public NetTrigger Trigger
        {
            get { return trigger; }
        }
        NetTrigger trigger = new NetTrigger();

        public HostID Hid { get { return mHid; } }
        HostID mHid = HostID.None;
        short mSeq = 0;

        protected virtual void OnConnected() { }
        protected virtual void OnDisConnected() { }

        public NetClient()
        {
            client.Connected += OnSession_Connected;
            client.Closed += OnSession_Closed;
            client.Error += new EventHandler<ErrorEventArgs>(OnSessionError);
            client.Initialize<NetClientPackageInfo>(new NetClientReceiveFilter(), OnDataReceived);
        }

        void OnSession_Connected(object sender, EventArgs e)
        {
            EasyClient c = sender as EasyClient;
            if (c != null && c.IsConnected == false)
            {
                Disconnect();
                return;
            }
            OnConnected();
        }

        void OnSession_Closed(object sender, EventArgs e)
        {
            OnDisConnected();
            mHid = HostID.None;
            mSeq = 0;
        }

        void OnDataReceived(NetClientPackageInfo package)
        {
            switch ((RMI_BASIC)package.Msg.Rmi)
            {
                case RMI_BASIC.INIT_HOST_ID:
                    // Setup Client HostID
                    if (mHid == HostID.None)
                    {
                        mHid = package.Msg.Hid;
                        mSeq = 1;
                    }
                    else
                    {
                        // warning
                    }
                    break;
                case RMI_BASIC.UNKNOWN_REQUEST:
                    Log.Error("Disconnected by server. reason = UNKNOWN_REQUEST");
                    Disconnect();
                    break;
                default:
                    var handler = this.OnReceiveData;
                    if (handler != null)
                    {
                        handler(this, package.Msg);
                    }
                    trigger.OnReceiveData(package.Msg);

                    NetBufferPool.Instance.Push(package.Msg);
                    break;
            }
        }

        void OnSessionError<DataEventArgs>(object sender, DataEventArgs e)
        {
            ErrorEventArgs ex = e as ErrorEventArgs;
            Log.Exception(ex.Exception);
        }

        public short GetCurrentSeq(HostID hid)
        {
            return 0;
        }

        public bool Send(NetBuffer msg)
        {
            if (client.IsConnected == false)
            {
                NetBufferPool.Instance.Push(msg);
                return false;
            }

            bool success = false;
            try
            {
                msg.UpdateHeader(mSeq++);
                client.Send(msg.Data);
                success = true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
            return success;
        }

#if !UNITY_5 && !UNITY_2017
        async System.Threading.Tasks.Task<bool> connect(EndPoint endPoint)
        {
            var ret = await client.ConnectAsync(endPoint);
            if (ret)
                Log.Debug("Connection established");
            else
                Log.Debug("[FAIL] Cannot connect to: {0}", endPoint);
            return true;
        }
#endif

        public bool Connect(string address, int port)
        {
            IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse(address), port);
            EndPoint endPoint = ipServer as EndPoint;
            if (endPoint == null)
            {
                return false;
            }
            Log.Debug("Connecting to {0}:{1}", address, port);
#if UNITY_5 || UNITY_2017
            client.BeginConnect(endPoint);
#else
            System.Threading.Tasks.Task<bool> result = connect(endPoint);
#endif
            return true;

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
            //return true;
        }

        public void Disconnect()
        {
            client.Close();
        }
    }
}

