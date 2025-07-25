using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 3;
    public int currentHealth;
    public PlayerHealthUI healthUI;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            FlyEnemy flyEnemy = collision.GetComponentInParent<FlyEnemy>();
            if (flyEnemy)
            {
                TakeDamage(flyEnemy.damage);
            }   
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHeart(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHeart(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Tìm thành phần DeadUI và hiển thị panel tử vong
        DeadUI deadUI = FindObjectOfType<DeadUI>();
        if (deadUI != null)
        {
            deadUI.ShowDeadPanel();
        }
        else
        {
            Debug.LogError("Không tìm thấy thành phần DeadUI trong cảnh!");
        }
        
        // Vô hiệu hóa điều khiển người chơi hoặc hoạt ảnh nếu cần
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}