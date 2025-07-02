using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Mảng các điểm patrol
    [SerializeField] private int currentPatrolIndex = 0; // Chỉ số điểm hiện tại
    
    [Header("Shooting Settings")]
    [SerializeField] private float attackRange = 3f; // Tầm bắn
    [SerializeField] private float attackCooldown = 2f; // Thời gian chờ giữa các lần bắn
    [SerializeField] private GameObject fireball; // Prefab cầu lửa
    [SerializeField] private Transform firePoint; // Điểm bắn
    
    private GameObject player;
    private Animator anim;
    
    // Biến cho patrol
    private Vector3 currentTarget;
    
    // Biến cho shooting
    private float lastAttackTime = 0f;

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

    private void Update()
    {
        if (player == null)
        {
            return;
        }
        
        // Kiểm tra tầm bắn
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            Shoot();
        }
        
        // Luôn patrol
        Patrol();
        Flip();
    }
    
    private void Shoot()
    {
        lastAttackTime = Time.time;
        
        // Trigger animation bắn
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
        
        // Tạo cầu lửa
        if (fireball != null && firePoint != null)
        {
            Instantiate(fireball, firePoint.position, Quaternion.identity);
        }
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
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
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
        
        // Vẽ tầm bắn
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
