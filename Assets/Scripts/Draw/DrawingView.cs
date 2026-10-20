using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using UnityEngine;

public class DrawingView : MonoBehaviour
{
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] LineRenderer circlePrefab;
    [SerializeField] GameObject circleCenterPrefab;
    [SerializeField] Color32 lineColor;
    [SerializeField] float lineWidth = 0.05f;
    private readonly Dictionary<int, LineRenderer> renderers = new();
    private readonly Dictionary<int, GameObject> circleCenters = new();

    //描き途中の表示
    LineRenderer preview;
    GameObject previewCenter;

    public void BeginLine(Vector2 start)
    {
        CancelPreview();

        preview = Instantiate(linePrefab);
        ConfigureRenderer(preview);
        preview.positionCount = 2;

        UpdateLine(start,start);
    }

    public void BeginCircle(Vector2 center)
    {
        CancelPreview();

        preview = Instantiate(circlePrefab);
        ConfigureRenderer(preview);
        preview.positionCount = Constants.CircleSegments + 1;

        previewCenter = Instantiate(circleCenterPrefab);
        previewCenter.transform.position = center;

        UpdateCircle(center,0f);
    }

    public void UpdateLine(Vector2 start, Vector2 end)
    {
        preview.SetPosition(0, start);
        preview.SetPosition(1, end);
    }

    public void UpdateCircle(Vector2 center, float radius)
    {
        for (int i = 0; i <= Constants.CircleSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / Constants.CircleSegments;

            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            preview.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    public void CommitPreview(int id)
    {
        renderers.Add(id,preview);

        if(previewCenter != null)
        {
            circleCenters.Add(id,previewCenter);
        }

        preview = null;
        previewCenter = null;
    }

    public void CancelPreview()
    {
        if (preview != null)
        {
            Destroy(preview.gameObject);
            preview = null;
        }
        if (previewCenter != null)
        {
            Destroy(previewCenter);
            previewCenter = null;
        }
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
        CancelPreview();

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



    void ConfigureRenderer(LineRenderer renderer)
    {
        renderer.useWorldSpace = true;
        renderer.startColor = lineColor;
        renderer.endColor = lineColor;
        renderer.startWidth = lineWidth;
        renderer.endWidth = lineWidth;
    }
}