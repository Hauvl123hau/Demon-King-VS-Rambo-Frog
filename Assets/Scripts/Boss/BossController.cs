using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player; // Reference đến player
    public Transform raycastOrigin; // Empty object để làm điểm xuất phát raycast
    public bool isAttacking = false;

    private enum BossState
    {
        Idle,
        Attacking,
        BreathingFire
    }
    private BossState currentState = BossState.Idle;

    [Header("State Timing")]
    public float idleDuration = 1f;
    public float stateTimer;
    
    [Header("Attack Pattern")]
    private bool useFireBreathNext = false;
    
    [Header("Distance-Based Attacks")]
    public float meleeAttackRange = 3f;        // Khoảng cách tấn công cận chiến
    public float fireBreathRange = 8f;         // Khoảng cách phun lửa
    public LayerMask obstacleLayerMask = ~0;   // Layer của vật cản (tất cả trừ player)
    public LayerMask playerLayer = 1;          // Layer của player
    
    private void Start()
    {
        stateTimer = idleDuration;
        if (anim == null)
        {
            Debug.LogError("Animator is not assigned in BossController.");
        }
        
        // Tự động tìm player nếu chưa được gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Nếu không có raycastOrigin, sử dụng transform của boss
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
            Debug.LogWarning("RaycastOrigin chưa được gán, sử dụng transform của Boss");
        }
    }

    private void Update()
    {
        if (isAttacking) return;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    ChooseAttackByDistance();
                    break;
            }
        }
    }

    void ChooseAttackByDistance()
    {
        if (player == null) return;
        
        float distanceToPlayer = GetDistanceToPlayer();
        
        // Kiểm tra xem có thể "nhìn thấy" player không
        if (CanSeePlayer())
        {
            if (distanceToPlayer <= meleeAttackRange)
            {
                // Tấn công cận chiến
                TriggerMeleeAttack();
                currentState = BossState.Attacking;
            }
            else if (distanceToPlayer <= fireBreathRange)
            {
                // Phun lửa tầm trung
                TriggerBreath();
                currentState = BossState.BreathingFire;
            }
            else
            {
                // Player quá xa, quay về idle
                stateTimer = idleDuration;
            }
        }
        else
        {
            // Không thể nhìn thấy player, quay về idle
            stateTimer = idleDuration;
        }
    }
    
    float GetDistanceToPlayer()
    {
        // Tính khoảng cách từ raycastOrigin đến player
        return Vector3.Distance(raycastOrigin.position, player.position);
    }
    
    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = GetDistanceToPlayer();
        
        // Raycast từ raycastOrigin đến player
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out hit, distanceToPlayer, obstacleLayerMask))
        {
            // Kiểm tra xem raycast có trúng player không
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Nhìn thấy player
            }
            else
            {
                return false; // Có vật cản
            }
        }
        
        return true; // Không có vật cản nào
    }
    
    // Vẽ debug gizmos để visualize các khoảng cách
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        
        // Vẽ vòng tròn tấn công cận chiến
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(raycastOrigin.position, meleeAttackRange);
        
        // Vẽ vòng tròn phun lửa
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastOrigin.position, fireBreathRange);
        
        // Vẽ raycast đến player
        if (player != null)
        {
            Gizmos.color = CanSeePlayer() ? Color.green : Color.yellow;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
            
            // Vẽ điểm xuất phát raycast
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(raycastOrigin.position, 0.2f);
        }
    }

    public void TriggerBreath()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("fireBreath");
    }

    public void TriggerMeleeAttack()
    {
        Debug.Log("TriggerMeleeAttack() được gọi!");
        isAttacking = true;
        anim.SetBool("isAttacking", true);
        anim.SetTrigger("attack");
        Debug.Log("Đã set trigger 'attack'");
    }
    
    // Giữ lại method cũ để tương thích
    public void TriggerAttack()
    {
        TriggerMeleeAttack();
    }

    public void EndAttack()
    {
        Debug.Log("EndAttack() trong BossController được gọi!");
        isAttacking = false;
        anim.SetBool("isAttacking", false);
        currentState = BossState.Idle;
        stateTimer = idleDuration;
        Debug.Log("Boss quay về Idle, timer = " + stateTimer);
    }
}
