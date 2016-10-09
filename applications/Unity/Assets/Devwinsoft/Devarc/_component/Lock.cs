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
using System.Threading;

namespace Devarc
{
    public abstract class ILockObject
    {
        public abstract bool IsReading();
        public abstract bool IsWriting();

        public abstract void LockRead();
        public abstract void UnLockRead();
        public abstract void LockWrite();
        public abstract void UnLockWrite();
    }

    public class LockObject<T> : ILockObject, IDisposable
    {
        private ReaderWriterLock m_Lock = new ReaderWriterLock();

        public LockObject()
        {
        }
        public void Dispose()
        {
            m_Lock = null;
        }

        public override bool IsReading() { return m_Lock.IsReaderLockHeld || m_Lock.IsWriterLockHeld; }
        public override bool IsWriting() { return m_Lock.IsWriterLockHeld; }
        public override void LockRead()
        {
            if (m_Lock.IsReaderLockHeld == false && m_Lock.IsWriterLockHeld == false)
            {
                m_Lock.AcquireReaderLock(-1);
            }
            else
            {
                throw new Exception("Twice Read Lock. " + typeof(T).ToString());
            }
        }
        public override void UnLockRead()
        {
            if (m_Lock.IsReaderLockHeld)
            {
                m_Lock.ReleaseReaderLock();
            }
        }

        public override void LockWrite()
        {
            if (m_Lock.IsReaderLockHeld == false && m_Lock.IsWriterLockHeld == false)
            {
                m_Lock.AcquireWriterLock(-1);
            }
            else
            {
                throw new Exception("Twice Write Lock. " + typeof(T).ToString());
            }
        }
        public override void UnLockWrite()
        {
            if (m_Lock.IsWriterLockHeld)
            {
                m_Lock.ReleaseWriterLock();
            }
        }
    }

    public class WLOCK : IDisposable
    {
        public WLOCK(ILockObject obj)
        {
            m_Lock = obj;
            m_Lock.LockWrite();
        }
        public virtual void Dispose()
        {
            m_Lock.UnLockWrite();
            m_Lock = null;
        }
        protected ILockObject m_Lock;
    }

    public class RLOCK : IDisposable
    {
        public RLOCK(ILockObject obj)
        {
            m_Lock = obj;
            m_Lock.LockRead();
        }
        public virtual void Dispose()
        {
            m_Lock.UnLockRead();
            m_Lock = null;
        }
        protected ILockObject m_Lock;
    }
}
