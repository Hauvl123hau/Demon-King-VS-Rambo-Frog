using UnityEngine;
using System.Collections;

public class DashAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    private BossController bossController; // Reference để sử dụng utility methods
    
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.5f;
    public float fireBreathRange = 4f;      // Khoảng cách để dừng dash và phun lửa
    public float dashCooldown = 3f;
    
    [Header("Fire Breath Settings")]
    public float fireBreathDelay = 0.2f;    // Delay trước khi phun lửa sau dash
    
    [Header("Raycast Settings")]
    public Transform raycastOrigin;          // Điểm xuất phát raycast (nếu null sẽ dùng transform)
    public LayerMask obstacleLayerMask = ~0; // Layer của vật cản
    public LayerMask playerLayer = 1;        // Layer của player
    
    private Vector3 dashStartPosition;
    private Vector3 dashTarget;
    private float dashStartTime;
    private bool isDashing = false;
    private bool isPerformingCombo = false;
    private Vector3 originalScale;
    
    public System.Action OnComboComplete; // Event để thông báo khi combo hoàn thành
    
    private void Start()
    {
        originalScale = transform.localScale;
        
        // Tìm BossController reference
        bossController = GetComponent<BossController>();
        if (bossController == null)
        {
            Debug.LogError("DashAttack: Không tìm thấy BossController component!");
        }
        
        // Nếu không có raycastOrigin, sử dụng transform của boss
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
            Debug.LogWarning("RaycastOrigin chưa được gán trong DashAttack, sử dụng transform của Boss");
        }
        
        // Tìm Animator trong component hiện tại hoặc trong children
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
    }
    
    // === ATTACK LOGIC ===
    private bool IsPlayerInFireBreathRange()
    {
        if (player == null) return false;
        
        float distanceToPlayer = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
        return distanceToPlayer <= fireBreathRange && canSee;
    }
    
    private bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = Vector3.Distance(raycastOrigin.position, player.position);
        
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
    
    public bool CanPerformDashAttack()
    {
        return !isPerformingCombo && !isDashing;
    }
    
    public void StartDashFireCombo(Transform target)
    {
        if (!CanPerformDashAttack() || target == null)
        {
            return;
        }
        
        player = target;
        isPerformingCombo = true;
        
        StartCoroutine(DashFireComboCoroutine());
    }
    
    private IEnumerator DashFireComboCoroutine()
    {
        // Phase 1: Chuẩn bị dash
        Debug.Log("Dash Attack: Chuẩn bị combo dash + fire breath!");
        
        // Kiểm tra xem player có trong tầm fire breath và có thể nhìn thấy không
        if (IsPlayerInFireBreathRange())
        {
            Debug.Log("Dash Attack: Player đã trong tầm fire breath và có thể nhìn thấy!");
            FaceTarget(player.position);
            yield return StartCoroutine(PerformFireBreath());
            CompleteCombo();
            yield break;
        }
        
        // Tính toán điểm đích dash (dừng ở khoảng cách fireBreathRange)
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        
        // Tính điểm đích để dừng ở khoảng cách fireBreathRange
        float dashDistance = distanceToPlayer - fireBreathRange;
        dashTarget = transform.position + directionToPlayer * dashDistance;
        dashStartPosition = transform.position;
        
        // Trigger animation chuẩn bị
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", true);
            anim.SetTrigger("prepareDash");
        }
        
        // Đợi một chút cho animation chuẩn bị
        yield return new WaitForSeconds(0.3f);
        
        // Phase 2: Thực hiện dash
        yield return StartCoroutine(PerformDash());
        
        // Phase 3: Delay ngắn trước khi phun lửa
        yield return new WaitForSeconds(fireBreathDelay);
        
        // Phase 4: Phun lửa
        yield return StartCoroutine(PerformFireBreath());
        
        // Phase 5: Hoàn thành combo
        CompleteCombo();
    }
    
    private IEnumerator PerformDash()
    {
        Debug.Log("Dash Attack: Bắt đầu dash!");
        isDashing = true;
        dashStartTime = Time.time;
        
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", false);
            anim.SetBool("isDashing", true);
            anim.SetTrigger("dash");
        }
        
        // Thực hiện dash movement
        while (Time.time - dashStartTime < dashDuration)
        {
            float dashProgress = (Time.time - dashStartTime) / dashDuration;
            dashProgress = Mathf.Clamp01(dashProgress);
            
            // Di chuyển theo đường thẳng từ start đến target
            transform.position = Vector3.Lerp(dashStartPosition, dashTarget, dashProgress);
            
            // Quay mặt về hướng dash
            Vector3 dashDirection = (dashTarget - dashStartPosition).normalized;
            FaceDirection(dashDirection);
            
            yield return null;
        }
        
        // Đảm bảo đến đúng vị trí cuối
        transform.position = dashTarget;
        isDashing = false;
        
        if (anim != null)
        {
            anim.SetBool("isDashing", false);
        }
        
        Debug.Log("Dash Attack: Hoàn thành dash!");
    }
    
    private IEnumerator PerformFireBreath()
    {
        Debug.Log("Dash Attack: Bắt đầu phun lửa!");
        
        // Quay mặt về phía player
        FaceTarget(player.position);
        
        // Kiểm tra xem có thể nhìn thấy player để phun lửa không
        bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        
        if (!canSee)
        {
            Debug.Log("Dash Attack: Không thể nhìn thấy player, bỏ qua fire breath!");
        }
        else if (distance > fireBreathRange)
        {
            Debug.Log("Dash Attack: Player quá xa để phun lửa!");
        }
        else
        {
            Debug.Log("Dash Attack: Player trong tầm, thực hiện fire breath!");
        }
        
        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("fireBreath");
        }
        
        // Đợi animation fire breath hoàn thành (có thể điều chỉnh thời gian này)
        yield return new WaitForSeconds(1.5f);
        
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
        
        Debug.Log("Dash Attack: Hoàn thành phun lửa!");
    }
    
    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        FaceDirection(directionToTarget);
    }
    
    private void FaceDirection(Vector3 direction)
    {
        if (direction.x > 0)
        {
            // Quay phải
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x < 0)
        {
            // Quay trái
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }
    
    private void CompleteCombo()
    {
        Debug.Log("Dash Attack: Hoàn thành combo!");
        isPerformingCombo = false;
        isDashing = false;
        
        // Reset tất cả animation flags
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", false);
            anim.SetBool("isDashing", false);
            anim.SetBool("isAttacking", false);
        }
        
        // Thông báo cho BossController rằng combo đã hoàn thành
        OnComboComplete?.Invoke();
    }
    
    // Method để force stop combo nếu cần
    public void StopCombo()
    {
        if (isPerformingCombo)
        {
            StopAllCoroutines();
            CompleteCombo();
        }
    }
    
    // Getters để BossController có thể check trạng thái
    public bool IsPerformingCombo => isPerformingCombo;
    public bool IsDashing => isDashing;
    
    // Vẽ debug gizmos để visualize fireBreathRange và raycast
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        
        // Vẽ vòng tròn fire breath range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastOrigin.position, fireBreathRange);
        
        // Vẽ raycast đến player nếu có
        if (player != null)
        {
            bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
            Gizmos.color = canSee ? Color.green : Color.yellow;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
            
            // Vẽ điểm xuất phát raycast
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(raycastOrigin.position, 0.15f);
        }
    }
}
