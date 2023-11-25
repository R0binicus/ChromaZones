using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Scene Tracker
    int _currentSceneIndex;
    int _numOfScenes;

    private void Awake()
    {
        // Get current Scene index and total number of scenes in game
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _numOfScenes = SceneManager.sceneCountInBuildSettings;

        // Create level events
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventSubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventSubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);

    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventUnsubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
    }

    // Listens for when NextLevelButton is pressed
    public void NextLevelHandler(object data)
    {
        if (_currentSceneIndex < _numOfScenes - 1)
        {
            SceneManager.LoadScene(_currentSceneIndex + 1);
        }
    }

    // Listens for when ReplayLevelButton is pressed
    public void RestartLevelHandler(object data)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        SceneManager.LoadScene(0);
    }
}
