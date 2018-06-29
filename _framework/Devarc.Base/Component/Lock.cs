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
    public class WLOCK<T> : IDisposable
    {
        public WLOCK(ReaderWriterLock obj)
        {
            m_Lock = obj;
            if (m_Lock.IsReaderLockHeld == false && m_Lock.IsWriterLockHeld == false)
            {
                m_Lock.AcquireWriterLock(-1);
            }
            else
            {
                throw new Exception("Twice Write Lock. " + typeof(T).ToString());
            }
        }
        public virtual void Dispose()
        {
            if (m_Lock.IsWriterLockHeld)
            {
                m_Lock.ReleaseWriterLock();
            }
            m_Lock = null;
        }
        protected ReaderWriterLock m_Lock;
    }

    public class RLOCK<T> : IDisposable
    {
        public RLOCK(ReaderWriterLock obj)
        {
            m_Lock = obj;
            if (m_Lock.IsReaderLockHeld == false && m_Lock.IsWriterLockHeld == false)
            {
                m_Lock.AcquireReaderLock(-1);
            }
            else
            {
                throw new Exception("Twice Read Lock. " + typeof(T).ToString());
            }
        }
        public virtual void Dispose()
        {
            if (m_Lock.IsReaderLockHeld)
            {
                m_Lock.ReleaseReaderLock();
            }
            m_Lock = null;
        }
        protected ReaderWriterLock m_Lock;
    }
}
