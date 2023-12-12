using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;
using Unity.Collections;

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

    // Scene Tracker
    int _currentSceneIndex;
    int _numOfScenes;

    // Internal Data
    bool _paused;

    private void Awake()
    {
        // Deactivate UI at start of game
        DeactivateUI();

        // Get current Scene index and total number of scenes in game
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _numOfScenes = SceneManager.sceneCountInBuildSettings;

        // Create UI events
        EventManager.EventInitialise(EventType.NEXT_LEVEL);
        EventManager.EventInitialise(EventType.RESTART_LEVEL);
        EventManager.EventInitialise(EventType.QUIT_LEVEL);
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        _paused = false;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventSubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventSubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventSubscribe(EventType.LEVEL_ENDED, LevelEnded);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
        EventManager.EventUnsubscribe(EventType.LEVEL_ENDED, LevelEnded);
    }

    // Button callback to go to the next level
    public void NextLevel()
    {
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.NEXT_LEVEL, null);
    }

    // Button callback to restart level
    public void Restart()
    {
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.RESTART_LEVEL, null);
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.QUIT_LEVEL, null);
    }

    public void TogglePause(object data)
    {
        _paused = !_paused;

        if (!_paused)
        {
            DeactivateUI();
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.0f;
            _nextLevelButton.SetActive(false);
            _buttonsPanel.SetActive(true);
            _pausePanel.SetActive(true);
        }
    }

    public void LevelEnded(object data)
    {
        DeactivateUI();
        _paused = false;
    }

    private void DeactivateUI()
    {
        _pausePanel.SetActive(false);
        _losePanel.SetActive(false);
        _winPanel.SetActive(false);
        _buttonsPanel.SetActive(false);
        _nextLevelButton.SetActive(false);
    }

    private void ShowLosePanel(object data)
    {
        _nextLevelButton.SetActive(false);
        _buttonsPanel.SetActive(true);
        _losePanel.SetActive(true);
    }

    private void ShowWinPanel(object data)
    {
        // If last level, do not show next level button
        if (_currentSceneIndex == _numOfScenes - 1)
        {
            _nextLevelButton.SetActive(false);
        }
        else
        {
            _nextLevelButton.SetActive(true);
        }

        _buttonsPanel.SetActive(true);
        _nextLevelButton.SetActive(true);
        _winPanel.SetActive(true);
    }
}
