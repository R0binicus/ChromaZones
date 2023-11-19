using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    // Menu Panels
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _playMenu;
    [SerializeField] GameObject _instructions;

    AudioSource _buttonSFXSource;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        _buttonSFXSource = GetComponent<AudioSource>();
    }

    public void PlayButton()
    {
        _mainMenu.SetActive(false);
        _playMenu.SetActive(true);
    }

    public void SelectLevelButton(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void InstructionsButton()
    {
        _instructions.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit(), quitDelay;
#endif
    }

    public void BackToMenuButton()
    {
        _instructions.SetActive(false);
        _playMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }
}
