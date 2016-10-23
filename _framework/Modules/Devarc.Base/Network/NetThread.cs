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
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Devarc
{
    class NetThreadList : ThreadContainer<NetThread>
    {
        public void Init(int thread_cnt, NetServer svr, IServerStub stub)
        {
            base.Init(thread_cnt);
            foreach (NetThread obj in m_Threads)
            {
                obj.Init(svr, stub);
            }
        }
    }

    class NetThread : ThreadBase
    {
        private NetServer m_Server;
        private IServerStub m_Stub;

        public void Init(NetServer svr, IServerStub stub)
        {
            m_Server = svr;
            m_Stub = stub;
        }

        protected override void OnStart()
        {
        }
        protected override void OnStop()
        {
        }

        protected override void Tick()
        {
            HostID host_from = 0;
            HostID host_to = 0;
            NetMessage obj = null;
            Int32 rid = 0;

            lock (m_Server.WORKING)
            {
                foreach (NetMessage temp in m_Server.WORKING.Values)
                {
                    if (temp.busy == false)
                    {
                        temp.busy = true;
                        obj = temp;
                        break;
                    }
                }
            } // UNLOCK
            if (obj == null)
                return;

            try
            {
                obj.msg.Rewind(0);
                host_from = obj.hid;
                host_to = obj.msg.ReadInt16();
                rid = obj.msg.ReadInt32();

                if (host_to == HostID.Server)
                {
                    if (m_Stub == null || m_Stub.OnReceive(rid, host_from, obj.msg) == false)
                    {
                        m_Server.Disconnect(host_from);
                        Log.Error("Missing RMI: " + rid.ToString() + " HostID:" + host_from.ToString());
                    }
                }
                else
                {
                    m_Server.RmiSend(obj.hid, host_to, obj.msg);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
            finally
            {
                // 완료 메시지 삭제
                // 보류 메시지 추가
                lock (m_Server.PENDING)
                lock (m_Server.WORKING)
                {
                    m_Server.WORKING.Remove(host_from);
                    NetMessage new_msg = null;
                    foreach (NetMessage temp in m_Server.PENDING)
                    {
                        if (m_Server.WORKING.ContainsKey(temp.hid) == false)
                        {
                            new_msg = temp;
                            break;
                        }
                    }
                    if (new_msg != null)
                    {
                        m_Server.WORKING.Add(new_msg.hid, new_msg);
                        m_Server.PENDING.Remove(new_msg);
                    }
                } // unlock
            }
        }
    }
}
