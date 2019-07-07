using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    void OnPoolEvent_Pop();
    void OnPoolEvent_Push();
}

public class ObjectPool<T> where T : MonoBehaviour
{
    public delegate string FUNC_RESOURCE_PATH();

    Transform mRoot;
    FUNC_RESOURCE_PATH mFuncResPath;
    Dictionary<string, T> mPrefabs = new Dictionary<string, T>();
    Dictionary<string, List<T>> mPool = new Dictionary<string, List<T>>();

    public void Init(Transform _root, FUNC_RESOURCE_PATH _func)
    {
        mRoot = _root;
        mFuncResPath = _func;
    }

    public void Clear(bool _unload)
    {
        foreach(var prefab in mPrefabs.Values)
        {
            GameObject.Destroy(prefab);
        }
        mPrefabs.Clear();
        mPool.Clear();
    }

    public T Pop(Vector3 _pos)
    {
        T compo;
        List<T> list;
        string path = mFuncResPath();
        if (mPool.TryGetValue(path, out list) == false)
        {
            list = new List<T>();
            mPool.Add(path, list);
        }

        if (list.Count > 0)
        {
            compo = list[0];
            list.RemoveAt(0);
            compo.transform.transform.position = _pos;
            compo.gameObject.SetActive(true);
            return compo;
        }

        // Prepare prefab
        T prefab;
        if (mPrefabs.TryGetValue(path, out prefab) == false)
        {
            prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                return null;
            }
            mPrefabs.Add(path, prefab);
        }

        // Instantiate
        T obj = GameObject.Instantiate<T>(prefab, _pos, Quaternion.identity, mRoot);
        compo = obj.GetComponent<T>();
        obj.gameObject.SetActive(true);
        return compo;
    }

    public void Push(T _obj)
    {
        List<T> list;
        string path = mFuncResPath();
        if (mPool.TryGetValue(path, out list) == false)
        {
            list = new List<T>();
            mPool.Add(path, list);
        }
        _obj.gameObject.SetActive(false);
        list.Add(_obj);
    }
}

