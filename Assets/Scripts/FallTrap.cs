using UnityEngine;
using UnityEngine.SceneManagement;
public class FallTrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool daroi = false;
    public Transform diemkhoiphuc;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !daroi)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            daroi = true;
            Invoke("khoiphuc", 2f);
        }
    }private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            DeadUI deadUI = FindObjectOfType<DeadUI>();
            if (deadUI != null)
            {
                deadUI.ShowDeadPanelWithMessage("Bị nghiền nát bởi bẫy !");
            }
            else
            {
               
                SceneManager.LoadScene("Lever-1");
            }
        }
    }    private void khoiphuc()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.position = diemkhoiphuc.position;
        
        daroi = false;
    }
}
