using UnityEngine;

public class TriggerAreaCheck : MonoBehaviour
{
    private GroundEnemy enemyParent;
    private void Awake()
    {
        enemyParent = GetComponentInParent<GroundEnemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyParent.target = collision.transform;
            enemyParent.inRange = true;
            enemyParent.hotZone.SetActive(true);
        }
    }   
}
