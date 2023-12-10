using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    // Menu Panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private GameObject _levelSelectMenu;
    [SerializeField] private GameObject _creditsMenu;
    private List<GameObject> _panels;
    [Header("Confirm Box")]
    [SerializeField] private GameObject _confirmBox;
    [SerializeField] private GameObject _cbContinueButton;
    [SerializeField] private GameObject _cbBackButton;
    [SerializeField] private TextMeshProUGUI _cbText;
    [SerializeField, TextArea] private string _newGameText;
    [SerializeField, TextArea] private string _loadGameFailedText;
    [SerializeField, TextArea] private string _loadGameSuccessText;

    private void Awake()
    {
        // If using the Unity editor or development build, enable debug logs
        Debug.unityLogger.logEnabled = Debug.isDebugBuild;
        //FadeIn();
    }

    private void Start()
    {
        _panels = new List<GameObject>() { _mainMenu, _playMenu, _levelSelectMenu, _creditsMenu };
        ConfirmBoxToggle(false);
        ShowPanel(_mainMenu);
    }

    #region Panel Functionality
    // Have list of panels to easily deactivate all of them
    private void DeactivateAllPanels()
    {
        foreach (GameObject panel in _panels)
        {
            panel.SetActive(false);
        }
    }

    // Show specified Menu Panel
    private void ShowPanel(GameObject panel)
    {
        DeactivateAllPanels();
        panel.SetActive(true);
    }
    #endregion

    #region Main Menu
    // Shows the play menu
    public void PlayButton()
    {
        ShowPanel(_playMenu);
    }

    // Shows credits
    public void CreditsButton()
    {
        ShowPanel(_creditsMenu);
    }
    
    // All back buttons will return to main menu
    public void BackButton()
    {
        ShowPanel(_mainMenu);
    }

    // Quits the game
    public void QuitButton()
    {
        //StartCoroutine(Fade(_fadeOutSpeed, Time.time));
        //StartCoroutine(QuitGame(_fadeOutSpeed.keys[_fadeOutSpeed.length - 1].time));
        StartCoroutine(QuitGame(0));
    }
    #endregion

    #region Play Menu
    public void NewGameButton()
    {
        CBNewGame();
        ConfirmBoxToggle(true);
    }

    // TODO: Wire up to PersistentDataManager event
    public void LoadGameButton()
    {
        //CBLoadSuccess();
        CBLoadFailed();
        ConfirmBoxToggle(true);
    }
    #endregion

    #region Level Select Menu
    public void LevelSelectButton(int index)
    {

    }
    #endregion

    #region Confirm Box
    private void ConfirmBoxToggle(bool toggle)
    {
        _confirmBox.SetActive(toggle);
    }

    private void ConfirmBoxPopulate(bool showBack, bool showContinue, string text)
    {
        _cbText.text = text;
        _cbBackButton.SetActive(showBack);
        _cbContinueButton.SetActive(showContinue);
    }

    public void CBLoadSuccess()
    {
        ConfirmBoxPopulate(false, true, _loadGameSuccessText);
    }

    public void CBLoadFailed()
    {
        ConfirmBoxPopulate(true, false, _loadGameFailedText);
    }

    public void CBNewGame()
    {
        ConfirmBoxPopulate(true, true, _newGameText);
    }

    public void ConfirmBoxContinueButton()
    {
        ConfirmBoxToggle(false);
        ShowPanel(_levelSelectMenu);
    }

    public void ConfirmBoxBackButton()
    {
        ConfirmBoxToggle(false);
    }
    #endregion

    IEnumerator QuitGame(float delayTime)
    {
        yield return new WaitForSeconds(delayTime - 0.1f);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
