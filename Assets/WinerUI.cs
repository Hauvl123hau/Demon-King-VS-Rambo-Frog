using UnityEngine;
using TMPro;

public class WinerUI : MonoBehaviour
{
    [SerializeField] private GameObject winnerPanel;  // Drag the WinnerPanel GameObject here in Inspector
    [SerializeField] private TextMeshProUGUI winMessageText; // Reference to the TextMeshPro component for winner message
    private BossHealth bossHealth; // Reference to the boss health
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure the winner panel is hidden at start
        if (winnerPanel != null)
        {
            winnerPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("WinnerPanel has not been assigned in Inspector!");
        }
        
        // Find the boss in the scene
        bossHealth = FindObjectOfType<BossHealth>();
        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealth component not found in scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if boss exists and is dead
        if (bossHealth != null && bossHealth.currentHealth <= 0)
        {
            ShowWinnerPanel();
        }
    }
    
    // Display the winner panel
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
