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
using System.Collections.Generic;
using System.Text;

namespace Devarc
{
    public class ThreadContainer<T>
        where T : ThreadBase, new()
    {
        public bool isStart { get { return m_isStart; } }

        protected bool m_isInit = false;
        protected bool m_isStart = false;
        protected int m_ThreadCnt;
        protected T[] m_Threads;
        private ManualResetEvent[] m_Events;

        public void Init(int thread_cnt)
        {
            if (m_isInit == false)
            {
                m_ThreadCnt = thread_cnt;
                m_Threads = new T[thread_cnt];
                m_Events = new ManualResetEvent[thread_cnt];
                for (int i = 0; i < thread_cnt; i++)
                {
                    m_Events[i] = new ManualResetEvent(false);
                    m_Threads[i] = new T();
                }
                m_isInit = true;
            }
        }

        public bool StartThread()
        {
            if (m_isInit == true && m_isStart == false)
            {
                m_isStart = true;
                for (int i = 0; i < m_ThreadCnt; i++)
                {
                    m_Threads[i].Start(m_Events[i]);
                }
            }
            return true;
        }

        public bool StopThread(bool wait)
        {
            if (m_isStart == true)
            {
                for (int i = 0; i < m_ThreadCnt; i++)
                {
                    m_Threads[i].Stop();
                }
                if (wait)
                {
                    foreach (ManualResetEvent evt in m_Events)
                    {
                        while (evt.WaitOne(100) == false)
                            Thread.Sleep(100);
                    }
                }
                m_isStart = false;
            }
            return true;
        }
    }
}
