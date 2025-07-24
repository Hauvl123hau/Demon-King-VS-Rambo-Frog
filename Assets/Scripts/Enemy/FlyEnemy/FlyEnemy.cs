using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlyEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private int currentPatrolIndex = 0;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackSpeed = 8f;
    [SerializeField] private float attackCooldown = 3f;
    public int damage = 1;

    [Header("Enemy States")]
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider healthBar;

    public bool chase = false;
    private GameObject player;
    private Animator anim;
    private Rigidbody2D rb;

    private Vector3 currentTarget;

    private bool isAttacking = false;
    private bool isPerformingAttack = false; 
    private float lastAttackTime = 0f;
    private Vector3 attackTarget;
    private bool hasDealtDamage = false; 

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            ogColor = spriteRenderer.color; 
        }
    }

    private void Update()
    {
        if (player == null || isDead)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (chase && distanceToPlayer <= attackRange && Time.deltaTime - lastAttackTime >= attackCooldown && !isAttacking && !isPerformingAttack)
        {
            StartAttack();
        }
        else if (!chase && (isAttacking || isPerformingAttack))
        {
            EndAttack();
        }
        else if (isPerformingAttack)
        {
            Attack();
        }
        else if (chase == true && !isAttacking && !isPerformingAttack)
        {
            Chase();
        }
        else if (!isAttacking && !isPerformingAttack)
        {
            Patrol();
        }

        Flip();
        UpdateAnimationStates();

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    private void UpdateAnimationStates()
    {
        if (anim != null)
        {
            anim.SetBool("isChasing", chase && !isAttacking && !isPerformingAttack);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isPerformingAttack = true; 
        attackTarget = player.transform.position;
        hasDealtDamage = false; 

        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
    }

    public void OnAttackAnimationEnd()
    {
        if (isAttacking)
        {
            EndAttack();
        }
    }
    public void RangedAttack()
    {
        isPerformingAttack = true;
        attackTarget = player.transform.position;
    }

    private void Attack()
    {
        transform.position = Vector2.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, attackTarget) < 0.2f)
        {
            EndAttack();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        isPerformingAttack = false;
        hasDealtDamage = false; 
        lastAttackTime = Time.deltaTime;
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

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        Vector3 targetPosition;

        if (isAttacking)
        {
            targetPosition = attackTarget;
        }
        else if (chase)
        {
            targetPosition = player.transform.position;
        }
        else
        {
            targetPosition = currentTarget;
        }

        if (transform.position.x < targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
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

        isAttacking = false;
        isPerformingAttack = false;
        chase = false;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isPerformingAttack && !hasDealtDamage && !isDead)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                hasDealtDamage = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hasDealtDamage = false;
        }
    }
}
