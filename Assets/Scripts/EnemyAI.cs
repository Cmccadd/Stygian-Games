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
    public float chaseDuration = 5f;  // Duration to keep chasing after losing sight
    private float chaseTimer = 0f;    // Timer to track chase duration

    [Header("Detection Settings")]
    public float sightRange = 30f;
    public float coneAngle = 60f;
    public int coneResolution = 20;
    public bool playerInSightRange;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 50f;
    public float raycastHeightOffset = 1.5f;
    private bool playerInRaycast;

    [Header("Key Drop Settings")]
    public GameObject keyPrefab;
    public Transform dropPosition;

    [Header("Hitbox Settings")]
    [SerializeField] private Collider enemyHitbox;

    [Header("Animation and Audio")]
    [SerializeField] private Animator enemyAnimator;  // Reference to the Animator
    [SerializeField] private AudioClip _enemyDie;
    [SerializeField] private AudioClip _enemyRoar;
    [SerializeField] private AudioSource _myAudioSource;

    private bool isExcised = false;
    private bool roared;
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
        if (playerController.isHidden)
        {
            chaseTimer = 0f;
            Patroling();
            return;
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && !playerController.isHidden;
        playerInRaycast = CheckConeForPlayer();

        if (playerInSightRange || playerInRaycast)
        {
            chaseTimer = chaseDuration;
            if (!roared)
            {
                roared = true;
                _myAudioSource.PlayOneShot(_enemyRoar);
            }
        }

        UpdateState();
        if (chaseTimer > 0) chaseTimer -= Time.deltaTime;
    }

    private void InitializeEnemy()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolIndex = 0;
        walkPointSet = true;
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
        if (chaseTimer > 0)
        {
            ChasePlayer();
            enemyAnimator.SetBool("Walking", true); // Start walking animation
            enemyAnimator.SetBool("Idle", false); // Disable idle animation during chase
        }
        else
        {
            Patroling();
            roared = false;
        }
    }

    private bool CheckConeForPlayer()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, raycastHeightOffset, 0);
        Vector3 forwardDirection = transform.forward;

        for (int i = 0; i <= coneResolution; i++)
        {
            float angle = -coneAngle / 2 + (coneAngle / coneResolution) * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 rayDirection = rotation * forwardDirection;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, whatIsPlayer | obstacleMask))
            {
                if (hit.transform.CompareTag("Player") && !playerController.isHidden)
                {
                    Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.green);
                    return true;
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
            enemyAnimator.SetBool("Walking", true); // Enable walking animation during patrol
            enemyAnimator.SetBool("Idle", false); // Disable idle animation
        }
        else if (!isWaiting)
        {
            enemyAnimator.SetBool("Walking", false); // Stop walking
            enemyAnimator.SetBool("Idle", true); // Enable idle animation
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
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        Debug.Log("Chasing player...");
    }

    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;
        enemyAnimator.SetBool("Walking", false); // Stop walking animation
        enemyAnimator.SetBool("Idle", true); // Enable idle animation during look-around

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
        enemyAnimator.SetBool("Idle", false); // Disable idle animation after look-around
        enemyAnimator.SetBool("Walking", true); // Resume walking animation
    }

    private void AttackPlayer()
    {
        enemyAnimator.SetTrigger("Attack"); // Play attack animation
        Debug.Log("Attacking player...");
    }

    public void Excise()
    {
        if (isExcised) return;

        Debug.Log("Enemy excised!");
        isExcised = true;
        _enemyNoticeObject.SetActive(false);
        _deathAnim.SetActive(true);

        if (enemyHitbox != null)
        {
            this.enemyHitbox.enabled = false;
        }

        DropKey();
        chaseSpeed = 0;
        patrolSpeed = 0;
        enemyAnimator.SetBool("Walking", false); // Stop walking animation on excision
        enemyAnimator.SetBool("Idle", true); // Switch to idle animation on excision
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
