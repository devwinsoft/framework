﻿//
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
using Devarc;
using SuperSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;


namespace Devarc
{
    public class NetServer : AppServer<NetSession, NetRequestInfo>, IProxyBase
    {
        public event NET_RECEIVER OnReceiveData;
        private Dictionary<HostID, NetSession> mSessions = new Dictionary<HostID, NetSession>();
        HostID mNextHostID = (HostID)1000;

        public NetServer()
            : base(new DefaultReceiveFilterFactory<NetServerReceiveFilter, NetRequestInfo>())
        {
        }

        public bool Send(HostID hid, ArraySegment<byte> data)
        {
            NetSession session = GetSession(hid);
            if (session == null || session.Connected == false)
            {
                return false;
            }
            session.Send(data);
            return true;
        }

        protected override void OnStarted()
        {
            base.OnStarted();
            Log.Debug("OnStarted");
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Log.Debug("OnStopped");
        }

        protected override void OnNewSessionConnected(NetSession session)
        {
            base.OnNewSessionConnected(session);

            RegisterSession(session);

            // Send Client Host ID
            NetBuffer msg = new NetBuffer();
            msg.Init(-1, session.Hid);
            msg.Write(msg.Hid);
            msg.UpdateData();
            session.Send(msg.Data);

            Log.Debug("OnNewSessionConnected");
        }

        protected override void OnSessionClosed(NetSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
            UnRegisterSession(session.Hid);

            Log.Debug("OnSessionClosed: {0}", reason);
        }


        protected override void ExecuteCommand(NetSession session, NetRequestInfo requestInfo)
        {
            var handler = this.OnReceiveData;
            if (handler != null)
            {
                handler(this, requestInfo.Msg);
            }
        }

        public NetSession GetSession(HostID hid)
        {
            NetSession session = null;
            lock (mSessions)
            {
                mSessions.TryGetValue(hid, out session);
            }
            return session;
        }

        void RegisterSession(NetSession session)
        {
            lock (mSessions)
            {
                HostID value = mNextHostID;
                while (mSessions.ContainsKey(value))
                {
                    if (value > 30000)
                    {
                        value = 1000;
                    }
                    else
                    {
                        value++;
                    }
                }
                mNextHostID = (HostID)(value + 1);

                session.Hid = value;
                mSessions.Add(value, session);
            }
        }

        bool UnRegisterSession(HostID hid)
        {
            lock (mSessions)
            {
                return mSessions.Remove(hid);
            }
        }
    } // end class
} // end of namespace
