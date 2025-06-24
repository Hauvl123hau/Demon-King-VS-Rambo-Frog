using UnityEngine;
using UnityEngine.SceneManagement;
public class FallTrap : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool daroi = false;
    public Transform diemkhoiphuc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !daroi)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            daroi = true;
            Invoke("khoiphuc", 2f); // Call khoiphuc after 2 seconds
        }
    }private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Lever-1");
        }
    }    private void khoiphuc()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.position = diemkhoiphuc.position;
        // Reset the rotation to the original state
        daroi = false;
    }
}
