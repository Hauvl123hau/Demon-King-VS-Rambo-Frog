using UnityEngine;

public class ShootFireballsAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    public GameObject fireballPrefab;
    public Transform firePoint; 
    private BossController bossController;

    [Header("Attack Settings")]
    public float fireballSpeed = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    private void Start()
    {
        bossController = GetComponent<BossController>();
        if (bossController == null)
        {
            bossController = GetComponentInParent<BossController>();
        }

        // Auto-find Animator if not assigned
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    // Không tự động bắn nữa, BossController sẽ gọi ShootFireball khi cần

    public void ShootFireball()
    {
        // Trigger animation
        if (anim != null)
        {
            anim.SetBool("isShooting", true);
            anim.SetTrigger("shootFireBall");
        }

        Vector3 targetPos = bossController.player.position;
        Vector3 direction = (targetPos - firePoint.position).normalized;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireballSpeed;
        }
        Debug.Log("Boss bắn fireball về phía người chơi!");
    }
}
