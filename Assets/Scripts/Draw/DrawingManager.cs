using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawingManager : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] LineRenderer circlePrefab;
    [SerializeField] GameObject circleCenterPrefab;


    [SerializeField] Color32 lineColor;
    [SerializeField] float lineWidth = 0.05f;

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
    int previousCompleteMoves = 0;

    //ほかのmanager
    UIManager uiManager;
    DrawingData drawingData;

    void Start()
    {
        uiManager = GetComponent<UIManager>();
        drawingData = GetComponent<DrawingData>();

        CreateGrid();
        LineMode();

        currentMoves = 0;

        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());

    }

    void CreateGrid()
    {
        var gridLines = GetComponent<GridGenerator>().GenerateGrid();
        drawingData.GridData(gridLines.intersections, gridLines.lines);
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
        startPos = drawingData.getIntersection.Snap(startPos);

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

            currentCircle.positionCount = Constants.CircleSegments + 1;
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
        mousePos = drawingData.getIntersection.Snap(mousePos);

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
        endPos = drawingData.getIntersection.Snap(endPos);

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

            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, endPos);

            float length = Vector2.Distance(startPos, endPos);

            drawingData.AddLine(currentIndex, currentLine, startPos, endPos);

            currentLine = null;
            currentMoves++;

            Debug.Log($"線分の長さ{length}");

            QuestionManager questionManager = GetComponent<QuestionManager>();
            int stageBeforeAnswer = questionManager.stageLevel;
            bool isCompleted = questionManager.CheckAnswer(length);

            if (isCompleted && questionManager.stageLevel == stageBeforeAnswer)
            {
                previousCompleteMoves = currentIndex;
            }

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

            float radius;
            if (mode == Constants.Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, endPos);

            fixedRadius = radius;

            DrawCircle(currentCircle, startPos, radius);

            //Dataに追加する
            drawingData.AddCircle(currentIndex, currentCircle, currentCircleCenter, startPos, radius);

            currentCircle = null;
            currentCircleCenter = null;
            currentMoves++;
        }

        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());

    }

    bool isLineOut(Vector2 position)
    {
        if (Constants.GridStartPos.x - 0.5f >= position.x || Constants.GridStartPos.x + Constants.GridNumber.x * Constants.GridSize + 0.5f <= position.x) return true;
        if (Constants.GridStartPos.y - 0.5f >= position.y || Constants.GridStartPos.y + Constants.GridNumber.y * Constants.GridSize + 0.5f <= position.y) return true;
        return false;
    }

    void DrawCircle(LineRenderer line, Vector2 center, float radius)
    {
        for (int i = 0; i <= Constants.CircleSegments; i++)
        {
            float angle = 2f * Mathf.PI * i / Constants.CircleSegments;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));
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

    void CancelDrawing()
    {
        drawing = false;

        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
        }
        if (currentCircle != null)
        {
            Destroy(currentCircle.gameObject);
            currentCircle = null;
        }
        if (currentCircleCenter != null)
        {
            Destroy(currentCircleCenter);
            currentCircleCenter = null;
        }

    }
    public void ResetBoard()
    {
        CancelDrawing();

        drawingData.ResetData();

        currentIndex = 0;
        previousCompleteMoves = 0;
        fixedRadius = 0f;

        CreateGrid();

        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());
    }

    public void BeginStage()
    {
        currentMoves = 0;
        ResetBoard();
    }

    public void Undo()
    {
        if (drawing) return;
        if (!CanUndo()) return;

        drawingData.UndoData(currentIndex);

        currentIndex--;
        currentMoves--;
        uiManager.UpdateMoveUI(currentMoves, Question.Levels(GetComponent<QuestionManager>().stageLevel), CanUndo());
    }

    public bool CanUndo()
    {
        return !(currentIndex <= previousCompleteMoves);
    }


    public void CompassMode()
    {
        if (drawing) return;
        mode = Constants.Mode.Compass;
        uiManager.CompassModeUI();
    }

    public void LineMode()
    {
        if (drawing) return;
        mode = Constants.Mode.Line;
        uiManager.LineModeUI();
    }

    public void FixedCompassMode()
    {
        if (drawing) return;
        mode = Constants.Mode.FixedCompass;
        uiManager.FixedCompassModeUI();
    }

}
