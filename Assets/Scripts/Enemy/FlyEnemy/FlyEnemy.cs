using UnityEngine;

public class FlyEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Mảng các điểm patrol
    [SerializeField] private int currentPatrolIndex = 0; // Chỉ số điểm hiện tại
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f; // Tầm đánh
    [SerializeField] private float attackSpeed = 8f; // Tốc độ lao vào khi tấn công
    [SerializeField] private float attackCooldown = 3f; // Thời gian chờ giữa các lần tấn công
    [SerializeField] private int damage = 1;
    
    public bool chase = false;
    private GameObject player;
    private Animator anim;
    
    // Biến cho patrol
    private Vector3 currentTarget;
    
    // Biến cho attack
    private bool isAttacking = false;
    private bool isPerformingAttack = false; // Biến mới để kiểm soát việc lao vào
    private float lastAttackTime = 0f;
    private Vector3 attackTarget;

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
        
        // Kiểm tra tầm tấn công
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            StartAttack();
        }
        else if (isPerformingAttack)
        {
            Attack();
        }
        else if (chase == true && !isAttacking)
        {
            Chase();
        }
        else if (!isAttacking)
        {
            Patrol();
        }
        
        Flip();
    }
    
    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        attackTarget = player.transform.position; // Lưu vị trí của player khi bắt đầu tấn công
        
        // Trigger animation tấn công
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
    }
    
    // Method này sẽ được gọi từ Animation Event "RangedAttack"
    public void RangedAttack()
    {
        isPerformingAttack = true;
        // Cập nhật lại vị trí target để theo sát player hơn
        attackTarget = player.transform.position;
    }
    
    private void Attack()
    {
        // Lao về phía vị trí player với tốc độ cao
        transform.position = Vector2.MoveTowards(transform.position, attackTarget, attackSpeed * Time.deltaTime);
        
        // Kiểm tra nếu đã đến gần vị trí tấn công hoặc đã đi qua
        if (Vector2.Distance(transform.position, attackTarget) < 0.2f)
        {
            isAttacking = false;
            isPerformingAttack = false;
            // Có thể thêm hiệu ứng damage ở đây
        }
    }
    
    // Method để kết thúc attack từ animation (nếu cần)
    public void EndAttack()
    {
        isAttacking = false;
        isPerformingAttack = false;
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
        
        // Vẽ tầm tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
