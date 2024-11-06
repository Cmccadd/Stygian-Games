using UnityEngine;

public class EnemyKill : MonoBehaviour
{
    [SerializeField] private AudioSource _myAudioSource;
    [SerializeField] private AudioClip _enemyRoar;
    [SerializeField] private AudioClip _playerDeath;

    // Start is called before the first frame update
    public void PlayAttackSound()
    {
        _myAudioSource.PlayOneShot(_enemyRoar);
    }

    public void PlayDeathSound()
    {
        _myAudioSource.PlayOneShot(_playerDeath);
    }
}
