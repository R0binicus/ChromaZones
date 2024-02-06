using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    private float _currentTimer = 0f;
    private List<float> _bestTimers = new List<float>();
    [SerializeField] private TMP_Text _currentTimerText;
    [SerializeField] private TMP_Text _bestTimerText;
    private bool _timerPaused = false;
    private bool _gameOver = false;

    // Scene Tracking
    private int _numOfScenes;
    private Scene _currentLevel;
    private int _currentTimerInt;

    private void Awake()
    {
        _numOfScenes = SceneManager.sceneCountInBuildSettings;

        CheckCurrentLevel();

        int x = 3;
        while (_numOfScenes > x)
        {
            _bestTimers.Add(0f);
            x++;
        }
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.WIN, WinHandler);
        EventManager.EventSubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventSubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventSubscribe(EventType.TIMER_LOAD, TimerLoadHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.WIN, WinHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventUnsubscribe(EventType.TIMER_LOAD, TimerLoadHandler);
    }
    
    void Start()
    {
        EventManager.EventTrigger(EventType.TIMER_SAVE, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_timerPaused && !_gameOver)
        {
            _currentTimer += Time.deltaTime;
            DisplayTime(_currentTimer, _currentTimerText);
        }
    }

    private void ResetTimer()
    {
        _currentTimer = 0f;
    }

    void DisplayTime(float displayTime, TMP_Text TMP)
    {
        float minutes = Mathf.FloorToInt(displayTime / 60);
        float seconds = Mathf.FloorToInt(displayTime % 60);
        TMP.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void WinHandler(object data)
    {
        _gameOver = true;
        if (_currentTimer < _bestTimers[_currentTimerInt])
        {
            _bestTimers[_currentTimerInt] = _currentTimer;
            DisplayTime(_bestTimers[_currentTimerInt], _bestTimerText);
        }
        else if (_bestTimers[_currentTimerInt] == 0f)
        {
            _bestTimers[_currentTimerInt] = _currentTimer;
            DisplayTime(_bestTimers[_currentTimerInt], _bestTimerText);
        }
        EventManager.EventTrigger(EventType.TIMER_SAVE, _bestTimers);
    }

    public void LoseHandler(object data)
    {
        _gameOver = true;
    }

    public void LevelStart(object data)
    {
        CheckCurrentLevel();
        
        _currentTimer = 0f;
        _gameOver = false;
        _timerPaused = false;

        DisplayTime(_bestTimers[_currentTimerInt], _bestTimerText);
    }

    public void TogglePause(object data)
    {
        _timerPaused = !_timerPaused;
    }

    public void CheckCurrentLevel()
    {
        
        int count = SceneManager.loadedSceneCount;

        for (int i = 0; i < count; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name != "Gameplay" && scene.name != "Services")
            {
                _currentLevel = scene;
            }
        }
        
        _currentTimerInt = _currentLevel.buildIndex - 3;
    }

    public void TimerLoadHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("TimerLoadHandler is null");
        }
        
        _bestTimers = (List<float>)data;
        
        if(_bestTimers.Count == 0)
        {
            int x = 3;
            while (_numOfScenes > x)
            {
                
                _bestTimers.Add(0f);
                x++;
            }
        }
    }
}
