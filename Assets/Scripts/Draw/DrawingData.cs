using System.Collections.Generic;
using UnityEngine;

public class DrawingData
{
    // 今までに描いた図形
    public Dictionary<int, LineData> lineDataList = new();
    public Dictionary<int, CircleData> circleDataList = new();

    //線そのものを管理
    public Dictionary<int, LineRenderer> lines = new();

    public Dictionary<int, GameObject> circleCenters = new();

    public GetIntersection getIntersection = new();

    public void AddLine(int currentIndex, LineRenderer currentLine, Vector2 startPos, Vector2 endPos)
    {
        lines.Add(currentIndex, currentLine);

        LineData newLine = new LineData(startPos, endPos);

        // 新しい線分と既存の線分
        foreach (LineData line in lineDataList.Values)
        {
            AddIntersections(getIntersection.GetLineLineIntersection(newLine, line), currentIndex);
        }

        // 新しい線分と既存の円
        foreach (CircleData circle in circleDataList.Values)
        {
            AddIntersections(getIntersection.GetLineCircleIntersections(newLine, circle), currentIndex);
        }

        // 線分を保存
        lineDataList.Add(currentIndex, newLine);
    }

    public void AddIntersections(List<Vector2> points, int currentIndex)
    {
        foreach (Vector2 point in points)
        {
            if (getIntersection.AddIntersection(currentIndex, point))
            {
                //Debug.Log("交点追加 : " + point);
            }
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
            AddIntersections(getIntersection.GetLineCircleIntersections(line, newCircle), currentIndex);
        }


        // 新しい円と既存の円
        foreach (CircleData circle in circleDataList.Values)
        {
            AddIntersections(getIntersection.GetCircleCircleIntersections(newCircle, circle), currentIndex);
        }

        // 円を保存
        circleDataList.Add(currentIndex, newCircle);
    }
}