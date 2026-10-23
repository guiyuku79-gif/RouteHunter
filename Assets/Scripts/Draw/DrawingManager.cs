using System;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    // 線のIdを決める
    int currentIndex = 0;

    Vector2 startPos;

    private Constants.Mode mode;

    // 現在描画中か
    bool isDrawing = false;
    public bool IsDrawing => isDrawing;

    bool isInputEnabled = true;
    public bool IsInputEnabled => isInputEnabled;

    float fixedRadius = 0;

    //現在の手数
    int currentMoves = 0;
    public int CurrentMoves => currentMoves;

    //これより前にはUndoできない
    int undoBoundaryId = 0;

    public event Action<int, LineData> LineCompleted;
    public event Action<int, bool> DrawingStateChanged;
    public event Action<Constants.Mode, bool, CircleData> ModeChanged;

    //ほかのmanager
    private readonly DrawingData drawingData = new DrawingData();
    DrawingView drawingView;

    void Awake()
    {
        drawingView = GetComponent<DrawingView>();
    }

    #region 初期化
    public void Initialize()
    {
        CreateGrid();
        LineMode();

        currentMoves = 0;

        NotifyDrawingStateChanged();
    }

    void CreateGrid()
    {
        var gridLines = GetComponent<GridGenerator>().GenerateGrid();
        drawingData.GridData(gridLines.intersections, gridLines.lines);
    }

    #endregion

    #region 線と円の描画
    public void BeginDrawing(Vector2 worldPosition)
    {
        if (!isInputEnabled || isDrawing) return;

        startPos = drawingData.Snap(worldPosition);

        if (isLineOut(startPos)) return;

        isDrawing = true;

        if (mode == Constants.Mode.Line)
        {
            drawingView.BeginLine(startPos);
        }
        else
        {
            drawingView.BeginCircle(startPos);
        }
    }

    public void UpdateDrawing(Vector2 worldPosition)
    {
        if (!isInputEnabled || !isDrawing) return;

        Vector2 snappedPosition = drawingData.Snap(worldPosition);

        if (mode == Constants.Mode.Line)
        {
            drawingView.UpdateLine(startPos, snappedPosition);
        }
        else
        {
            float radius = mode == Constants.Mode.FixedCompass
                ? fixedRadius
                : Vector2.Distance(startPos, snappedPosition);

            drawingView.UpdateCircle(startPos, radius);
        }
    }

    public void FinishDrawing(Vector2 worldPosition)
    {
        if (!isInputEnabled || !isDrawing) return;

        Vector2 endPos = drawingData.Snap(worldPosition);

        isDrawing = false;

        //範囲外の線は消す
        if (isLineOut(startPos) || isLineOut(endPos))
        {
            drawingView.CancelPreview();
            return;
        }

        //小さすぎる線は消す
        if (Vector2.Distance(startPos, endPos) <= 0.1f)
        {
            drawingView.CancelPreview();
            return;
        }

        currentIndex++;

        if (mode == Constants.Mode.Line)
        {
            drawingView.UpdateLine(startPos, endPos);
            drawingData.AddLine(currentIndex, startPos, endPos);
            drawingView.CommitPreview(currentIndex);

            currentMoves++;
            float length = Vector2.Distance(startPos, endPos);
            Debug.Log($"線分の長さ{length}");

            LineCompleted?.Invoke(currentIndex, new LineData(startPos, endPos));

        }
        else
        {

            float radius;
            if (mode == Constants.Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, endPos);

            fixedRadius = radius;

            drawingView.UpdateCircle(startPos, radius);
            drawingData.AddCircle(currentIndex, startPos, radius, mode);
            drawingView.CommitPreview(currentIndex);

            currentMoves++;
        }

        NotifyDrawingStateChanged();

        ModeChanged?.Invoke(mode, fixedRadius != 0, drawingData.LatestCircleData());

    }

    bool isLineOut(Vector2 position)
    {
        if (Constants.GridStartPos.x - 0.5f >= position.x || Constants.GridStartPos.x + Constants.GridNumber.x * Constants.GridSize + 0.5f <= position.x) return true;
        if (Constants.GridStartPos.y - 0.5f >= position.y || Constants.GridStartPos.y + Constants.GridNumber.y * Constants.GridSize + 0.5f <= position.y) return true;
        return false;
    }

    public void CancelDrawing()
    {
        isDrawing = false;
        drawingView.CancelPreview();

    }

    #endregion

    public void ResetBoard()
    {
        if (!isInputEnabled) return;
        CancelDrawing();

        drawingView.Clear();
        drawingData.ResetData();

        currentIndex = 0;
        undoBoundaryId = 0;
        fixedRadius = 0f;

        CreateGrid();

        NotifyDrawingStateChanged();
    }

    public void BeginStage()
    {
        currentMoves = 0;
        ResetBoard();
    }

    public void Undo()
    {
        if (!isInputEnabled) return;
        if (isDrawing) return;
        if (!CanUndo()) return;

        drawingData.UndoData(currentIndex);
        drawingView.RemoveDrawing(currentIndex);

        currentIndex--;
        currentMoves--;
        NotifyDrawingStateChanged();
    }

    public bool CanUndo()
    {
        return isInputEnabled && currentIndex > undoBoundaryId;
    }

    public void LockUndoThroughCurrentDrawing()
    {
        undoBoundaryId = currentIndex;
    }

    void NotifyDrawingStateChanged()
    {
        DrawingStateChanged?.Invoke(currentMoves, CanUndo());
    }

    #region  モード変更
    public void CompassMode()
    {
        SetMode(Constants.Mode.Compass);
    }
    public void LineMode()
    {
        SetMode(Constants.Mode.Line);
    }
    public void FixedCompassMode()
    {
        if (fixedRadius == 0) return;
        SetMode(Constants.Mode.FixedCompass);
    }

    void SetMode(Constants.Mode newMode)
    {
        if (!isInputEnabled) return;
        if (isDrawing) return;

        mode = newMode;
        ModeChanged?.Invoke(mode, fixedRadius != 0, drawingData.LatestCircleData());
    }

    #endregion
    
    public void StopInput()
    {
        isInputEnabled = false;

        CancelDrawing();
        NotifyDrawingStateChanged();
    }

    public void RemoveSolvedLine(int lineId)
    {
        drawingData.UndoData(lineId);
        drawingView.RemoveDrawing(lineId);
    }

}