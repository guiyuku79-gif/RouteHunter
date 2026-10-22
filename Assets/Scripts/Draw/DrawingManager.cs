using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawingManager : MonoBehaviour
{
    // 線のIdを決める
    int currentIndex = 0;

    Vector2 startPos;

    private Constants.Mode mode;

    // 現在描画中か
    bool drawing = false;

    bool inputEnabled = true;

    float fixedRadius = 0;

    //現在の手数
    int currentMoves = 0;

    //これより前にはUndoできない
    int undoBoundaryId = 0;

    public event Action<float> LineCompleted;
    public event Action<int, bool> DrawingStateChanged;
    public event Action<Constants.Mode, bool, CircleData> ModeChanged;

    //ほかのmanager
    private readonly DrawingData drawingData = new DrawingData();
    DrawingView drawingView;

    private Pointer drawingPointer;
    private Vector2 pointerScreenPosition;

    void Awake()
    {
        drawingView = GetComponent<DrawingView>();
    }

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

    // void Update()
    // {
    //     if (!inputEnabled) return;

    //     // 描画開始
    //     if (Mouse.current.leftButton.wasPressedThisFrame)
    //     {
    //         StartDrawing();
    //     }

    //     if (drawing)
    //     {
    //         UpdateDrawing();
    //         if (Mouse.current.leftButton.wasReleasedThisFrame)
    //         {
    //             FinishDrawing();
    //         }
    //     }
    //     else
    //     {
    //         if (Keyboard.current.lKey.wasPressedThisFrame) LineMode();
    //         if (Keyboard.current.cKey.wasPressedThisFrame) CompassMode();
    //         if (Keyboard.current.zKey.wasPressedThisFrame) Undo();
    //     }
    // }

    void Update()
    {
        if (!inputEnabled) return;

        // 描画中は、開始したデバイスを使い続ける
        Pointer pointer = drawing ? drawingPointer : Pointer.current;

        if (pointer != null)
        {
            pointerScreenPosition = pointer.position.ReadValue();

            if (!drawing && pointer.press.wasPressedThisFrame)
            {
                drawingPointer = pointer;
                StartDrawing();
            }

            if (drawing)
            {
                // タッチがOSやブラウザによって中断された場合
                bool touchCanceled =
                    pointer is Touchscreen touchscreen &&
                    touchscreen.primaryTouch.phase.ReadValue()
                        == UnityEngine.InputSystem.TouchPhase.Canceled;

                if (touchCanceled)
                {
                    CancelDrawing();
                }
                else
                {
                    UpdateDrawing();

                    if (pointer.press.wasReleasedThisFrame)
                    {
                        FinishDrawing();
                    }
                    else if (!pointer.press.isPressed)
                    {
                        // 入力が途切れた場合はプレビューを破棄
                        CancelDrawing();
                    }
                }
            }
        }
        else if (drawing)
        {
            CancelDrawing();
        }

        // キーボードのない端末でもエラーにしない
        var keyboard = Keyboard.current;
        if (!drawing && keyboard != null)
        {
            if (keyboard.lKey.wasPressedThisFrame) LineMode();
            if (keyboard.cKey.wasPressedThisFrame) CompassMode();
            if (keyboard.zKey.wasPressedThisFrame) Undo();
        }
    }

    // 別タブへの移動などで描画が残るのを防ぐ
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && drawing)
        {
            CancelDrawing();
        }
    }




    void StartDrawing()
    {
        startPos = drawingData.Snap(GetMouseWorldPosition());

        if (isLineOut(startPos)) return;

        drawing = true;

        if (mode == Constants.Mode.Line)
        {
            drawingView.BeginLine(startPos);
        }
        else
        {
            drawingView.BeginCircle(startPos);
        }
    }

    void UpdateDrawing()
    {
        Vector2 mousePos = GetMouseWorldPosition();
        mousePos = drawingData.Snap(mousePos);

        if (mode == Constants.Mode.Line)
        {
            drawingView.UpdateLine(startPos, mousePos);
        }
        else
        {
            float radius = mode == Constants.Mode.FixedCompass
            ? fixedRadius
            : Vector2.Distance(startPos, mousePos);

            drawingView.UpdateCircle(startPos, radius);
        }
    }

    void FinishDrawing()
    {
        Vector2 endPos = drawingData.Snap(GetMouseWorldPosition());

        drawing = false;

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

            LineCompleted?.Invoke(length);

        }
        else
        {

            float radius;
            if (mode == Constants.Mode.FixedCompass) radius = fixedRadius;
            else radius = Vector2.Distance(startPos, endPos);

            fixedRadius = radius;

            drawingView.UpdateCircle(startPos, radius);
            drawingData.AddCircle(currentIndex, startPos, radius);
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

    // マウス座標 → ワールド座標
    // マウスまたはタッチの画面座標 → ワールド座標
    Vector2 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;

        Vector3 worldPosition = cam.ScreenToWorldPoint(
            new Vector3(
                pointerScreenPosition.x,
                pointerScreenPosition.y,
                -cam.transform.position.z
            )
        );

        return new Vector2(worldPosition.x, worldPosition.y);
    }

    void CancelDrawing()
    {
        drawing = false;
        drawingView.CancelPreview();

    }
    public void ResetBoard()
    {
        if (!inputEnabled) return;
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
        if (!inputEnabled) return;
        if (drawing) return;
        if (!CanUndo()) return;

        drawingData.UndoData(currentIndex);
        drawingView.RemoveDrawing(currentIndex);

        currentIndex--;
        currentMoves--;
        NotifyDrawingStateChanged();
    }

    public bool CanUndo()
    {
        return inputEnabled && currentIndex > undoBoundaryId;
    }

    public void LockUndoThroughCurrentDrawing()
    {
        undoBoundaryId = currentIndex;
    }

    void NotifyDrawingStateChanged()
    {
        DrawingStateChanged?.Invoke(currentMoves, CanUndo());
    }

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
        if (!inputEnabled) return;
        if (drawing) return;

        mode = newMode;
        ModeChanged?.Invoke(mode, fixedRadius != 0, drawingData.LatestCircleData());
    }

    public void StopInput()
    {
        inputEnabled = false;

        CancelDrawing();
        NotifyDrawingStateChanged();
    }

}