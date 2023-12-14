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
    int _mainMenuIndex;
    int _gameplayIndex;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _mainMenuIndex = GetBuildIndex("MainMenu");
        _gameplayIndex = GetBuildIndex("Gameplay");

        // Create level events
        EventManager.EventInitialise(EventType.LEVEL_STARTED);
        EventManager.EventInitialise(EventType.LEVEL_ENDED);
        EventManager.EventInitialise(EventType.SAVE_GAME);
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
            _fadePanel.alpha = 1f;
            StartCoroutine(LoadScene(_mainMenuIndex));
            StartCoroutine(Fade(_fadeInSpeed, Time.time));
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

            EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
        }
    }

    #region Game UI Response
    // Listens for when NextLevelButton is pressed
    public void NextLevelHandler(object data)
    {
        // Check if last level
        if (_currentLevel.buildIndex < _numOfScenes - 1)
        {
            EventManager.EventTrigger(EventType.SAVE_GAME, _currentLevel.buildIndex + 1);
            StartCoroutine(LevelChanger(_currentLevel.buildIndex, _currentLevel.buildIndex + 1));
        }
    }

    // Listens for when ReplayLevelButton is pressed
    public void RestartLevelHandler(object data)
    {
        StartCoroutine(LevelChanger(_currentLevel.buildIndex, _currentLevel.buildIndex));
    }

    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        EventManager.EventTrigger(EventType.SAVE_GAME, _currentLevel.buildIndex);
        StartCoroutine(LevelToMenu());
    }
    #endregion

    #region Main Menu UI Response
    public void LevelSelected(object data)
    {
        if (data == null)
        {
            Debug.LogError("Level has not been chosen!");
        }

        int sceneIndex = (int)data + 2;
        StartCoroutine(MenuToLevel(sceneIndex));
    }
    #endregion

    #region Scene Loading/Unloading/Ordering
    IEnumerator LevelChanger(int prevLevel, int newLevel)
    {
        yield return StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        yield return StartCoroutine(UnloadLevel(prevLevel));
        yield return StartCoroutine(LoadLevel(newLevel));
        yield return StartCoroutine(Fade(_fadeInSpeed, Time.time));
    }

    IEnumerator LevelToMenu()
    {
        yield return StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        yield return StartCoroutine(UnloadLevel(_currentLevel.buildIndex));
        yield return StartCoroutine(UnloadScene(_gameplayIndex));
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
        yield return StartCoroutine(Fade(_fadeInSpeed, Time.time));
    }

    IEnumerator MenuToLevel(int levelSelected)
    {
        yield return StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        yield return StartCoroutine(UnloadScene(_mainMenuIndex));
        yield return StartCoroutine(LoadScene(_gameplayIndex));
        yield return StartCoroutine(LoadLevel(levelSelected));
        yield return StartCoroutine(Fade(_fadeInSpeed, Time.time));
    }
    #endregion

    #region Level Loading/Unloading
    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        yield return StartCoroutine(LoadScene(index));
        EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.SetActiveScene(_currentLevel);
    }

    IEnumerator UnloadLevel(int index)
    {
        EventManager.EventTrigger(EventType.LEVEL_ENDED, null);
        yield return StartCoroutine(UnloadScene(index));
    }
    #endregion

    #region Scene Functions
    IEnumerator LoadScene(int index)
    {
        var levelAsync = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        // Wait until the scene fully loads to fade in
        while (!levelAsync.isDone)
        {
            yield return null;
        }        
    }

    IEnumerator UnloadScene(int index)
    {
        var levelAsync = SceneManager.UnloadSceneAsync(index);

        // Wait until the scene fully unloads
        while (!levelAsync.isDone)
        {
            yield return null;
        }
    }

    public int GetBuildIndex(string name)
    {
        for (int index = 0; index < _numOfScenes; index++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(index));

            if (sceneName == name)
            {
                return index;
            }
        }

        Debug.LogError("Scene name not found");
        return -1;
    }
    #endregion

    #region Scene Fading
    IEnumerator Fade(AnimationCurve fadeCurve, float startTime)
    {
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
        _fadePanel.alpha = fadeCurve.keys[fadeCurve.length - 1].value;
    }
    #endregion
}