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

namespace Devarc
{
    public class Container_S1<ME>
        where ME : IContents, new()
    {
        public RLOCK READ_LOCK() { return new RLOCK(m_Lock); }
        public WLOCK WRITE_LOCK() { return new WLOCK(m_Lock); }

        private LockObject<ME> m_Lock = new LockObject<ME>();
        protected List<ME> m_ObjList = new List<ME>();
        private Queue<ME> m_BufList = new Queue<ME>();

        public Container_S1(int size)
        {
            for (int i = 0; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void SetCapacity(int size)
        {
            for (int i = m_ObjList.Count + m_BufList.Count; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void Clear()
        {
            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Clear without write-lock. name:" + typeof(ME).ToString());
            }
            foreach (ME obj in ToArray())
            {
                Free(obj);
            }
        }

        public ME Alloc()
        {
            ME obj = default(ME);

            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Alloc without write-lock. name:" + typeof(ME).ToString());
            }
            if (m_BufList.Count == 0)
            {
                Log.Message(LOG_TYPE.DEBUG, "Not enough buf class. name:" + typeof(ME).ToString());
                return obj;
            }

            if (m_BufList.Count == 0)
                return default(ME);
            obj = m_BufList.Dequeue();
            m_ObjList.Add(obj);
            obj.OnAlloc();
            return obj;
        }

        public void Free(ME obj)
        {
            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Free without write-lock. name:" + typeof(ME).ToString());
            }
            if (m_ObjList.Contains(obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }
        public ME ElementAt(int index)
        {
            if (m_ObjList.Count <= index)
            {
                return default(ME);
            }
            return m_ObjList[index];
        }
        public ME[] ToArray()
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("ToArray() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjList.ToArray();
        }
        public int Count
        {
            get
            {
                if (m_Lock.IsReading() == false)
                {
                    throw new Exception("Count() without lock. name:" + typeof(ME).ToString());
                }
                return m_ObjList.Count;
            }
        }
    }



    public class Container_S1<ME, KEY1>
        where ME : IContents<KEY1>, new()
    {
        public RLOCK READ_LOCK() { return new RLOCK(m_Lock); }
        public WLOCK WRITE_LOCK() { return new WLOCK(m_Lock); }

        private LockObject<ME> m_Lock = new LockObject<ME>();
        private Dictionary<KEY1, ME> m_ObjTable1 = new Dictionary<KEY1, ME>();
        protected List<ME> m_ObjList = new List<ME>();
        private Queue<ME> m_BufList = new Queue<ME>();

        public Container_S1(int size)
        {
            for (int i = 0; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void SetCapacity(int size)
        {
            for (int i = m_ObjList.Count + m_BufList.Count; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void Clear()
        {
            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Clear without write-lock. name:" + typeof(ME).ToString());
            }
            foreach (ME obj in ToArray())
            {
                Free1(obj.GetKey1());
            }
        }

        private ME _Alloc(KEY1 key1)
        {
            ME obj = default(ME);

            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Alloc without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }
            if (m_BufList.Count == 0)
            {
                Log.Message(LOG_TYPE.DEBUG, "Not enough buf class. name:" + typeof(ME).ToString());
                return obj;
            }
            if (m_ObjTable1.ContainsKey(key1))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key1]:" + key1);
                return obj;
            }
            if (m_BufList.Count == 0)
                return default(ME);
            obj = m_BufList.Dequeue();
            m_ObjTable1.Add(key1, obj);
            m_ObjList.Add(obj);
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
            if (m_Lock.IsWriting() == false)
            {
                throw new Exception("Free without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }
            ME obj;
            if (m_ObjTable1.TryGetValue(key1, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(key1);
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }

        public ME GetAt1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("operator[] without lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }

            ME obj;
            return m_ObjTable1.TryGetValue(key1, out obj) ? obj : default(ME);
        }
        public bool Contains1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable1.ContainsKey(key1);
        }
        public ME ElementAt(int index)
        {
            if (m_ObjList.Count <= index)
            {
                return default(ME);
            }
            return m_ObjList[index];
        }
        public ME[] ToArray()
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("ToArray() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjList.ToArray();
        }
        public int Count
        {
            get
            {
                if (m_Lock.IsReading() == false)
                {
                    throw new Exception("Count() without lock. name:" + typeof(ME).ToString());
                }
                return m_ObjList.Count;
            }
        }
    }





    public class Container_S1<ME, KEY1, KEY2>
        where ME : IContents<KEY1, KEY2>, new()
    {
        public RLOCK READ_LOCK() { return new RLOCK(m_Lock); }
        public WLOCK WRITE_LOCK() { return new WLOCK(m_Lock); }

        private LockObject<ME> m_Lock = new LockObject<ME>();
        private Dictionary<KEY1, ME> m_ObjTable1 = new Dictionary<KEY1, ME>();
        private Dictionary<KEY2, ME> m_ObjTable2 = new Dictionary<KEY2, ME>();
        protected List<ME> m_ObjList = new List<ME>();
        private Queue<ME> m_BufList = new Queue<ME>();

        public Container_S1(int pool_size)
        {
            for (int i = 0; i < pool_size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void SetCapacity(int size)
        {
            for (int i = m_ObjList.Count + m_BufList.Count; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void Clear()
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Clear without write-lock. name:" + typeof(ME).ToString());
            }
            foreach (ME obj in ToArray())
            {
                Free1(obj.GetKey1());
            }
        }

        private ME _Alloc(KEY1 key1, KEY2 key2)
        {
            ME obj = default(ME);
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Alloc without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString() + " key2:" + key2.ToString());
            }
            if (m_BufList.Count == 0)
            {
                Log.Message(LOG_TYPE.DEBUG, "Not enough buf class. name:" + typeof(ME).ToString());
                return obj;
            }
            if (m_ObjTable1.ContainsKey(key1))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key1]:" + key1);
                return obj;
            }
            if (m_ObjTable2.ContainsKey(key2))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key2]:" + key2);
                return obj;
            }
            if (m_BufList.Count == 0)
                return default(ME);
            obj = m_BufList.Dequeue();
            m_ObjTable1.Add(key1, obj);
            m_ObjTable2.Add(key2, obj);
            m_ObjList.Add(obj);
            return obj;
        }
        public ME Alloc(KEY1 key1, KEY2 key2)
        {
            ME obj = _Alloc(key1, key2);
            if (obj == null)
            {
                return obj;
            }
            obj.OnAlloc(key1, key2);
            return obj;
        }

        public void Free1(KEY1 key1)
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Free without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }
            ME obj;
            if (m_ObjTable1.TryGetValue(key1, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(obj.GetKey1());
            m_ObjTable2.Remove(obj.GetKey2());
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }
        public void Free2(KEY2 key2)
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Free without write-lock. name:" + typeof(ME).ToString() + " key2:" + key2.ToString());
            }
            ME obj;
            if (m_ObjTable2.TryGetValue(key2, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(obj.GetKey1());
            m_ObjTable2.Remove(obj.GetKey2());
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }

        public ME GetAt1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "operator[] without lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }

            ME obj;
            return m_ObjTable1.TryGetValue(key1, out obj) ? obj : default(ME);
        }
        public ME GetAt2(KEY2 key2)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "operator[] without lock. name:" + typeof(ME).ToString() + " key2:" + key2.ToString());
            }

            ME obj;
            return m_ObjTable2.TryGetValue(key2, out obj) ? obj : default(ME);
        }
        public bool Contains1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable1.ContainsKey(key1);
        }
        public bool Contains2(KEY2 key2)
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable2.ContainsKey(key2);
        }
        public ME ElementAt(int index)
        {
            if (m_ObjList.Count <= index)
            {
                return default(ME);
            }
            return m_ObjList[index];
        }
        public ME[] ToArray()
        {
            if (m_Lock.IsReading() == false)
            {
                throw new Exception("ToArray() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjList.ToArray();
        }
        public int Count
        {
            get
            {
                if (m_Lock.IsReading() == false)
                {
                    throw new Exception("Count() without lock. name:" + typeof(ME).ToString());
                }
                return m_ObjList.Count;
            }
        }
    }




    public class Container_S1<ME, KEY1, KEY2, KEY3>
        where ME : IContents<KEY1, KEY2, KEY3>, new()
    {
        public RLOCK READ_LOCK() { return new RLOCK(m_Lock); }
        public WLOCK WRITE_LOCK() { return new WLOCK(m_Lock); }

        private LockObject<ME> m_Lock = new LockObject<ME>();
        private Dictionary<KEY1, ME> m_ObjTable1 = new Dictionary<KEY1, ME>();
        private Dictionary<KEY2, ME> m_ObjTable2 = new Dictionary<KEY2, ME>();
        private Dictionary<KEY3, ME> m_ObjTable3 = new Dictionary<KEY3, ME>();
        protected List<ME> m_ObjList = new List<ME>();
        private Queue<ME> m_BufList = new Queue<ME>();

        public Container_S1(int pool_size)
        {
            for (int i = 0; i < pool_size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void SetCapacity(int size)
        {
            for (int i = m_ObjList.Count + m_BufList.Count; i < size; i++)
            {
                m_BufList.Enqueue(new ME());
            }
        }
        public void Clear()
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Clear without write-lock. name:" + typeof(ME).ToString());
            }
            foreach (ME obj in ToArray())
            {
                Free1(obj.GetKey1());
            }
        }

        private ME _Alloc(KEY1 key1, KEY2 key2, KEY3 key3)
        {
            ME obj = default(ME);
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Alloc without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString() + " key2:" + key2.ToString());
            }
            if (m_BufList.Count == 0)
            {
                Log.Message(LOG_TYPE.DEBUG, "Not enough buf class. name:" + typeof(ME).ToString());
                return obj;
            }
            if (m_ObjTable1.ContainsKey(key1))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key1]:" + key1);
                return obj;
            }
            if (m_ObjTable2.ContainsKey(key2))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key2]:" + key2);
                return obj;
            }
            if (m_ObjTable3.ContainsKey(key3))
            {
                Log.Message(LOG_TYPE.DEBUG, "Cannot alloc. [name]:" + typeof(ME).ToString() + "[key3]:" + key3);
                return obj;
            }
            if (m_BufList.Count == 0)
                return default(ME);
            obj = m_BufList.Dequeue();
            m_ObjTable1.Add(key1, obj);
            m_ObjTable2.Add(key2, obj);
            m_ObjTable3.Add(key3, obj);
            m_ObjList.Add(obj);
            return obj;
        }
        public ME Alloc(KEY1 key1, KEY2 key2, KEY3 key3)
        {
            ME obj = _Alloc(key1, key2, key3);
            if (obj == null)
            {
                return obj;
            }
            obj.OnAlloc(key1, key2, key3);
            return obj;
        }

        public void Free1(KEY1 key1)
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Free without write-lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }
            ME obj;
            if (m_ObjTable1.TryGetValue(key1, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(obj.GetKey1());
            m_ObjTable2.Remove(obj.GetKey2());
            m_ObjTable3.Remove(obj.GetKey3());
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }
        public void Free2(KEY2 key2)
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Free without write-lock. name:" + typeof(ME).ToString() + " key2:" + key2.ToString());
            }
            ME obj;
            if (m_ObjTable2.TryGetValue(key2, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(obj.GetKey1());
            m_ObjTable2.Remove(obj.GetKey2());
            m_ObjTable3.Remove(obj.GetKey3());
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }
        public void Free3(KEY3 key3)
        {
            if (m_Lock.IsWriting() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Free without write-lock. name:" + typeof(ME).ToString() + " key3:" + key3.ToString());
            }
            ME obj;
            if (m_ObjTable3.TryGetValue(key3, out obj) == false)
            {
                return;
            }
            obj.OnFree();
            m_ObjTable1.Remove(obj.GetKey1());
            m_ObjTable2.Remove(obj.GetKey2());
            m_ObjTable3.Remove(obj.GetKey3());
            m_ObjList.Remove(obj);
            m_BufList.Enqueue(obj);
            obj = default(ME);
        }

        public ME GetAt1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "operator[] without lock. name:" + typeof(ME).ToString() + " key1:" + key1.ToString());
            }

            ME obj;
            return m_ObjTable1.TryGetValue(key1, out obj) ? obj : default(ME);
        }
        public ME GetAt2(KEY2 key2)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "operator[] without lock. name:" + typeof(ME).ToString() + " key2:" + key2.ToString());
            }

            ME obj;
            return m_ObjTable2.TryGetValue(key2, out obj) ? obj : default(ME);
        }
        public ME GetAt3(KEY3 key3)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "operator[] without lock. name:" + typeof(ME).ToString() + " key3:" + key3.ToString());
            }

            ME obj;
            return m_ObjTable3.TryGetValue(key3, out obj) ? obj : default(ME);
        }
        public bool Contains1(KEY1 key1)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable1.ContainsKey(key1);
        }
        public bool Contains2(KEY2 key2)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable2.ContainsKey(key2);
        }
        public bool Contains3(KEY3 key3)
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "Contains() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjTable3.ContainsKey(key3);
        }
        public ME ElementAt(int index)
        {
            if (m_ObjList.Count <= index)
            {
                return default(ME);
            }
            return m_ObjList[index];
        }
        public ME[] ToArray()
        {
            if (m_Lock.IsReading() == false)
            {
                Log.Message(LOG_TYPE.DEBUG, "ToArray() without lock. name:" + typeof(ME).ToString());
            }
            return m_ObjList.ToArray();
        }
        public int Count
        {
            get
            {
                if (m_Lock.IsReading() == false)
                {
                    Log.Message(LOG_TYPE.DEBUG, "Count() without lock. name:" + typeof(ME).ToString());
                }
                return m_ObjList.Count;
            }
        }
    }

}
