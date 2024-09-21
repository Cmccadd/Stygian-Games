using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5.5f;
    public GameObject groundCheck;
    public LayerMask ground;

    private Vector2 inputDirection;
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Collider playerCollider;
    public bool isHidden;
    private bool nearHideableObject;
    private GameObject[] enemies;
    private Interactable currentInteractable;

    [SerializeField] private CheckpointManager _checkpointManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();

        _checkpointManager = FindObjectOfType<CheckpointManager>();
        transform.position = _checkpointManager.LastCheckPointPos;

        isHidden = false;
        nearHideableObject = false;
    }

    private void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void OnMove(InputValue value)
    {
        if (!isHidden)
        {
            inputDirection = value.Get<Vector2>();
        }
    }

    public void OnJump(InputValue value)
    {
        if (!isHidden && value.isPressed && isGrounded())
        {
            Jump();
        }
    }

    public void OnHide(InputValue value)
    {
        if (value.isPressed && nearHideableObject)
        {
            ToggleHide();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && currentInteractable != null)
        {
            currentInteractable.InteractWith(this); // Pass the player reference
        }
    }

    private void FixedUpdate()
    {
        if (!isHidden)
        {
            MovePlayer();
            spriteRenderer.flipX = rb.velocity.x < 0f;
        }
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y).normalized * moveSpeed;
        movement.y = rb.velocity.y;
        rb.velocity = movement;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.transform.position, 0.1f, ground);
    }

    private void ToggleHide()
    {
        isHidden = !isHidden;
        rb.velocity = Vector3.zero;
        spriteRenderer.enabled = !isHidden;
        rb.isKinematic = isHidden;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentInteractable = null;
    }
}
