using System.Collections.Generic;
using UnityEngine;

public class SnapPointStore
{
    const float DuplicateEpsilon = 0.0001f;
    const float SnapRadius = 0.2f;

    private readonly List<(int createdById, Vector2 position)> points = new();

    public bool Add(int createdById, Vector2 position)
    {
        foreach (var existing in points)
        {
            if (Vector2.Distance(existing.position, position) < DuplicateEpsilon)
            {
                return false;
            }
        }

        points.Add((createdById, position));
        return true;
    }

    public void RemoveCreatedBy(int drawingId)
    {
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (points[i].createdById == drawingId)
            {
                points.RemoveAt(i);
            }
        }
    }

    public Vector2 Snap(Vector2 position)
    {
        Vector2 result = position;
        float nearestDistance = float.MaxValue;

        foreach (var point in points)
        {
            float distance = Vector2.Distance(position, point.position);

            if (distance <= SnapRadius && distance <= nearestDistance)
            {
                result = point.position;
                nearestDistance = distance;
            }
        }

        return result;
    }

    public void Clear()
    {
        points.Clear();
    }
}