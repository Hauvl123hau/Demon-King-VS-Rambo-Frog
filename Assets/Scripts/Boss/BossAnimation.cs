using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    private BossController bossController;

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
    }

    public void OnEndAttack()
    {
        if (bossController != null)
        {
            bossController.EndAttack();
        }

    }
}
