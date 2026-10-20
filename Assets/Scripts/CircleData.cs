using UnityEngine;

public class CircleData
{
    public Vector2 Center { get; }
    public float Radius { get; }

    public CircleData(Vector2 center, float radius)
    {
        this.Center = center;
        this.Radius = radius;
    }
}
