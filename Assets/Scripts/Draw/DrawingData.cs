using System.Collections.Generic;
using UnityEngine;

public class DrawingData : MonoBehaviour
{
    // 今までに描いた図形
    private readonly Dictionary<int, LineData> lineDataList = new();
    private readonly Dictionary<int, CircleData> circleDataList = new();

    //線そのものを管理
    private readonly Dictionary<int, LineRenderer> lines = new();

    private readonly Dictionary<int, GameObject> circleCenters = new();

    private readonly GetIntersection getIntersection = new();

    public void AddLine(int currentIndex, LineRenderer currentLine, Vector2 startPos, Vector2 endPos)
    {
        lines.Add(currentIndex, currentLine);

        LineData newLine = new LineData(startPos, endPos);

        // 新しい線分と既存の線分
        foreach (LineData line in lineDataList.Values)
        {
            AddIntersections(IntersectionCalculator.GetLineLineIntersection(newLine, line), currentIndex);
        }

        // 新しい線分と既存の円
        foreach (CircleData circle in circleDataList.Values)
        {
            AddIntersections(IntersectionCalculator.GetLineCircleIntersections(newLine, circle), currentIndex);
        }

        // 線分を保存
        lineDataList.Add(currentIndex, newLine);
    }

    private void AddIntersections(List<Vector2> points, int currentIndex)
    {
        foreach (Vector2 point in points)
        {
            getIntersection.AddIntersection(currentIndex, point);
        }
    }

    public void AddCircle(int currentIndex, LineRenderer currentCircle, GameObject currentCircleCenter, Vector2 startPos, float radius)
    {
        lines.Add(currentIndex, currentCircle);
        circleCenters.Add(currentIndex, currentCircleCenter);

        CircleData newCircle = new CircleData(startPos, radius);

        // 新しい円と既存の線分
        foreach (LineData line in lineDataList.Values)
        {
            AddIntersections(IntersectionCalculator.GetLineCircleIntersections(line, newCircle), currentIndex);
        }


        // 新しい円と既存の円
        foreach (CircleData circle in circleDataList.Values)
        {
            AddIntersections(IntersectionCalculator.GetCircleCircleIntersections(newCircle, circle), currentIndex);
        }

        // 円を保存
        circleDataList.Add(currentIndex, newCircle);
    }

    public void ResetData()
    {
        foreach (LineRenderer lineRenderer in lines.Values)
        {
            Destroy(lineRenderer.gameObject);
        }
        lines.Clear();
        foreach (GameObject gameObject in circleCenters.Values)
        {
            Destroy(gameObject);
        }
        circleCenters.Clear();


        lineDataList.Clear();
        circleDataList.Clear();
        getIntersection.Clear();
    }

    public void UndoData(int currentIndex)
    {
        if (lineDataList.ContainsKey(currentIndex))
        {
            lineDataList.Remove(currentIndex);
        }
        else
        {
            circleDataList.Remove(currentIndex);
            Destroy(circleCenters[currentIndex]);
            circleCenters.Remove(currentIndex);
        }

        Destroy(lines[currentIndex].gameObject);
        lines.Remove(currentIndex);

        getIntersection.UndoIntersection(currentIndex);
    }

    public void GridData(List<Vector2> intersections, List<LineData> lines)
    {
        AddIntersections(intersections, -1);
        for (int i = 0; i < lines.Count; i++)
        {
            lineDataList.Add(-i, lines[i]);
        }
    }

    public Vector2 Snap(Vector2 position)
    {
        return getIntersection.Snap(position);
    }
}