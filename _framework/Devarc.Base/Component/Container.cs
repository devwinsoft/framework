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
#if UNITY_5 || UNITY_2017
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif

namespace Devarc
{
    public class Table<K, T> where T : IBaseObejct, new()
    {
        private Dictionary<K, T> mMap = new Dictionary<K, T>();
        protected List<T> mList = new List<T>();

        public void Clear()
        {
            mMap.Clear();
            mList.Clear();
        }

        public T Alloc(K key1)
        {
            if (mMap.ContainsKey(key1))
            {
                Log.Debug("Cannot alloc. [name]:" + typeof(T).ToString() + "[key1]:" + key1);
                return default(T);
            }
            T obj = new T();
            mMap.Add(key1, obj);
            mList.Add(obj);
            return obj;
        }

        public void Free(K _key)
        {
            T obj;
            if (mMap.TryGetValue(_key, out obj) == false)
            {
                return;
            }
            mMap.Remove(_key);
            mList.Remove(obj);
            obj = default(T);
        }

        public T GetAt(K _key)
        {
            T obj;
            return mMap.TryGetValue(_key, out obj) ? obj : default(T);
        }

        public bool Contains(K key)
        {
            return mMap.ContainsKey(key);
        }

        public T ElementAt(int index)
        {
            if (mList.Count <= index)
            {
                return default(T);
            }
            return mList[index];
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return mList.GetEnumerator();
        }

        public int Count { get { return mList.Count; } }
    }


    public class Container<ITEM, KEY> where ITEM : IBaseObejct<KEY>, new()
    {
        private Dictionary<KEY, ITEM> mMap = new Dictionary<KEY, ITEM>();
        protected List<ITEM> mList = new List<ITEM>();

        public Container()
        {
        }

        public void Clear()
        {
            mMap.Clear();
            mList.Clear();
        }

        public ITEM Alloc(KEY key1)
        {
            if (mMap.ContainsKey(key1))
            {
                Log.Debug("Cannot alloc. [name]:" + typeof(ITEM).ToString() + "[key1]:" + key1);
                return default(ITEM);
            }
            ITEM obj = new ITEM();
            mMap.Add(key1, obj);
            mList.Add(obj);
            return obj;
        }

        public void Free(KEY _key)
        {
            ITEM obj;
            if (mMap.TryGetValue(_key, out obj) == false)
            {
                return;
            }
            mMap.Remove(_key);
            mList.Remove(obj);
            obj = default(ITEM);
        }

        public ITEM GetAt(KEY _key)
        {
            ITEM obj;
            return mMap.TryGetValue(_key, out obj) ? obj : default(ITEM);
        }

        static readonly ITEM defaultObject = new ITEM();
        public ITEM GetAt(SQLite_Session _session, KEY _key)
        {
            ITEM obj = default(ITEM);
            if (mMap.TryGetValue(_key, out obj))
            {
                return obj;
            }

            try
            {
                SqliteDataReader reader = _session.ExecuteReader(defaultObject.GetSelectQuery(_key));
                if (reader.Read())
                {
                    obj = new ITEM();
                    obj.Initialize(reader);
                    mMap.Add(obj.GetKey(), obj);
                    mList.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return obj;
        }

        public bool Contains(KEY key)
        {
            return mMap.ContainsKey(key);
        }
        public ITEM ElementAt(int index)
        {
            if (mList.Count <= index)
            {
                return default(ITEM);
            }
            return mList[index];
        }
        public List<ITEM>.Enumerator GetEnumerator()
        {
            return mList.GetEnumerator();
        }
        public int Count { get { return mList.Count; } }
    }


    //public class Container<ME, KEY1, KEY2> where ME : IContents<KEY1, KEY2>, new()
    //{
    //    private Dictionary<KEY1, ME> m_ObjTable1 = new Dictionary<KEY1, ME>();
    //    private Dictionary<KEY2, ME> m_ObjTable2 = new Dictionary<KEY2, ME>();
    //    protected List<ME> m_ObjList = new List<ME>();

    //    public Container()
    //    {
    //    }
    //    public void Clear()
    //    {
    //        m_ObjTable1.Clear();
    //        m_ObjTable2.Clear();
    //        m_ObjList.Clear();
    //    }

    //    private ME _Alloc(KEY1 key1, KEY2 key2)
    //    {
    //        if (m_ObjTable1.ContainsKey(key1))
    //        {
    //            Log.Debug("Cannot alloc. [name]:" + typeof(ME).ToString() + "[key1]:" + key1);
    //            return default(ME);
    //        }
    //        if (m_ObjTable2.ContainsKey(key2))
    //        {
    //            Log.Debug("Cannot alloc. [name]:" + typeof(ME).ToString() + "[key2]:" + key2);
    //            return default(ME);
    //        }

    //        ME obj = new ME();
    //        m_ObjTable1.Add(key1, obj);
    //        m_ObjTable2.Add(key2, obj);
    //        m_ObjList.Add(obj);
    //        return obj;
    //    }
    //    public ME Alloc(KEY1 key1, KEY2 key2)
    //    {
    //        ME obj = _Alloc(key1, key2);
    //        if (obj == null)
    //        {
    //            return obj;
    //        }
    //        obj.OnAlloc(key1, key2);
    //        return obj;
    //    }

    //    public void Free1(KEY1 key1)
    //    {
    //        ME obj;
    //        if (m_ObjTable1.TryGetValue(key1, out obj) == false)
    //        {
    //            return;
    //        }
    //        obj.OnFree();
    //        m_ObjTable1.Remove(obj.GetKey1());
    //        m_ObjTable2.Remove(obj.GetKey2());
    //        m_ObjList.Remove(obj);
    //        obj = default(ME);
    //    }

    //    public void Free2(KEY2 key2)
    //    {
    //        ME obj;
    //        if (m_ObjTable2.TryGetValue(key2, out obj) == false)
    //        {
    //            return;
    //        }
    //        obj.OnFree();
    //        m_ObjTable1.Remove(obj.GetKey1());
    //        m_ObjTable2.Remove(obj.GetKey2());
    //        m_ObjList.Remove(obj);
    //        obj = default(ME);
    //    }

    //    public ME GetAt1(KEY1 key1)
    //    {
    //        ME obj;
    //        return m_ObjTable1.TryGetValue(key1, out obj) ? obj : default(ME);
    //    }

    //    public ME GetAt2(KEY2 key2)
    //    {
    //        ME obj;
    //        return m_ObjTable2.TryGetValue(key2, out obj) ? obj : default(ME);
    //    }
    //    public bool Contains1(KEY1 key1)
    //    {
    //        return m_ObjTable1.ContainsKey(key1);
    //    }
    //    public bool Contains2(KEY2 key2)
    //    {
    //        return m_ObjTable2.ContainsKey(key2);
    //    }
    //    public ME ElementAt(int index)
    //    {
    //        if (m_ObjList.Count <= index)
    //        {
    //            return default(ME);
    //        }
    //        return m_ObjList[index];
    //    }
    //    public List<ME>.Enumerator GetEnumerator()
    //    {
    //        return m_ObjList.GetEnumerator();
    //    }
    //    public int Count { get { return m_ObjList.Count; } }
    //}

}
