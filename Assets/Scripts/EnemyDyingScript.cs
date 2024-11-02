using UnityEngine;

public class EnemyDyingScript : MonoBehaviour
{
public EnemyAI EnemyAI;

    public void Die()
    {
        EnemyAI.Die();
    }

    public void DeathSound()
    {
        EnemyAI.DeathSound();
    }
}
