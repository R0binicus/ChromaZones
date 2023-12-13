using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUIManager : MonoBehaviour
{
    // Menu Panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private GameObject _levelSelectMenu;
    [SerializeField] private GameObject _creditsMenu;
    private List<GameObject> _panels;

    // Confirm Box
    [Header("Confirm Box")]
    [SerializeField] private GameObject _confirmBox;
    [SerializeField] private GameObject _cbContinueButton;
    [SerializeField] private GameObject _cbBackButton;
    [SerializeField] private TextMeshProUGUI _cbText;
    [SerializeField, TextArea] private string _newGameText;
    [SerializeField, TextArea] private string _loadGameFailedText;
    [SerializeField, TextArea] private string _loadGameSuccessText;
    
    // Level Buttons
    [Header("Level Buttons Panel")]
    [SerializeField] private GameObject _levelButtonPanel;

    // Internal Data
    private bool _startNewGame;
    private Button[] _levelButtonsArr;
    private List<Button> _levelButtons;

    #region Init
    private void Awake()
    {
        EventManager.EventInitialise(EventType.LEVEL_SELECTED);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOAD_GAME_SUCCESS, LoadSuccessHandler);
        EventManager.EventSubscribe(EventType.LOAD_GAME_FAILED, LoadFailedHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOAD_GAME_SUCCESS, LoadSuccessHandler);
        EventManager.EventUnsubscribe(EventType.LOAD_GAME_FAILED, LoadFailedHandler);
    }

    private void Start()
    {
        CreateLists();
        DeactivateAllLevelButtons();
        ConfirmBoxToggle(false);
        ShowPanel(_mainMenu);
        _startNewGame = false;
    }
    #endregion

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
        StartCoroutine(QuitGame(0));
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
    #endregion

    #region Play Menu
    public void NewGameButton()
    {
        ConfirmBoxNewGame();
        _startNewGame = true;
    }

    public void LoadGameButton()
    {
        _startNewGame = false;
        EventManager.EventTrigger(EventType.LOAD_GAME_REQUEST, null);
    }
    #endregion

    #region Level Select Menu
    public void LevelSelectButton(int levelNum)
    {
        EventManager.EventTrigger(EventType.LEVEL_SELECTED, levelNum);
    }

    public void DeactivateAllLevelButtons()
    {
        foreach (Button button in _levelButtons)
        {
            button.interactable = false;
            button.GetComponent<Image>().color = Color.grey;
            button.GetComponent<ColourRegionUI>().DisableColourChange();
        }

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
        ConfirmBoxToggle(true);
    }

    public void LoadSuccessHandler(object data)
    {
        ConfirmBoxPopulate(false, true, _loadGameSuccessText);
    }

    public void LoadFailedHandler(object data)
    {
        ConfirmBoxPopulate(true, false, _loadGameFailedText);
    }

    // Confirm box pop up for clicking on New Game button
    public void ConfirmBoxNewGame()
    {
        ConfirmBoxPopulate(true, true, _newGameText);
    }

    // If continue is pressed when confirm box pops up, send to level select menu
    public void ConfirmBoxContinueButton()
    {
        if (_startNewGame)
        {
            EventManager.EventTrigger(EventType.NEW_GAME_REQUEST, null);
        }
        ConfirmBoxToggle(false);
        ShowPanel(_levelSelectMenu);
    }

    // If back button is pressed on confirm box pop up
    public void ConfirmBoxBackButton()
    {
        ConfirmBoxToggle(false);
    }
    #endregion

    private void CreateLists()
    {
        _panels = new List<GameObject>() { _mainMenu, _playMenu, _levelSelectMenu, _creditsMenu };
        _levelButtonsArr = _levelButtonPanel.GetComponentsInChildren<Button>();
        _levelButtons = _levelButtonsArr.ToList<Button>();
        _levelButtons.Remove(_levelButtons.Last<Button>());
    }
}
