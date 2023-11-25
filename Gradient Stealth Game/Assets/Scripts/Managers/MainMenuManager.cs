using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    // Menu Panels
    [Header("Menu Panels")]
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _playMenu;
    [SerializeField] GameObject _instructions;

    [Header("Fading Data")]
    [SerializeField] Image _fadePanel;
    [SerializeField] AnimationCurve _fadeInSpeed;
    [SerializeField] AnimationCurve _fadeOutSpeed;

    AudioSource _buttonSFXSource;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        _buttonSFXSource = GetComponent<AudioSource>();
        FadeIn();
    }

    public void PlayButton()
    {
        _mainMenu.SetActive(false);
        _playMenu.SetActive(true);
    }

    public void SelectLevelButton(int level)
    {
        StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        StartCoroutine(LoadScene(level, _fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time));
    }

    public void InstructionsButton()
    {
        _instructions.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void QuitButton()
    {
        StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        StartCoroutine(QuitGame(_fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time));
    }

    public void BackToMenuButton()
    {
        _instructions.SetActive(false);
        _playMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    IEnumerator LoadScene(int index, float delayTime)
    {
        yield return new WaitForSeconds(delayTime - 0.1f);
        SceneManager.LoadScene(index);
    }

    IEnumerator QuitGame(float delayTime)
    {
        yield return new WaitForSeconds(delayTime - 0.1f);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

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
