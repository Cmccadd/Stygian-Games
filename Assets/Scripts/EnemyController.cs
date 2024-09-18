using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;  // NavMeshAgent for controlling movement
    [SerializeField] private EnemyFieldOfView fieldOfView;  // Reference to the EnemyFieldOfView script
    [SerializeField] private Transform player;  // Reference to the player

    // Patroling variables
    public Transform[] patrolPoints;  // Set patrol points in the inspector as Transform objects
    private int currentPatrolIndex;
    private bool walkPointSet;
    public float patrolWaitTime = 2f;  // Time to wait at each patrol point
    private bool waiting;

    private void Start()
    {
        // Initialize components
        agent = GetComponent<NavMeshAgent>();

        // Ensure EnemyFieldOfView is assigned
        if (fieldOfView == null)
        {
            fieldOfView = GetComponent<EnemyFieldOfView>();
        }

        // Find the player by tag if not directly assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        // Initialize patrol settings
        currentPatrolIndex = 0;
        walkPointSet = true;
    }

    private void Update()
    {
        // Check if the player is within the field of view
        bool playerInSightRange = fieldOfView.playerInSight;

        if (!playerInSightRange)
        {
            if (!waiting) Patroling();  // Patrol if the player is not in sight and enemy isn't waiting
        }
        else
        {
            ChasePlayer();  // Chase the player when detected
        }
    }

    private void Patroling()
    {
        if (!walkPointSet && patrolPoints.Length > 0)
        {
            SetNextPatrolPoint();  // Set the next patrol point
        }

        if (walkPointSet && agent.remainingDistance < 1f && !waiting)  // If agent reached patrol point
        {
            walkPointSet = false;  // Clear the patrol point
            StartCoroutine(LookAround());  // Look around before moving to the next point
        }
    }

    private void SetNextPatrolPoint()
    {
        // Set the destination to the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        walkPointSet = true;

        // Update the patrol index to loop through the points
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);  // Move towards the player's position
        }
    }

    // Coroutine to stop and look around at a patrol point or after losing the player
    private IEnumerator LookAround()
    {
        waiting = true;
        agent.isStopped = true;  // Stop the agent from moving

        float lookTime = patrolWaitTime;  // Time to wait and look around
        while (lookTime > 0f)
        {
            // Optional: Add rotation code if you want the enemy to rotate while looking around
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;  // Resume agent movement
        waiting = false;
    }

    // Visualize patrol points and sight range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfView.viewRadius);

        Gizmos.color = Color.green;
        if (patrolPoints != null)
        {
            foreach (Transform point in patrolPoints)
            {
                Gizmos.DrawSphere(point.position, 0.5f);
            }
        }
    }
}
