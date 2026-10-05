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

    // 今までに描いた図形
    List<LineData> lines = new();
    List<CircleData> circles = new();

    // 全ての交点
    List<Vector2> intersections = new();

    // 現在描画中の線分
    LineRenderer currentLine;

    // 現在描画中の円
    LineRenderer currentCircle;

    Vector2 startPos;

    // trueなら線分、falseなら円
    bool drawingLine = true;

    // 現在描画中か
    bool drawing = false;


    void Update()
    {
        // Lキーで線分
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            drawingLine = true;
        }

        // Cキーで円
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            drawingLine = false;
        }


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


    // 描画開始
    void StartDrawing()
    {
        startPos = GetMouseWorldPosition();

        drawing = true;

        if (drawingLine)
        {
            // 線分を作る
            currentLine = Instantiate(linePrefab);

            currentLine.positionCount = 2;

            currentLine.SetPosition(0, startPos);

            currentLine.SetPosition(1, startPos);
        }
        else
        {
            // 円を作る
            currentCircle = Instantiate(circlePrefab);

            currentCircle.positionCount = circleSegments + 1;

            DrawCircle(currentCircle, startPos, 0);
        }
    }


    // 描画中
    void UpdateDrawing()
    {
        Vector2 mousePos = GetMouseWorldPosition();

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


    // 描画確定
    void FinishDrawing()
    {
        Vector2 endPos = GetMouseWorldPosition();

        drawing = false;


        if (drawingLine)
        {
            // 線分を確定

            currentLine.SetPosition(0, startPos);

            currentLine.SetPosition(1, endPos);


            LineData newLine = new LineData(startPos, endPos);


            // 新しい線分と既存の線分
            foreach (LineData line in lines)
            {
                List<Vector2> points = GetLineLineIntersection(newLine, line);

                foreach (Vector2 point in points)
                {
                    AddIntersection(point);
                }
            }

            // 新しい線分と既存の円
            foreach (CircleData circle in circles)
            {
                List<Vector2> points =
                    GetLineCircleIntersections(newLine, circle);

                foreach (Vector2 point in points)
                {
                    AddIntersection(point);
                }
            }

            // 線分を保存
            lines.Add(newLine);

            currentLine = null;
        }
        else
        {
            // 円を確定

            float radius = Vector2.Distance(startPos, endPos);


            DrawCircle(currentCircle, startPos, radius);


            CircleData newCircle = new CircleData(startPos, radius);


            // 新しい円と既存の線分
            foreach (LineData line in lines)
            {
                List<Vector2> points = GetLineCircleIntersections(line, newCircle);

                foreach (Vector2 point in points)
                {
                    AddIntersection(point);
                }
            }


            // 新しい円と既存の円
            foreach (CircleData circle in circles)
            {
                List<Vector2> points = GetCircleCircleIntersections(newCircle, circle);

                foreach (Vector2 point in points)
                {
                    AddIntersection(point);
                }
            }


            // 円を保存
            circles.Add(newCircle);

            currentCircle = null;
        }

        Debug.Log("現在の交点数 : " + intersections.Count);
    }


    // 円を描画
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


    // 線分 × 線分
    List<Vector2> GetLineLineIntersection(LineData line1, LineData line2)
    {
        List<Vector2> results = new();
        Vector2 p = line1.start;
        Vector2 r = line1.end - line1.start;

        Vector2 q = line2.start;
        Vector2 s = line2.end - line2.start;

        float cross = Cross(r, s);

        // 平行
        if (Mathf.Abs(cross) < 0.00001f) return null;

        Vector2 qp = q - p;

        float t = Cross(qp, s) / cross;
        float u = Cross(qp, r) / cross;

        // 線分の範囲内か
        if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
        {
            results.Add(p + t * r);
            return results;
        }

        return results;
    }


    // 線分 × 円
    List<Vector2> GetLineCircleIntersections(LineData line, CircleData circle)
    {
        List<Vector2> result = new();

        Vector2 d = line.end - line.start;

        Vector2 f = line.start - circle.center;


        float a = Vector2.Dot(d, d);

        float b = 2f * Vector2.Dot(f, d);

        float c = Vector2.Dot(f, f) - circle.radius * circle.radius;

        float discriminant = b * b - 4f * a * c;

        // 交点なし
        if (discriminant < 0f) return result;

        // 数値誤差対策
        discriminant = Mathf.Max(0f, discriminant);

        float sqrt = Mathf.Sqrt(discriminant);

        float t1 = (-b - sqrt) / (2f * a);

        float t2 = (-b + sqrt) / (2f * a);


        // 1つ目
        if (t1 >= 0f && t1 <= 1f)
        {
            Vector2 point = line.start + t1 * d;

            result.Add(point);
        }

        // 2つ目
        if (t2 >= 0f && t2 <= 1f && !Approximately(t1, t2))
        {
            Vector2 point = line.start + t2 * d;
            result.Add(point);
        }

        return result;
    }


    // 円 × 円
    List<Vector2> GetCircleCircleIntersections(CircleData circle1, CircleData circle2
    )
    {
        List<Vector2> result = new();

        Vector2 diff = circle2.center - circle1.center;

        float distance = diff.magnitude;

        // 中心が同じ
        if (distance < 0.00001f) return result;

        // 円が離れすぎている
        if (distance > circle1.radius + circle2.radius) return result;

        // 片方の円が完全に内側
        if (distance < Mathf.Abs(circle1.radius - circle2.radius)) return result;

        float a =
            (
                circle1.radius * circle1.radius -
                circle2.radius * circle2.radius +
                distance * distance
            )
            / (2f * distance);


        float hSquared = circle1.radius * circle1.radius - a * a;


        // 数値誤差対策
        hSquared = Mathf.Max(0f, hSquared);
        float h = Mathf.Sqrt(hSquared);

        // 円1の中心から円2方向への単位ベクトル
        Vector2 direction = diff / distance;

        // 2つの円の交点の中心
        Vector2 middle = circle1.center + direction * a;


        // 垂直方向
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);


        Vector2 point1 = middle + perpendicular * h;
        Vector2 point2 = middle - perpendicular * h;

        result.Add(point1);

        // 接している場合は1点だけ
        if (!Approximately(point1, point2)) result.Add(point2);

        return result;
    }


    // 交点を保存
    void AddIntersection(Vector2 point)
    {
        // 同じ場所の交点がすでに存在する場合は追加しない
        foreach (Vector2 existing in intersections)
        {
            if (Approximately(existing, point)) return;
        }

        intersections.Add(point);

        Debug.Log("交点追加 : " + point);
    }


    // Vector2の近似判定
    bool Approximately(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b) < 0.0001f;
    }

    // floatの近似判定
    bool Approximately(float a, float b)
    {
        return Mathf.Abs(a - b) < 0.0001f;
    }

    // 外積
    float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
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

    // データクラス

    class LineData
    {
        public Vector2 start;
        public Vector2 end;


        public LineData(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
    }


    class CircleData
    {
        public Vector2 center;
        public float radius;


        public CircleData(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
}