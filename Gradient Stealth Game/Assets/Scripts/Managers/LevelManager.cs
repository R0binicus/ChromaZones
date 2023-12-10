using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
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
        // Get total number of scenes in game
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

    // After Services Scene is loaded in, additively load in the MainMenu scene
    private void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }

    // Listens for when NextLevelButton is pressed
    public void NextLevelHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("NextLevelHandler did not receive a float as data");
        }

        float delayTime = (float)data;

        if (_currentLoadableScene < _numOfScenes - 1)
        {
            StartCoroutine(LoadScene(_currentLoadableScene + 1, delayTime));
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
        StartCoroutine(LoadScene(_currentLoadableScene, delayTime));
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
