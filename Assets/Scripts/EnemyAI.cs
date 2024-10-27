using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer, obstacleMask;
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
    public float sightRange = 30f; // Adjust this value to cover the whole main room
    public float coneAngle = 60f; // Angle of the vision cone
    public int coneResolution = 20; // Number of rays to cast in the cone
    public bool playerInSightRange;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 50f; // Make this long enough to reach across the entire room
    public float raycastHeightOffset = 1.5f;
    private bool playerInRaycast;

    [Header("Key Drop Settings")]
    public GameObject keyPrefab;
    public Transform dropPosition;

    [Header("Hitbox Settings")]
    [SerializeField] private Collider enemyHitbox; // Reference to the enemy's hitbox (collider)

    private bool isExcised = false;
    [SerializeField] private AudioClip _enemyDie;
    [SerializeField] private AudioClip _enemyRoar;
    [SerializeField] private AudioSource _myAudioSource;
    private bool roared;
    [SerializeField] private Animator _enemyNoticeAnimator;
    [SerializeField] private GameObject _key;
    [SerializeField] private GameObject _deathAnim;
    [SerializeField] private GameObject _enemyNoticeObject;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();

        InitializeEnemy();
    }

    private void Update()
    {
        // Check if the player is hidden
        if (playerController.isHidden)
        {
            Patroling();
            return;
        }

        // Check if the player is in sight range and not hidden
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && !playerController.isHidden;

        // Perform raycast cone check to see if the player is visible within the cone of vision
        playerInRaycast = CheckConeForPlayer();

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
        if ((playerInSightRange || playerInRaycast) && !playerController.isHidden)
        {
            ChasePlayer();
            _enemyNoticeAnimator.SetBool("Noticed", true);
            _enemyNoticeAnimator.SetBool("Unnoticed", false);
        }
        else
        {
            Patroling();
            roared = false;
            _enemyNoticeAnimator.SetBool("Noticed", false);
            _enemyNoticeAnimator.SetBool("Unnoticed", true);
        }
    }

    private bool CheckConeForPlayer()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 forwardDirection = transform.forward;

        // Iterate through the cone using the coneResolution to cast multiple rays
        for (int i = 0; i <= coneResolution; i++)
        {
            // Calculate the angle for this ray based on the cone
            float angle = -coneAngle / 2 + (coneAngle / coneResolution) * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = rotation * forwardDirection;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, whatIsPlayer | obstacleMask))
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
        }

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
        if (player != null)
        {
            if (!roared)
            {
                roared = true;
                _myAudioSource.PlayOneShot(_enemyRoar);
            }

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
        if (isExcised) return;

        Debug.Log("Enemy excised!");
        isExcised = true;
        _enemyNoticeObject.SetActive(false);
        _deathAnim.SetActive(true);

        // Disable the hitbox to prevent damage to the player
        if (enemyHitbox != null)
        {
            enemyHitbox.enabled = false;
        }

        DropKey();
        chaseSpeed = 0;
        patrolSpeed = 0;
    }

    public void DeathSound()
    {
        _myAudioSource.PlayOneShot(_enemyDie);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void DropKey()
    {
        _key.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 forwardDirection = transform.forward;

        // Draw the cone for visual representation
        for (int i = 0; i <= coneResolution; i++)
        {
            float angle = -coneAngle / 2 + (coneAngle / coneResolution) * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = rotation * forwardDirection;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(rayOrigin, rayDirection * raycastDistance);
        }
    }
}
