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
    public class NetClient : IProxyBase
    {
        public event NET_RECEIVER OnReceiveData;

        public EasyClient Client { get { return client; } }
        EasyClient client = new EasyClient();

        public HostID Hid { get { return mHid; } }
        HostID mHid = HostID.None;

        public bool Send(HostID hid, ArraySegment<byte> data)
        {
            client.Send(data);
            return true;
        }

        async System.Threading.Tasks.Task<bool> connect(EndPoint endPoint)
        {
            var ret = await client.ConnectAsync(endPoint);
            Log.Debug("Connection established");
            return true;
        }

        public bool Connect(string address, int port)
        {
            client.Connected += OnSession_Connected;
            client.Closed += OnSession_Closed;
            client.Error += new EventHandler<ErrorEventArgs>(OnSession_Error);
            client.Initialize<NetClientPackageInfo>(new NetClientReceiveFilter(), OnSession_DataReceived);

            //var taskCompleteSrc = new System.Threading.Tasks.TaskCompletionSource<NetClientPackageInfo>();
            //client.Initialize(new NetClientReceiveFilter(), (p) =>
            //{
            //    taskCompleteSrc.SetResult(p);
            //});

            IPEndPoint ipServer = new IPEndPoint(IPAddress.Parse(address), port);
            EndPoint epTemp = ipServer as EndPoint;
            if (epTemp == null)
            {
                return false;
            }

            System.Threading.Tasks.Task<bool> result = connect(epTemp);
            return true;

            //switch (m_State)
            //{
            //    case STATE.DISCONNECTED:
            //        m_State = STATE.CONNECTING;
            //        m_HostID = HostID.None;
            //        break;
            //    case STATE.CONNECTING:
            //        Log.Info("Already connecting.");
            //        return false;
            //    case STATE.CONNECTED:
            //        Log.Info("Already connected.");
            //        return false;
            //    default:
            //        Log.Info("Cannot connect now.");
            //        return false;
            //}
            //if (m_Session != null && m_Session.IsConnected)
            //{
            //    Log.Info("Already connected.");
            //    return false;
            //}

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

        public void Disconnect(DISCONNECTION_REASON reason)
        {
            mHid = HostID.None;
            client.Close();
        }

        void OnSession_Connected(object sender, EventArgs e)
        {
            EasyClient c = sender as EasyClient;
            if (c != null)
            {
                if (c.IsConnected)
                {
                }
                else
                {
                    Disconnect(DISCONNECTION_REASON.CONNECTION_FAIL);
                }
            }
        }

        void OnSession_Closed(object sender, EventArgs e)
        {
            Disconnect(DISCONNECTION_REASON.BY_SERVER);
        }

        void OnSession_DataReceived(NetClientPackageInfo package)
        {
            if (package.Msg.Rmi == -1)
            {
                // Setup Client HostID
                if (mHid == 0)
                {
                    mHid = package.Msg.Hid;
                }
                else
                {
                    // warning
                }
            }
            else
            {
                var handler = this.OnReceiveData;
                if (handler != null)
                {
                    handler(this, package.Msg);
                }
            }
        }

        void OnSession_Error<DataEventArgs>(object sender, DataEventArgs e)
        {
            ErrorEventArgs ex = e as ErrorEventArgs;
            Log.Exception(ex.Exception);
			throw new NotImplementedException();
        }
    }
}

