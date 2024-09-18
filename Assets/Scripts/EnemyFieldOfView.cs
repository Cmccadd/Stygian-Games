using UnityEngine;

public class EnemyFieldOfView : MonoBehaviour
{
    [Header("Field of View Settings")]
    public float viewRadius = 10f;  // Max distance the enemy can see
    [Range(0, 360)]
    public float viewAngle = 90f;  // Angle of the cone for FOV

    public LayerMask targetMask;  // Layer of objects that can be detected (e.g., the player)
    public LayerMask obstacleMask;  // Layer of objects that can block vision (e.g., walls)

    public bool playerInSight;  // Is the player within the enemy's field of vision?

    private void Update()
    {
        playerInSight = IsPlayerInFieldOfView();
    }

    // Check if the player is within the enemy's field of vision
    private bool IsPlayerInFieldOfView()
    {
        // Find all objects tagged as "Player" within the view radius
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (var playerCollider in playersInRange)
        {
            if (playerCollider.CompareTag("Player"))
            {
                Transform playerTransform = playerCollider.transform;

                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

                // Only consider the X and Z axes for field of view, ignore Y (height)
                directionToPlayer.y = 0;

                // Check if the player is within the view angle
                float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleBetweenEnemyAndPlayer < viewAngle / 2f)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                    // Perform a raycast to check for obstacles between the enemy and the player
                    if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
                    {
                        // Player is within field of view and no obstacles block the vision
                        return true;
                    }
                }
            }
        }

        // No player is within the field of view
        return false;
    }

    // Visualize the field of view in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);  // Draw view radius in the scene view

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        // Draw the view angle (cone)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);

        if (playerInSight)
        {
            Gizmos.color = Color.green;  // If the player is detected, draw a green line to the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                Gizmos.DrawLine(transform.position, player.transform.position);
            }
        }
    }

    // Helper method to get the direction from a specific angle
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
