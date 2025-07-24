using UnityEngine;
using System.Collections;

public class DashAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    private BossController bossController;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.5f;
    public float fireBreathRange = 4f;
    public float dashCooldown = 3f;

    [Header("Fire Breath Settings")]
    public float fireBreathDelay = 0.2f;

    [Header("Raycast Settings")]
    public Transform raycastOrigin;
    public LayerMask obstacleLayerMask = ~0;
    public LayerMask playerLayer = 1;

    private Vector3 dashStartPosition;
    private Vector3 dashTarget;
    private float dashStartTime;
    private bool isDashing = false;
    private bool isPerformingCombo = false;
    private Vector3 originalScale;

    public System.Action OnComboComplete;
    
    private void Start()
    {
        originalScale = transform.localScale;
        bossController = GetComponent<BossController>();
        if (raycastOrigin == null)
        {
            raycastOrigin = transform;
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
    }
    
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
        if (IsPlayerInFireBreathRange())
        {
            FaceTarget(player.position);
            yield return StartCoroutine(PerformFireBreath());
            CompleteCombo();
            yield break;
        }
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        float dashDistance = distanceToPlayer - fireBreathRange;
        dashTarget = transform.position + directionToPlayer * dashDistance;
        dashStartPosition = transform.position;
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", true);
            anim.SetTrigger("prepareDash");
        }
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(PerformDash());
        yield return new WaitForSeconds(fireBreathDelay);
        yield return StartCoroutine(PerformFireBreath());
        CompleteCombo();
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", false);
            anim.SetBool("isDashing", true);
            anim.SetTrigger("dash");
        }
        while (Time.time - dashStartTime < dashDuration)
        {
            float dashProgress = (Time.time - dashStartTime) / dashDuration;
            dashProgress = Mathf.Clamp01(dashProgress);
            transform.position = Vector3.Lerp(dashStartPosition, dashTarget, dashProgress);
            Vector3 dashDirection = (dashTarget - dashStartPosition).normalized;
            FaceDirection(dashDirection);
            yield return null;
        }
        transform.position = dashTarget;
        isDashing = false;
        if (anim != null)
        {
            anim.SetBool("isDashing", false);
        }
    }

    private IEnumerator PerformFireBreath()
    {
        FaceTarget(player.position);
        bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("fireBreath");
        }
        yield return new WaitForSeconds(1.5f);
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
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
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    private void CompleteCombo()
    {
        isPerformingCombo = false;
        isDashing = false;
        if (anim != null)
        {
            anim.SetBool("isPreparingDash", false);
            anim.SetBool("isDashing", false);
            anim.SetBool("isAttacking", false);
        }
        OnComboComplete?.Invoke();
    }

    public void StopCombo()
    {
        if (isPerformingCombo)
        {
            StopAllCoroutines();
            CompleteCombo();
        }
    }

    public bool IsPerformingCombo => isPerformingCombo;
    public bool IsDashing => isDashing;

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastOrigin.position, fireBreathRange);
        if (player != null)
        {
            bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
            Gizmos.color = canSee ? Color.green : Color.yellow;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(raycastOrigin.position, 0.15f);
        }
    }
}
