using UnityEngine;
using System;

public class ChaseAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    public Transform raycastOrigin;
    private BossController bossController; // Reference để sử dụng utility methods
    
    [Header("Chase Settings")]
    public float chaseSpeed = 3f;
    public float meleeAttackRange = 4f;
    public float stopDistance = 0.5f; // Khoảng cách dừng lại để tránh va chạm
    
    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;
    
    [Header("Vision Settings")]
    public LayerMask obstacleLayerMask = ~0;
    public LayerMask playerLayer = 1;
    
    // Events
    public event Action OnChaseAttackComplete;
    
    // State
    private bool isPerformingChaseAttack = false;
    private bool isAttacking = false;
    private Vector3 originalScale;
    private float chaseStartTime; // Thời điểm bắt đầu đuổi theo
    
    public bool IsPerformingChaseAttack => isPerformingChaseAttack;
    
    // Method để BossController kiểm tra thời gian đã trôi qua
    public float GetElapsedTime()
    {
        if (!isPerformingChaseAttack) return 0f;
        return Time.time - chaseStartTime;
    }
    
    private void Start()
    {
        // Lưu scale gốc
        originalScale = transform.localScale;
        
        // Tìm BossController reference
        bossController = GetComponent<BossController>();
        if (bossController == null)
        {
            Debug.LogError("ChaseAttack: Không tìm thấy BossController component!");
        }
        
        // Auto-find components if not assigned
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
        }
    }
    
    public void StartChaseAttack(Transform targetPlayer)
    {
        if (isPerformingChaseAttack) return;
        
        player = targetPlayer;
        isPerformingChaseAttack = true;
        chaseStartTime = Time.time; // Ghi lại thời điểm bắt đầu
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        Debug.Log($"Boss bắt đầu đuổi theo và tấn công cận chiến! Distance to player: {distance:F2}");
    }
    
    public void StopChaseAttack()
    {
        isPerformingChaseAttack = false;
        isAttacking = false;
        
        // Reset animation parameters khi dừng chase
        if (anim != null)
        {
            anim.SetBool("isChasing", false);
            anim.SetBool("isAttacking", false);
        }
        
        Debug.Log("Boss dừng đuổi theo!");
        OnChaseAttackComplete?.Invoke();
    }
    
    private void Update()
    {
        if (!isPerformingChaseAttack || player == null) return;
        
        // Sử dụng BossController để tính khoảng cách (nếu có), nếu không thì tính trực tiếp
        float distanceToPlayer = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        float elapsedTime = Time.time - chaseStartTime;
        
        // Debug thông tin mỗi frame
        if (Time.frameCount % 60 == 0) // Log mỗi giây
        {
            Debug.Log($"Chase Update - Distance: {distanceToPlayer:F2}, Elapsed: {elapsedTime:F2}s, IsAttacking: {isAttacking}");
        }
        
        // Kiểm tra khoảng cách để tấn công
        if (distanceToPlayer <= meleeAttackRange && !isAttacking)
        {
            // Trong tầm tấn công - thực hiện tấn công
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PerformMeleeAttack();
            }
        }
        else if (!isAttacking)
        {
            // Ngoài tầm tấn công - di chuyển đến gần player (nhưng không quá gần)
            if (distanceToPlayer > stopDistance)
            {
                ChasePlayer();
            }
        }
        
        // Cập nhật animation parameters
        UpdateAnimations();
    }
    
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // Di chuyển về phía player - BossController sẽ lo việc quay mặt
        transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        Debug.Log($"Boss đang chase player. Distance: {distance:F2}");
    }
    
    private void PerformMeleeAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        Debug.Log("Boss thực hiện tấn công cận chiến!");
        
        // Trigger animation
        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("attack");
        }
    }
    
    
    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = Vector3.Distance(raycastOrigin.position, player.position);
        
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        return true;
    }
    
    private void UpdateAnimations()
    {
        if (anim == null) return;
        
        bool isMoving = !isAttacking && isPerformingChaseAttack;
        anim.SetBool("isChasing", isMoving);
    }
    
    // Animation Event: Được gọi khi melee attack animation kết thúc
    public void EndMeleeAttack()
    {
        Debug.Log("Chase Attack: Melee attack kết thúc!");
        isAttacking = false;
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
        
        // Kiểm tra xem có nên tiếp tục đuổi theo không
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        if (isPerformingChaseAttack && distance > meleeAttackRange)
        {
            // Tiếp tục đuổi theo nếu player vẫn trong tầm
            Debug.Log("Tiếp tục đuổi theo player...");
        }
    }
    
    // Debug Gizmos
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        
        // Vẽ vòng tròn tấn công cận chiến
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastOrigin.position, meleeAttackRange);
        
        // Vẽ raycast đến player
        if (player != null && isPerformingChaseAttack)
        {
            bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
        }
    }
}
