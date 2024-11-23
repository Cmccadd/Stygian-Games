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
    [SerializeField] private bool canLook;
    public float patrolWaitTime = 2f;
    [SerializeField] private bool isWaiting;

    [Header("Speed Settings")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float maxVelocity = 5f;
    public float acceleration = 5f;

    [Header("Chasing Settings")]
    public float chaseDuration = 5f;
    private float chaseTimer = 0f;

    [Header("Detection Settings")]
    public float sightRange = 30f;
    public float attackRange = 2f;
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
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Animator _enemyNoticeAnimator;
    [SerializeField] private AudioClip _enemyDie;
    [SerializeField] private AudioClip _enemyRoar;
    [SerializeField] private AudioSource _myAudioSource;

    private bool isExcised = false;
    private bool roared;
    private bool isAttacking = false;
    [SerializeField] private GameObject _key;
    [SerializeField] private GameObject _deathAnim;
    [SerializeField] private GameObject _enemyNoticeObject;
    private bool isCurrentlyIdle = false;
    private bool isCurrentlyWalking = false;


    private bool chasing;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        InitializeEnemy();
    }

    public void EnemyRoars()
    {
        _myAudioSource.PlayOneShot(_enemyRoar);
    }

    private void Update()
    {
        if (chasing)
        {
            agent.SetDestination(player.position);
        }

        if (playerController.isHidden)
        {
            playerController.CanHide();
            chasing = false;
            chaseTimer = 0f;
            _enemyNoticeAnimator.SetBool("Noticed", false);
            Patroling();
            StopAttack();
            return;
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && !playerController.isHidden;
        playerInRaycast = CheckConeForPlayer();

        if (playerInSightRange || playerInRaycast)
        {
            chaseTimer = chaseDuration;
            _enemyNoticeAnimator.SetBool("Noticed", true);
            if (!roared)
            {
                roared = true;
                _myAudioSource.PlayOneShot(_enemyRoar);
            }
        }
        else
        {
            _enemyNoticeAnimator.SetBool("Noticed", false);
        }

        if (chaseTimer > 0) chaseTimer -= Time.deltaTime;

        if (!isWaiting) agent.isStopped = false;

        LimitVelocity();
        UpdateState();

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            StartAttack();
        }
        else
        {
            StopAttack();
        }

        // Handle animation transitions
        float currentSpeed = agent.velocity.magnitude;
        if (currentSpeed > 0.1f)
        {
            if (!isCurrentlyWalking)
            {
                SetForwardAnimation();
                isCurrentlyWalking = true;
                isCurrentlyIdle = false;
            }
        }
        else
        {
            if (!isCurrentlyIdle)
            {
                SetIdleAnimation();
                isCurrentlyIdle = true;
                isCurrentlyWalking = false;
            }
        }
    }

    private void InitializeEnemy()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = 0.5f;

        patrolIndex = 0;
        walkPointSet = true;

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
        }
        else
        {
            Patroling();
            playerController.CanHide();
            chasing = false;
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
        }

        // Check if agent is moving to the patrol point
        if (walkPointSet && agent.remainingDistance > agent.stoppingDistance)
        {
            if (!isCurrentlyWalking)
            {
                SetForwardAnimation();
                isCurrentlyWalking = true;
                isCurrentlyIdle = false;
            }
        }
        // Check if agent has arrived at the patrol point
        else if (walkPointSet && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isCurrentlyIdle)
            {
                SetIdleAnimation();
                isCurrentlyIdle = true;
                isCurrentlyWalking = false;

                // Handle patrol wait/look around behavior
                if (canLook)
                {
                    canLook = false;
                    walkPointSet = false;
                    StartCoroutine(LookAroundAtPatrolPoint());
                }
            }
        }
    }

    private void SetNextPatrolPoint()
    {
        ResetAllAnimations();
        agent.SetDestination(patrolPoints[patrolIndex].position);
        walkPointSet = true;
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

        SetForwardAnimation(); // Start walking to the next point
        isCurrentlyWalking = true;
        isCurrentlyIdle = false;
    }

    private void ChasePlayer()
    {
        ResetAllAnimations();
        playerController.CantHide();
        chasing = true;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        SetForwardAnimation();
    }

    private IEnumerator LookAroundAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;


        float lookTime = patrolWaitTime;
        float rotationSpeed = 30f; // Slower rotation speed for looking around

        while (lookTime > 0f)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            lookTime -= Time.deltaTime;
            yield return null;
        }

        isWaiting = false;
        agent.isStopped = false;
        walkPointSet = false;

        yield return new WaitForSeconds(2);
        canLook = true;
    }

    private void ResetAllAnimations()
    {
        enemyAnimator.SetBool("Forward", false);
        enemyAnimator.SetBool("isAttacking", false);
        enemyAnimator.SetBool("Idle", false);
    }

private void SetIdleAnimation()
{
    ResetAllAnimations();
    enemyAnimator.SetBool("Idle", true);
    Debug.Log("Idle animation triggered at patrol point");
}

private void SetForwardAnimation()
{
    ResetAllAnimations();
    enemyAnimator.SetBool("Forward", true);
    Debug.Log("Forward animation triggered while moving");
}

    private void StartAttack()
    {
        if (!isAttacking)
        {
            ResetAllAnimations();
            isAttacking = true;
            agent.isStopped = true;
            enemyAnimator.SetBool("isAttacking", true);
        }
    }

    private void StopAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            agent.isStopped = false;
            ResetAllAnimations();
            if (chasing)
            {
                SetForwardAnimation();
            }
        }
    }

    public void Excise()
    {
        if (isExcised) return;

        isExcised = true;
        _enemyNoticeObject.SetActive(false);
        _deathAnim.SetActive(true);

        if (enemyHitbox != null)
        {
            enemyHitbox.enabled = false;
        }

        DropKey();
        chaseSpeed = 0;
        patrolSpeed = 0;
        ResetAllAnimations();
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

    private void LimitVelocity()
    {
        if (agent.velocity.magnitude > maxVelocity)
        {
            agent.velocity = agent.velocity.normalized * maxVelocity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.yellow;

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
