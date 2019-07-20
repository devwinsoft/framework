using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObjectEx<INFO>
{
    INFO Info { get; }
    void OnPoolEvent_Pop(INFO _info);
    void OnPoolEvent_Push();
}

public class ObjectPoolEx<INFO, T> where T : MonoBehaviour, IPoolObjectEx<INFO>
{
    public delegate string FUNC_RESOURCE_PATH(INFO _key);

    Transform mGlobalRoot;
    FUNC_RESOURCE_PATH mFuncResPath;
    Dictionary<string, T> mPrefabs = new Dictionary<string, T>();
    Dictionary<string, List<T>> mPool = new Dictionary<string, List<T>>();

    public void Init(Transform _root, FUNC_RESOURCE_PATH _func)
    {
        mGlobalRoot = _root;
        mFuncResPath = _func;
    }

    public void Clear(bool _unload)
    {
        mGlobalRoot = null;
        mFuncResPath = null;
        foreach (var prefab in mPrefabs.Values)
        {
            GameObject.Destroy(prefab);
        }
        mPrefabs.Clear();
        mPool.Clear();
    }

    public T Pop(INFO _info, Vector3 _worldPos)
    {
        if (mFuncResPath == null)
        {
            Debug.LogFormat("ObejctPoolEx<{0}> is not initialized.", typeof(T));
            return null;
        }

        T compo;
        List<T> list;
        string path = mFuncResPath(_info);
        if (mPool.TryGetValue(path, out list) == false)
        {
            list = new List<T>();
            mPool.Add(path, list);
        }

        if (list.Count > 0)
        {
            compo = list[0];
            list.RemoveAt(0);
            compo.transform.SetParent(mGlobalRoot);
            compo.transform.SetPositionAndRotation(_worldPos, Quaternion.identity);
            compo.gameObject.SetActive(true);
            compo.OnPoolEvent_Pop(_info);
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
        compo = GameObject.Instantiate<T>(prefab, mGlobalRoot);
        if (compo == null)
        {
            return null;
        }

        compo.transform.SetParent(mGlobalRoot);
        compo.transform.SetPositionAndRotation(_worldPos, Quaternion.identity);
        compo.gameObject.SetActive(true);
        compo.OnPoolEvent_Pop(_info);
        return compo;
    }

    public T Pop(INFO _info, Transform _parent, Vector3 _localPos)
    {
        if (mFuncResPath == null)
        {
            Debug.LogErrorFormat("ObejctPoolEx<{0}> is not initialized.", typeof(T));
            return null;
        }

        T compo;
        List<T> list;
        Transform root = _parent != null ? _parent : mGlobalRoot;
        string path = mFuncResPath(_info);
        if (mPool.TryGetValue(path, out list) == false)
        {
            list = new List<T>();
            mPool.Add(path, list);
        }

        if (list.Count > 0)
        {
            compo = list[0];
            list.RemoveAt(0);
            compo.transform.SetParent(root);
            compo.transform.localPosition = _localPos;
            compo.gameObject.SetActive(true);
            compo.OnPoolEvent_Pop(_info);
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
        compo = GameObject.Instantiate<T>(prefab, root);
        if (compo == null)
        {
            return null;
        }

        compo.transform.SetParent(root);
        compo.transform.localPosition = _localPos;
        compo.gameObject.SetActive(true);
        compo.OnPoolEvent_Pop(_info);
        return compo;
    }

    public void Push(INFO _info, T _obj)
    {
        List<T> list;
        string path = mFuncResPath(_info);
        if (mPool.TryGetValue(path, out list) == false)
        {
            list = new List<T>();
            mPool.Add(path, list);
        }
        _obj.OnPoolEvent_Push();
        _obj.gameObject.SetActive(false);
        list.Add(_obj);
    }
}


