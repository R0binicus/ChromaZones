using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSystemManager : MonoBehaviour
{
    [Header("Scene Fading Data")]
    [SerializeField] CanvasGroup _fadePanel;
    [SerializeField] AnimationCurve _fadeInSpeed;
    [SerializeField] AnimationCurve _fadeOutSpeed;

    // Scene Tracking
    int _currentLoadableScene;
    int _numOfScenes;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;

        // Get total number of scenes in game
        _numOfScenes = SceneManager.sceneCountInBuildSettings;

        // Create level events
        EventManager.EventInitialise(EventType.SCENE_LOAD);
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
        //SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    #region Game UI Response
    // Listens for when NextLevelButton is pressed
    public void NextLevelHandler(object data)
    {
        // Check if last level
        if (_currentLoadableScene < _numOfScenes - 1)
        {
            StartCoroutine(LoadLevel(_currentLoadableScene + 1));
        }
    }

    // Listens for when ReplayLevelButton is pressed
    public void RestartLevelHandler(object data)
    {
        StartCoroutine(LoadLevel(_currentLoadableScene));
    }

    // Listens for when UIManager QuitButton is pressed
    public void QuitLevelHandler(object data)
    {
        Debug.Log("Quitting in level manager");
        SceneManager.UnloadSceneAsync("Level02");
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

    IEnumerator LoadLevel(int index)
    {
        EventManager.EventTrigger(EventType.SCENE_LOAD, null);
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(index, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneAt(index);
        SceneManager.SetActiveScene(scene);
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
