using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    // Patrolling
    public Transform[] patrolPoints;  // Set patrol points in the inspector
    private int patrolIndex;
    private bool walkPointSet;
    public float patrolWaitTime = 2f;  // Time to wait at each patrol point
    private bool isWaiting;

    // Chasing
    public float chaseTime = 5f;  // Time to chase the player after losing sight
    private float chaseTimer;

    // States
    public float sightRange;
    public bool playerInSightRange;
    private bool playerInChaseState;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolIndex = 0;
        walkPointSet = true;  // Start with a set patrol point
        playerInChaseState = false;
        chaseTimer = chaseTime;  // Initialize chase timer

        // Ensure we have the player reference
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("Will");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        // Check if the player is in sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange && !playerInChaseState)
        {
            Patroling();  // Patrol if the player is not in sight
        }
        else if (playerInSightRange)
        {
            ChasePlayer();  // Chase the player if detected
        }
        else if (playerInChaseState && !playerInSightRange)
        {
            // If the player was being chased but now lost, look around before returning to patrol
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
    }

    private void Patroling()
    {
        // Ensure the agent is not stopped while patrolling
        agent.isStopped = false;

        if (!walkPointSet && patrolPoints.Length > 0 && !isWaiting)
        {
            SetNextPatrolPoint();  // Set the next patrol point
        }

        // Check if the agent has reached the patrol point
        if (walkPointSet && agent.remainingDistance < 1f && !isWaiting)
        {
            walkPointSet = false;
            StartCoroutine(LookAroundAtPatrolPoint());  // Look around after reaching a patrol point
        }
    }

    private void SetNextPatrolPoint()
    {
        // Set destination to the next patrol point
        agent.SetDestination(patrolPoints[patrolIndex].position);
        walkPointSet = true;

        // Update patrol index to loop through points
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            playerInChaseState = true;
            chaseTimer = chaseTime;  // Reset chase timer
            agent.isStopped = false;  // Ensure the agent is moving
            agent.SetDestination(player.position);  // Move towards the player
        }
    }

    private void LookAround()
    {
        // Stop the agent from moving
        agent.isStopped = true;
        // Optionally rotate the enemy to simulate looking around
        transform.Rotate(0, 120 * Time.deltaTime, 0);
    }

    // Coroutine to look around at patrol points before moving on
    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;  // Stop the agent from moving

        float lookTime = patrolWaitTime;  // Time to wait and look around
        while (lookTime > 0f)
        {
            // Rotate to simulate looking around
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;  // Resume movement after looking around
        isWaiting = false;
        walkPointSet = false;  // Allow setting the next patrol point
    }

    // Visualize patrol points and sight range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);  // Visualize sight range

        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            foreach (Transform point in patrolPoints)
            {
                Gizmos.DrawSphere(point.position, 0.5f);  // Visualize patrol points
            }
        }
    }
}
