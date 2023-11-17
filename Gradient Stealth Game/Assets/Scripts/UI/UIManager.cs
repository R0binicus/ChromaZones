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

    // Button callback to replay game
    public void Replay()
    {
        SceneManager.LoadScene(1);
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
}
