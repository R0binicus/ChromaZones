using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [field:SerializeField] public Vector3 PlayerSpawn { get; set; }
    [SerializeField] private Sound _music;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.PLAYER_SPAWNPOINT);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, SendPlayerSpawn);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, PlayLevelMusic);
    }

    private void OnDisable()
    {       
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, SendPlayerSpawn);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, PlayLevelMusic);
    }

    public void SendPlayerSpawn(object data)
    {
        EventManager.EventTrigger(EventType.PLAYER_SPAWNPOINT, PlayerSpawn);
    }

    public void PlayLevelMusic(object data)
    {
        EventManager.EventTrigger(EventType.MUSIC, _music);
    }
}
