using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeadUI : MonoBehaviour
{
    [SerializeField] private GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI deathMessageText;
    
    void Start()
    {
        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DeadPanel chưa được gán trong Inspector!");
        }
    }
    
    public void ShowDeadPanel()
    {
        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
            Time.timeScale = 0;
            
            if (deathMessageText != null)
            {
                deathMessageText.text = "You are Dead!";
            }
        }
    }
    
    public void ShowDeadPanelWithMessage(string message)
    {
        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
            Time.timeScale = 0;
            
            if (deathMessageText != null)
            {
                deathMessageText.text = message;
            }
        }
    }
    
    public void Restart()
    {
        SceneManager.LoadScene("Lever-1");
        Time.timeScale = 1;
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    
    public void QuitGame()
    {
        Debug.Log("Đang thoát trò chơi");
        Application.Quit();
    }
}
