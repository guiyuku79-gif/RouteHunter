using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] LineRenderer circlePrefab;
    [SerializeField] GameObject circleCenterPrefab;

    [Header("Circle")]
    [SerializeField] int circleSegments = 100;

    [SerializeField] Color32 lineColor;
    [SerializeField] float lineWidth = 0.05f;


    [SerializeField] Vector2 CampasWidthRange;
    [SerializeField] Vector2 CampasHeightRange;

    // 今までに描いた図形
    Dictionary<int, LineData> lineDataList = new();
    Dictionary<int, CircleData> circleDataList = new();

    //線そのものを管理
    Dictionary<int, LineRenderer> lines = new();

    Dictionary<int, GameObject> circleCenters = new();

    // 交点計算と交点一覧の管理
    GetIntersection getIntersection = new();

    // 現在描画中の線分
    LineRenderer currentLine;

    // 現在描画中の円
    LineRenderer currentCircle;

    GameObject currentCircleCenter;

    // 線のIdを決める
    int currentIndex = 0;

    Vector2 startPos;

    public Constants.Mode mode;

    // 現在描画中か
    bool drawing = false;

    float fixedRadius = 0;

    //現在の手数
    int currentMoves = 0;
    //直近で線が完成したときの手数
    int priviousCompleteMoves = 0;

    //ほかのmanager
    UIManager uiManager;

    void Start()
    {

        uiManager = GetComponent<UIManager>();

        CreateGrid();
        LineMode();

        currentMoves = 0;

        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());

    }

    void CreateGrid()
    {
        AddIntersections(GetComponent<GridGenerator>().GenerateGrid());
    }

    void Update()
    {
        // 描画開始
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartDrawing();
        }

        if (drawing)
        {
            UpdateDrawing();
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                FinishDrawing();
            }
        }
        else
        {
            if (Keyboard.current.lKey.wasPressedThisFrame) LineMode();
            if (Keyboard.current.cKey.wasPressedThisFrame) CompassMode();
            if (Keyboard.current.zKey.wasPressedThisFrame) Undo();
        }
    }


    void StartDrawing()
    {
        startPos = GetMouseWorldPosition();
        startPos = getIntersection.Snap(startPos);

        if (isLineOut(startPos)) return;

        drawing = true;

        if (mode == Constants.Mode.Line)
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

            currentCircleCenter = Instantiate(circleCenterPrefab);
            currentCircleCenter.transform.position = startPos;

        }
    }

    void UpdateDrawing()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        mousePos = getIntersection.Snap(mousePos);

        if (mode == Constants.Mode.Line)
        {
            // 線分の終点を更新
            currentLine.SetPosition(1, mousePos);
        }
        else
        {
            float radius;
            if (mode == Constants.Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, mousePos);

            DrawCircle(currentCircle, startPos, radius);
        }
    }

    void FinishDrawing()
    {
        Vector2 endPos = GetMouseWorldPosition();
        endPos = getIntersection.Snap(endPos);

        drawing = false;

        if (mode == Constants.Mode.Line)
        {
            //範囲外の線は消す
            if (isLineOut(startPos) || isLineOut(endPos))
            {
                Destroy(currentLine.gameObject);
                currentLine = null;
                return;
            }
            currentIndex++;

            lines.Add(currentIndex, currentLine);
            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, endPos);

            LineData newLine = new LineData(startPos, endPos);

            // 新しい線分と既存の線分
            foreach (LineData line in lineDataList.Values)
            {
                AddIntersections(getIntersection.GetLineLineIntersection(newLine, line));
            }

            // 新しい線分と既存の円
            foreach (CircleData circle in circleDataList.Values)
            {
                AddIntersections(getIntersection.GetLineCircleIntersections(newLine, circle));
            }

            // 線分を保存
            lineDataList.Add(currentIndex, newLine);


            currentLine = null;

            Debug.Log($"線分の長さ{Vector2.Distance(startPos, endPos)}");
            bool isCompleted = GetComponent<QuestionManager>().CheckAnswer(Vector2.Distance(startPos, endPos));
            if (isCompleted) priviousCompleteMoves = currentIndex;

            currentMoves++;
        }
        else
        {
            //範囲外の線は消す
            if (isLineOut(startPos) || isLineOut(endPos))
            {
                Destroy(currentCircle.gameObject);
                Destroy(currentCircleCenter);
                currentCircle = null;
                currentCircleCenter = null;
                return;
            }
            currentIndex++;

            lines.Add(currentIndex, currentCircle);
            circleCenters.Add(currentIndex, currentCircleCenter);
            float radius;
            if (mode == Constants.Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, endPos);

            fixedRadius = radius;

            DrawCircle(currentCircle, startPos, radius);

            CircleData newCircle = new CircleData(startPos, radius);

            // 新しい円と既存の線分
            foreach (LineData line in lineDataList.Values)
            {
                AddIntersections(getIntersection.GetLineCircleIntersections(line, newCircle));
            }


            // 新しい円と既存の円
            foreach (CircleData circle in circleDataList.Values)
            {
                AddIntersections(getIntersection.GetCircleCircleIntersections(newCircle, circle));
            }

            // 円を保存
            circleDataList.Add(currentIndex, newCircle);

            currentCircle = null;
            currentMoves++;
        }

        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());

    }

    bool isLineOut(Vector2 position)
    {
        if (CampasWidthRange.x >= position.x || CampasWidthRange.y <= position.x) return true;
        if (CampasHeightRange.x >= position.y || CampasHeightRange.y <= position.y) return true;
        return false;
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
            if (getIntersection.AddIntersection(currentIndex, point))
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

    public void Reset()
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
        getIntersection.intersections.Clear();

        priviousCompleteMoves = 0;
        CreateGrid();
    }

    public void Undo()
    {
        if (!CanUndo()) return;

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
        currentIndex--;
        currentMoves--;
        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());

    }

    public bool CanUndo()
    {
        return !(currentIndex <= priviousCompleteMoves);
    }


    public void CompassMode()
    {
        mode = Constants.Mode.Compass;
        uiManager.CompassModeUI();
    }

    public void LineMode()
    {
        mode = Constants.Mode.Line;
        uiManager.LineModeUI();
    }

    public void FixedCompassMode()
    {
        mode = Constants.Mode.FixedCompass;
        uiManager.FixedCompassModeUI();
    }

}
