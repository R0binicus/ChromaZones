using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private float _currentTimer = 0f;
    private float _bestTimer = 0f;
    [SerializeField] private TMP_Text _currentTimerText;
    [SerializeField] private TMP_Text _bestTimerText;
    private bool _timerPaused = false;
    private bool _gameOver = false;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.WIN, WinHandler);
        EventManager.EventSubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventSubscribe(EventType.PAUSE_TOGGLE, TogglePause);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.WIN, WinHandler);
        EventManager.EventUnsubscribe(EventType.LOSE, LoseHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStart);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
    }
    
    void Start()
    {
        DisplayTime(_bestTimer, _bestTimerText);
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
        if (_currentTimer < _bestTimer)
        {
            _bestTimer = _currentTimer;
            DisplayTime(_bestTimer, _bestTimerText);
        }
        else if (_bestTimer == 0f)
        {
            _bestTimer = _currentTimer;
            DisplayTime(_bestTimer, _bestTimerText);
        }
    }

    public void LoseHandler(object data)
    {
        _gameOver = true;
    }

    public void LevelStart(object data)
    {
        _currentTimer = 0f;
        _gameOver = false;
        _timerPaused = false;
    }

    public void TogglePause(object data)
    {
        _timerPaused = !_timerPaused;
    }
}
