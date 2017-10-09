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
using System.Runtime.CompilerServices;
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

    public class WorkThreadBase
    {
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }

        private ManualResetEvent mStopRequest;
        private ManualResetEvent mStopNotify;
        private Thread m_Thread = null;

        private NetServer mServer;
        private WorkThreadContainer mContainer;
        private THREAD_STATE mState = THREAD_STATE.STOP;

        public WorkThreadBase(NetServer _server, WorkThreadContainer _container)
        {
            mServer = _server;
            mContainer = _container;
        }


        public void Start()
        {
            if (mState == THREAD_STATE.STOP)
            {
                mStopRequest = new ManualResetEvent(false);
                mStopNotify = null;

                ThreadStart start = new ThreadStart(main);
                m_Thread = new Thread(start);
                m_Thread.IsBackground = true;
                m_Thread.Start();
            }
        }

        public void Stop(ManualResetEvent evt)
        {
            if (mState == THREAD_STATE.RUN)
            {
                mStopRequest.Set();
                mStopNotify = evt;
                mState = THREAD_STATE.READY_TO_STOP;
            }
        }



        void main()
        {
            mState = THREAD_STATE.RUN;
            try
            {
                OnStart();
                while (true)
                {
                    NetBuffer msg = mContainer.GetWork();
                    if (msg != null)
                    {
                        mServer.DispatchMsg(this, msg);
                        mContainer.CompleteWork(msg);
                        NetBufferPool.Instance.Push(msg);
                    }
                    if (mStopRequest.WaitOne(1))
                        break;
                }
                OnStop();
            }
            finally
            {
                if (mStopNotify != null)
                {
                    mStopNotify.Set();
                    mStopNotify = null;
                }

                mStopRequest.Close();
                mStopRequest = null;
                m_Thread = null;
            }
        }
    }
}
