using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawingManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] LineRenderer circlePrefab;

    [Header("Circle")]
    [SerializeField] int circleSegments = 100;

    [SerializeField] float snapRadius = 0.1f;

    [SerializeField] Color32 gridColor;
    [SerializeField] Color32 lineColor;

    // 今までに描いた図形
    List<LineData> lineDataList = new();
    List<CircleData> circleDataList = new();

    //線そのものを管理
    List<LineRenderer> lines = new();

    // 交点計算と交点一覧の管理
    GetIntersection getIntersection = new();

    // 現在描画中の線分
    LineRenderer currentLine;

    // 現在描画中の円
    LineRenderer currentCircle;

    Vector2 startPos;

    // trueなら線分、falseなら円
    bool drawingLine = true;

    // 現在描画中か
    bool drawing = false;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            LineRenderer Line = Instantiate(linePrefab);
            Line.positionCount = 2;
            Line.SetPosition(0, new Vector2(4.5f, -4f + i));
            Line.SetPosition(1, new Vector2(-4.5f, -4f + i));
            Line.startColor = gridColor;
            Line.endColor = gridColor;
            LineData newLine = new LineData(new Vector2(4.5f, -4f + i), new Vector2(-4.5f, -4f + i));
            lineDataList.Add(newLine);
        }
        for (int i = 0; i < 9; i++)
        {
            LineRenderer Line = Instantiate(linePrefab);
            Line.positionCount = 2;
            Line.SetPosition(0, new Vector2(-4f + i, 4.5f));
            Line.SetPosition(1, new Vector2(-4f + i, -4.5f));
            Line.startColor = gridColor;
            Line.endColor = gridColor;

            LineData newLine = new LineData(new Vector2(-4f + i, 4.5f), new Vector2(-4f + i, -4.5f));

            foreach (LineData line in lineDataList)
            {
                AddIntersections(getIntersection.GetLineLineIntersection(newLine, line));
            }
            lineDataList.Add(newLine);
        }
    }


    void Update()
    {
        // Lキーで線分
        if (Keyboard.current.lKey.wasPressedThisFrame) drawingLine = true;
        // Cキーで円
        if (Keyboard.current.cKey.wasPressedThisFrame) drawingLine = false;

        // 描画開始
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartDrawing();
        }

        if (drawing)
        {
            // マウスを動かしている間
            UpdateDrawing();
            // マウスを離したら確定
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                FinishDrawing();
            }
        }
    }


    void StartDrawing()
    {
        startPos = GetMouseWorldPosition();
        startPos = Snap(startPos);

        drawing = true;

        if (drawingLine)
        {
            // 線分を作る
            currentLine = Instantiate(linePrefab);

            lines.Add(currentLine);

            currentLine.positionCount = 2;
            currentLine.startColor = lineColor;
            currentLine.endColor = lineColor;

            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, startPos);
        }
        else
        {
            // 円を作る
            currentCircle = Instantiate(circlePrefab);
            lines.Add(currentCircle);
            currentCircle.positionCount = circleSegments + 1;
            currentCircle.startColor = lineColor;
            currentCircle.endColor = lineColor;

            DrawCircle(currentCircle, startPos, 0);
        }
    }

    void UpdateDrawing()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        mousePos = Snap(mousePos);

        if (drawingLine)
        {
            // 線分の終点を更新
            currentLine.SetPosition(1, mousePos);
        }
        else
        {
            // 中心からマウスまでを半径にする
            float radius = Vector2.Distance(startPos, mousePos);

            DrawCircle(currentCircle, startPos, radius);
        }
    }

    void FinishDrawing()
    {
        Vector2 endPos = GetMouseWorldPosition();
        endPos = Snap(endPos);

        drawing = false;

        if (drawingLine)
        {
            // 線分を確定

            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, endPos);

            LineData newLine = new LineData(startPos, endPos);

            // 新しい線分と既存の線分
            foreach (LineData line in lineDataList)
            {
                AddIntersections(getIntersection.GetLineLineIntersection(newLine, line));
            }

            // 新しい線分と既存の円
            foreach (CircleData circle in circleDataList)
            {
                AddIntersections(getIntersection.GetLineCircleIntersections(newLine, circle));
            }

            // 線分を保存
            lineDataList.Add(newLine);

            currentLine = null;

            Debug.Log($"線分の長さ{Vector2.Distance(startPos, endPos)}");
        }
        else
        {
            // 円を確定
            float radius = Vector2.Distance(startPos, endPos);

            DrawCircle(currentCircle, startPos, radius);

            CircleData newCircle = new CircleData(startPos, radius);

            // 新しい円と既存の線分
            foreach (LineData line in lineDataList)
            {
                AddIntersections(getIntersection.GetLineCircleIntersections(line, newCircle));
            }


            // 新しい円と既存の円
            foreach (CircleData circle in circleDataList)
            {
                AddIntersections(getIntersection.GetCircleCircleIntersections(newCircle, circle));
            }


            // 円を保存
            circleDataList.Add(newCircle);

            currentCircle = null;
        }

        Debug.Log("現在の交点数 : " + getIntersection.Points.Count);
    }

    void DrawCircle(LineRenderer line, Vector2 center, float radius)
    {
        for (int i = 0; i <= circleSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / circleSegments;

            float x = center.x + Mathf.Cos(angle) * radius;

            float y = center.y + Mathf.Sin(angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    void AddIntersections(List<Vector2> points)
    {
        foreach (Vector2 point in points)
        {
            if (getIntersection.AddIntersection(point))
            {
                Debug.Log("交点追加 : " + point);
            }
        }
    }

    // マウス座標 → ワールド座標
    Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    -Camera.main.transform.position.z
                )
            );

        return new Vector2(worldPosition.x, worldPosition.y);
    }

    Vector2 Snap(Vector2 pos)
    {
        Vector2 result = pos;
        foreach (Vector2 interSection in getIntersection.intersections)
        {
            if (Vector2.Distance(pos, interSection) <= snapRadius)
            {
                result = interSection;
            }
        }
        return result;
    }

    public void Reset()
    {
        foreach (LineRenderer lineRenderer in lines)
        {
            Destroy(lineRenderer.gameObject);
        }
        lines.Clear();
        getIntersection.intersections.Clear();

        CreateGrid();
    }
}
