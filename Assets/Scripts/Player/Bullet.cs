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
    }
}
