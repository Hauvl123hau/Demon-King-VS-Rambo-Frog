using UnityEngine;

// Ví dụ về Boss enemy với damage cao hơn
public class BossEnemy : MonoBehaviour, IDamageDealer
{
    [SerializeField] private int bossDamage = 3; // Boss gây damage nhiều hơn
    
    public int GetDamage()
    {
        return bossDamage;
    }
    
    public EnemyType GetEnemyType()
    {
        return EnemyType.Boss;
    }
    
    // Có thể thêm logic khác cho boss ở đây
}
