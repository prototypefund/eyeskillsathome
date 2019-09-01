using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyeSkills;
using System;

public class AudioManagerMock : IAudioManager
{
    public string providedKey;

    public AudioSource Say(string key)
    {
        throw new NotImplementedException();
    }

    public AudioSource Say(string key, Action _audioFinishedCallback)
    {
        providedKey = key;
        _audioFinishedCallback();
        return (new AudioSource());
    }

    public AudioSource Say(string key, Action _audioFinishedCallback, float endDelay)
    {
        throw new NotImplementedException();
    }

    public void StopAudioOnly()
    {
        throw new NotImplementedException();
    }
}
