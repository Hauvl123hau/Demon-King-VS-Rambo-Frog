using UnityEngine;

public class LeftBoundary : MonoBehaviour
{
    [Header("Lực đẩy người chơi ra khỏi boundary")]
    public float pushForce = 5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Transform playerTransform = other.transform;
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            
            if (playerRb != null)
            {
                float boundaryRightEdge = transform.position.x + GetComponent<Collider2D>().bounds.size.x / 2;
                
                if (playerTransform.position.x < boundaryRightEdge)
                {
                    Vector3 newPos = playerTransform.position;
                    newPos.x = boundaryRightEdge + 0.1f; 
                    playerTransform.position = newPos;
                    
                    if (playerRb.linearVelocity.x < 0)
                    {
                        Vector2 velocity = playerRb.linearVelocity;
                        velocity.x = 0;
                        playerRb.linearVelocity = velocity;
                    }
                }
            }
        }
    }
}
