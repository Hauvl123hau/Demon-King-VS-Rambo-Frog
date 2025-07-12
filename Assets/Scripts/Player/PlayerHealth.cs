using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public PlayerHealthUI healthUI;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Chỉ nhận damage từ attack hitbox, không phải từ enemy body
        if (collision.CompareTag("EnemyAttack"))
        {
            GroundEnemy enemy = collision.GetComponentInParent<GroundEnemy>();
            if (enemy)
            {
                TakeDamage(enemy.damage);
            }
        }
    }

     public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHeart(currentHealth);
        if (currentHealth <= 0)
        {
            //Die();
        }
    }

    private void Die()
    {
        // Handle player death (e.g., respawn, game over)
    }
}