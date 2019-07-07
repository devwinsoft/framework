using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : BaseTrigger
{
    protected class TRIGGER_SOUND
    {
        public System.Action Callback;
        public int OwnerID;

        public TRIGGER_SOUND(int _ownerID, System.Action _callback)
        {
            OwnerID = _ownerID;
            Callback = _callback;
        }
    }
    List<TRIGGER_SOUND> mSoundList = new List<TRIGGER_SOUND>();

    public override void Clear()
    {
        base.Clear();
        mSoundList.Clear();
    }

    public void Register_SoundStop(int _ownerID, System.Action _callback)
    {
        mSoundList.Add(new TRIGGER_SOUND(_ownerID, _callback));
    }

    public void OnSoundStop(int _ownerID)
    {
        for (int i = mSoundList.Count - 1; i >= 0; i--)
        {
            TRIGGER_SOUND data = mSoundList[i];
            if (data.OwnerID != _ownerID)
                continue;
            mSoundList.RemoveAt(i);
            data.Callback();
        }
    }
}


