using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneSystemManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] bool _editingLevel = false;

    // Scene Fader
    Fader _fader;

    // Scene Tracking
    Scene _currentLevel;
    int _numOfScenes; // Number of total scenes in the game
    int _mainMenuIndex;
    int _gameplayIndex;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Cache child fader
        _fader = GetComponentInChildren<Fader>();

        // Get total number of scenes in game and indexes for main menu and gameplay scenes
        _numOfScenes = SceneManager.sceneCountInBuildSettings;
        _mainMenuIndex = GetBuildIndex("MainMenu");
        _gameplayIndex = GetBuildIndex("Gameplay");

        // Create level events
        EventManager.EventInitialise(EventType.LEVEL_STARTED);
        EventManager.EventInitialise(EventType.LEVEL_ENDED);
        EventManager.EventInitialise(EventType.SAVE_GAME);
        EventManager.EventInitialise(EventType.SCENE_COUNT);
        EventManager.EventInitialise(EventType.FADING);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventSubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventSubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventSubscribe(EventType.WIN, WinHandler);
        EventManager.EventSubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
        EventManager.EventSubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_SELECTED, LevelSelected);
        EventManager.EventUnsubscribe(EventType.NEXT_LEVEL, NextLevelHandler);
        EventManager.EventUnsubscribe(EventType.RESTART_LEVEL, RestartLevelHandler);
        EventManager.EventUnsubscribe(EventType.WIN, WinHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_LEVEL, QuitLevelHandler);
        EventManager.EventUnsubscribe(EventType.QUIT_GAME, QuitGameHandler);
    }

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        if (!_editingLevel)
        {
            StartCoroutine(LoadScene(_mainMenuIndex));
            StartCoroutine(_fader.NormalFadeIn());
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
    // Listens for when game is won
    public void WinHandler(object data)
    {
        // Check if last level
        if (_currentLevel.buildIndex < _numOfScenes - 1)
        {
            EventManager.EventTrigger(EventType.SAVE_GAME, _currentLevel.buildIndex + 1);
        }
    }

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
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadLevel(prevLevel));
        yield return StartCoroutine(LoadLevel(newLevel));
        yield return StartCoroutine(_fader.CircleFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    IEnumerator LevelToMenu()
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadLevel(_currentLevel.buildIndex));
        yield return StartCoroutine(UnloadScene(_gameplayIndex));
        yield return StartCoroutine(LoadScene(_mainMenuIndex));
        yield return StartCoroutine(_fader.NormalFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }

    IEnumerator MenuToLevel(int levelSelected)
    {
        EventManager.EventTrigger(EventType.FADING, false);
        yield return StartCoroutine(_fader.NormalFadeOut());
        yield return StartCoroutine(UnloadScene(_mainMenuIndex));
        yield return StartCoroutine(LoadScene(_gameplayIndex));
        yield return StartCoroutine(LoadLevel(levelSelected));
        yield return StartCoroutine(_fader.CircleFadeIn());
        EventManager.EventTrigger(EventType.FADING, true);
    }
    #endregion

    #region Level Loading/Unloading
    // Only loads levels, does not load MainMenu scene or core scenes
    IEnumerator LoadLevel(int index)
    {
        yield return StartCoroutine(LoadScene(index));
        EventManager.EventTrigger(EventType.LEVEL_STARTED, null);
        _currentLevel = SceneManager.GetSceneByBuildIndex(index);
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

        Scene scene = SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1);
        SceneManager.SetActiveScene(scene);
        _currentLevel = scene;

        // Send event that says if this is the last level in the build
        if (index == _numOfScenes - 1 && (index != _mainMenuIndex || index != _gameplayIndex))
        {
            EventManager.EventTrigger(EventType.SCENE_COUNT, true);
        }
        else
        {
            EventManager.EventTrigger(EventType.SCENE_COUNT, false);
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

    public void QuitGameHandler(object data)
    {
        StartCoroutine(QuitGame());
    }

    IEnumerator QuitGame()
    {
        yield return StartCoroutine(_fader.NormalFadeOut());

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
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
}
