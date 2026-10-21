using UnityEngine;

public class LineData
{
    public Vector2 Start { get; }
    public Vector2 End { get; }

    public LineData(Vector2 start, Vector2 end)
    {
        this.Start = start;
        this.End = end;
    }
}