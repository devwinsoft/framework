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

    public class TLockObject<T, KEY>
    {
        ReaderWriterLock m_Lock = new ReaderWriterLock();

        public RLOCK<T> READ_LOCK() { return new RLOCK<T>(m_Lock); }
        public WLOCK<T> WRITE_LOCK() { return new WLOCK<T>(m_Lock); }

        public virtual void OnAlloc(KEY k1) { }
        public virtual void OnFree() { }
        public virtual KEY GetKey1() { return default(KEY); }
    }

    public class TLockContainer<T, KEY> where T : TLockObject<T, KEY>, new()
    {
        public RLOCK<T> READ_LOCK() { return new RLOCK<T>(m_Lock); }
        public WLOCK<T> WRITE_LOCK() { return new WLOCK<T>(m_Lock); }

        private ReaderWriterLock m_Lock = new ReaderWriterLock();
        private Dictionary<KEY, T> m_Table = new Dictionary<KEY, T>();
        protected List<T> m_List = new List<T>();
        private Queue<T> m_Pool = new Queue<T>();

        public TLockContainer(int size)
        {
            for (int i = 0; i < size; i++)
            {
                m_Pool.Enqueue(new T());
            }
        }

        public void SetCapacity(int size)
        {
            for (int i = m_List.Count + m_Pool.Count; i < size; i++)
            {
                m_Pool.Enqueue(new T());
            }
        }

        public void Clear()
        {
            List<T>.Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.OnFree();
                m_Pool.Enqueue(enumerator.Current);
            }
            m_Table.Clear();
            m_List.Clear();
        }

        private T _Alloc(KEY key1)
        {
            T obj = default(T);
            if (m_Pool.Count == 0)
            {
                Log.Debug("Not enough buf class. name:" + typeof(T).ToString());
                return obj;
            }
            if (m_Table.ContainsKey(key1))
            {
                Log.Debug("Cannot alloc. [name]:" + typeof(T).ToString() + "[key1]:" + key1);
                return obj;
            }
            if (m_Pool.Count == 0)
                return default(T);
            obj = m_Pool.Dequeue();
            m_Table.Add(key1, obj);
            m_List.Add(obj);
            return obj;
        }
        public T Alloc(KEY key1)
        {
            T obj = _Alloc(key1);
            if (obj == null)
            {
                return obj;
            }
            obj.OnAlloc(key1);
            return obj;
        }

        public void Free1(KEY key1)
        {
            using (WRITE_LOCK())
            {
                T obj;
                if (m_Table.TryGetValue(key1, out obj) == false)
                {
                    return;
                }
                obj.OnFree();
                m_Table.Remove(key1);
                m_List.Remove(obj);
                m_Pool.Enqueue(obj);
            }
        }

        public T GetAt1(KEY key1)
        {
            T obj;
            m_Table.TryGetValue(key1, out obj);
            return obj;
        }
        public bool Contains1(KEY key1)
        {
            return m_Table.ContainsKey(key1);
        }
        public T ElementAt(int index)
        {
            if (m_List.Count <= index)
            {
                return default(T);
            }
            return m_List[index];
        }
        public List<T>.Enumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }
        public int Count
        {
            get
            {
                return m_List.Count;
            }
        }
    }
}
