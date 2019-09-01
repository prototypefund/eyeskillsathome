/*
Copyright 2019 Dr. Thomas Benjamin Senior, Michael Zoeller 

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EyeSkills
{
    /// <summary>
    /// EyeSkills Audio Manager.  Simplifies the loading and playing of pre-generated audio resources.
    /// </summary>
    public class AudioManagerTest : MonoBehaviour,IAudioManager
    {
        public string language = "en";
        public string ttsAudioDir = "Audio";

        AudioSource audioSource;
        AudioClip notFoundClip;
        Action audioFinishedCallback;

        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // singleton like access pattern
        public static AudioManagerTest instance = null;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            notFoundClip = Resources.Load(ttsAudioDir + "/DoesNotExist") as AudioClip;

            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        public AudioSource Say(string key)
        {
            string audioFile = ttsAudioDir + "/" + language + "-" + key;
            //string audioFile = ttsAudioDir + "/" + key;
            //Debug.Log("Looking for " + audioFile);

            AudioClip clip = null;
            try
            {
                clip = Resources.Load<AudioClip>(audioFile);
            }
            catch (UnityException e)
            {
                Debug.LogWarning(e);
            }

            if (clip == null)
                audioSource.clip = notFoundClip;
            else
                audioSource.clip = clip;

            audioSource.Play();
            return audioSource;
        }

        /// <summary>
        /// Monitors the audio clip and calls a callback when it is finished
        /// </summary>
        IEnumerator MonitorAudioClip(){
            while (audioSource.isPlaying){
                yield return null;
            }
            audioFinishedCallback();
        }

        /// <summary>
        /// Say the specified key and call audioFinishedCallback when the clip has completed
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="audioFinishedCallback">Audio finished callback.</param>
        public AudioSource Say(string key, Action _audioFinishedCallback)
        {
            audioFinishedCallback = _audioFinishedCallback;
            Say(key);
            //Now start a coroutine to trigger the callback on completion
            StartCoroutine("MonitorAudioClip");
            return audioSource;
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
}