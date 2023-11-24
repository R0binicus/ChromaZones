using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // UI Panels
    [SerializeField] GameObject _gameCanvas;
    [SerializeField] GameObject _winPanel;
    [SerializeField] GameObject _losePanel;
    [SerializeField] GameObject _nextLevelButton;

    // Scene Tracker
    int _currentSceneIndex;
    int _numOfScenes;

    private void Awake()
    {
        // Deactivate UI at start of game
        DeactivateUI();

        // Get current Scene index and total number of scenes in game
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
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
        if (_currentSceneIndex < _numOfScenes - 1)
        {
            SceneManager.LoadScene(_currentSceneIndex + 1);
        }
    }

    // Button callback to replay game
    public void Replay()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        SceneManager.LoadScene(0);
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
}
