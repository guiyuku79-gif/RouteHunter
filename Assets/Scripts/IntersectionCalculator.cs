using System.Collections.Generic;
using UnityEngine;

public static class IntersectionCalculator
{
    const float ParallelEpsilon = 0.00001f;
    const float DuplicateEpsilon = 0.0001f;

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

    static public List<Vector2> GetLineLineIntersection(LineData line1, LineData line2)
    {
        List<Vector2> results = new();
        Vector2 p = line1.Start;
        Vector2 r = line1.End - line1.Start;
        Vector2 q = line2.Start;
        Vector2 s = line2.End - line2.Start;

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

    static public List<Vector2> GetLineCircleIntersections(LineData line, CircleData circle)
    {
        List<Vector2> results = new();
        Vector2 direction = line.End - line.Start;
        Vector2 offset = line.Start - circle.Center;

        float a = Vector2.Dot(direction, direction);
        if (a < ParallelEpsilon * ParallelEpsilon) return results;

        float b = 2f * Vector2.Dot(offset, direction);
        float c = Vector2.Dot(offset, offset) - circle.Radius * circle.Radius;
        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) return results;

        float sqrt = Mathf.Sqrt(Mathf.Max(0f, discriminant));
        float t1 = (-b - sqrt) / (2f * a);
        float t2 = (-b + sqrt) / (2f * a);

        if (t1 >= 0f && t1 <= 1f)
        {
            results.Add(line.Start + t1 * direction);
        }

        if (t2 >= 0f && t2 <= 1f && !Approximately(t1, t2))
        {
            results.Add(line.Start + t2 * direction);
        }

        return results;
    }

    static public List<Vector2> GetCircleCircleIntersections(CircleData circle1, CircleData circle2)
    {
        List<Vector2> results = new();
        Vector2 diff = circle2.Center - circle1.Center;
        float distance = diff.magnitude;

        if (distance < ParallelEpsilon) return results;
        if (distance > circle1.Radius + circle2.Radius) return results;
        if (distance < Mathf.Abs(circle1.Radius - circle2.Radius)) return results;

        float a = (circle1.Radius * circle1.Radius - circle2.Radius * circle2.Radius + distance * distance)
            / (2f * distance);
        float hSquared = Mathf.Max(0f, circle1.Radius * circle1.Radius - a * a);
        float h = Mathf.Sqrt(hSquared);

        Vector2 direction = diff / distance;
        Vector2 middle = circle1.Center + direction * a;
        Vector2 perpendicular = new(-direction.y, direction.x);
        Vector2 point1 = middle + perpendicular * h;
        Vector2 point2 = middle - perpendicular * h;

        results.Add(point1);
        if (!Approximately(point1, point2)) results.Add(point2);
        return results;
    }

}