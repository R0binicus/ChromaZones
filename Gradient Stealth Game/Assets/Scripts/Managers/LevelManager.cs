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
        EventManager.EventInitialise(EventType.SCENE_LOAD);
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
        if (data == null)
        {
            Debug.LogError("NextLevelHandler did not receive a float as data");
        }

        float delayTime = (float)data;

        if (_currentSceneIndex < _numOfScenes - 1)
        {
            StartCoroutine(LoadScene(_currentSceneIndex + 1, delayTime));
        }
    }

    // Listens for when ReplayLevelButton is pressed
    public void RestartLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("RestartLevelHandler did not receive a float as data");
        }

        float delayTime = (float)data;
        StartCoroutine(LoadScene(_currentSceneIndex, delayTime));
    }

    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("QuitLevelHandler did not receive a float as data");
        }

        float delayTime = (float)data;
        StartCoroutine(LoadScene(0, delayTime));
    }

    IEnumerator LoadScene(int index, float delayTime)
    {
        EventManager.EventTrigger(EventType.SCENE_LOAD, null);
        yield return new WaitForSeconds(delayTime - 0.1f);
        SceneManager.LoadScene(index);
    }
}
