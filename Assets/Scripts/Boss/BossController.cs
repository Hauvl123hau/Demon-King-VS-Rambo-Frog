using UnityEngine;

public class BossController : MonoBehaviour
{
    public Animator anim;
    public Transform player;
    public Transform raycastOrigin;
    public DashAttack dashAttack;
    public ChaseAttack chaseAttack;
    public ShootFireballsAttack shootFireballs;
    public BossHealth bossHealth;
    public bool dashAttackOn = false;
    public bool chaseAttackOn = false;
    public bool shootFireballsOn = false;
    public bool RandomAttackLoop = true;
    public bool moveToA = false;
    public bool moveToB = false;
    public Transform targetA;
    public Transform targetB;
    public float moveSpeed = 3f;
    private enum BossState1
    {
        Idle,
        DashFireCombo,
        ShootFireballs,
        ChaseAttack       
    }
    private BossState1 currentState = BossState1.Idle;
    public float idleDuration = 1f;
    public float stateTimer;
    public float dashFireComboRange = 10f;
    public LayerMask obstacleLayerMask = ~0;
    public LayerMask playerLayer = 1;
    public float chaseAttackDuration = 5f;
    private Vector3 originalScale;
    public int maxFireballShots = 3;
    private int fireballShotsCount = 0;
    public float fireballAttackDuration = 3f;
    private float fireballAttackTimer = 0f;

    private void Start()
    {
        stateTimer = idleDuration;
        originalScale = transform.localScale;
        if (anim == null) anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        if (raycastOrigin == null) raycastOrigin = transform;
        if (dashAttackOn && dashAttack == null) dashAttack = GetComponent<DashAttack>() ?? gameObject.AddComponent<DashAttack>();
        if (chaseAttackOn && chaseAttack == null) chaseAttack = GetComponent<ChaseAttack>() ?? gameObject.AddComponent<ChaseAttack>();
        if (dashAttackOn && dashAttack != null)
        {
            dashAttack.anim = anim;
            dashAttack.player = player;
            dashAttack.fireBreathRange = 4f;
            dashAttack.raycastOrigin = raycastOrigin;
            dashAttack.obstacleLayerMask = obstacleLayerMask;
            dashAttack.playerLayer = playerLayer;
            dashAttack.OnComboComplete += OnDashFireComboComplete;
        }
        if (chaseAttackOn && chaseAttack != null)
        {
            chaseAttack.anim = anim;
            chaseAttack.player = player;
            chaseAttack.raycastOrigin = raycastOrigin;
            chaseAttack.OnChaseAttackComplete += OnChaseAttackComplete;
        }
        if (shootFireballsOn && shootFireballs != null)
        {
            shootFireballs.anim = anim;
            shootFireballs.player = player;
            if (shootFireballs.firePoint == null) shootFireballs.firePoint = raycastOrigin != null ? raycastOrigin : transform;
        }
    }

    private void Update()
    {
        if (player != null && currentState != BossState1.DashFireCombo) FacePlayer();
        bool isDashAttacking = dashAttackOn && dashAttack != null && dashAttack.IsPerformingCombo;
        bool isChaseAttacking = chaseAttackOn && chaseAttack != null && chaseAttack.IsPerformingChaseAttack;
        if (isDashAttacking || isChaseAttacking) return;
        stateTimer -= Time.deltaTime;
        bool isMovingToTarget = (moveToA && targetA != null) || (moveToB && targetB != null);
        if (anim != null) anim.SetBool("isChasing", isMovingToTarget);
        switch (currentState)
        {
            case BossState1.Idle:
                if (moveToA && targetA != null)
                {
                    MoveAndJumpToPlatform(targetA.position, 8f, 1.5f);
                    FaceMoveDirection(targetA.position);
                }
                else if (moveToB && targetB != null)
                {
                    MoveAndJumpToPlatform(targetB.position, 8f, 1.5f);
                    FaceMoveDirection(targetB.position);
                }
                if (stateTimer <= 0f)
                {
                    ChooseRandomAttack();
                }
                break;
            case BossState1.DashFireCombo:
                break;
            case BossState1.ChaseAttack:
                if (chaseAttack != null && chaseAttack.IsPerformingChaseAttack)
                {
                    float elapsedTime = chaseAttack.GetElapsedTime();
                    if (elapsedTime >= chaseAttackDuration) chaseAttack.StopChaseAttack();
                }
                break;
            case BossState1.ShootFireballs:
                fireballAttackTimer += Time.deltaTime;
                if (fireballShotsCount < maxFireballShots)
                {
                    if (shootFireballs != null)
                    {
                        shootFireballs.ShootFireball();
                        fireballShotsCount++;
                    }
                }
                else
                {
                    currentState = BossState1.Idle;
                    stateTimer = idleDuration;
                }
                break;
        }
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(raycastOrigin.position, player.position);
    }

    public bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (player.position - raycastOrigin.position).normalized;
        float distanceToPlayer = GetDistanceToPlayer();
        int obstacleOnlyMask = obstacleLayerMask & ~playerLayer;
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, directionToPlayer, out hit, distanceToPlayer, obstacleOnlyMask)) return false;
        return true;
    }

    public void FacePlayer()
    {
        if (player == null) return;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (directionToPlayer.x > 0) transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (directionToPlayer.x < 0) transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    public void FaceMoveDirection(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.x > 0) transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (direction.x < 0) transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    }

    void ChooseAttackByDistance()
    {
        if (player == null) return;
        float distanceToPlayer = GetDistanceToPlayer();
        bool canSeePlayer = CanSeePlayer();
        if (canSeePlayer)
        {
            if (shootFireballsOn && shootFireballs != null)
            {
                TriggerShootFireballs();
                currentState = BossState1.ShootFireballs;
            }
            else if (distanceToPlayer <= dashFireComboRange && dashAttackOn && dashAttack != null && dashAttack.CanPerformDashAttack())
            {
                TriggerDashFireCombo();
                currentState = BossState1.DashFireCombo;
            }
            else if (chaseAttackOn && chaseAttack != null)
            {
                TriggerChaseAttack();
                currentState = BossState1.ChaseAttack;
            }
            else stateTimer = idleDuration;
        }
        else
        {
            if (chaseAttackOn && chaseAttack != null)
            {
                TriggerChaseAttack();
                currentState = BossState1.ChaseAttack;
            }
            else stateTimer = idleDuration;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin == null) return;
        if (chaseAttackOn && chaseAttack != null) Gizmos.DrawWireSphere(raycastOrigin.position, chaseAttack.meleeAttackRange);
        if (dashAttackOn) Gizmos.DrawWireSphere(raycastOrigin.position, dashFireComboRange);
        if (player != null)
        {
            Gizmos.DrawLine(raycastOrigin.position, player.position);
            Gizmos.DrawWireSphere(raycastOrigin.position, 0.2f);
        }
    }

    // Chọn đòn tấn công ngẫu nhiên cho BossState1
    private void ChooseRandomAttack()
    {
        if (player == null) return;
        // Nếu bật RandomAttackLoop thì random trạng thái attack
        if (RandomAttackLoop)
        {
            dashAttackOn = UnityEngine.Random.value > 0.5f;
            chaseAttackOn = UnityEngine.Random.value > 0.5f;
            shootFireballsOn = UnityEngine.Random.value > 0.5f;
        }
        int attackCount = 0;
        System.Collections.Generic.List<BossState1> possibleAttacks = new System.Collections.Generic.List<BossState1>();
        if (shootFireballsOn && shootFireballs != null) { possibleAttacks.Add(BossState1.ShootFireballs); attackCount++; }
        if (dashAttackOn && dashAttack != null && dashAttack.CanPerformDashAttack()) { possibleAttacks.Add(BossState1.DashFireCombo); attackCount++; }
        if (chaseAttackOn && chaseAttack != null) { possibleAttacks.Add(BossState1.ChaseAttack); attackCount++; }
        if (possibleAttacks.Count == 0)
        {
            stateTimer = idleDuration;
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, possibleAttacks.Count);
        BossState1 chosenAttack = possibleAttacks[randomIndex];
        switch (chosenAttack)
        {
            case BossState1.ShootFireballs:
                TriggerShootFireballs();
                currentState = BossState1.ShootFireballs;
                break;
            case BossState1.DashFireCombo:
                TriggerDashFireCombo();
                currentState = BossState1.DashFireCombo;
                break;
            case BossState1.ChaseAttack:
                TriggerChaseAttack();
                currentState = BossState1.ChaseAttack;
                break;
        }
    }

    public void TriggerShootFireballs()
    {
        if (!shootFireballsOn || shootFireballs == null) return;
        fireballShotsCount = 0;
        fireballAttackTimer = 0f;
    }
    public void TriggerChaseAttack()
    {
        if (!chaseAttackOn || chaseAttack == null) return;
        currentState = BossState1.ChaseAttack;
        chaseAttack.StartChaseAttack(player);
    }
    public void SetChaseAttackDuration(float newDuration)
    {
        chaseAttackDuration = Mathf.Max(0.1f, newDuration);
    }
    public float GetChaseAttackDuration()
    {
        return chaseAttackDuration;
    }
    private void OnChaseAttackComplete()
    {
        currentState = BossState1.Idle;
        stateTimer = idleDuration;
    }
    public void TriggerDashFireCombo()
    {
        if (!dashAttackOn || dashAttack == null) return;
        currentState = BossState1.DashFireCombo;
        dashAttack.StartDashFireCombo(player);
    }
    private void OnDashFireComboComplete()
    {
        currentState = BossState1.Idle;
        stateTimer = idleDuration;
    }
    private void OnDestroy()
    {
        if (dashAttackOn && dashAttack != null) dashAttack.OnComboComplete -= OnDashFireComboComplete;
        if (chaseAttackOn && chaseAttack != null) chaseAttack.OnChaseAttackComplete -= OnChaseAttackComplete;
    }

    public void MoveAndJumpToPlatform(Vector3 targetPosition, float jumpForce, float moveDistance)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float horizontalDistance = Mathf.Abs(targetPosition.x - transform.position.x);
        float verticalDistance = targetPosition.y - transform.position.y;
        // Nếu chưa gần vị trí nhảy, di chuyển lại gần
        if (horizontalDistance > moveDistance)
        {
            Vector3 moveTarget = transform.position + new Vector3(Mathf.Sign(targetPosition.x - transform.position.x) * moveDistance, 0, 0);
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
        }
        // Nếu đã gần vị trí nhảy hoặc đang ở dưới platform, thực hiện nhảy thẳng tới vị trí
        else if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            Vector2 jumpDir = (targetPosition - transform.position).normalized;
            // Tính lực nhảy để boss bay thẳng tới vị trí platform
            float jumpX = (targetPosition.x - transform.position.x);
            float jumpY = Mathf.Max(1f, verticalDistance);
            rb.AddForce(new Vector2(jumpX, jumpY) * jumpForce, ForceMode2D.Impulse);
        }
    }
}
