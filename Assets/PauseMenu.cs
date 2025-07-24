using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }
    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1; // Resume the game
    }
    public void Exit()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }
    public void Restart()
    {
        SceneManager.LoadScene("Lever-1");
        Time.timeScale = 1; // Resume the game
    }
}
