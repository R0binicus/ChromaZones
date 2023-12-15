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

    private List<Sound> CurrentSoundsList = new List<Sound>();

    private void Awake()
    {
        EventManager.EventInitialise(EventType.SFX);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.SFX, SFXEventHandler);
        EventManager.EventSubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventSubscribe(EventType.LEVEL_ENDED, StopMusic);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.SFX, SFXEventHandler);
        EventManager.EventUnsubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, StopMusic);
        StopAllCoroutines();
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
            Debug.LogError("SoundAudioClip's sound not found " + sound);
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
                if (!CurrentSoundsList.Contains(sound))
                {
                    source.PlayOneShot(clipSound.audioClip, clipSound.volume);
                    StartCoroutine(DoNotPlayMultipleOfSame(sound));
                }
            }
        }
    }

    public void MusicEventHandler(object data)
    {        
        if (data == null) return;

        StopMusic(null);

        Sound music = (Sound)data;

        SoundAudioClip musicClip = Array.Find(MusicAudioClipArray, x => x.sound == music);
        
        if (musicClip == null)
        {
            Debug.LogError("SoundAudioClip's music track not found " + music);
        }
        else
        {
            MusicSource.PlayOneShot(musicClip.audioClip, musicClip.volume);
        }
    }

    public void StopMusic(object data)
    {
        MusicSource.Stop();
    }

    private IEnumerator DoNotPlayMultipleOfSame(Sound sound)
    {
        CurrentSoundsList.Add(sound);
        yield return new WaitForSeconds(0.1f);
        CurrentSoundsList.Remove(sound);
    }
}

[Serializable]
public class SoundAudioClip
{
    public Sound sound;
    public AudioClip audioClip;
    [Range(0, 1)] public float volume = 1f;
}