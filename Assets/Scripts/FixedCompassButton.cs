using UnityEngine;

public class FixedCompassButton : MonoBehaviour
{
    [SerializeField] DrawingManager drawingManager;

    void OnMouseDown()
    {
        drawingManager.FixedCompassMode();
    }
}
