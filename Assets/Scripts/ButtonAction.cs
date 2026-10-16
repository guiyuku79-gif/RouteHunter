using UnityEngine;
using UnityEngine.Events;

public class ButtonAction : MonoBehaviour
{
    [SerializeField] UnityEvent onClick;

    void OnMouseDown()
    {
        onClick?.Invoke();
    }
}
