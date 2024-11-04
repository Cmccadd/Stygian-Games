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
    public float maxVelocity = 5f;
    public float acceleration = 5f;

    [Header("Chasing Settings")]
    public float chaseDuration = 5f;
    private float chaseTimer = 0f;

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
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Animator _enemyNoticeAnimator;
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
            _enemyNoticeAnimator.SetBool("Noticed", false);
            Patroling();
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

        // Always reset movement to active in the Update loop if not waiting
        if (!isWaiting) agent.isStopped = false;

        LimitVelocity();

        UpdateAnimationDirectionAndTurning();

        UpdateState();
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
        else if (!isWaiting)
        {
            ResetAllAnimations();
        }

        if (walkPointSet && agent.remainingDistance < agent.stoppingDistance && !isWaiting)
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

        isWaiting = false;
        agent.isStopped = false;
        walkPointSet = false;
    }

    private void UpdateAnimationDirectionAndTurning()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

        ResetAllAnimations();

        if (angle > -22.5f && angle <= 22.5f)
            enemyAnimator.SetBool("Forward", true);
        else if (angle > 22.5f && angle <= 67.5f)
            enemyAnimator.SetBool("ForwardRight", true);
        else if (angle > 67.5f && angle <= 112.5f)
            enemyAnimator.SetBool("Right", true);
        else if (angle > 112.5f && angle <= 157.5f)
            enemyAnimator.SetBool("BackwardRight", true);
        else if ((angle > 157.5f && angle <= 180f) || (angle <= -157.5f && angle >= -180f))
            enemyAnimator.SetBool("Backward", true);
        else if (angle > -157.5f && angle <= -112.5f)
            enemyAnimator.SetBool("BackwardLeft", true);
        else if (angle > -112.5f && angle <= -67.5f)
            enemyAnimator.SetBool("Left", true);
        else if (angle > -67.5f && angle <= -22.5f)
            enemyAnimator.SetBool("ForwardLeft", true);
    }

    private void ResetAllAnimations()
    {
        enemyAnimator.SetBool("Forward", false);
        enemyAnimator.SetBool("ForwardRight", false);
        enemyAnimator.SetBool("Right", false);
        enemyAnimator.SetBool("BackwardRight", false);
        enemyAnimator.SetBool("Backward", false);
        enemyAnimator.SetBool("BackwardLeft", false);
        enemyAnimator.SetBool("Left", false);
        enemyAnimator.SetBool("ForwardLeft", false);
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
