using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // UI Panels
    [Header("Canvases")]
    [SerializeField] GameObject _gameCanvas;
    [Header("Game Panels")]
    [SerializeField] GameObject _winPanel;
    [SerializeField] GameObject _losePanel;
    [SerializeField] GameObject _pausePanel;
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

    private void Update()
    {
        // Toggle pause
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventSubscribe(EventType.WIN, ShowWinPanel);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
    }

    public void NextLevel()
    {
        EventManager.EventTrigger(EventType.NEXT_LEVEL, null);
    }

    // Button callback to restart level
    public void Restart()
    {
        EventManager.EventTrigger(EventType.RESTART_LEVEL, null);
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        EventManager.EventTrigger(EventType.QUIT_LEVEL, null);
    }

    public void TogglePause()
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
            _gameCanvas.SetActive(_paused);
            _pausePanel.SetActive(_paused);
        }
    }

    private void DeactivateUI()
    {
        _gameCanvas.SetActive(false);
        _losePanel.SetActive(false);
        _winPanel.SetActive(false);
        _nextLevelButton.SetActive(false);
    }

    private void ShowLosePanel(object data)
    {
        _nextLevelButton.SetActive(false);
        _gameCanvas.SetActive(true);
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

        _gameCanvas.SetActive(true);
        _winPanel.SetActive(true);
    }

    IEnumerator FadeIn()
    {
        yield return null;
    }
}
