using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;

public enum SOUND_CHANNEL
{
    BGM,
    EFFECT,
    UI,
    MAX,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get { return msInstance; } }
    static SoundManager msInstance = null;

    public SoundTrigger Trigger = new SoundTrigger();

    Dictionary<string, List<SOUND>> mSoundList = new Dictionary<string, List<SOUND>>();

    SoundChannel[] mChannels = new SoundChannel[(int)SOUND_CHANNEL.MAX];
    Dictionary<string, AudioClip> mClips = new Dictionary<string, AudioClip>();

    public void Clear()
    {
        Trigger.Clear();
    }

    public void Init()
    {
        var enumer = Table.T_SOUND.GetEnumerator();
        while (enumer.MoveNext())
        {
            SOUND data = enumer.Current;
            List<SOUND> list;
            if (mSoundList.TryGetValue(data.SOUND_ID, out list) == false)
            {
                list = new List<SOUND>();
                mSoundList.Add(data.SOUND_ID, list);
            }
            list.Add(data);
        }
    }

    private void Awake()
    {
        msInstance = this;

        _create_channel(SOUND_CHANNEL.BGM, 1);
        _create_channel(SOUND_CHANNEL.EFFECT, 10);
        _create_channel(SOUND_CHANNEL.UI, 10);

        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    void _create_channel(SOUND_CHANNEL _channel, int _pool)
    {
        GameObject obj = new GameObject(_channel.ToString());
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        mChannels[(int)_channel] = obj.AddComponent<SoundChannel>();
        mChannels[(int)_channel].Create(_channel, _pool);
    }

    AudioClip _get_clip(string _path)
    {
        AudioClip clip = null;
        if (mClips.TryGetValue(_path, out clip) == false)
        {
            clip = Resources.Load<AudioClip>(_path);
            if (clip != null)
            {
                mClips.Add(_path, clip);
            }
        }
        return clip;
    }

    int _play_sound(SOUND_CHANNEL _channel, int _ownerID, string _path, bool _loop, float _waitTime, float _fadeInTime)
    {
        AudioClip clip = _get_clip(_path);
        if (clip == null)
        {
            return 0;
        }
        return mChannels[(int)_channel].Play(_ownerID, _path, clip, _loop, _waitTime, _fadeInTime);
    }

    public bool IsPlaying(SOUND_CHANNEL _channel)
    {
        return mChannels[(int)_channel].IsPlaying;
    }

    public int PlayFile_BGM(string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.BGM, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public int PlaySound_Effect(int _ownerID, string _soundID)
    {
        List<SOUND> list = null;
        if (mSoundList.TryGetValue(_soundID, out list))
        {
            int cnt = list.Count;
            SOUND data = list[Random.Range(0, cnt)];
            return _play_sound(SOUND_CHANNEL.EFFECT, _ownerID, data.PATH, data.LOOP, 0f, 0f);
        }
        return 0;
    }

    public int PlayFile_Effect(string _path, bool _loop)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, 0, _path, _loop, 0f, 0f);
    }

    public int PlayFile_Effect(int _ownerID, string _path, bool _loop)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, _ownerID, _path, _loop, 0f, 0f);
    }

    public int PlayFile_Effect(string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public int PlayFile_Effect(int _ownerID, string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, _ownerID, _path, _loop, _waitTime, _fadeTime);
    }

    public int PlayFile_UI(string _path, bool _loop, float _waitTime, float _fadeTime = 0f)
    {
        return _play_sound(SOUND_CHANNEL.UI, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public void Stop_BGM(int _sound_id, float _fadeOutTime = 0f)
    {
        mChannels[(int)SOUND_CHANNEL.BGM].Stop(_sound_id, _fadeOutTime);
    }

    public void Stop_EffectSound_OwnerID(int _ownerID, float _fadeOutTime = 0f)
    {
        mChannels[(int)SOUND_CHANNEL.EFFECT].StopWithOwnerID(_ownerID, _fadeOutTime);
    }

    public void SetLoop_EffectSound_OwnerID(int _ownerID, bool _loop)
    {
        mChannels[(int)SOUND_CHANNEL.EFFECT].SetLoop(_ownerID, _loop);
    }
}
