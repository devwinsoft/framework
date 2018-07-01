using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    SoundChannel[] mChannels = new SoundChannel[(int)SOUND_CHANNEL.MAX];
    Dictionary<string, AudioClip> mClips = new Dictionary<string, AudioClip>();

    public void Clear()
    {
    }

    private void Awake()
    {
        msInstance = this;
        DontDestroyOnLoad(gameObject);

        gameObject.AddComponent<AudioListener>();

        _create_channel(SOUND_CHANNEL.BGM, 1);
        _create_channel(SOUND_CHANNEL.EFFECT, 10);
        _create_channel(SOUND_CHANNEL.UI, 10);
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

    public int Play_BGM(string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.BGM, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public int Play_EffectSound(string _path, bool _loop)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, 0, _path, _loop, 0f, 0f);
    }

    public int Play_EffectSound(int _ownerID, string _path, bool _loop)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, _ownerID, _path, _loop, 0f, 0f);
    }

    public int Play_EffectSound(string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public int Play_EffectSound(int _ownerID, string _path, bool _loop, float _waitTime, float _fadeTime)
    {
        return _play_sound(SOUND_CHANNEL.EFFECT, _ownerID, _path, _loop, _waitTime, _fadeTime);
    }

    public int Play_UISound(string _path, bool _loop, float _waitTime, float _fadeTime = 0f)
    {
        return _play_sound(SOUND_CHANNEL.UI, 0, _path, _loop, _waitTime, _fadeTime);
    }

    public void Stop_BGM(int _sound_id, float _fadeOutTime = 0f)
    {
        mChannels[(int)SOUND_CHANNEL.BGM].Stop(_sound_id, _fadeOutTime);
    }

    public void Stop_EffectSound(int _sound_id, float _fadeOutTime = 0f)
    {
        mChannels[(int)SOUND_CHANNEL.EFFECT].Stop(_sound_id, _fadeOutTime);
    }
}
