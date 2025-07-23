using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    private BossController bossController;
    private DashAttack dashAttack;
    private ChaseAttack chaseAttack;

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
        dashAttack = GetComponentInParent<DashAttack>();
        chaseAttack = GetComponentInParent<ChaseAttack>();
    }

    // Animation Event: Được gọi khi melee attack animation kết thúc
    private void EndAttack()
    {
        // Kiểm tra xem attack này thuộc về BossController hay ChaseAttack
        if (chaseAttack != null && chaseAttack.IsPerformingChaseAttack)
        {
            // Đây là attack từ ChaseAttack
            chaseAttack.EndMeleeAttack();
        }
        else if (bossController != null)
        {
            // Đây là attack từ BossController (legacy)
            bossController.EndAttack();
        }
    }
    
    // Animation Event: Được gọi khi prepare dash animation hoàn thành và sẵn sàng bắt đầu dash
    private void StartDash()
    {
        // Chỉ cần để trống vì DashAttack sẽ tự xử lý dash movement
        // Animation event này có thể được sử dụng để trigger effects hoặc sounds
        Debug.Log("Animation Event: Dash bắt đầu!");
    }
    
    // Animation Event: Được gọi khi fire breath animation hoàn thành
    private void EndFireBreath()
    {
        // Chỉ cần để trống vì DashAttack sẽ tự xử lý end fire breath
        // Animation event này có thể được sử dụng để trigger effects hoặc sounds
        Debug.Log("Animation Event: Fire breath kết thúc!");
    }
}
