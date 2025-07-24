using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GroundEnemy : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float attackDistance; //Minimum distance for attack
    public float moveSpeed;
    public float timer; //Timer for cooldown between attacks
    public Transform leftLimit;
    public Transform rightLimit;
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;
    public GameObject hotZone;
    public GameObject triggerZone;

    [Header("Attack Settings")]

    private float distance; 
    private bool attackMode;
    private bool cooling; 
    private float intTimer;
    public int damage = 1;

    [Header("Enemy States")]
    public int maxHealth = 3;
    public int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color ogColor;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider healthBar;

    void Awake()
    {
        SelectTarget();
        intTimer = timer; 
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); 
        if (spriteRenderer != null)
        {
            ogColor = spriteRenderer.color; 
        }
    }

    void Update()
    {
        if (isDead) return; 

        if (!attackMode)
        {
            Move();
        }

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            SelectTarget();
        }

        if (inRange)
        {
            EnemyLogic();
        }
        healthBar.value = currentHealth;

    }

    void EnemyLogic()
    {
        if (isDead) return; 

        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    void Move()
    {
        if (isDead) return; 

        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (isDead) return; 

        timer = intTimer; 
        attackMode = true; 

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    public void DealDamageToPlayer()
    {
        if (isDead) return;

        if (target != null && target.CompareTag("Player"))
        {
            float distanceToPlayer = Vector2.Distance(transform.position, target.position);
            if (distanceToPlayer <= attackDistance)
            {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }
        Flip();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        StartCoroutine(Flash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        attackMode = false;
        cooling = false;
        inRange = false;

        if (anim != null)
        {
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", false);
            anim.SetTrigger("die");
        }

        if (hotZone != null)
            hotZone.SetActive(false);
        if (triggerZone != null)
            triggerZone.SetActive(false);
    }
    public void DieAnimation()
    {
        Destroy(gameObject, 0.1f); 
    }
    private IEnumerator Flash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = ogColor;
        }
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180;
        }
        else
        {
            Debug.Log("Twist");
            rotation.y = 0;
        }

        transform.eulerAngles = rotation;
        
        
    }

}
