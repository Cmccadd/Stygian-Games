using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // Speed at which the player moves
    public float moveThreshold = 0.1f;  // Minimum input value to register movement
    public float jumpForce = 5.5f;      // Force applied when jumping
    public GameObject groundCheck;      // Object to check if the player is grounded
    public LayerMask ground;            // Layer representing the ground

    private Vector2 inputDirection;     // Direction of input from the player (X and Z plane)
    private Rigidbody rb;               // Reference to Rigidbody component for 3D physics
    private SpriteRenderer spriteRenderer;
    private bool isJumping;             // To track if the player is jumping

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // This method will be called by the PlayerInput component automatically
    public void OnMove(InputValue value)
    {
        // Read the input from the input system (X, Z)
        inputDirection = value.Get<Vector2>();
    }

    // This method will be called by the PlayerInput component automatically for jumping
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        spriteRenderer.flipX = rb.velocity.x <0f;
    }
    
    private void MovePlayer()
    {
        // Normalize the input to avoid diagonal speed boost and apply speed
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y).normalized * moveSpeed;

        // Preserve the player's current Y velocity (e.g., jumping or falling)
        movement.y = rb.velocity.y;

        // Apply the movement
        rb.velocity = movement;
    }

    private void Jump()
    {
        // Apply jump force along the Y-axis
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    // Check if the player is grounded by using the groundCheck GameObject and Physics.CheckSphere
    private bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.transform.position, 0.1f, ground);
    }
}
