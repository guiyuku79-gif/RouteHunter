using System.Collections.Generic;
using UnityEngine;

public class DrawingView : MonoBehaviour
{
    private readonly Dictionary<int, LineRenderer> renderers = new();
    private readonly Dictionary<int, GameObject> circleCenters = new();

    public void AddLine(int id, LineRenderer line)
    {
        renderers.Add(id, line);
    }

    public void AddCircle(int id, LineRenderer circle, GameObject center)
    {
        renderers.Add(id, circle);
        circleCenters.Add(id, center);
    }

    public void RemoveDrawing(int id)
    {
        LineRenderer renderer = renderers[id];
        Destroy(renderer.gameObject);
        renderers.Remove(id);

        if (circleCenters.TryGetValue(id, out GameObject center))
        {
            Destroy(center);
            circleCenters.Remove(id);
        }
    }

    public void Clear()
    {
        foreach (LineRenderer renderer in renderers.Values)
        {
            Destroy(renderer.gameObject);
        }

        foreach (GameObject center in circleCenters.Values)
        {
            Destroy(center);
        }

        renderers.Clear();
        circleCenters.Clear();
    }
}