using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyEnemyShooting : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Mảng các điểm patrol
    [SerializeField] private int currentPatrolIndex = 0; // Chỉ số điểm hiện tại

    [Header("Shooting Settings")]
    [SerializeField] private float shootingRange = 3f;
    [SerializeField] private float shootingCooldown;
    [SerializeField] private GameObject fireball;
    [SerializeField] private Transform firePoint;

    [Header("Enemy States")]
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider healthBar;

    private GameObject player;
    private Animator anim;

    // Biến cho patrol
    private Vector3 currentTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        // Thiết lập điểm patrol đầu tiên
        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            ogColor = spriteRenderer.color; // Lưu màu gốc của sprite
        }
    }

    private void Update()
    {
        if (isDead) return; // Không làm gì nếu enemy đã chết

        shootingRange = Vector2.Distance(transform.position, player.transform.position);
        if (shootingRange < 10)
        {
            shootingCooldown += Time.deltaTime;

            if (shootingCooldown > 2)
            {
                shootingCooldown = 0;
                Shoot();
            }
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        // Luôn patrol
        Patrol();
        Flip();
    }

    private void Shoot()
    {
        Instantiate(fireball, firePoint.position, Quaternion.identity);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Di chuyển đến điểm mục tiêu
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        // Kiểm tra nếu đã đến điểm mục tiêu
        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            // Chuyển sang điểm tiếp theo
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void Flip()
    {
        Vector3 targetPosition = currentTarget;

        // Xoay mặt theo hướng di chuyển
        if (transform.position.x < targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Hiển thị đường đi trong Scene view
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            // Vẽ điểm patrol
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);

            // Vẽ đường nối giữa các điểm
            if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }

        // Vẽ đường từ điểm cuối về điểm đầu
        if (patrolPoints.Length > 2 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
        }

        // Vẽ tầm bắn của enemy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f); // Tầm bắn 10 units
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("die");
        }
    }

    public void DieAnimation()
    {
        Destroy(gameObject, 0.1f);
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = ogColor;
        }
    }
}
