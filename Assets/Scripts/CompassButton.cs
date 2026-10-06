using UnityEngine;

public class CompassButton : MonoBehaviour
{
    [SerializeField] DrawingManager drawingManager;

    void OnMouseDown()
    {
        drawingManager.CompassMode();
    }
}
