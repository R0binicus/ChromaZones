using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI Panels
    [Header("Canvases")]
    [SerializeField] GameObject _gameCanvas;
    [Header("Game Panels")]
    [SerializeField] GameObject _winPanel;
    [SerializeField] GameObject _losePanel;
    [SerializeField] GameObject _pausePanel;
    [SerializeField] GameObject _buttonsPanel;
    [Header("Buttons")]
    [SerializeField] GameObject _nextLevelButton;

    [Header("Sounds")]
    [SerializeField] private Sound _buttonSFX;

    // Internal Data
    bool _paused = false;
    bool _levelEnded = false;
    bool _lastLevel = false;
    bool _buttonPressed = false;

    private void Awake()
    {
        // Deactivate UI at start of game
        DeactivateUI();

        // Create UI events
        EventManager.EventInitialise(EventType.NEXT_LEVEL);
        EventManager.EventInitialise(EventType.RESTART_LEVEL);
        EventManager.EventInitialise(EventType.QUIT_LEVEL);
        EventManager.EventInitialise(EventType.PAUSE_MUSIC);
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        _paused = false;
        _levelEnded = false;
        _buttonPressed = false;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventSubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventSubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventSubscribe(EventType.LEVEL_ENDED, LevelEnded);
        EventManager.EventSubscribe(EventType.LEVEL_STARTED, LevelStarted);
        EventManager.EventSubscribe(EventType.SCENE_COUNT, LastLevelHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, LevelEnded);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStarted);
        EventManager.EventUnsubscribe(EventType.SCENE_COUNT, LastLevelHandler);
    }

    // Button callback to go to the next level
    public void NextLevel()
    {
        Time.timeScale = 1.0f;

        if (!_buttonPressed)
        {
            EventManager.EventTrigger(EventType.NEXT_LEVEL, null);
            _buttonPressed = true;
        }
    }

    // Button callback to restart level
    public void Restart()
    {
        Time.timeScale = 1.0f;

        if (!_buttonPressed)
        {
            EventManager.EventTrigger(EventType.RESTART_LEVEL, null);
            _buttonPressed = true;
        }
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        Time.timeScale = 1.0f;

        if (!_buttonPressed)
        {
            EventManager.EventTrigger(EventType.QUIT_LEVEL, null);
            _buttonPressed = true;
        }
    }

    public void TogglePause(object data)
    {
        if (!_levelEnded)
        {
            _paused = !_paused;
            EventManager.EventTrigger(EventType.PAUSE_MUSIC, _paused);

            if (!_paused)
            {
                DeactivateUI();
                Time.timeScale = 1.0f;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0.0f;
                _nextLevelButton.SetActive(false);
                _buttonsPanel.SetActive(true);
                _pausePanel.SetActive(true);
            }
        }
    }

    public void LevelStarted(object data)
    {
        _paused = false;
        _levelEnded = false;
        _buttonPressed = false;
    }

    public void LevelEnded(object data)
    {
        DeactivateUI();
        _paused = false;
        _levelEnded = true;
    }

    private void DeactivateUI()
    {
        _pausePanel.SetActive(false);
        _losePanel.SetActive(false);
        _winPanel.SetActive(false);
        _buttonsPanel.SetActive(false);
        _nextLevelButton.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LastLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Last Level Handler has not received a bool");
        }

        _lastLevel = (bool)data;
    }

    private void ShowLosePanel(object data)
    {
        _levelEnded = true;
        StopMusicRaiseEvent();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _nextLevelButton.SetActive(false);
        _buttonsPanel.SetActive(true);
        _losePanel.SetActive(true);
    }

    private void ShowWinPanel(object data)
    {
        StopMusicRaiseEvent();
        _levelEnded = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // If last level, do not show next level button
        if (_lastLevel)
        {
            _nextLevelButton.SetActive(false);
        }
        else
        {
            _nextLevelButton.SetActive(true);
        }

        _buttonsPanel.SetActive(true);
        _winPanel.SetActive(true);
    }

    public void ButtonSFX()
    {
        EventManager.EventTrigger(EventType.SFX, _buttonSFX);
    }

    private void StopMusicRaiseEvent()
    {
        EventManager.EventTrigger(EventType.STOP_MUSIC, null);
    }
}
