using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [Header("UI Panel Saat Level Selesai")]
    public GameObject finishPanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger masuk oleh: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player menyentuh pintu finish!");
            Time.timeScale = 0f; // Pause game

            if (finishPanel != null)
            {
                finishPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("finishPanel belum di-assign di Inspector!");
            }
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Scene berikutnya tidak ditemukan di Build Settings!");
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Ganti sesuai nama scene menu utama kamu
    }
}