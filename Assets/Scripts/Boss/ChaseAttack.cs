using UnityEngine;
using System;

public class ChaseAttack : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform player;
    public Transform raycastOrigin;
    private BossController bossController;

    [Header("Chase Settings")]
    public float chaseSpeed = 3f;
    public float meleeAttackRange = 4f;
    public float stopDistance = 0.5f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Vision Settings")]
    public LayerMask obstacleLayerMask = ~0;
    public LayerMask playerLayer = 1;

    public event Action OnChaseAttackComplete;

    private bool isPerformingChaseAttack = false;
    private bool isAttacking = false;
    private Vector3 originalScale;
    private float chaseStartTime;

    public bool IsPerformingChaseAttack => isPerformingChaseAttack;

    public float GetElapsedTime()
    {
        if (!isPerformingChaseAttack) return 0f;
        return Time.time - chaseStartTime;
    }

    private void Start()
    {
        originalScale = transform.localScale;
        bossController = GetComponent<BossController>();
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
        chaseStartTime = Time.time;
    }

    public void StopChaseAttack()
    {
        isPerformingChaseAttack = false;
        isAttacking = false;
        if (anim != null)
        {
            anim.SetBool("isChasing", false);
            anim.SetBool("isAttacking", false);
        }
        OnChaseAttackComplete?.Invoke();
    }

    private void Update()
    {
        if (!isPerformingChaseAttack || player == null) return;
        float distanceToPlayer = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
        float elapsedTime = Time.time - chaseStartTime;
        if (distanceToPlayer <= meleeAttackRange && !isAttacking)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PerformMeleeAttack();
            }
        }
        else if (!isAttacking)
        {
            if (distanceToPlayer > stopDistance)
            {
                ChasePlayer();
            }
        }
        UpdateAnimations();
    }

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;
    }

    private void PerformMeleeAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
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

    public void EndMeleeAttack()
    {
        isAttacking = false;
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
        }
        float distance = bossController != null ? bossController.GetDistanceToPlayer() : Vector3.Distance(raycastOrigin.position, player.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastOrigin.position, meleeAttackRange);
        if (player != null && isPerformingChaseAttack)
        {
            bool canSee = bossController != null ? bossController.CanSeePlayer() : CanSeePlayer();
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(raycastOrigin.position, player.position);
        }
    }
}
