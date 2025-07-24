using UnityEngine;
using UnityEngine.SceneManagement;

public class Deadzone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Hiển thị UI tử vong khi người chơi rơi xuống hố
            DeadUI deadUI = FindObjectOfType<DeadUI>();
            if (deadUI != null)
            {
                deadUI.ShowDeadPanelWithMessage("Bạn đã rơi xuống hố!");
            }
            else
            {
                // Quay lại tải trực tiếp cảnh nếu không tìm thấy DeadUI
                SceneManager.LoadScene("Lever-1");
                Debug.Log("Người chơi rơi xuống hố. Đang tải lại Lever 1.");
            }
        }
    }
}

  