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
    public class WorkThreadContainer
    {
        //public RLOCK<WorkList> READ_LOCK() { return new RLOCK<WorkList>(m_Lock); }
        //public WLOCK<WorkList> WRITE_LOCK() { return new WLOCK<WorkList>(m_Lock); }
        //ReaderWriterLock m_Lock = new ReaderWriterLock();

        public bool IsStart { get { return mIsStart; } }

        protected bool mIsInit = false;
        protected bool mIsStart = false;
        protected int m_ThreadCnt;
        protected WorkThreadBase[] mThreads;
        private ManualResetEvent[] mEvents;
        private object mLockObject = new object();

        private NetServer mServer;
        private HashSet<HostID> mBusyList = new HashSet<HostID>();
        private WorkList mWorkList = new WorkList();

        public WorkThreadContainer(NetServer _server)
        {
            mServer = _server;
        }

        public void Init(int _threadCnt)
        {
            if (mIsInit == false)
            {
                m_ThreadCnt = _threadCnt;
                mThreads = new WorkThreadBase[_threadCnt];
                mEvents = new ManualResetEvent[_threadCnt];
                for (int i = 0; i < _threadCnt; i++)
                {
                    mEvents[i] = new ManualResetEvent(false);
                    mThreads[i] = new WorkThreadBase(mServer, this);
                }
                mIsInit = true;
            }
        }


        public bool StartAll()
        {
            lock (mLockObject)
            {
                if (mIsInit == false || mIsStart)
                {
                    return false;
                }
                mIsStart = true;
                for (int i = 0; i < m_ThreadCnt; i++)
                {
                    mThreads[i].Start();
                }
            }

            return true;
        }

        public bool StopAll(bool _wait)
        {
            lock (mLockObject)
            {
                if (mIsStart == false)
                {
                    return false;
                }
                for (int i = 0; i < m_ThreadCnt; i++)
                {
                    ManualResetEvent evt = mEvents[i];
                    mThreads[i].Stop(evt);
                }
            }
            if (_wait)
            {
                for (int i = 0; i < mEvents.Length; i++)
                {
                    ManualResetEvent evt = mEvents[i];
                    while (evt.WaitOne(100) == false)
                        Thread.Sleep(100);
                }
            }
            mIsStart = false;
            return true;
        }

        public void RegisterWork(NetBuffer _msg)
        {
            lock (mLockObject)
            {
                mWorkList.Push(_msg);
            }
        }

        public void CompleteWork(NetBuffer _msg)
        {
            lock (mLockObject)
            {
                mBusyList.Remove(_msg.Hid);
            }
        }

        public NetBuffer GetWork()
        {
            lock (mLockObject)
            {
                NetBuffer msg = mWorkList.Pop(mBusyList);
                if (msg != null)
                {
                    mBusyList.Add(msg.Hid);
                }
                return msg;
            }
        }
    }
}
