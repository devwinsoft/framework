using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundChannel : MonoBehaviour
{
    SOUND_CHANNEL mChannel;

    Dictionary<int, SoundPlay> mPlayDict = new Dictionary<int, SoundPlay>();
    List<SoundPlay> mPlayList = new List<SoundPlay>();

    List<SoundPlay> mPool = new List<SoundPlay>();
    Dictionary<string, int> mRef = new Dictionary<string, int>();
    List<SoundPlay> mTempList = new List<SoundPlay>();
    int mStartID = 0;
    int mNextID = 0;

    public bool IsPlaying
    {
        get { return mPlayDict.Count > 0; }
    }

    public void Clear()
    {
        var enumer = mPlayDict.GetEnumerator();
        while (enumer.MoveNext())
        {
            SoundPlay data = enumer.Current.Value;
            data.mAudio.Stop();
            Destroy(data.mAudio.gameObject);
        }
        mPlayDict.Clear();
        mPlayList.Clear();
        mRef.Clear();
        mTempList.Clear();
    }

    public void Create(SOUND_CHANNEL _channel, int _playCount)
    {
        mChannel = _channel;
        mStartID = mNextID = (int)_channel * 1000;

        for (int i = 0; i < _playCount; i++)
        {
            SoundPlay data = new SoundPlay();
            GameObject obj = new GameObject("audio");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            data.mAudio = obj.AddComponent<AudioSource>();

            mPool.Add(data);
        }
    }

    int generateNextID()
    {
        do
        {
            mNextID++;
            if (mNextID >= mStartID + 1000)
            {
                mNextID = mStartID;
            }
        } while (mPlayDict.ContainsKey(mNextID));
        return mNextID;
    }

    SoundPlay pop(int _ownerID, string _path, AudioClip _clip, bool _loop, float _waitTime, float _fadeInTime)
    {
        SoundPlay obj = null;
        if (mPool.Count > 0)
        {
            obj = mPool[0];
            mPool.RemoveAt(0);
        }
        else if (mPlayList.Count > 0)
        {
            push(mPlayList[0]);
            obj = mPool[0];
            mPool.RemoveAt(0);
        }
        if (mRef.ContainsKey(_path))
        {
            mRef[_path]++;
        }
        else
        {
            mRef.Add(_path, 1);
        }
        int sound_id = generateNextID();
        obj.OnInit(_ownerID, sound_id, _path, _clip, _loop, _waitTime, _fadeInTime);
        mPlayDict.Add(sound_id, obj);
        mPlayList.Add(obj);
        return obj;
    }

    void push(SoundPlay _play)
    {
        mPlayDict.Remove(_play.SoundID);
        mPlayList.Remove(_play);

        _play.OnStop();

        mRef[_play.AudioPath]--;
        mPool.Add(_play);

        SoundManager.Instance.Trigger.OnSoundStop(_play.OwnerID);
    }

    public int Play(int _ownerID, string _path, AudioClip _clip, bool _loop, float _waitTime, float _fadeInTime)
    {
        if (_ownerID != 0)
        {
            int stopID = 0;
            var enumer = mPlayList.GetEnumerator();
            while (enumer.MoveNext())
            {
                SoundPlay playData = enumer.Current;
                if (playData.OwnerID == _ownerID)
                {
                    stopID = playData.SoundID;
                }
            }
            if (stopID != 0)
            {
                Stop(stopID, 0f);
            }
        }

        SoundPlay obj = pop(_ownerID, _path, _clip, _loop, _waitTime, _fadeInTime);
        if (obj == null)
        {
            return 0;
        }
        return obj.SoundID;
    }

    public void StopWithOwnerID(int _ownerID, float _fadeOutTime)
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (sound.OwnerID != _ownerID)
                continue;
            if (_fadeOutTime <= 0f)
            {
                push(sound);
            }
            else
            {
                sound.OnFadeOut(_fadeOutTime);
            }
        }
    }

    public void SetLoop(int _ownerID, bool _loop)
    {
        for (int i = mPlayList.Count - 1; i >= 0; i--)
        {
            SoundPlay sound = mPlayList[i];
            if (sound.OwnerID != _ownerID)
                continue;
            if (sound.mAudio != null)
            {
                sound.mAudio.loop = _loop;
            }
        }
    }


    public void Stop(int _sound_id, float _fadeOutTime)
    {
        SoundPlay play_info = null;
        if (mPlayDict.TryGetValue(_sound_id, out play_info))
        {
            if (_fadeOutTime <= 0f)
            {
                push(play_info);
            }
            else
            {
                play_info.OnFadeOut(_fadeOutTime);
            }
        }
    }

    public void StopAll()
    {
        var enumer = mPlayDict.GetEnumerator();
        while (enumer.MoveNext())
        {
            SoundPlay data = enumer.Current.Value;
            mTempList.Add(data);
        }
        for (int i = 0; i < mTempList.Count; i++)
        {
            push(mTempList[i]);
        }
        mTempList.Clear();
    }

    private void Update()
    {
        var enumerPlay = mPlayDict.GetEnumerator();
        while(enumerPlay.MoveNext())
        {
            SoundPlay play = enumerPlay.Current.Value;

            if (play.Tick(Time.unscaledDeltaTime))
            {
                switch (play.State)
                {
                    case SoundPlay.SOUND_PLAY_STATE.WAIT:
                        play.OnWait_End();
                        break;
                    case SoundPlay.SOUND_PLAY_STATE.FADE_IN:
                        play.OnFadeIn_End();
                        break;
                    case SoundPlay.SOUND_PLAY_STATE.FADE_OUT:
                    case SoundPlay.SOUND_PLAY_STATE.PLAY:
                        mTempList.Add(play);
                        break;
                    default:
                        break;
                }
            }
        }

        for (int i = 0; i < mTempList.Count; i++)
        {
            push(mTempList[i]);
        }
        mTempList.Clear();
    }
}
