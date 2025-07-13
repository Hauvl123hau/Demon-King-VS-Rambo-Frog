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
            Destroy(gameObject); // Destroy the bullet on hit
        }
        FlyEnemy flyEnemy = collision.GetComponent<FlyEnemy>();
        if (flyEnemy)
        {
            flyEnemy.TakeDamage(bulletDamage);
            Destroy(gameObject); // Destroy the bullet on hit
        }
        FlyEnemyShooting flyEnemyShooting = collision.GetComponent<FlyEnemyShooting>();
        if (flyEnemyShooting)
        {
            flyEnemyShooting.TakeDamage(bulletDamage);
            Destroy(gameObject); // Destroy the bullet on hit
        }
    }
}
