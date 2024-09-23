using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    private PlayerController playerController;

    [Header("Patrolling Settings")]
    public Transform[] patrolPoints;
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
    public float sightRange = 10f;
    public float coneAngle = 45f;  // Cone angle for field of view detection
    public bool playerInSightRange;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 20f;  // Extended distance for cone detection
    public float raycastHeightOffset = 1.5f;
    private bool playerInRaycast;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        InitializeEnemy();
    }

    private void Update()
    {
        // Check if the player is in sight range and not hidden
        playerInSightRange = CheckConeForPlayer() && !playerController.isHidden;

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
        // Check if the player is within the sight range or raycast detection
        if (playerInSightRange && !playerController.isHidden)
        {
            ChasePlayer();
        }
        else
        {
            Patroling();
        }
    }

    // Check for player within a cone-shaped field of view
    private bool CheckConeForPlayer()
    {
        Vector3 origin = transform.position + new Vector3(0, raycastHeightOffset, 0);

        // OverlapSphere to get objects in the area
        Collider[] objectsInRange = Physics.OverlapSphere(origin, raycastDistance, whatIsPlayer);

        foreach (Collider col in objectsInRange)
        {
            if (col.CompareTag("Player"))
            {
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = (col.transform.position - origin).normalized;
                // Get the angle between the enemy's forward direction and the direction to the player
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                // Check if the player is within the cone's angle
                if (angle < coneAngle / 2f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(origin, directionToPlayer, out hit, raycastDistance))
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            Debug.DrawRay(origin, directionToPlayer * raycastDistance, Color.green);  // Visualize successful detection
                            return true;  // Player detected within the cone
                        }
                    }
                }
            }
        }

        // Visualize failure to detect player
        Debug.DrawRay(origin, transform.forward * raycastDistance, Color.red);
        return false;  // Player not detected
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
        if (player != null)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            Debug.Log("Chasing player...");
        }
    }

    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        float lookTime = patrolWaitTime;
        while (lookTime > 0f)
        {
            transform.Rotate(0, 120 * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        isWaiting = false;
        walkPointSet = false;
    }

    public void Excise()
    {
        Debug.Log("Enemy excised!");
        Destroy(gameObject);  // Destroy the enemy when excised
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the cone of view and detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);  // Normal sight range

        // Draw the cone field of view as an arc
        Vector3 coneStartDirection = Quaternion.Euler(0, -coneAngle / 2f, 0) * transform.forward;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + new Vector3(0, raycastHeightOffset, 0), coneStartDirection * raycastDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + new Vector3(0, raycastHeightOffset, 0), transform.forward * raycastDistance);
    }
}
