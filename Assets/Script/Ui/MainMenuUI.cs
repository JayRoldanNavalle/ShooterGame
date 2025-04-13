using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject settingsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button controlsButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Back Buttons")]
    public Button controlsBackButton;
    public Button settingsBackButton;

    [Header("Scene Settings")]
    public string gameSceneName = "GameScene"; // Replace with your actual scene name

    private void Start()
    {
        
        // Hook up button events
        playButton.onClick.AddListener(PlayGame);
        controlsButton.onClick.AddListener(OpenControls);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);

        controlsBackButton.onClick.AddListener(ReturnToMainMenu);
        settingsBackButton.onClick.AddListener(ReturnToMainMenu);

        // Start on main menu
        ShowPanel(mainMenuPanel);
    }

    void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenControls()
    {
        ShowPanel(controlsPanel);
    }

    void OpenSettings()
    {
        ShowPanel(settingsPanel);
    }

    void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    void ReturnToMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        // Disable all panels first
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        // Enable the target panel
        panelToShow.SetActive(true);
    }
}
