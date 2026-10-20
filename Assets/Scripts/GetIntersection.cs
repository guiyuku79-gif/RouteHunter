using System.Collections.Generic;
using UnityEngine;

public class GetIntersection
{
    const float DuplicateEpsilon = 0.0001f;

    const float SnapRadius = 0.1f;

    private readonly List<(int id, Vector2 point)> intersections = new();


    public bool AddIntersection(int currentIndex, Vector2 point)
    {
        foreach (var existing in intersections)
        {
            if (Approximately(existing.point, point)) return false;
        }

        intersections.Add((currentIndex, point));
        return true;
    }

    public void UndoIntersection(int currentIndex)
    {
        for (int i = intersections.Count - 1; i >= 0; i--)
        {
            if (intersections[i].id == currentIndex)
            {
                intersections.RemoveAt(i);
            }
        }
    }

    static bool Approximately(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b) < DuplicateEpsilon;
    }


    public Vector2 Snap(Vector2 pos)
    {
        Vector2 result = pos;
        float minDis = float.MaxValue;
        foreach (var interSection in intersections)
        {
            float distance = Vector2.Distance(pos, interSection.point);
            if (distance <= SnapRadius && distance <= minDis)
            {
                result = interSection.point;
                minDis = distance;
            }
        }
        return result;
    }

    public void Clear()
    {
        intersections.Clear();
    }
}

public class LineData
{
    public Vector2 start;
    public Vector2 end;

    public LineData(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}

public class CircleData
{
    public Vector2 center;
    public float radius;

    public CircleData(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }
}
