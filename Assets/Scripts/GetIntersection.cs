using System.Collections.Generic;
using UnityEngine;

public class GetIntersection
{
    const float ParallelEpsilon = 0.00001f;
    const float DuplicateEpsilon = 0.0001f;

    readonly List<Vector2> intersections = new();

    public IReadOnlyList<Vector2> Points => intersections;

    public List<Vector2> GetLineLineIntersection(LineData line1, LineData line2)
    {
        List<Vector2> results = new();
        Vector2 p = line1.start;
        Vector2 r = line1.end - line1.start;
        Vector2 q = line2.start;
        Vector2 s = line2.end - line2.start;

        float cross = Cross(r, s);
        if (Mathf.Abs(cross) < ParallelEpsilon) return results;

        Vector2 qp = q - p;
        float t = Cross(qp, s) / cross;
        float u = Cross(qp, r) / cross;

        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            results.Add(p + t * r);
        }

        return results;
    }

    public List<Vector2> GetLineCircleIntersections(LineData line, CircleData circle)
    {
        List<Vector2> results = new();
        Vector2 direction = line.end - line.start;
        Vector2 offset = line.start - circle.center;

        float a = Vector2.Dot(direction, direction);
        if (a < ParallelEpsilon * ParallelEpsilon) return results;

        float b = 2f * Vector2.Dot(offset, direction);
        float c = Vector2.Dot(offset, offset) - circle.radius * circle.radius;
        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return results;

        float sqrt = Mathf.Sqrt(Mathf.Max(0f, discriminant));
        float t1 = (-b - sqrt) / (2f * a);
        float t2 = (-b + sqrt) / (2f * a);

        if (t1 >= 0f && t1 <= 1f)
        {
            results.Add(line.start + t1 * direction);
        }

        if (t2 >= 0f && t2 <= 1f && !Approximately(t1, t2))
        {
            results.Add(line.start + t2 * direction);
        }

        return results;
    }

    public List<Vector2> GetCircleCircleIntersections(CircleData circle1, CircleData circle2)
    {
        List<Vector2> results = new();
        Vector2 diff = circle2.center - circle1.center;
        float distance = diff.magnitude;

        if (distance < ParallelEpsilon) return results;
        if (distance > circle1.radius + circle2.radius) return results;
        if (distance < Mathf.Abs(circle1.radius - circle2.radius)) return results;

        float a = (circle1.radius * circle1.radius - circle2.radius * circle2.radius + distance * distance)
            / (2f * distance);
        float hSquared = Mathf.Max(0f, circle1.radius * circle1.radius - a * a);
        float h = Mathf.Sqrt(hSquared);

        Vector2 direction = diff / distance;
        Vector2 middle = circle1.center + direction * a;
        Vector2 perpendicular = new(-direction.y, direction.x);
        Vector2 point1 = middle + perpendicular * h;
        Vector2 point2 = middle - perpendicular * h;

        results.Add(point1);
        if (!Approximately(point1, point2)) results.Add(point2);
        return results;
    }

    /// <summary>Adds a point unless an approximately equal intersection is already stored.</summary>
    public bool AddIntersection(Vector2 point)
    {
        foreach (Vector2 existing in intersections)
        {
            if (Approximately(existing, point)) return false;
        }

        intersections.Add(point);
        return true;
    }

    static bool Approximately(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b) < DuplicateEpsilon;
    }

    static bool Approximately(float a, float b)
    {
        return Mathf.Abs(a - b) < DuplicateEpsilon;
    }

    static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
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
