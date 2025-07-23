using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    private BossController bossController;

    private void Awake()
    {
        bossController = GetComponentInParent<BossController>();
    }

    private void EndAttack()
    {
        if(bossController != null)
        {
            bossController.EndAttack();
        }
    }
}
