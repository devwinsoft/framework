using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public delegate void NetTriggerCallback();
    class NetTrigger
    {
        public void Register(short _rmi, NetTriggerCallback _callback, NetTriggerCallback _fallback)
        {
            TriggerData trigger = new TriggerData();
            trigger.rmi = _rmi;
            trigger.time = DateTime.Now;
            trigger.callback = _callback;
            trigger.fallback = _fallback;
            mCallbacks.Add(trigger);
        }

        public void OnReceiveData(NetBuffer _msg)
        {
            DateTime now = DateTime.Now;
            TimeSpan delta = TimeSpan.FromSeconds(10);
            for (int i = mCallbacks.Count - 1; i >= 0; i--)
            {
                TriggerData trigger = mCallbacks[i];
                if (trigger.rmi == _msg.Rmi)
                {
                    mCallbacks.RemoveAt(i);
                    if (trigger.callback != null)
                        trigger.callback.Invoke();
                }
                else if (now - trigger.time > delta)
                {
                    mCallbacks.RemoveAt(i);
                    if (trigger.fallback != null)
                        trigger.fallback.Invoke();
                }
            }
        }

        class TriggerData
        {
            public short rmi;
            public DateTime time;
            public NetTriggerCallback callback;
            public NetTriggerCallback fallback;
        }
        List<TriggerData> mCallbacks = new List<TriggerData>();
    }
}
