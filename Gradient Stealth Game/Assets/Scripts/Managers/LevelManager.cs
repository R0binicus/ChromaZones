using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [field:SerializeField] public Vector3 PlayerSpawn { get; set; }

    private void Awake()
    {
        EventManager.EventInitialise(EventType.PLAYER_SPAWNPOINT);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, SendPlayerSpawn);
    }

    private void OnDisable()
    {       
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, SendPlayerSpawn);
    }

    public void SendPlayerSpawn(object data)
    {
        EventManager.EventTrigger(EventType.PLAYER_SPAWNPOINT, PlayerSpawn);
    }
}
