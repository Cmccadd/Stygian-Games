using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIPriest : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;
    private PlayerController playerController;

    [Header("Idle/Look Around Settings")]
    public float lookAroundTime = 5f; // Time to look around when idle
    private bool isLookingAround = true;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int patrolIndex;
    private bool walkPointSet;

    [Header("Speed Settings")]
    public float idleLookSpeed = 1f;  // Speed of rotation during idle looking around
    public float chaseSpeed = 6f;     // Speed while chasing the player
    public float patrolSpeed = 3.5f;  // Speed during patrolling

    [Header("Detection Settings")]
    public float sightRange = 10f;    // Detection range
    public float fieldOfViewAngle = 90f;  // Field of view for the priest (vision cone)
    private bool playerInSightRange;

    [Header("Lost Player Settings")]
    public float timeToLosePlayer = 5f; // Time after which the priest loses interest if it can't find the player
    private float timeSinceLastSeenPlayer;
    private bool isWaiting;
    private float patrolWaitTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        patrolIndex = 0;
        walkPointSet = false;
        agent.speed = patrolSpeed;

        // Start the priest in the looking around state
        StartCoroutine(LookAround());
    }

    private void Update()
    {
        // Check if player is within sight range
        playerInSightRange = IsPlayerInSightRange();

        // If player is seen, start chasing
        if (playerInSightRange)
        {
            ChasePlayer();
        }
        // If the player is lost for a certain amount of time, go back to patrolling
        else if (timeSinceLastSeenPlayer >= timeToLosePlayer)
        {
            Patroling();
        }
        else
        {
            // Increment the timer for losing the player
            timeSinceLastSeenPlayer += Time.deltaTime;
        }
    }

    // Coroutine to handle idle looking around
    private IEnumerator LookAround()
    {
        while (isLookingAround)
        {
            // Rotate to simulate looking around
            transform.Rotate(0, idleLookSpeed * Time.deltaTime, 0);
            yield return null;

            // Exit idle if player is detected
            if (playerInSightRange)
            {
                isLookingAround = false;
            }
        }
    }

    // Check if the player is within the field of view and in sight range
    private bool IsPlayerInSightRange()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleBetweenPriestAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if within sight range and field of view angle
        if (angleBetweenPriestAndPlayer < fieldOfViewAngle / 2 && Vector3.Distance(transform.position, player.position) < sightRange)
        {
            // Check if there are no obstacles blocking the view
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightRange, whatIsPlayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    timeSinceLastSeenPlayer = 0f; // Reset player lost timer
                    Debug.DrawRay(transform.position, directionToPlayer * sightRange, Color.green);
                    return true;
                }
            }
        }

        Debug.DrawRay(transform.position, directionToPlayer * sightRange, Color.red);
        return false;
    }

    // Chase the player when detected
    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        Debug.Log("Chasing player...");
    }

    // Patrol between set points
    private void Patroling()
    {
        agent.speed = patrolSpeed;

        if (!walkPointSet)
        {
            SetNextPatrolPoint();
        }

        if (walkPointSet && agent.remainingDistance < 1f)
        {
            walkPointSet = false;
            StartCoroutine(LookAroundAtPatrolPoint());
        }
    }

    private void SetNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            walkPointSet = true;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    // Coroutine to simulate looking around at patrol points
    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        float lookTime = patrolWaitTime;
        while (lookTime > 0f)
        {
            transform.Rotate(0, idleLookSpeed * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isWaiting = false;
        walkPointSet = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the sight range and field of view for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 forward = transform.forward * sightRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
