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
    private Collider playerCollider;    // Reference to the player's collider
    private bool isJumping;             // To track if the player is jumping
    public bool isHidden;               // To track if the player is hiding
    private bool nearHideableObject;    // To check if the player is near a hideable object
    private GameObject currentHideableObject;  // The hideable object the player is interacting with
    private GameObject[] enemies;       // Array to store all enemies with the "Enemy" tag

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();  // Get the player's collider
        isHidden = false;
        nearHideableObject = false;
    }

    private void Start()
    {
        // Find all enemies by tag and store them in the enemies array
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // This method will be called by the PlayerInput component automatically
    public void OnMove(InputValue value)
    {
        if (!isHidden) // Only allow movement if not hiding
        {
            inputDirection = value.Get<Vector2>();
        }
    }

    // This method will be called by the PlayerInput component automatically for jumping
    public void OnJump(InputValue value)
    {
        if (!isHidden && value.isPressed && isGrounded())
        {
            Jump();
        }
    }

    // This method will be called by the PlayerInput component automatically for hiding
    public void OnHide(InputValue value)
    {
        if (value.isPressed && nearHideableObject)
        {
            ToggleHide();
        }
    }

    private void FixedUpdate()
    {
            if (!isHidden) // Only allow movement if not hiding
            {
                MovePlayer();
                spriteRenderer.flipX = rb.velocity.x < 0f;
            }

            // Always check for grounding
            if (isGrounded())
            {
                // Logic for when grounded, if needed
            }

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

    // Handle the hiding mechanic
    private void ToggleHide()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            // When hiding, stop player movement and make the player invisible
            rb.velocity = Vector3.zero;  // Stop the player from moving
            spriteRenderer.enabled = false;  // Hide the player's sprite

            // Disable collisions with all enemies
            foreach (GameObject enemy in enemies)
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, enemyCollider, true);
                }
            }

            // Lock the player's position to avoid falling through the floor
            rb.isKinematic = true;
        }
        else
        {
            // Unhide the player
            spriteRenderer.enabled = true;  // Show the player's sprite

            // Re-enable collisions with all enemies
            foreach (GameObject enemy in enemies)
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, enemyCollider, false);
                }
            }

            // Unlock the player's movement
            rb.isKinematic = false;
        }
    }

    // Detect when the player enters or exits a hideable object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hideable"))
        {
            nearHideableObject = true;
            currentHideableObject = other.gameObject;
            Debug.Log("Entered hideable object");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hideable"))
        {
            nearHideableObject = false;
            currentHideableObject = null;
            Debug.Log("Exited hideable object");
        }
    }
}
