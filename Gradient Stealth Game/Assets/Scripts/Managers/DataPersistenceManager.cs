using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    // Internal Data
    private SaveData _gameSaveData;
    private FileDataHandler _fileHandler;

    private List<float> _bestTimers = new List<float>();

    private void Awake()
    {
        EventManager.EventInitialise(EventType.LOAD_GAME_SUCCESS);
        EventManager.EventInitialise(EventType.LOAD_GAME_FAILED);
        EventManager.EventInitialise(EventType.TIMER_SAVE);
        EventManager.EventInitialise(EventType.TIMER_LOAD);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.NEW_GAME_REQUEST, NewGameHandler);
        EventManager.EventSubscribe(EventType.LOAD_GAME_REQUEST, LoadGameHandler);
        EventManager.EventSubscribe(EventType.SAVE_GAME, SaveGameHandler);
        EventManager.EventSubscribe(EventType.TIMER_SAVE, TimerSaveHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.NEW_GAME_REQUEST, NewGameHandler);
        EventManager.EventUnsubscribe(EventType.LOAD_GAME_REQUEST, LoadGameHandler);
        EventManager.EventUnsubscribe(EventType.SAVE_GAME, SaveGameHandler);
        EventManager.EventUnsubscribe(EventType.TIMER_SAVE, TimerSaveHandler);
    }

    private void Start()
    {
        _fileHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
    }

    public void NewGameHandler(object data)
    {
        // Create new save data and save to file
        _gameSaveData = new SaveData();
        _bestTimers = _gameSaveData.LevelTimers;
        _fileHandler.Save(_gameSaveData);
    }

    public void LoadGameHandler(object data)
    {
        // Load data from file
        _gameSaveData = _fileHandler.Load();

        if (_gameSaveData == null)
        {
            EventManager.EventTrigger(EventType.LOAD_GAME_FAILED, null);
        }
        else
        {
            EventManager.EventTrigger(EventType.LOAD_GAME_SUCCESS, _gameSaveData);
            _bestTimers = _gameSaveData.LevelTimers;
        }
    }

    public void SaveGameHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Data to save has not been passed.");
        }

        if (_gameSaveData != null)
        {
            // Check current progress so no later levels unlocked are overridden and locked again
            // Also need to minus 2 since data is the buildIndex NOT the level number
            if (_gameSaveData.LevelUnlocked < (int)data - 2)
            {
                _gameSaveData.LevelUnlocked = (int)data - 2;
            }
            _gameSaveData.LevelTimers = _bestTimers;
            _fileHandler.Save(_gameSaveData);
        }
        else
        {
            Debug.Log("No save game data, possibly in leveleditormode");
        }
    }

    public void TimerSaveHandler(object data)
    {
        if (data == null)
        {
            EventManager.EventTrigger(EventType.TIMER_LOAD, _bestTimers);
        }
        else
        {
            _bestTimers = (List<float>)data;
            EventManager.EventTrigger(EventType.SAVE_GAME, _gameSaveData.LevelUnlocked);
        }
    }
}