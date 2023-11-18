using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _instructions;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
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
        _mainMenu.SetActive(true);
    }
}
