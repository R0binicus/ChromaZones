using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource MusicSource;

    public AudioSource[] AudioSourceArray;

    public SoundAudioClip[] SoundAudioClipArray;

    public SoundAudioClip[] MusicAudioClipArray;

    private List<Sound> CurrentSoundsArray = new List<Sound>();

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
            Debug.LogError("SoundAudioClip's sound not found;" + sound);
        }
        else
        {
            //Find first AudioSource that is not playing
            AudioSource source = Array.Find(AudioSourceArray, x => x.isPlaying == false);
            if (source == null)
            {
                Debug.Log("No audio source available to play this sound!");
            }
            else
            {
                if (!CurrentSoundsArray.Contains(sound))
                {
                    source.PlayOneShot(clipSound.audioClip, clipSound.volume);
                    StartCoroutine(DoNotPlayMultipleOfSame(sound));
                }
            }
        }
    }

    [Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
        [Range(0, 1)] public float volume = 1f;
    }

    private IEnumerator DoNotPlayMultipleOfSame(Sound sound)
    {
        CurrentSoundsArray.Add(sound);
        yield return new WaitForSeconds(0.5f);
        CurrentSoundsArray.Remove(sound);
    }
}