using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player; // Reference đến player
    public Transform raycastOrigin; // Empty object để làm điểm xuất phát raycast
    public DashAttack dashAttack; // Reference đến DashAttack component
    public ChaseAttack chaseAttack; // Reference đến ChaseAttack component
    public bool isAttacking = false;
    
    [Header("Attack Settings")]
    public bool dashAttackOn = false; 
    public bool chaseAttackOn = false; 

    private enum BossState
    {
        Idle,
        DashFireCombo,
        ChaseAttack
    }
    private BossState currentState = BossState.Idle;

    [Header("State Timing")]
    public float idleDuration = 1f;
    public float stateTimer;
    
    [Header("Attack Pattern")]
    private bool useFireBreathNext = false;
    
    [Header("Distance-Based Attacks")]
    public float meleeAttackRange = 4f;        // Khoảng cách tấn công cận chiến (chase attack)
    public float dashFireComboRange = 10f;     // Khoảng cách để thực hiện combo dash + fire
    public LayerMask obstacleLayerMask = ~0;   // Layer của vật cản (tất cả trừ player)
    public LayerMask playerLayer = 1;          // Layer của player
    
    [Header("Dash Settings")]
    public float dashCooldown = 3f;            // Thời gian cooldown giữa các combo
    private Vector3 originalScale;             // Scale gốc của boss
    
    private void Start()
    {
        stateTimer = idleDuration;
        
        // Lưu scale gốc của boss
        originalScale = transform.localScale;
        
        if (anim == null)
        {
            // Tìm Animator trong component hiện tại hoặc trong children
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            
            if (anim == null)
            {
                Debug.LogError("Animator is not found in BossController or its children.");
            }
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
        
        // Tự động tìm hoặc tạo DashAttack component (chỉ khi được bật)
        if (dashAttackOn)
        {
            if (dashAttack == null)
            {
                dashAttack = GetComponent<DashAttack>();
                if (dashAttack == null)
                {
                    dashAttack = gameObject.AddComponent<DashAttack>();
                }
            }
        }
        
        // Tự động tìm hoặc tạo ChaseAttack component (chỉ khi được bật)
        if (chaseAttackOn)
        {
            if (chaseAttack == null)
            {
                chaseAttack = GetComponent<ChaseAttack>();
                if (chaseAttack == null)
                {
                    chaseAttack = gameObject.AddComponent<ChaseAttack>();
                }
            }
        }
        
        // Thiết lập DashAttack (chỉ khi được bật)
        if (dashAttackOn && dashAttack != null)
        {
            dashAttack.anim = anim;
            dashAttack.player = player;
            dashAttack.fireBreathRange = 4f; // Đặt cố định ở đây
            dashAttack.raycastOrigin = raycastOrigin; // Sử dụng cùng raycastOrigin
            dashAttack.obstacleLayerMask = obstacleLayerMask; // Sử dụng cùng obstacle layer mask
            dashAttack.playerLayer = playerLayer; // Sử dụng cùng player layer
            
            // Subscribe to event
            dashAttack.OnComboComplete += OnDashFireComboComplete;
        }
        
        // Thiết lập ChaseAttack (chỉ khi được bật)
        if (chaseAttackOn && chaseAttack != null)
        {
            chaseAttack.anim = anim;
            chaseAttack.player = player;
            chaseAttack.meleeAttackRange = meleeAttackRange;
            chaseAttack.raycastOrigin = raycastOrigin;
            chaseAttack.obstacleLayerMask = obstacleLayerMask;
            chaseAttack.playerLayer = playerLayer;
            
            // Subscribe to event
            chaseAttack.OnChaseAttackComplete += OnChaseAttackComplete;
        }
    }

    private void Update()
    {
        // Boss luôn quay mặt về phía player (trừ khi đang thực hiện combo hoặc chase)
        if (player != null && currentState != BossState.DashFireCombo && currentState != BossState.ChaseAttack)
        {
            FacePlayer();
        }

        if (isAttacking || (dashAttackOn && dashAttack?.IsPerformingCombo == true) || (chaseAttackOn && chaseAttack?.IsPerformingChaseAttack == true)) return;

        stateTimer -= Time.deltaTime;
        
        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0f)
                {
                    ChooseAttackByDistance();
                }
                break;
                
            case BossState.DashFireCombo:
                // DashAttack component sẽ tự xử lý, chỉ cần đợi nó hoàn thành
                break;
                
            case BossState.ChaseAttack:
                // ChaseAttack component sẽ tự xử lý, chỉ cần đợi nó hoàn thành
                break;
        }
    }
    
    void FacePlayer()
    {
        if (player == null) return;
        
        // Tính hướng từ boss đến player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // Quay mặt về phía player bằng cách thay đổi scale
        if (directionToPlayer.x > 0)
        {
            // Player ở bên phải, boss quay phải
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (directionToPlayer.x < 0)
        {
            // Player ở bên trái, boss quay trái
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    void ChooseAttackByDistance()
    {
        if (player == null) return;
        
        float distanceToPlayer = GetDistanceToPlayer();
        
        // Kiểm tra xem có thể "nhìn thấy" player không
        if (CanSeePlayer())
        {
            // Ưu tiên theo khoảng cách và kiểu tấn công được bật
            if (distanceToPlayer <= meleeAttackRange && chaseAttackOn)
            {
                // Tấn công cận chiến bằng chase attack (nếu được bật)
                TriggerChaseAttack();
                currentState = BossState.ChaseAttack;
            }
            else if (distanceToPlayer <= dashFireComboRange && dashAttackOn && dashAttack.CanPerformDashAttack())
            {
                // Thực hiện combo dash + fire breath (nếu được bật)
                TriggerDashFireCombo();
                currentState = BossState.DashFireCombo;
            }
            else if (distanceToPlayer <= meleeAttackRange && !chaseAttackOn)
            {
                // Fallback: sử dụng melee attack cũ nếu chase attack bị tắt
                TriggerMeleeAttack();
                currentState = BossState.Idle; // Sẽ quay về idle sau khi attack xong
            }
            else
            {
                // Player quá xa hoặc không có attack nào được bật, quay về idle
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
        
        // Vẽ vòng tròn tấn công cận chiến (chase attack) - chỉ khi được bật
        if (chaseAttackOn)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(raycastOrigin.position, meleeAttackRange);
        }
        
        // Vẽ vòng tròn dash fire combo - chỉ khi được bật
        if (dashAttackOn)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(raycastOrigin.position, dashFireComboRange);
        }
        
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

    // === CHASE ATTACK METHODS ===
    public void TriggerChaseAttack()
    {
        if (!chaseAttackOn || chaseAttack == null) return;
        
        Debug.Log("Boss bắt đầu đuổi theo và tấn công cận chiến!");
        currentState = BossState.ChaseAttack;
        
        // Gọi ChaseAttack component để thực hiện chase attack
        chaseAttack.StartChaseAttack(player);
    }
    
    private void OnChaseAttackComplete()
    {
        Debug.Log("Chase Attack hoàn thành!");
        currentState = BossState.Idle;
        stateTimer = idleDuration;
    }

    // === LEGACY MELEE ATTACK METHODS (giữ lại để tương thích) ===

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
        anim.SetBool("isPreparingDash", false);
        anim.SetBool("isDashing", false);
        currentState = BossState.Idle;
        stateTimer = idleDuration;
        Debug.Log("Boss quay về Idle, timer = " + stateTimer);
    }
    
    // === DASH FIRE COMBO METHODS ===
    public void TriggerDashFireCombo()
    {
        if (!dashAttackOn || dashAttack == null) return;
        
        Debug.Log("Boss thực hiện combo Dash + Fire Breath!");
        currentState = BossState.DashFireCombo;
        
        // Gọi DashAttack component để thực hiện combo
        dashAttack.StartDashFireCombo(player);
    }
    
    private void OnDashFireComboComplete()
    {
        Debug.Log("Dash Fire Combo hoàn thành!");
        currentState = BossState.Idle;
        stateTimer = idleDuration;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe events để tránh memory leak
        if (dashAttackOn && dashAttack != null)
        {
            dashAttack.OnComboComplete -= OnDashFireComboComplete;
        }
        
        if (chaseAttackOn && chaseAttack != null)
        {
            chaseAttack.OnChaseAttackComplete -= OnChaseAttackComplete;
        }
    }
}
