using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI Panels
    [Header("Canvases")]
    [SerializeField] private GameObject _gameCanvas;
    [Header("Game Panels")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _timerPanel;
    [Header("Buttons")]
    [SerializeField] private GameObject _nextLevelButton;

    [Header("Sounds")]
    [SerializeField] private Sound _buttonSFX;
    [Header("Debugging")]
    [SerializeField] private GameObject _debugCanvas;

    // Internal Data
    private bool _paused = false;
    private bool _levelEnded = false;
    private bool _lastLevel = false;
    private bool _lastMainLevel = false;
    private bool _buttonPressed = false;
    private TextMeshProUGUI _nextLevelButtonText;

    private void Awake()
    {
        // Get NextLevelButton Text and assign string
        _nextLevelButtonText = _nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
        _nextLevelButtonText.text = "Next Level";
        _debugCanvas.SetActive(false);
        
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
        EventManager.EventSubscribe(EventType.BONUS_LEVEL_START, LastMainLevelHandler);
        EventManager.EventSubscribe(EventType.DEBUG_GAME, DebuggingHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, LevelEnded);
        EventManager.EventUnsubscribe(EventType.LEVEL_STARTED, LevelStarted);
        EventManager.EventUnsubscribe(EventType.SCENE_COUNT, LastLevelHandler);
        EventManager.EventUnsubscribe(EventType.BONUS_LEVEL_START, LastMainLevelHandler);
        EventManager.EventUnsubscribe(EventType.DEBUG_GAME, DebuggingHandler);
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
                _timerPanel.SetActive(true);
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
        _timerPanel.SetActive(false);
        _buttonsPanel.SetActive(false);
        _nextLevelButton.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DebuggingHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("DebuggingHandler has not received a bool");
        }
        
        _debugCanvas.SetActive((bool)data);
    }

    public void LastLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Last Level Handler has not received a bool");
        }

        _lastLevel = (bool)data;
    }

    public void LastMainLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Last Main Level Handler has not received a bool");
        }

        _lastMainLevel = (bool)data;
    }

    private void ShowLosePanel(object data)
    {
        _levelEnded = true;
        StopMusicRaiseEvent();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _nextLevelButton.SetActive(false);
        _timerPanel.SetActive(true);
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
            if (_lastMainLevel)
            {
                _nextLevelButtonText.text = "Bonus";
            }
            else
            {
                _nextLevelButtonText.text = "Next Level";
            }
            _nextLevelButton.SetActive(true);
        }

        _timerPanel.SetActive(true);
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
