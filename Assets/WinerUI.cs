using UnityEngine;
using TMPro;

public class WinerUI : MonoBehaviour
{
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TextMeshProUGUI winMessageText;
    private BossHealth bossHealth;
    
    void Start()
    {
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("WinnerPanel has not been assigned in Inspector!");
        }
        
        bossHealth = FindObjectOfType<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealth component not found in scene!");
        }
    }

    void Update()
    {
        if (bossHealth != null && bossHealth.currentHealth <= 0)
        {
            ShowWinnerPanel();
        }
    }
    
    public void ShowWinnerPanel()
    {
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
            if (winMessageText != null)
            {
                winMessageText.text = "YOU WIN!";
            }
        }
    }
}
