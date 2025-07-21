using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GroundEnemy enemy = collision.GetComponent<GroundEnemy>();
        if (enemy)
        {
            enemy.TakeDamage(bulletDamage);
            Destroy(gameObject); 
        }
        FlyEnemy flyEnemy = collision.GetComponent<FlyEnemy>();
        if (flyEnemy)
        {
            flyEnemy.TakeDamage(bulletDamage);
            Destroy(gameObject); 
        }
        FlyEnemyShooting flyEnemyShooting = collision.GetComponent<FlyEnemyShooting>();
        if (flyEnemyShooting)
        {
            flyEnemyShooting.TakeDamage(bulletDamage);
            Destroy(gameObject); 
        }
        BossHealth bossHealth = collision.GetComponent<BossHealth>();
        if (bossHealth)
        {
            bossHealth.TakeDamage(bulletDamage);
            Destroy(gameObject); 
        }
    }
}
