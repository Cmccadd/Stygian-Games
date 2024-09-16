using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent; // NavMeshAgent for controlling movement
    [SerializeField] private Transform player;   // Reference to the player
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer; // LayerMask for detection

    // Patroling variables
    public Transform[] patrolPoints; // Set patrol points in the inspector as Transform objects
    private int currentPatrolIndex;
    private bool walkPointSet;

    // States
    public float sightRange;       // Range for detecting player
    public bool playerInSightRange;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find the player by tag
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentPatrolIndex = 0; // Start at the first patrol point
        walkPointSet = true;    // Ensure the first patrol point is set
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if the player is within sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange) Patroling();
        if (playerInSightRange) ChasePlayer();
    }

    // Patrol between predefined points
    private void Patroling()
    {
        if (!walkPointSet && patrolPoints.Length > 0)
        {
            SetNextPatrolPoint(); // Set the next patrol point
        }

        if (walkPointSet && agent.remainingDistance < 1f) // If agent is near destination
        {
            walkPointSet = false; // Clear walk point, set new one on next update
        }
    }

    // Set the next patrol point from the list
    private void SetNextPatrolPoint()
    {
        // Set agent destination to the position of the next patrol point
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        walkPointSet = true;

        // Loop through patrol points
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    // Chase the player when they are within sight range
    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    // Visualize sight range and patrol points in the editor
    private void OnDrawGizmosSelected()
    {
        // Draw sight range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw patrol points
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
