using UnityEngine;

public class LineEffectController : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void Init(LineData lineData, int boxIndex)
    {

    }
}
