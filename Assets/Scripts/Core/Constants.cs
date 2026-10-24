using UnityEngine;
public static class Constants
{
    public const float BoxMoveDistance = 18f;
    public const float BoxMoveDuration = 1.2f;

    public const int CircleSegments = 100;

    public static readonly Vector2 GridStartPos = new Vector2(-6f, -4.5f);
    public static readonly Vector2Int GridNumber = new Vector2Int(12, 6);
    public const float GridSize = 1f;

    public const float FirstBoxX = -6f;
    public const float BoxSpacing = 4f;
    public const float BoxY = 4.2f;

    public enum Mode
    {
        Line,
        Compass,
        FixedCompass
    }
}