using UnityEngine;

public class MakeFireBlue : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    void Awake()
    {
        _animator.SetBool("Blue", true);
    }

}
