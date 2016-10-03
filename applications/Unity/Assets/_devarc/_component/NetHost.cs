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
    internal class NetHost : IContents<HostID>
    {
        public enum STATE
        {
            CONNECTING,
            CONNECTED,
            DISCONNECTING,
            DISCONNECTED,
        }

        public HostID GetKey1()
        {
            return host_id;
        }
        public void OnAlloc(HostID key1)
        {
            state = STATE.DISCONNECTED;
            host_id = key1;
            send.Clear();
            recv.Clear();
        }
        public void OnFree()
        {
            socket = null;
            foreach (NetBuffer msg in send)
            {
                msg.Clear();
            }
        }

        public STATE state { get; set; }
        public HostID host_id { get; private set; }
        public Socket socket { get; set; }
        public List<NetBuffer> send = new List<NetBuffer>();
        public NetBuffer recv = new NetBuffer();
    }

    internal class NetHostList : Container_S1<NetHost, HostID>
    {
        public NetHostList(int pool_size)
            : base(pool_size)
        {
            m_NextHostID = HostID.TcpHostStart;
        }

        public NetHost Create(Socket sock)
        {
            NetHost obj = null;
            while (Contains1(m_NextHostID))
            {
                m_NextHostID++;
                if (m_NextHostID > HostID.TcpHostEnd)
                {
                    m_NextHostID = HostID.TcpHostStart;
                }
            }
            obj = Alloc(m_NextHostID);
            obj.state = NetHost.STATE.CONNECTED;
            obj.socket = sock;
            m_NextHostID++;
            return obj;
        }

        private HostID m_NextHostID;
    }
}
