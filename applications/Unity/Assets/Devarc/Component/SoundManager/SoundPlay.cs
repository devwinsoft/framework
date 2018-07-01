using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlay
{
    public enum SOUND_PLAY_STATE
    {
        STOP,
        WAIT,
        FADE_IN,
        PLAY,
        FADE_OUT,
    }

    public SOUND_PLAY_STATE State
    {
        get { return mState; }
        private set
        {
            mState = value;
            mElapsedTime = 0f;
        }
    }
    SOUND_PLAY_STATE mState = SOUND_PLAY_STATE.STOP;
    float mElapsedTime = 0f;

    public int OwnerID
    {
        get { return mOwnerID; }
    }
    int mOwnerID;

    public int SoundID
    {
        get { return mSoundID; }
    }
    int mSoundID;

    public AudioSource mAudio;

    public string AudioPath
    {
        get { return mAudioPath; }
    }
    public string mAudioPath;

    float mMaxWaitTime;
    float mMaxFadeTime; // fade-in / fade-out time
    List<int> mTempList = new List<int>();

    public void OnInit(int _ownerID, int _sound_id, string _path, AudioClip _clip, bool _loop, float _waitTime, float _fadeInTime)
    {
        State = SOUND_PLAY_STATE.WAIT;
        mOwnerID = _ownerID;
        mSoundID = _sound_id;
        mAudio.clip = _clip;
        mAudio.loop = _loop;
        mAudioPath = _path;
        mMaxWaitTime = _waitTime;
        mMaxFadeTime = _fadeInTime;

        if (_waitTime <= 0)
        {
            OnWait_End();
        }
    }

    public void OnWait_End()
    {
        if (mMaxFadeTime > 0)
        {
            State = SOUND_PLAY_STATE.FADE_IN;
            mAudio.volume = 0f;
            mAudio.Play();
        }
        else
        {
            State = SOUND_PLAY_STATE.PLAY;
            mAudio.volume = 1f;
            mAudio.Play();
        }
    }

    public void OnFadeIn_End()
    {
        State = SOUND_PLAY_STATE.PLAY;
        mAudio.volume = 1f;
    }

    public void OnFadeOut(float _fadeOutTime)
    {
        State = SOUND_PLAY_STATE.FADE_OUT;
        mAudio.volume = 1f;
    }

    public void OnStop()
    {
        State = SOUND_PLAY_STATE.STOP;
        mSoundID = 0;
        mAudio.Stop();
        mAudio.clip = null;
    }

    public bool Tick(float _deltaTime)
    {
        if (State == SOUND_PLAY_STATE.STOP)
            return false;

        mElapsedTime += _deltaTime;
        switch (State)
        {
            case SOUND_PLAY_STATE.WAIT:
                if (mMaxWaitTime < mElapsedTime)
                    return true;
                break;
            case SOUND_PLAY_STATE.FADE_IN:
                if (mMaxFadeTime < mElapsedTime)
                    return true;
                mAudio.volume = Mathf.Min(1f, mElapsedTime / mMaxFadeTime);
                break;
            case SOUND_PLAY_STATE.FADE_OUT:
                if (mMaxFadeTime < mElapsedTime)
                    return true;
                mAudio.volume = Mathf.Max(0f, 1f - mElapsedTime / mMaxFadeTime);
                break;
            case SOUND_PLAY_STATE.PLAY:
                if (mAudio.isPlaying == false)
                    return true;
                break;
            default:
                break;
        }
        return false;
    }
}

