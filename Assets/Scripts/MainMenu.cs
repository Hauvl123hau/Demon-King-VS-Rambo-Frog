using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
      SceneManager.LoadScene("Lever-1");
    }

    public void QuitGame()
    {
       Application.Quit();
    }
}
