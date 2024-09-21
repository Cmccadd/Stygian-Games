using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // --- Fields ---

    [Header("General Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    [Header("Patrolling Settings")]
    public Transform[] patrolPoints;
    private int patrolIndex;
    private bool walkPointSet;
    public float patrolWaitTime = 2f;
    private bool isWaiting;

    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float lookAroundRotationSpeed = 120f;  // Speed of rotation during looking around

    [Header("Chasing Settings")]
    public float chaseTime = 5f;
    private float chaseTimer;

    [Header("Hiding Timer Settings")]
    public float hidingTimerDuration = 3f;
    private float hidingTimer;

    [Header("Detection Settings")]
    public float sightRange;
    public bool playerInSightRange;
    private bool playerInChaseState;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 10f;  // Distance of the raycast
    public float raycastHeightOffset = 1.5f;  // Height offset for the raycast (enemy's eyes)
    private bool playerInRaycast;

    // --- Unity Methods ---

    private void Start()
    {
        InitializeEnemy();
    }

    private void Update()
    {
        UpdateState();
    }

    // --- Initialization ---

    private void InitializeEnemy()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolIndex = 0;
        walkPointSet = true;
        playerInChaseState = false;
        chaseTimer = chaseTime;
        hidingTimer = hidingTimerDuration;

        // Ensure we have the player reference
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Will");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // Set the initial patrol speed
        agent.speed = patrolSpeed;
    }

    // --- State Handling ---

    private void UpdateState()
    {
        // Check if the player is in sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        // Perform raycast in front of the enemy to detect player
        playerInRaycast = CheckRaycastForPlayer();

        // Handle the hiding timer
        HandleHidingTimer();

        // Determine enemy behavior based on detection state
        if (!playerInSightRange && !playerInRaycast && !playerInChaseState)
        {
            Patroling();
        }
        else if ((playerInSightRange || playerInRaycast) && !IsPlayerHidden())
        {
            ChasePlayer();
        }
        else if (playerInChaseState && !playerInSightRange && !playerInRaycast)
        {
            HandleLookAround();
        }
    }

    private void HandleHidingTimer()
    {
        if (IsPlayerHidden())
        {
            hidingTimer -= Time.deltaTime;
            if (hidingTimer <= 0)
            {
                playerInChaseState = false;  // Stop chasing if the player stays hidden long enough
                Patroling();
            }
        }
        else
        {
            hidingTimer = hidingTimerDuration;  // Reset hiding timer if player is not hidden
        }
    }

    private void HandleLookAround()
    {
        if (chaseTimer > 0)
        {
            chaseTimer -= Time.deltaTime;
            LookAround();
        }
        else
        {
            playerInChaseState = false;
            Patroling();
        }
    }

    // --- Player Detection ---

    // Perform raycast detection in front of the enemy
    private bool CheckRaycastForPlayer()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 rayDirection = transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, raycastDistance, whatIsPlayer))
        {
            // Check if the player is hit and if the player is not hiding
            if (hit.transform.CompareTag("Player"))
            {
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                if (playerController != null && !playerController.isHidden)
                {
                    Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.green);  // Visualize ray when player is hit
                    return true;
                }
            }
        }

        Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);  // Visualize ray when no player is hit
        return false;
    }

    // Helper method to check if the player is hidden
    private bool IsPlayerHidden()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            return playerController.isHidden;
        }
        return false;
    }

    // --- Behavior Methods ---

    private void Patroling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;  // Set the speed to patrol speed

        if (!walkPointSet && patrolPoints.Length > 0 && !isWaiting)
        {
            SetNextPatrolPoint();
        }

        if (walkPointSet && agent.remainingDistance < 1f && !isWaiting)
        {
            walkPointSet = false;
            StartCoroutine(LookAroundAtPatrolPoint());
        }
    }

    private void SetNextPatrolPoint()
    {
        agent.SetDestination(patrolPoints[patrolIndex].position);
        walkPointSet = true;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            playerInChaseState = true;
            chaseTimer = chaseTime;
            agent.isStopped = false;
            agent.speed = chaseSpeed;  // Set the speed to chase speed
            agent.SetDestination(player.position);
        }
    }

    private void LookAround()
    {
        agent.isStopped = true;
        // Rotate to simulate looking around
        transform.Rotate(0, lookAroundRotationSpeed * Time.deltaTime, 0);
    }

    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        float lookTime = patrolWaitTime;
        while (lookTime > 0f)
        {
            // Rotate to simulate looking around
            transform.Rotate(0, lookAroundRotationSpeed * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isWaiting = false;
        walkPointSet = false;
    }

    // --- Gizmos (Debugging) ---

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            foreach (Transform point in patrolPoints)
            {
                Gizmos.DrawSphere(point.position, 0.5f);
            }
        }

        // Visualize the raycast detection in front of the enemy
        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 rayDirection = transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin, rayDirection * raycastDistance);
    }
}
