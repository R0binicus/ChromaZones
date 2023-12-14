using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private SaveData _gameSaveData;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.LOAD_GAME_SUCCESS);
        EventManager.EventInitialise(EventType.LOAD_GAME_FAILED);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.NEW_GAME_REQUEST, NewGameHandler);
        EventManager.EventSubscribe(EventType.LOAD_GAME_REQUEST, LoadGameHandler);
        EventManager.EventSubscribe(EventType.SAVE_GAME, SaveGameHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.NEW_GAME_REQUEST, NewGameHandler);
        EventManager.EventUnsubscribe(EventType.LOAD_GAME_REQUEST, LoadGameHandler);
        EventManager.EventUnsubscribe(EventType.SAVE_GAME, SaveGameHandler);
    }

    public void NewGameHandler(object data)
    {
        _gameSaveData = new SaveData();
    }

    public void LoadGameHandler(object data)
    {
        if (_gameSaveData == null)
        {
            EventManager.EventTrigger(EventType.LOAD_GAME_FAILED, null);
        }
        else
        {
            EventManager.EventTrigger(EventType.LOAD_GAME_SUCCESS, _gameSaveData);
        }
    }

    public void SaveGameHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Save data has not been passed.");
        }

        // Need to minus 2 since data is the buildIndex NOT the level number
        _gameSaveData.LevelUnlocked = (int)data - 2;
    }
}
