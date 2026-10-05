using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject container;
    public GameObject pauseUI;
    public GameObject optionsUI;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauza();
        }
    }

    public void TogglePauza()
    {
        if (isPaused)
        {
            ResumeButton();
        }
        else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        container.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void OpenOptions()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsUI.SetActive(false);
        pauseUI.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}