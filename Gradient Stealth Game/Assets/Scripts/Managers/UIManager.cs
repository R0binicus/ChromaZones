using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;
using Unity.Collections;

public class UIManager : MonoBehaviour
{
    // UI Panels
    [Header("Canvases")]
    [SerializeField] GameObject _gameCanvas;
    [Header("Game Panels")]
    [SerializeField] GameObject _winPanel;
    [SerializeField] GameObject _losePanel;
    [SerializeField] GameObject _pausePanel;
    [SerializeField] GameObject _buttonsPanel;
    [Header("Buttons")]
    [SerializeField] GameObject _nextLevelButton;
    [Header("Fading Data")]
    [SerializeField] Image _fadePanel;
    [SerializeField] AnimationCurve _fadeInSpeed;
    [SerializeField] AnimationCurve _fadeOutSpeed;

    [Header("Sounds")]
    [SerializeField] private Sound _soundButtonSFX;

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
        FadeIn();
        Time.timeScale = 1.0f;
        _paused = false;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventSubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventSubscribe(EventType.SCENE_LOAD, FadeOut);
        EventManager.EventSubscribe(EventType.PAUSE_TOGGLE, TogglePause);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
        EventManager.EventUnsubscribe(EventType.SCENE_LOAD, FadeOut);
        EventManager.EventUnsubscribe(EventType.PAUSE_TOGGLE, TogglePause);
    }

    public void NextLevel()
    {
        EventManager.EventTrigger(EventType.SFX, _soundButtonSFX);
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.NEXT_LEVEL, _fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time);
    }

    // Button callback to restart level
    public void Restart()
    {
        EventManager.EventTrigger(EventType.SFX, _soundButtonSFX);
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.RESTART_LEVEL, _fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time);
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        EventManager.EventTrigger(EventType.SFX, _soundButtonSFX);
        Time.timeScale = 1.0f;
        EventManager.EventTrigger(EventType.QUIT_LEVEL, _fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time);
    }

    public void TogglePause(object data)
    {
        _paused = !_paused;
        Debug.Log("Dobug");

        if (!_paused)
        {
            DeactivateUI();
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.0f;
            _nextLevelButton.SetActive(false);
            _buttonsPanel.SetActive(true);
            _pausePanel.SetActive(true);
        }
    }

    private void DeactivateUI()
    {
        _pausePanel.SetActive(false);
        _losePanel.SetActive(false);
        _winPanel.SetActive(false);
        _buttonsPanel.SetActive(false);
        _nextLevelButton.SetActive(false);
    }

    private void ShowLosePanel(object data)
    {
        _nextLevelButton.SetActive(false);
        _buttonsPanel.SetActive(true);
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

        _buttonsPanel.SetActive(true);
        _nextLevelButton.SetActive(true);
        _winPanel.SetActive(true);
    }

    // Listens for when a scene is about to change
    public void FadeOut(object data)
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
            _fadePanel.color = new Color(0, 0, 0, Mathf.Lerp
            (
                fadeCurve.keys[0].time,
                fadeCurve.keys[fadeCurve.length - 1].time,
                fadeCurve.Evaluate(Time.time - startTime)
            ));
            yield return null;
        }

        _fadePanel.gameObject.SetActive(false);
    }
}
