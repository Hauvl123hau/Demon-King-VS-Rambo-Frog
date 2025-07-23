using UnityEngine;
using System;

public class ChaseAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    public Transform raycastOrigin;
    
    [Header("Chase Settings")]
    public float chaseSpeed = 3f;
    public float meleeAttackRange = 4f;
    public float maxChaseDistance = 15f; // Khoảng cách tối đa để đuổi theo
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
    
    public bool IsPerformingChaseAttack => isPerformingChaseAttack;
    
    private void Start()
    {
        // Lưu scale gốc
        originalScale = transform.localScale;
        
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
        Debug.Log("Boss bắt đầu đuổi theo và tấn công cận chiến!");
    }
    
    public void StopChaseAttack()
    {
        isPerformingChaseAttack = false;
        isAttacking = false;
        Debug.Log("Boss dừng đuổi theo!");
        OnChaseAttackComplete?.Invoke();
    }
    
    private void Update()
    {
        if (!isPerformingChaseAttack || player == null) return;
        
        float distanceToPlayer = GetDistanceToPlayer();
        
        // Kiểm tra xem player có quá xa không
        if (distanceToPlayer > maxChaseDistance || !CanSeePlayer())
        {
            StopChaseAttack();
            return;
        }
        
        // Quay mặt về phía player
        FacePlayer();
        
        // Kiểm tra khoảng cách để tấn công
        if (distanceToPlayer <= meleeAttackRange && !isAttacking)
        {
            // Trong tầm tấn công - thực hiện tấn công
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PerformMeleeAttack();
            }
        }
        else if (distanceToPlayer > meleeAttackRange + stopDistance && !isAttacking)
        {
            // Ngoài tầm tấn công - di chuyển đến gần player
            ChasePlayer();
        }
        
        // Cập nhật animation parameters
        UpdateAnimations();
    }
    
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // Di chuyển về phía player
        transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;
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
    
    private void FacePlayer()
    {
        if (player == null) return;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        if (directionToPlayer.x > 0)
        {
            // Player ở bên phải
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (directionToPlayer.x < 0)
        {
            // Player ở bên trái
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }
    
    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(raycastOrigin.position, player.position);
    }
    
    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = GetDistanceToPlayer();
        
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
        if (isPerformingChaseAttack && GetDistanceToPlayer() > meleeAttackRange)
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
        
        // Vẽ vòng tròn đuổi theo tối đa
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f); // Orange color
        Gizmos.DrawWireSphere(raycastOrigin.position, maxChaseDistance);
        
        // Vẽ raycast đến player
        if (player != null && isPerformingChaseAttack)
        {
            Gizmos.color = CanSeePlayer() ? Color.green : Color.red;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
        }
    }
}
