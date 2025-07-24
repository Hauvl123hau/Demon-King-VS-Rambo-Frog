using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DeadUI : MonoBehaviour
{
    [SerializeField] private GameObject deadPanel;  // Kéo GameObject DeadPanel vào đây trong Inspector
    [SerializeField] private TextMeshProUGUI deathMessageText; // Tham chiếu đến thành phần TextMeshPro cho thông báo tử vong
    
    // Start được gọi một lần trước khi Update thực thi lần đầu tiên sau khi MonoBehaviour được tạo
    void Start()
    {
        // Đảm bảo panel tử vong bị ẩn khi bắt đầu
        if (deadPanel != null)
        {
            deadPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DeadPanel chưa được gán trong Inspector!");
        }
    }
    
    // Hiển thị panel tử vong
    public void ShowDeadPanel()
    {
        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
            Time.timeScale = 0; // Tạm dừng trò chơi
            
            // Đặt thông báo tử vong mặc định
            if (deathMessageText != null)
            {
                deathMessageText.text = "Bạn đã chết!";
            }
        }
    }
    
    // Hiển thị panel tử vong với thông báo tùy chỉnh
    public void ShowDeadPanelWithMessage(string message)
    {
        if (deadPanel != null)
        {
            deadPanel.SetActive(true);
            Time.timeScale = 0; // Tạm dừng trò chơi
            
            // Đặt thông báo tử vong tùy chỉnh nếu thành phần văn bản tồn tại
            if (deathMessageText != null)
            {
                deathMessageText.text = message;
            }
        }
    }
    
    // Các chức năng nút cho Panel tử vong
    
    // Khởi động lại màn chơi hiện tại
    public void Restart()
    {
        SceneManager.LoadScene("Lever-1");
        Time.timeScale = 1; // Tiếp tục trò chơi
    }
    
    // Quay lại menu chính
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1; // Tiếp tục trò chơi
    }
    
    // Thoát trò chơi
    public void QuitGame()
    {
        Debug.Log("Đang thoát trò chơi");
        Application.Quit();
    }
}
