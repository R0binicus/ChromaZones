using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSystemManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool _editingLevel = false;

    [Header("Scene Fading Data")]
    [SerializeField] CanvasGroup _fadePanel;
    [SerializeField] AnimationCurve _fadeInSpeed;
    [SerializeField] AnimationCurve _fadeOutSpeed;

    // Scene Tracking
    Scene _currentLevel;
    int _numOfScenes;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game
        _numOfScenes = SceneManager.sceneCountInBuildSettings;

        // Create level events
        EventManager.EventInitialise(EventType.LEVEL_STARTED);
        EventManager.EventInitialise(EventType.LEVEL_ENDED);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventSubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventSubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventSubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventUnsubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventUnsubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
    }

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        if (!_editingLevel)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
        // Make sure current level loaded in editor is assigned as the current level
        else
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
        }
    }

    #region Game UI Response
    // Listens for when NextLevelButton is pressed
    public void NextLevelHandler(object data)
    {
        // Check if last level
        if (_currentLevel.buildIndex < _numOfScenes - 1)
        {
            StartCoroutine(LoadLevel(_currentLevel.buildIndex + 1));
        }
    }

    // Listens for when ReplayLevelButton is pressed
    public void RestartLevelHandler(object data)
    {
        UnloadLevel();
        StartCoroutine(LoadLevel(_currentLevel.buildIndex));
    }

    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        Debug.Log("Quitting in level manager");
        UnloadLevel();
        SceneManager.UnloadSceneAsync("Gameplay");
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
    #endregion

    #region Main Menu UI Response
    public void LevelSelected(object data)
    {
        if (data == null)
        {
            Debug.LogError("Level has not been chosen!");
        }

        SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        int sceneIndex = (int)data + 2;
        StartCoroutine(LoadLevel(sceneIndex));
    }
    #endregion

    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        var levelAsync = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        // Wait until the level fully loads to trigger the level started event
        while (!levelAsync.isDone)
        {
            Debug.Log("Level Loading...");
            yield return null;
        }

        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.SetActiveScene(_currentLevel);
        EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
    }

    private void UnloadLevel()
    {
        EventManager.EventTrigger(EventType.LEVEL_ENDED, null);
        SceneManager.UnloadSceneAsync(_currentLevel);
    }

    #region Scene Fading
    public void FadeOut()
    {
        StartCoroutine(Fade(_fadeOutSpeed, Time.time));
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(_fadeInSpeed, Time.time));
    }

    IEnumerator Fade(AnimationCurve fadeCurve, float startTime)
    {
        _fadePanel.gameObject.SetActive(true);

        while (Time.time - startTime < fadeCurve.keys[fadeCurve.length - 1].time)
        {
            _fadePanel.alpha = Mathf.Lerp
            (
                fadeCurve.keys[0].time,
                fadeCurve.keys[fadeCurve.length - 1].time,
                fadeCurve.Evaluate(Time.time - startTime)
            );
            yield return null;
        }

        _fadePanel.gameObject.SetActive(false);
    }
    #endregion
}
