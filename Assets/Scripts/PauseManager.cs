using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Drag panel Pause Menu ke sini di Inspector

    private bool isPaused = false;

    void Start()
    {
        // Pastikan game berjalan normal saat scene dimulai
        Time.timeScale = 1f;

        // Sembunyikan panel pause
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PauseMenuUI belum di-assign di Inspector!");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
                Debug.Log("Game dipause. Menampilkan panel pause.");
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
                Debug.Log("Game dilanjutkan. Menyembunyikan panel pause.");
            }
        }
    }
}