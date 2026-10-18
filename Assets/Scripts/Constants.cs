using UnityEngine;
public static class Constants
{
    public const float BoxMoveDistance = 18f;
    public const float BoxMoveDuration = 1.5f;

    public const int CircleSegments = 100;


    public enum Mode
    {
        Line,
        Compass,
        FixedCompass
    }
}