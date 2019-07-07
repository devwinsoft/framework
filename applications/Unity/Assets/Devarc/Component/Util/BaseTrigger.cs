using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTrigger
{
    public virtual void Clear()
    {
        mMessageList.Clear();
        mTimerList.Clear();
    }

    class TRIGGER_MSG
    {
        public System.Action<System.ValueType[]> Callback;
        public string Key;
        public bool OneTime;
        public TRIGGER_MSG(string _key, bool _oneTime, System.Action<System.ValueType[]> _callback)
        {
            Callback = _callback;
            Key = _key;
            OneTime = _oneTime;
        }
    }
    List<TRIGGER_MSG> mMessageList = new List<TRIGGER_MSG>();

    public void Register_Message(string _key, bool _oneTime, System.Action<System.ValueType[]> _callback)
    {
        mMessageList.Add(new TRIGGER_MSG(_key, _oneTime, _callback));
    }

    public void UnRegister_Message(string _key)
    {
        for (int i = mMessageList.Count - 1; i >= 0; i--)
        {
            TRIGGER_MSG data = mMessageList[i];
            if (string.Equals(_key, data.Key) == false)
                continue;
            mMessageList.RemoveAt(i);
        }
    }

    public void OnMessage(string _key, params System.ValueType[] args)
    {
        for (int i = mMessageList.Count - 1; i >= 0; i--)
        {
            TRIGGER_MSG data = mMessageList[i];
            if (string.Equals(_key, data.Key) == false)
                continue;
            if (data.OneTime)
                mMessageList.RemoveAt(i);
            data.Callback(args);
        }
    }

    class TRIGGER_TIMER
    {
        public System.Action Callback;
        public float RemainTime;
        public TRIGGER_TIMER(float _wait_time, System.Action _callback)
        {
            Callback = _callback;
            RemainTime = _wait_time;
        }
    }
    List<TRIGGER_TIMER> mTimerList = new List<TRIGGER_TIMER>();

    public void Register_Timer(float _waitTime, System.Action _callback)
    {
        mTimerList.Add(new TRIGGER_TIMER(_waitTime, _callback));
    }

    public void OnUpdate(float _deltaTime)
    {
        for (int i = mTimerList.Count - 1; i >= 0; i--)
        {
            TRIGGER_TIMER data = mTimerList[i];
            data.RemainTime -= _deltaTime;
            if (data.RemainTime > 0f)
                continue;
            mTimerList.RemoveAt(i);
            data.Callback();
        }
    }
}
