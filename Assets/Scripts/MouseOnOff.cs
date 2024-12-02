using UnityEngine;

public class MouseOnOff : MonoBehaviour
{
    [SerializeField] private bool _mouseOn;
    private void Awake()
    {
        if (_mouseOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } 
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

}