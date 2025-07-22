using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    

    [Header("Movement Settings")]
    [SerializeField] private float speed;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f; // Tầm đánh
    [SerializeField] private float attackSpeed = 8f; // Tốc độ lao vào khi tấn công
    [SerializeField] private float attackCooldown = 3f; // Thời gian chờ giữa các lần tấn công
    [SerializeField] private int damage = 1;
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;
        ChasePlayer();
        Flip();
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Quay mặt về bên trái
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Quay mặt về bên phải
        }
    }
}
