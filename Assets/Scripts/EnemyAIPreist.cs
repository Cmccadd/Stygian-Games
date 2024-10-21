using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPreist : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    private PlayerController playerController;

    [Header("Patrolling Settings")]
    public Transform[] patrolPoints;  // Points for patrolling when the player is lost
    private int patrolIndex;
    private bool walkPointSet;
    public float patrolWaitTime = 2f;
    private bool isWaiting;

    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6f;

    [Header("Chasing Settings")]
    public float chaseTime = 5f;
    private float chaseTimer;

    [Header("Detection Settings")]
    public float sightRange;
    public bool playerInSightRange;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 10f;
    public float raycastHeightOffset = 1.5f;
    private bool playerInRaycast;


    [Header("Look Around Settings")]
    public float standStillLookAroundTime = 3f;  // How long the priest looks around while standing
    private bool isStandingStill;

    private bool isExcised = false;  // Prevent multiple excision calls

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        InitializeEnemy();
    }

    private void Update()
    {
        if (playerController.isHidden)
        {
            StandStillAndLookAround();
            return;  // If the player is hidden, stay in place
        }

        // Check if the player is in sight range and not hidden
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && !playerController.isHidden;

        // Perform raycast to check for player and ensure player is not hidden
        playerInRaycast = CheckRaycastForPlayer();

        // Update state based on whether the player is detected
        UpdateState();
    }

    private void InitializeEnemy()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolIndex = 0;
        walkPointSet = true;
        chaseTimer = chaseTime;
        agent.speed = patrolSpeed;
        isStandingStill = true;  // Start by standing still

        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void UpdateState()
    {
        if ((playerInSightRange || playerInRaycast) && !playerController.isHidden)
        {
            ChasePlayer();  // If the player is in sight, chase them
        }
        else if (!isStandingStill)
        {
            Patroling();  // If the player is lost, patrol between points
        }
    }

    private bool CheckRaycastForPlayer()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 rayDirection = transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, whatIsPlayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                if (playerController != null && !playerController.isHidden)
                {
                    Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.green);
                    return true;
                }
            }
        }

        Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);
        return false;
    }

    private void Patroling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;

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
        isStandingStill = false;
        if (player != null)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            Debug.Log("Chasing player...");
        }
    }

    private void StandStillAndLookAround()
    {
        if (!isStandingStill)
        {
            isStandingStill = true;
            StartCoroutine(LookAroundWhileStanding());
        }
    }

    private IEnumerator LookAroundWhileStanding()
    {
        agent.isStopped = true;
        float elapsedTime = 0f;

        while (elapsedTime < standStillLookAroundTime)
        {
            // Rotate in place to simulate looking around
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isStandingStill = false;  // After looking around, go back to normal behavior
    }

    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        float lookTime = patrolWaitTime;
        while (lookTime > 0f)
        {
            // Rotate to simulate looking around
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isWaiting = false;
        walkPointSet = false;
    }

    // Ensures excision only happens once per enemy
    public void Excise()
    {
        if (isExcised) return;  // Prevent multiple excise calls

        Debug.Log("Enemy excised!");
        isExcised = true;

        // Delayed destroy to allow key drop to occur properly
        Destroy(gameObject, 0.1f);  // Slight delay to ensure key dropping works
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 rayDirection = transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin, rayDirection * raycastDistance);
    }
}
