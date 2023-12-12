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

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _mainMenuIndex = GetBuildIndex("MainMenu");

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
            _fadePanel.alpha = 1f;
            StartCoroutine(LoadScene(_mainMenuIndex));
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

    IEnumerator LevelChanger(int prevLevel, int newLevel)
    {
        yield return StartCoroutine(UnloadLevel(prevLevel));
        yield return StartCoroutine(LoadLevel(newLevel));
    }

    IEnumerator LevelToMenu()
    {
        yield return StartCoroutine(UnloadLevel(_currentLevel.buildIndex));
        yield return StartCoroutine(UnloadGameplay());
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
    }

    IEnumerator MenuToLevel(int levelSelected)
    {
        yield return StartCoroutine(UnloadScene(_mainMenuIndex));
        yield return StartCoroutine(LoadGameplay());
        yield return StartCoroutine(LoadLevel(levelSelected));
    }

    #region Services Loading/Unloading
    IEnumerator LoadGameplay()
    {
        var levelAsync = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);

        // Wait until the scene fully loads to fade in
        while (!levelAsync.isDone)
        {
            yield return null;
        }
    }

    IEnumerator UnloadGameplay()
    {
        var levelAsync = SceneManager.UnloadSceneAsync("Gameplay");
        
        while (!levelAsync.isDone)
        {
            yield return null;
        }
    }
    #endregion

    #region Level Loading/Unloading
    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        yield return StartCoroutine(LoadScene(index));
        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
        SceneManager.SetActiveScene(_currentLevel);
        EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
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
        
        yield return Fade(_fadeInSpeed, Time.time);
    }

    IEnumerator UnloadScene(int index)
    {
        // Wait until fadeout is complete to unload the level
        yield return Fade(_fadeOutSpeed, Time.time);

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
