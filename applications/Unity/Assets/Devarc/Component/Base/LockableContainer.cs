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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Devarc
{
    public class LockableContainer<ME, KEY1> where ME : TContents<ME, KEY1>, new()
    {
        public RLOCK<ME> READ_LOCK() { return new RLOCK<ME>(m_Lock); }
        public WLOCK<ME> WRITE_LOCK() { return new WLOCK<ME>(m_Lock); }

        private ReaderWriterLock m_Lock = new ReaderWriterLock();
        private Dictionary<KEY1, ME> m_Table = new Dictionary<KEY1, ME>();
        protected List<ME> m_List = new List<ME>();
        private Queue<ME> m_Pool = new Queue<ME>();

        public LockableContainer(int size)
        {
            for (int i = 0; i < size; i++)
            {
                m_Pool.Enqueue(new ME());
            }
        }

        public void SetCapacity(int size)
        {
            for (int i = m_List.Count + m_Pool.Count; i < size; i++)
            {
                m_Pool.Enqueue(new ME());
            }
        }

        public void Clear()
        {
            List<ME>.Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.OnFree();
                m_Pool.Enqueue(enumerator.Current);
            }
            m_Table.Clear();
            m_List.Clear();
        }

        private ME _Alloc(KEY1 key1)
        {
            ME obj = default(ME);
            if (m_Pool.Count == 0)
            {
                Log.Debug("Not enough buf class. name:" + typeof(ME).ToString());
                return obj;
            }
            if (m_Table.ContainsKey(key1))
            {
                Log.Debug("Cannot alloc. [name]:" + typeof(ME).ToString() + "[key1]:" + key1);
                return obj;
            }
            if (m_Pool.Count == 0)
                return default(ME);
            obj = m_Pool.Dequeue();
            m_Table.Add(key1, obj);
            m_List.Add(obj);
            return obj;
        }
        public ME Alloc(KEY1 key1)
        {
            ME obj = _Alloc(key1);
            if (obj == null)
            {
                return obj;
            }
            obj.OnAlloc(key1);
            return obj;
        }

        public void Free1(KEY1 key1)
        {
            using (WRITE_LOCK())
            {
                ME obj;
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

        public ME GetAt1(KEY1 key1)
        {
            ME obj;
            m_Table.TryGetValue(key1, out obj);
            return obj;
        }
        public bool Contains1(KEY1 key1)
        {
            return m_Table.ContainsKey(key1);
        }
        public ME ElementAt(int index)
        {
            if (m_List.Count <= index)
            {
                return default(ME);
            }
            return m_List[index];
        }
        public List<ME>.Enumerator GetEnumerator()
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
