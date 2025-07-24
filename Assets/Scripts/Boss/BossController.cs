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
    public ShootFireballsAttack shootFireballs; // Reference đến ShootFireballsAttack component
    
    [Header("Attack Settings")]
    public bool dashAttackOn = false; 
    public bool chaseAttackOn = false; 
    public bool shootFireballsOn = false; 

    private enum BossState
    {
        Idle,
        DashFireCombo,
        ChaseAttack,
        ShootFireballs
    }
    private BossState currentState = BossState.Idle;

    [Header("State Timing")]
    public float idleDuration = 1f;
    public float stateTimer;
    
    [Header("Distance-Based Attacks")]
    public float dashFireComboRange = 10f;     // Khoảng cách để thực hiện combo dash + fire
    public LayerMask obstacleLayerMask = ~0;   // Layer của vật cản (tất cả trừ player)
    public LayerMask playerLayer = 1;          // Layer của player
    
    [Header("Chase Attack Settings")]
    public float chaseAttackDuration = 5f;     // Thời gian chase attack diễn ra
    
    private Vector3 originalScale;             // Scale gốc của boss

    [Header("Fireball Attack Settings")]
    public int maxFireballShots = 3;
    private int fireballShotsCount = 0;
    public float fireballAttackDuration = 3f; // Thời gian tối đa cho đợt bắn fireball (tuỳ chọn)
    private float fireballAttackTimer = 0f;
    
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
            chaseAttack.raycastOrigin = raycastOrigin;
            // Subscribe to event
            chaseAttack.OnChaseAttackComplete += OnChaseAttackComplete;
        }

        // Thiết lập ShootFireballsAttack (chỉ khi được bật)
        if (shootFireballsOn && shootFireballs != null)
        {
            shootFireballs.anim = anim;
            shootFireballs.player = player;
            // Nếu chưa có firePoint, dùng raycastOrigin hoặc transform
            if (shootFireballs.firePoint == null)
            {
                shootFireballs.firePoint = raycastOrigin != null ? raycastOrigin : transform;
            }
        }
    }

    private void Update()
    {
        // Boss luôn quay mặt về phía player (bao gồm cả khi đang chase attack)
        if (player != null && currentState != BossState.DashFireCombo)
        {
            FacePlayer();
        }

        // Kiểm tra các điều kiện chặn update (an toàn hơn)
        bool isDashAttacking = dashAttackOn && dashAttack != null && dashAttack.IsPerformingCombo;
        bool isChaseAttacking = chaseAttackOn && chaseAttack != null && chaseAttack.IsPerformingChaseAttack;
        
        if (isDashAttacking || isChaseAttacking) 
        {
            // Debug log để theo dõi trạng thái
            if (Time.frameCount % 120 == 0) // Log mỗi 2 giây
            {
                Debug.Log($"Update blocked - isDashAttacking: {isDashAttacking}, isChaseAttacking: {isChaseAttacking}");
            }
            return;
        }

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
                // Kiểm tra thời gian chase attack và dừng nếu hết thời gian
                if (chaseAttack != null && chaseAttack.IsPerformingChaseAttack)
                {
                    float elapsedTime = chaseAttack.GetElapsedTime();
                    if (elapsedTime >= chaseAttackDuration)
                    {
                        Debug.Log($"Chase Attack timeout từ BossController! Elapsed: {elapsedTime:F2}s >= {chaseAttackDuration}s");
                        chaseAttack.StopChaseAttack();
                    }
                }
                break;

            case BossState.ShootFireballs:
                // Kiểm soát số lần bắn fireball
                fireballAttackTimer += Time.deltaTime;
                if (fireballShotsCount < maxFireballShots)
                {
                    if (shootFireballs != null)
                    {
                        shootFireballs.ShootFireball();
                        fireballShotsCount++;
                        Debug.Log($"Boss bắn fireball số {fireballShotsCount}/{maxFireballShots}");
                    }
                }
                else
                {
                    Debug.Log("Boss đã bắn đủ số fireball, chuyển về Idle");
                    currentState = BossState.Idle;
                    stateTimer = idleDuration;
                }
                break;
        }
    }
    
    // === UTILITY METHODS (dùng chung cho tất cả attack types) ===
    
    public float GetDistanceToPlayer()
    {
        // Tính khoảng cách từ raycastOrigin đến player
        return Vector3.Distance(raycastOrigin.position, player.position);
    }
    
    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = GetDistanceToPlayer();
        
        // Raycast từ raycastOrigin đến player, chỉ kiểm tra obstacles (loại trừ player layer)
        int obstacleOnlyMask = obstacleLayerMask & ~playerLayer;
        RaycastHit hit;
        
        if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out hit, distanceToPlayer, obstacleOnlyMask))
        {
            // Có vật cản giữa boss và player
            Debug.Log($"CanSeePlayer: Có vật cản {hit.collider.name} giữa boss và player");
            return false;
        }
        
        // Không có vật cản, boss có thể nhìn thấy player
        return true;
    }
    
    public void FacePlayer()
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
    
    // === ATTACK DECISION LOGIC ===
    
    void ChooseAttackByDistance()
    {
        if (player == null) return;
        
        float distanceToPlayer = GetDistanceToPlayer();
        bool canSeePlayer = CanSeePlayer();
        
        Debug.Log($"ChooseAttackByDistance: Distance={distanceToPlayer:F2}, CanSee={canSeePlayer}, ChaseAttackOn={chaseAttackOn}, DashAttackOn={dashAttackOn}");
        
        // Kiểm tra xem có thể "nhìn thấy" player không
        if (canSeePlayer)
        {
            // Ưu tiên theo khoảng cách và kiểu tấn công được bật
            if (shootFireballsOn && shootFireballs != null)
            {
                Debug.Log("Triggering ShootFireballs!");
                TriggerShootFireballs();
                currentState = BossState.ShootFireballs;
            }
            else if (distanceToPlayer <= dashFireComboRange && dashAttackOn && dashAttack != null && dashAttack.CanPerformDashAttack())
            {
                Debug.Log($"Triggering DashFireCombo - Distance: {distanceToPlayer:F2} <= {dashFireComboRange}");
                TriggerDashFireCombo();
                currentState = BossState.DashFireCombo;
            }
            else if (chaseAttackOn && chaseAttack != null)
            {
                Debug.Log($"Triggering ChaseAttack - Distance: {distanceToPlayer:F2}");
                TriggerChaseAttack();
                currentState = BossState.ChaseAttack;
            }
            else
            {
                Debug.Log("No attack triggered - returning to idle");
                stateTimer = idleDuration;
            }
        }
        else
        {
            // Không thể nhìn thấy player
            if (chaseAttackOn && chaseAttack != null)
            {
                // Vẫn thử chase attack để tìm player
                Debug.Log("Cannot see player - triggering ChaseAttack to find player");
                TriggerChaseAttack();
                currentState = BossState.ChaseAttack;
            }
            else
            {
                // Không có chase attack, quay về idle
                Debug.Log("Cannot see player and no chase attack - returning to idle");
                stateTimer = idleDuration;
            }
        }
    }
    
    // Vẽ debug gizmos để visualize các khoảng cách
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        
        // Vẽ vòng tròn tấn công cận chiến (chase attack) - chỉ khi được bật
        if (chaseAttackOn && chaseAttack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(raycastOrigin.position, chaseAttack.meleeAttackRange);
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
    // === SHOOT FIREBALLS ATTACK METHODS ===
    public void TriggerShootFireballs()
    {
        if (!shootFireballsOn || shootFireballs == null)
        {
            Debug.LogWarning($"TriggerShootFireballs failed - shootFireballsOn: {shootFireballsOn}, shootFireballs: {shootFireballs}");
            return;
        }
        Debug.Log($"Boss bắt đầu bắn fireball tối đa {maxFireballShots} lần!");
        fireballShotsCount = 0;
        fireballAttackTimer = 0f;
        // Nếu muốn delay giữa các lần bắn, có thể dùng Coroutine hoặc thêm cooldown logic
    }
    public void TriggerChaseAttack()
    {
        if (!chaseAttackOn || chaseAttack == null) 
        {
            Debug.LogWarning($"TriggerChaseAttack failed - chaseAttackOn: {chaseAttackOn}, chaseAttack: {chaseAttack}");
            return;
        }
        
        Debug.Log($"Boss bắt đầu đuổi theo và tấn công cận chiến trong {chaseAttackDuration} giây!");
        currentState = BossState.ChaseAttack;
        
        // Gọi ChaseAttack component để thực hiện chase attack
        chaseAttack.StartChaseAttack(player);
    }
    
    // Method để thay đổi chase attack duration từ bên ngoài
    public void SetChaseAttackDuration(float newDuration)
    {
        chaseAttackDuration = Mathf.Max(0.1f, newDuration); // Đảm bảo duration >= 0.1s
    }
    
    // Getter để lấy chase attack duration hiện tại
    public float GetChaseAttackDuration()
    {
        return chaseAttackDuration;
    }
    
    private void OnChaseAttackComplete()
    {
        Debug.Log("Chase Attack hoàn thành!");
        currentState = BossState.Idle;
        stateTimer = idleDuration;
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
