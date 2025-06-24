using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed of the player movement
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    [SerializeField] private float jumpForce = 15f; // Force applied when the player jumps
    [SerializeField] private LayerMask groundLayer; // Layer mask to identify ground objects
    [SerializeField] private Transform groundCheck; // Transform to check if the player is grounded
    private bool isGrounded; // Flag to check if the player is on the ground
    void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
    }
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrow keys)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); // Set the horizontal velocity
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1); // Face right
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1); // Face left
    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // Check if the jump button is pressed and the player is grounded
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply jump force
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer); // Check if the player is grounded
    }
}
