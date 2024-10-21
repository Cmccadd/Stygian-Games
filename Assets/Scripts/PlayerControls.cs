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
    [SerializeField] private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Collider playerCollider;
    public bool isHidden;
    private bool nearHideableObject;
    private bool insideHideSpot;  // Track if inside hideable object
    private GameObject[] enemies;
    private Interactable currentInteractable;

    private bool facingLeft = false;  // To track the last direction faced

    [SerializeField] private CheckpointManager _checkpointManager;

    // Add a reference to the player's inventory
    public Inventory inventory;

    // Name of the exorcism item, for example, "Sigil"
    public string excursionItemName = "Sigil";

    // Exorcism detection variables
    public float exorcismRange = 3f;  // Radius of the exorcism detection sphere
    private Collider[] enemiesInRange;

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _walkingSFX;
    [SerializeField] private AudioClip _jumpingSFX;
    [SerializeField] private AudioClip _grabSFX;
    [SerializeField] private AudioSource _myAudioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider>();

        _checkpointManager = FindObjectOfType<CheckpointManager>();
        transform.position = _checkpointManager.LastCheckPointPos;

        isHidden = false;
        nearHideableObject = false;
        insideHideSpot = false;  // Default to false
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
            _walkingSFX.SetActive(true);
        }
    }

    public void OnJump(InputValue value)
    {
        if (!isHidden && value.isPressed && isGrounded())
        {
            Jump();
            _myAudioSource.PlayOneShot(_jumpingSFX);
        }
    }

    public void OnHide(InputValue value)
    {
        // Only allow hiding if the player is near or inside a hideable object
        if (value.isPressed && nearHideableObject && insideHideSpot)
        {
            ToggleHide();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            if (currentInteractable != null)
            {
                _animator.Play("Will_Grab_Anim");
                currentInteractable.InteractWith(this); // Pass the player reference
                _myAudioSource.PlayOneShot(_grabSFX);
            }
            else
            {
                // Prevent exorcizing if the player is hidden
                if (!isHidden)
                {
                    // Check for enemies in the exorcism range before using the Sigil
                    UseExcursionItemOnEnemies();
                }
                else
                {
                    Debug.Log("Cannot exorcize while hidden.");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isHidden)
        {
            MovePlayer();
            HandleSpriteFlip();  // Handle sprite flipping based on movement direction
        }
        if (rb.velocity.y < -.5 && !isGrounded())
        {
            _animator.SetBool("Falling", true);
            _animator.SetBool("Jumping", false);
        }
        else if (rb.velocity.y <= 0 && rb.velocity.y > -.5)
        {
            _animator.SetBool("Falling", false);
            _animator.SetBool("Jumping", false);
        }
        else if (rb.velocity.y > 0 && !isGrounded())
        {
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Falling", false);
        }

        if (rb.velocity.x == 0 && rb.velocity.z == 0)
        {
            _animator.SetBool("Moving", false);
            _walkingSFX.SetActive(false);
        }
        else if (rb.velocity.x != 0 && rb.velocity.y == 0 || rb.velocity.z != 0 && rb.velocity.y == 0)
        {
            _walkingSFX.SetActive(true);
        }

        if (rb.velocity.z > 0f)
        {
            _animator.SetBool("MovingUP", true);
        }
        else if (rb.velocity.z < 0f)
        {
            _animator.SetBool("MovingDOWN", true);
        }
        else if (rb.velocity.z == 0f)
        {
            _animator.SetBool("MovingUP", false);
            _animator.SetBool("MovingDOWN", false);
        }

        if (!isGrounded())
        {
            _walkingSFX.SetActive(false);
        }
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.y).normalized * moveSpeed;
        movement.y = rb.velocity.y;
        rb.velocity = movement;
        _animator.SetBool("Moving", true);
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

        if (isHidden)
        {
            rb.velocity = Vector3.zero;
            spriteRenderer.enabled = false;

            foreach (GameObject enemy in enemies)
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, enemyCollider, true);
                }
            }
            rb.isKinematic = true;
        }
        else
        {
            spriteRenderer.enabled = true;

            foreach (GameObject enemy in enemies)
            {
                Collider enemyCollider = enemy.GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    Physics.IgnoreCollision(playerCollider, enemyCollider, false);
                }
            }
            rb.isKinematic = false;
        }
    }

    // Handle flipping the sprite based on movement direction
    private void HandleSpriteFlip()
    {
        if (inputDirection.x > 0 && facingLeft)
        {
            spriteRenderer.flipX = false;
            facingLeft = false;
        }
        else if (inputDirection.x < 0 && !facingLeft)
        {
            spriteRenderer.flipX = true;
            facingLeft = true;
        }
        // If no movement input, the sprite remains facing its last direction
    }

    // Check for enemies in the exorcism range and use the excursion item if available
    private void UseExcursionItemOnEnemies()
    {
        // Check if the player has the required excursion item in the inventory
        if (inventory.HasItem(excursionItemName))
        {
            // Get all colliders in the exorcism range
            enemiesInRange = Physics.OverlapSphere(transform.position, exorcismRange, LayerMask.GetMask("Enemy"));

            bool enemyInRange = false; // Track if any enemy is in range

            foreach (Collider enemyCollider in enemiesInRange)
            {
                // Check if the enemyCollider is still valid (not null) before accessing its EnemyAI component
                if (enemyCollider != null)
                {
                    EnemyAI enemy = enemyCollider.GetComponent<EnemyAI>();
                    EnemyAIPreist preist = enemyCollider.GetComponent<EnemyAIPreist>();
                    if (preist != null) // Ensure the enemy is not destroyed
                    {
                        preist.Excise();
                        enemyInRange = true; // Mark that an enemy was in range
                    }
                }
            }

            // Only use the Sigil if an enemy was exorcised
            if (enemyInRange)
            {
                _animator.Play("Will_Exorcise_Anim");
                inventory.UseItem(excursionItemName);
                Debug.Log("Sigil used to exorcise enemy.");
            }
            else
            {
                Debug.Log("No enemy in exorcism range. Sigil not used.");
            }
        }
        else
        {
            Debug.Log("No excursion item available in inventory.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            currentInteractable = interactable;
        }

        if (other.CompareTag("Hideable"))
        {
            insideHideSpot = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentInteractable = null;

        if (other.CompareTag("Hideable"))
        {
            insideHideSpot = false; // Reset when leaving hideable area
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hideable"))
        {
            nearHideableObject = true;
        }
    }

    // Visualize the exorcism detection range using Gizmos in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, exorcismRange);
        }
    }
}
