using UnityEngine;

public class TurnOnOff : MonoBehaviour
{
    [SerializeField] private GameObject _object;

    public void TurnOn()
    {
        _object.SetActive(true);
    }
    
    public void TurnOff()
    {
        _object.SetActive(false);
    }
}
