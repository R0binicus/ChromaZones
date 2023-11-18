using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _gameCanvas;
    [SerializeField] GameObject _winPanel;
    [SerializeField] GameObject _losePanel;

    private void Awake()
    {
        // Deactivate UI at start of game
        DeactivateUI();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventSubscribe(EventType.WIN, ShowWinPanel);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LOSE, ShowLosePanel);
        EventManager.EventUnsubscribe(EventType.WIN, ShowWinPanel);
    }

    // Button callback to replay game
    public void Replay()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    //Button callback to go back to main menu
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    private void DeactivateUI()
    {
        _gameCanvas.SetActive(false);
        _losePanel.SetActive(false);
        _winPanel.SetActive(false);
    }

    private void ShowLosePanel(object data)
    {
        _gameCanvas.SetActive(true);
        _losePanel.SetActive(true);
    }

    private void ShowWinPanel(object data)
    {
        _gameCanvas.SetActive(true);
        _winPanel.SetActive(true);
    }
}
