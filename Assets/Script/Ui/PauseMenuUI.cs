using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject UIPanel;
    public GameObject BossUIPanel;

    [Header("Buttons")]
    public Button resumeButton;
    public Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        AudioManager.Instance.PlayBGM("BGM");
        // Hide the pause menu initially
        pauseMenuPanel.SetActive(false);

        // Assign button events
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitToMainMenu);
    }

    void Update()
    {
        // Check for Escape key (you can also map this to a new Input Action)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // freeze time
        pauseMenuPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UIPanel.SetActive(false);
        BossUIPanel.SetActive(false);
        // Optional: Play pause SFX or glitch FX
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        UIPanel.SetActive(true);
        BossUIPanel.SetActive(true);
        /* Cursor.visible = false;
         Cursor.lockState = CursorLockMode.Locked;*/
        // Optional: Resume ambient audio
    }

    void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Make sure time resumes
        SceneManager.LoadScene("MainMenu"); // Change this to your actual scene name
    }
}
