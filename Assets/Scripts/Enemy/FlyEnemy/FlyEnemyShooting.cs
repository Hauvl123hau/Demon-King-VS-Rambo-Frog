using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlyEnemyShooting : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; 
    [SerializeField] private int currentPatrolIndex = 0; 

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

    private Vector3 currentTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();

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
            ogColor = spriteRenderer.color; 
        }
    }

    private void Update()
    {
        if (isDead) return;

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

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void Flip()
    {
        Vector3 targetPosition = currentTarget;

        if (transform.position.x < targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null) continue;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);

            if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }

        if (patrolPoints.Length > 2 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f); 
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
