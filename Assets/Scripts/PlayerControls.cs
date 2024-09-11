using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;        // Speed at which the player moves
    public float moveThreshold = 0.1f;  // Minimum input value to register movement
    private Vector2 inputDirection;     // Direction of input from the player (X and Z plane)

    private Rigidbody rb;               // Reference to Rigidbody component for 3D physics

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This method will be called by the PlayerInput component automatically
    public void OnMove(InputValue value)
    {
        // Read the input from the input system (X, Z)
        inputDirection = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // Only move if the input is above the threshold
        if (inputDirection.magnitude > moveThreshold)
        {
            MovePlayer();
        }
        else
        {
            // Stop the player if there's no input
            rb.velocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        // Normalize the input to avoid diagonal speed boost and apply speed
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y).normalized * moveSpeed;

      
        rb.velocity = movement;
    }
}
