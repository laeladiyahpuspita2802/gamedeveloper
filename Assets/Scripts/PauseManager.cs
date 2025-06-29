using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;     // Panel berisi Resume + Main Menu
    public GameObject pauseButtonUI;   // Tombol pause utama

    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        else
            Debug.LogWarning("PauseMenuUI belum di-assign!");

        if (pauseButtonUI != null)
            pauseButtonUI.SetActive(true);
        else
            Debug.LogWarning("PauseButtonUI belum di-assign!");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        if (pauseButtonUI != null)
            pauseButtonUI.SetActive(false);

        Debug.Log("Game dipause.");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (pauseButtonUI != null)
            pauseButtonUI.SetActive(true);

        Debug.Log("Game dilanjutkan dari Resume.");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Ganti sesuai nama scene MainMenu kamu
    }
}