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

    // Biến cho patrol
    private Vector3 currentTarget;

    // Biến cho attack
    private bool isAttacking = false;
    private bool isPerformingAttack = false; // Biến mới để kiểm soát việc lao vào
    private float lastAttackTime = 0f;
    private Vector3 attackTarget;
    private bool hasDealtDamage = false; // Biến để đảm bảo chỉ gây sát thương một lần mỗi lần tấn công

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        // Thiết lập điểm patrol đầu tiên
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
            ogColor = spriteRenderer.color; // Lưu màu gốc của sprite
        }
    }

    private void Update()
    {
        if (player == null || isDead)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Kiểm tra nếu player vào tầm tấn công và enemy đang chase
        if (chase && distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown && !isAttacking && !isPerformingAttack)
        {
            StartAttack();
        }
        // Nếu không còn chase, hủy attack đang diễn ra
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

        // Cập nhật health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    private void UpdateAnimationStates()
    {
        if (anim != null)
        {
            // Chỉ cập nhật trạng thái chase, attack sẽ dùng trigger
            anim.SetBool("isChasing", chase && !isAttacking && !isPerformingAttack);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isPerformingAttack = true; // Đảm bảo reset trạng thái
        attackTarget = player.transform.position;
        hasDealtDamage = false; // Reset trạng thái đã gây sát thương

        // Kích hoạt animation tấn công bằng trigger
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
    }

    // Hàm này được gọi từ Animation Event tại cuối animation attack
    public void OnAttackAnimationEnd()
    {
        // Kết thúc attack nếu đang trong trạng thái tấn công
        if (isAttacking)
        {
            EndAttack();
        }
    }
    // Hàm này được gọi từ Animation Event
    public void RangedAttack()
    {
        isPerformingAttack = true;
        // Cập nhật lại target để đảm bảo lao về phía player hiện tại
        attackTarget = player.transform.position;
    }

    private void Attack()
    {
        // Di chuyển về phía target với tốc độ tấn công
        transform.position = Vector2.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);

        // Kiểm tra nếu đã đến gần target thì kết thúc tấn công
        if (Vector2.Distance(transform.position, attackTarget) < 0.2f)
        {
            EndAttack();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        isPerformingAttack = false;
        hasDealtDamage = false; // Đặt lại biến đã gây sát thương
        // Cập nhật thời gian tấn công cuối cùng để cooldown hoạt động
        lastAttackTime = Time.time;

        // Không cần reset animation attack vì dùng trigger
        // Animation sẽ tự động quay về idle/flying state
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

        // Vẽ tầm tấn công
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
