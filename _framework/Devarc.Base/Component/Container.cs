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
//

using System;
using System.Collections;
using System.Collections.Generic;

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
        public ITEM GetAt(IBaseSession _session, KEY _key)
        {
            ITEM obj = default(ITEM);
            if (mMap.TryGetValue(_key, out obj))
            {
                return obj;
            }

            try
            {
                IBaseReader reader = _session.Execute_Reader(defaultObject.GetQuery_Select(_key));
                if (reader.Read())
                {
                    obj = new ITEM();
                    obj.Initialize(reader);
                    mMap.Add(obj.GetKey(), obj);
                    mList.Add(obj);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return obj;
        }

        public int GetWhere(IBaseSession _session, string _where, ITEM[] _inoutList)
        {
            int cnt = 0;
            if (_inoutList == null || _inoutList.Length == 0)
            {
                return 0;
            }
            try
            {
                IBaseReader reader = _session.Execute_Reader(defaultObject.GetQuery_SelectWhere(_where));
                while (reader.Read())
                {
                    ITEM obj = new ITEM();
                    obj.Initialize(reader);
                    mMap.Add(obj.GetKey(), obj);
                    mList.Add(obj);
                    reader.Close();

                    if (cnt < _inoutList.Length)
                    {
                        _inoutList[cnt] = obj;
                        cnt++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return cnt;
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
}
