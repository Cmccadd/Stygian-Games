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
    public float sightRange;
    public bool playerInSightRange;

    [Header("Raycast Detection Settings")]
    public float raycastDistance = 10f;
    public float raycastHeightOffset = 1.5f;
    private bool playerInRaycast;

    [Header("Key Drop Settings")]
    public GameObject keyPrefab;  // Reference to the key prefab
    public Transform dropPosition;  // Optional: Position where the key should drop

    private bool isExcised = false; // Prevent multiple excision calls

    //[SerializeField] private GameObject _enemyWalk;
    [SerializeField] private AudioClip _enemyDie;
    [SerializeField] private AudioClip _enemyRoar;
    [SerializeField] private AudioSource _myAudioSource;
    private bool roared;
    [SerializeField] private Animator _enemyNoticeAnimator;
    [SerializeField] private GameObject _key;

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
            return; // Exit detection logic if the player is hidden
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
        if (player != null)
        {
            if (roared == false)
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

    // Ensures excision only happens once per enemy
    public void Excise()
    {
        if (isExcised) return; // Prevent multiple excise calls

        Debug.Log("Enemy excised!");
        isExcised = true;

        // Drop the key when the enemy is excised
        DropKey();
        _myAudioSource.PlayOneShot(_enemyDie);
        // Delayed destroy to allow key drop to occur properly
        chaseSpeed = 0;
        patrolSpeed = 0;
        Destroy(gameObject, 2f);  // Slight delay to ensure key dropping works
    }

    private void DropKey()
    {
        // Drop the key at the enemy's position (if no specific drop position is set, use enemy's transform)
        _key.SetActive(true);
        //if (keyPrefab != null)
        //{
        //    Vector3 dropPos = (dropPosition != null) ? dropPosition.position : transform.position;  // Use dropPosition if set, otherwise enemy's position
        //    Instantiate(keyPrefab, dropPos, Quaternion.identity);  // Drop the key at the calculated position
        //    Debug.Log("Key dropped.");
        //}
        //else
        //{
        //    Debug.LogWarning("Key prefab is not set.");
        //}
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
