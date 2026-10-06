using UnityEngine;

public class LineButton : MonoBehaviour
{
    [SerializeField] DrawingManager drawingManager;

    void OnMouseDown()
    {
        drawingManager.LineMode();
    }
}
