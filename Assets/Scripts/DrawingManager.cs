using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] LineRenderer circlePrefab;
    [SerializeField] LineRenderer gridPrefab;

    [Header("Circle")]
    [SerializeField] int circleSegments = 100;

    [SerializeField] float snapRadius = 0.1f;

    [SerializeField] Color32 gridColor;
    [SerializeField] float gridWidth = 0.03f;
    [SerializeField] Color32 lineColor;
    [SerializeField] float lineWidth = 0.05f;


    [SerializeField] SpriteRenderer compassButton;
    [SerializeField] SpriteRenderer lineButton;
    [SerializeField] SpriteRenderer fixedCompassButton;

    [SerializeField] float CampasWidth;
    [SerializeField] float CampasHeight;

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


    //現在のモードを構造体にする
    public enum Mode
    {
        Line,
        Compass,
        FixedCompass
    }
    public Mode mode;

    // 現在描画中か
    bool drawing = false;

    float fixedRadius = 0;

    void Start()
    {
        CreateGrid();

        LineMode();

        Fraction fraction = new Fraction(
            new List<Monomial>
            {
                new Monomial(number:1),
                new Monomial(Coefficient:2,number:2,isRotted:true)
            },
            new List<Monomial>
            {
                new Monomial(number:1)
            }
        );
        Debug.Log(fraction.FractionToString());
        Debug.Log(fraction.ToFloat());
    }

    void CreateGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            LineRenderer Line = Instantiate(gridPrefab);
            Line.positionCount = 2;
            Line.SetPosition(0, new Vector2(4.5f, -4f + i));
            Line.SetPosition(1, new Vector2(-4.5f, -4f + i));
            Line.startColor = gridColor;
            Line.endColor = gridColor;
            Line.startWidth = gridWidth;
            Line.endWidth = gridWidth;

            LineData newLine = new LineData(new Vector2(4.5f, -4f + i), new Vector2(-4.5f, -4f + i));
            lineDataList.Add(newLine);
        }
        for (int i = 0; i < 9; i++)
        {
            LineRenderer Line = Instantiate(gridPrefab);
            Line.positionCount = 2;
            Line.SetPosition(0, new Vector2(-4f + i, 4.5f));
            Line.SetPosition(1, new Vector2(-4f + i, -4.5f));
            Line.startColor = gridColor;
            Line.endColor = gridColor;
            Line.startWidth = gridWidth;
            Line.endWidth = gridWidth;

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
        if (Keyboard.current.lKey.wasPressedThisFrame) LineMode();
        // Cキーで円
        if (Keyboard.current.cKey.wasPressedThisFrame) CompassMode();

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

        if (Mathf.Abs(startPos.x) > CampasWidth || Mathf.Abs(startPos.y) > CampasHeight) return;

        drawing = true;

        if (mode == Mode.Line)
        {

            // 線分を作る
            currentLine = Instantiate(linePrefab);

            currentLine.positionCount = 2;
            currentLine.startColor = lineColor;
            currentLine.endColor = lineColor;
            currentLine.startWidth = lineWidth;
            currentLine.endWidth = lineWidth;

            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, startPos);
        }
        else
        {
            // 円を作る
            currentCircle = Instantiate(circlePrefab);

            currentCircle.positionCount = circleSegments + 1;
            currentCircle.startColor = lineColor;
            currentCircle.endColor = lineColor;
            currentCircle.startWidth = lineWidth;
            currentCircle.endWidth = lineWidth;

            DrawCircle(currentCircle, startPos, 0);
        }
    }

    void UpdateDrawing()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        mousePos = Snap(mousePos);

        if (mode == Mode.Line)
        {
            // 線分の終点を更新
            currentLine.SetPosition(1, mousePos);
        }
        else
        {
            float radius;
            if (mode == Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, mousePos);

            DrawCircle(currentCircle, startPos, radius);
        }
    }

    void FinishDrawing()
    {
        Vector2 endPos = GetMouseWorldPosition();
        endPos = Snap(endPos);

        drawing = false;



        if (mode == Mode.Line)
        {
            //範囲外の線は消す
            if (Mathf.Abs(endPos.x) > CampasWidth || Mathf.Abs(endPos.y) > CampasHeight ||
            Mathf.Abs(startPos.x) > CampasWidth || Mathf.Abs(startPos.y) > CampasHeight)
            {
                Destroy(currentLine.gameObject);
                currentLine = null;
                return;
            }
            lines.Add(currentLine);
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
            //範囲外の線は消す
            if (Mathf.Abs(endPos.x) > CampasWidth || Mathf.Abs(endPos.y) > CampasHeight ||
            Mathf.Abs(startPos.x) > CampasWidth || Mathf.Abs(startPos.y) > CampasHeight)
            {
                Destroy(currentCircle.gameObject);
                currentCircle = null;
                return;
            }

            lines.Add(currentCircle);
            float radius;
            if (mode == Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, endPos);

            fixedRadius = radius;

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

        //Debug.Log("現在の交点数 : " + getIntersection.Points.Count);
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
                //Debug.Log("交点追加 : " + point);
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

    public void CompassMode()
    {
        mode = Mode.Compass;
        compassButton.color = Color.gray;
        lineButton.color = Color.white;
        fixedCompassButton.color = Color.white;
    }

    public void LineMode()
    {
        mode = Mode.Line;
        compassButton.color = Color.white;
        lineButton.color = Color.gray;
        fixedCompassButton.color = Color.white;
    }

    public void FixedCompassMode()
    {
        mode = Mode.FixedCompass;
        compassButton.color = Color.white;
        lineButton.color = Color.white;
        fixedCompassButton.color = Color.gray;
    }
}
