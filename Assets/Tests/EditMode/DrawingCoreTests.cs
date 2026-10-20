using NUnit.Framework;
using UnityEngine;

public class DrawingCoreTests
{
    const float Tolerance = 0.0001f;

    static void AssertPoint(Vector2 actual, Vector2 expected)
    {
        Assert.That(
            Vector2.Distance(actual, expected),
            Is.LessThan(Tolerance)
        );
    }

    [Test]
    public void CrossingSegments_ReturnTheirIntersection()
    {
        var horizontal = new LineData(
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f)
        );

        var vertical = new LineData(
            new Vector2(0f, -1f),
            new Vector2(0f, 1f)
        );

        var points = IntersectionCalculator.GetLineLineIntersection(
            horizontal,
            vertical
        );

        Assert.That(points.Count, Is.EqualTo(1));
        AssertPoint(points[0], Vector2.zero);
    }

    [Test]
    public void IntersectionOutsideSegment_ReturnsNoPoint()
    {
        var horizontal = new LineData(
            new Vector2(0f, 0f),
            new Vector2(1f, 0f)
        );

        var vertical = new LineData(
            new Vector2(2f, -1f),
            new Vector2(2f, 1f)
        );

        var points = IntersectionCalculator.GetLineLineIntersection(
            horizontal,
            vertical
        );

        Assert.That(points.Count, Is.EqualTo(0));
    }

    [Test]
    public void TangentLineAndCircle_ReturnOnePoint()
    {
        var line = new LineData(
            new Vector2(-2f, 1f),
            new Vector2(2f, 1f)
        );

        var circle = new CircleData(Vector2.zero, 1f);

        var points = IntersectionCalculator.GetLineCircleIntersections(
            line,
            circle
        );

        Assert.That(points.Count, Is.EqualTo(1));
        AssertPoint(points[0], new Vector2(0f, 1f));
    }

    [Test]
    public void TouchingCircles_ReturnOnePoint()
    {
        var first = new CircleData(Vector2.zero, 1f);
        var second = new CircleData(new Vector2(2f, 0f), 1f);

        var points = IntersectionCalculator.GetCircleCircleIntersections(
            first,
            second
        );

        Assert.That(points.Count, Is.EqualTo(1));
        AssertPoint(points[0], new Vector2(1f, 0f));
    }

    [Test]
    public void Snap_SelectsNearestPoint_AndIgnoresDistantPoints()
    {
        var store = new SnapPointStore();

        store.Add(1, Vector2.zero);
        store.Add(2, new Vector2(0.08f, 0f));

        // 両方の点が範囲内だが、0.08の方が近い。
        AssertPoint(
            store.Snap(new Vector2(0.07f, 0f)),
            new Vector2(0.08f, 0f)
        );

        // スナップ範囲外なら入力位置を維持する。
        Vector2 farPosition = new Vector2(1f, 1f);
        AssertPoint(store.Snap(farPosition), farPosition);
    }

    [Test]
    public void RemovingLaterDrawing_PreservesEarlierSharedPoint()
    {
        var store = new SnapPointStore();

        Assert.That(store.Add(1, Vector2.zero), Is.True);
        Assert.That(store.Add(2, Vector2.zero), Is.False);

        store.RemoveCreatedBy(2);

        AssertPoint(
            store.Snap(new Vector2(0.05f, 0f)),
            Vector2.zero
        );
    }

    [Test]
    public void Undo_RemovesNewIntersection_AndAllowsRedrawing()
    {
        var data = new DrawingData();

        data.AddLine(
            1,
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f)
        );

        data.AddLine(
            2,
            new Vector2(0f, -1f),
            new Vector2(0f, 1f)
        );

        Vector2 nearby = new Vector2(0.03f, 0.02f);

        AssertPoint(data.Snap(nearby), Vector2.zero);

        data.UndoData(2);

        // 交点が消えたので、スナップせず入力位置を返す。
        AssertPoint(data.Snap(nearby), nearby);

        // 同じIDを使って再び作図できる。
        data.AddLine(
            2,
            new Vector2(0f, -1f),
            new Vector2(0f, 1f)
        );

        AssertPoint(data.Snap(nearby), Vector2.zero);
    }

    [Test]
    public void Reset_ClearsOldData_AndAllowsReusingIds()
    {
        var data = new DrawingData();

        data.AddLine(
            1,
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f)
        );

        data.AddLine(
            2,
            new Vector2(0f, -1f),
            new Vector2(0f, 1f)
        );

        data.ResetData();

        Vector2 nearOldIntersection = new Vector2(0.03f, 0.02f);
        AssertPoint(data.Snap(nearOldIntersection), nearOldIntersection);

        // 同じIDで、別の場所に作図する。
        data.AddLine(
            1,
            new Vector2(2f, 3f),
            new Vector2(4f, 3f)
        );

        data.AddLine(
            2,
            new Vector2(3f, 2f),
            new Vector2(3f, 4f)
        );

        AssertPoint(
            data.Snap(new Vector2(3.03f, 3.02f)),
            new Vector2(3f, 3f)
        );

        AssertPoint(data.Snap(nearOldIntersection), nearOldIntersection);
    }
}