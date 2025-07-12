using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public interface IDamageDealer
{
    int GetDamage();
    EnemyType GetEnemyType();
}

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth = 3;
    public PlayerHealthUI healthUI;
    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra interface IDamageDealer trước
        IDamageDealer damageDealer = collision.GetComponent<IDamageDealer>();
        if (damageDealer != null)
        {
            TakeDamage(damageDealer.GetDamage(), damageDealer.GetEnemyType());
            return;
        }

        // Kiểm tra các loại enemy cụ thể
        GroundEnemy groundEnemy = collision.GetComponent<GroundEnemy>();
        if (groundEnemy != null)
        {
            TakeDamage(1, EnemyType.Ground); // GroundEnemy damage = 1
            return;
        }

        FlyEnemy flyEnemy = collision.GetComponent<FlyEnemy>();
        if (flyEnemy != null)
        {
            TakeDamage(1, EnemyType.Flying); // FlyEnemy damage = 1
            return;
        }

        EnemyBulletScript bullet = collision.GetComponent<EnemyBulletScript>();
        if (bullet != null)
        {
            TakeDamage(1, EnemyType.Bullet); // Bullet damage = 1
            return;
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHeart(currentHealth);
        if (currentHealth <= 0)
        {
            //Die();
        }
    }

    private void TakeDamage(int damage, EnemyType enemyType)
    {
        // Có thể thêm logic xử lý khác nhau cho từng loại enemy
        switch (enemyType)
        {
            case EnemyType.Ground:
                Debug.Log("Bị tấn công bởi Ground Enemy!");
                break;
            case EnemyType.Flying:
                Debug.Log("Bị tấn công bởi Flying Enemy!");
                break;
            case EnemyType.Bullet:
                Debug.Log("Bị bắn bởi Enemy Bullet!");
                break;
            case EnemyType.Boss:
                Debug.Log("Bị tấn công bởi Boss!");
                break;
        }
        
        currentHealth -= damage;
        healthUI.UpdateHeart(currentHealth);
        if (currentHealth <= 0)
        {
            //Die();
        }
    }
}    
