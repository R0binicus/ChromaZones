using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.WIN, StopMusic);
        EventManager.EventSubscribe(EventType.LOSE, StopMusic);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.WIN, StopMusic);
        EventManager.EventUnsubscribe(EventType.LOSE, StopMusic);
    }

    public void StopMusic(object data)
    {
        _musicSource.Stop();
    }
}
