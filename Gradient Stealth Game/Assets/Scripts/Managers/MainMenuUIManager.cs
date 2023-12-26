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
    // Title Card
    [Header("Title")]
    [SerializeField] private GameObject _title;

    // Menu Panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private GameObject _levelSelectMenu;
    [SerializeField] private GameObject _creditsMenu;
    private List<GameObject> _panels;

    [Header("Credits")]
    [SerializeField] private List<GameObject> _credits;
    private int _activeCreditIndex;

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

    [Header("Sound")]
    [SerializeField] Sound _titleMusic; 
    [SerializeField] Sound _buttonSFX;

    [Header("Debugging")]
    [SerializeField] private bool _unlockAllLevels;

    // Internal Data
    private SaveData _loadData;
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
        _activeCreditIndex = 0;
        EventManager.EventTrigger(EventType.MUSIC, _titleMusic);
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

        if (panel == _mainMenu || _playMenu)
        {
            _title.SetActive(true);
        }
        else
        {
            _title.SetActive(false);
        }

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
            button.GetComponent<ColourRegionUI>().Enabled = false;
        }
    }

    public void ActivateLevelButtons(int levelUnlocked)
    {
        for (int i = 0; i < levelUnlocked; i++)
        {
            _levelButtons[i].interactable = true;
            _levelButtons[i].GetComponent<ColourRegionUI>().Enabled = true;
        }
    }
    #endregion

    #region Credits
    public void CreditsScrollButton(int dir)
    {
        _credits[_activeCreditIndex].SetActive(false);

        _activeCreditIndex += dir;

        // Loop to back
        if (_activeCreditIndex == -1)
        {
            _activeCreditIndex = _credits.Count() - 1;
        }
        // Loop to front
        else if (_activeCreditIndex == _credits.Count())
        {
            _activeCreditIndex = 0;
        }

        _credits[_activeCreditIndex].SetActive(true);
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
        if (data == null)
        {
            Debug.LogError("Loading has failed.");
        }

        _loadData = (SaveData)data;
        ConfirmBoxPopulate(false, true, _loadGameSuccessText);
    }

    public void LoadFailedHandler(object data)
    {
        _loadData = null;
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
        DeactivateAllLevelButtons();
        
        if (_unlockAllLevels)
        {
            ActivateLevelButtons(_levelButtons.Count);
        }
        else
        {
            if (_startNewGame)
            {
                _loadData = null;
                EventManager.EventTrigger(EventType.NEW_GAME_REQUEST, null);
                ActivateLevelButtons(1);
            }
            else
            {
                if (_loadData != null)
                {
                    ActivateLevelButtons(_loadData.LevelUnlocked);
                }
                else
                {
                    Debug.LogError("Load has failed when confirming.");
                }
            }
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

        // Create a List of Level Buttons from the array given by GetComponent
        _levelButtonsArr = _levelButtonPanel.GetComponentsInChildren<Button>();
        _levelButtons = _levelButtonsArr.ToList<Button>();
        
        // Remove the Back Button from the list
        _levelButtons.Remove(_levelButtons.Last<Button>());
    }

    public void ButtonSFX()
    {
        EventManager.EventTrigger(EventType.SFX, _buttonSFX);
    }
}
