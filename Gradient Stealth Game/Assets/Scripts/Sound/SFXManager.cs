using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource MusicSource;

    public AudioSource[] AudioSourceArray;

    public SoundAudioClip[] SoundAudioClipArray;

    public SoundAudioClip[] MusicAudioClipArray;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.SFX);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.SFX, SFXEventHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.SFX, SFXEventHandler);
    }

    // Handles SFXEvent with incoming SFX data to play at specified cue source
    public void SFXEventHandler(object data)
    {
        if (data == null) return;

        Sound sound = (Sound)data;

        //Find SoundAudioClip from array that has the same sound variable as the input
        SoundAudioClip clipSound = Array.Find(SoundAudioClipArray, x => x.sound == sound);

        if (clipSound == null)
        {
            Debug.Log("SoundAudioClip's sound not found");
        }
        else
        {
            //Find first AudioSource that is not playing
            AudioSource source = Array.Find(AudioSourceArray, x => x.isPlaying == false);
            if (source == null)
            {
                Debug.Log("AudioSourceArray's source not found");
            }
            else
            {
                source.PlayOneShot(clipSound.audioClip, clipSound.volume);
            }
        }
        
        //foreach (var SoundAudioClip in SoundAudioClipArray)
        //{
        //    if (SoundAudioClip.sound == sound)
        //    {
        //        foreach (var source in AudioSourceArray)
        //        {
        //            if (!source.isPlaying)
        //            {
        //                source.PlayOneShot(SoundAudioClip.audioClip, SoundAudioClip.volume);
        //                return;
        //            }
        //        }
        //        return;
        //    }
        //}
    }

    [Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume = 1f;
    }
}