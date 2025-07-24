using UnityEngine;

public class BossAnimationEvents : MonoBehaviour
{
    private BossController bossController;
    private BossHealth bossHealth;
    private DashAttack dashAttack;
    private ChaseAttack chaseAttack;
    private ShootFireballsAttack shootFireballsAttack;
    

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
        bossHealth = GetComponentInParent<BossHealth>();
        dashAttack = GetComponentInParent<DashAttack>();
        chaseAttack = GetComponentInParent<ChaseAttack>();
        shootFireballsAttack = GetComponentInParent<ShootFireballsAttack>();
    }

    // Animation Event: Được gọi khi melee attack animation kết thúc (cho ChaseAttack)
    private void EndMeleeAttack()
    {
        if (chaseAttack != null)
        {
            chaseAttack.EndMeleeAttack();
            Debug.Log("Animation Event: Melee Attack kết thúc!");
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

    private void ShootFireball()
    {
        ShootFireballsAttack shootFireballs = GetComponentInParent<ShootFireballsAttack>();
        if (shootFireballs != null)
        {
            shootFireballs.ShootFireball();
            Debug.Log("Animation Event: Boss bắn fireball!");
        }
        else
        {
            Debug.LogWarning("ShootFireballsAttack component không tìm thấy!");
        }
    }

    // Animation Event: Được gọi khi animation chết kết thúc
    private void EndDieAnimation()
    {
        if (bossHealth != null)
        {
            bossHealth.DieAnimation();
            Debug.Log("Animation Event: Boss chết hoàn tất!");
        }
        else
        {
            Debug.LogWarning("BossHealth component không tìm thấy!");
        }
    }
}
