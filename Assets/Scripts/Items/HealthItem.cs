using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 1; // Amount of health to restore when collected

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject); // Destroy the health item after collection
            }
        }
    }
}
