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
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    public enum THREAD_STATE
    {
        STOP,
        RUN,
        READY_TO_STOP,
    }

    public abstract class ThreadBase
    {
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void Tick();

        private ManualResetEvent m_Event;
        private Thread m_Thread = null;
        private object m_BaseLockObj = new object();
        private THREAD_STATE m_State = THREAD_STATE.STOP;

        public void Start(ManualResetEvent evt)
        {
            if (m_State == THREAD_STATE.STOP)
            {
                ThreadStart start = new ThreadStart(main);
                m_Event = evt;
                m_Thread = new Thread(start);
                m_Thread.IsBackground = true;
                m_Thread.Start();
            }
        }

        public void Stop()
        {
            lock (m_BaseLockObj)
            {
                if (m_State != THREAD_STATE.STOP)
                {
                    m_State = THREAD_STATE.READY_TO_STOP;
                    m_Thread = null;
                }
            }
        }

        void main()
        {
            lock (m_BaseLockObj)
            {
                m_State = THREAD_STATE.RUN;
            }
            try
            {
                OnStart();
                while (true)
                {
                    if (m_State == THREAD_STATE.READY_TO_STOP)
                    {
                        break;
                    }
                    Tick();
                    System.Threading.Thread.Sleep(10);
                }
                lock (m_BaseLockObj)
                {
                    m_State = THREAD_STATE.STOP;
                }
                OnStop();
            }
            finally
            {
                m_Event.Set();
            }
        }
    }
}
