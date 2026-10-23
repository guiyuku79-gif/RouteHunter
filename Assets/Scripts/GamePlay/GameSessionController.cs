using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] QuestionManager questionManager;
    [SerializeField] DrawingManager drawingManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] GameStateManager gameStateManager;

    bool stageTransitionPending;

    void Start()
    {
        questionManager.Initialize();
        drawingManager.Initialize();
        gameStateManager.StartTimer();

        HandleScoreChanged();
    }

    void OnEnable()
    {
        drawingManager.LineCompleted += HandleLineCompleted;
        drawingManager.DrawingStateChanged += HandleDrawingStateChanged;
        drawingManager.ModeChanged += HandleModeChanged;

        gameStateManager.TimeChanged += HandleTimeChanged;
        gameStateManager.TimeExpired += HandleTimeExpired;
    }

    void OnDisable()
    {
        if (drawingManager != null)
        {
            drawingManager.LineCompleted -= HandleLineCompleted;
            drawingManager.DrawingStateChanged -= HandleDrawingStateChanged;
            drawingManager.ModeChanged -= HandleModeChanged;
        }

        gameStateManager.TimeChanged -= HandleTimeChanged;
        gameStateManager.TimeExpired -= HandleTimeExpired;
    }
    void HandleLineCompleted(float length)
    {
        if (stageTransitionPending) return;

        var result = questionManager.CheckAnswer(length);

        HandleScoreChanged();

        // 最後の正解も、移行に失敗した場合にUndoできないようにする。
        if (result.solvedAny)
        {
            drawingManager.LockUndoThroughCurrentDrawing();
        }

        if (result.stageCompleted)
        {
            stageTransitionPending = true;
            AdvanceToNextStage();
        }
    }

    void AdvanceToNextStage()
    {
        var result = questionManager.TryAdvanceStage();
        if (!result.canAdvanced)
        {
            return;
        }

        gameStateManager.AddExtraTime(drawingManager.CurrentMoves, result.currentTargetMoves);

        stageTransitionPending = false;

        uiManager.MoveConveyorUI();
        drawingManager.BeginStage();
    }

    void HandleDrawingStateChanged(int currentMoves, bool canUndo)
    {
        uiManager.UpdateMoveUI(currentMoves, questionManager.GetTargetMoves(), canUndo);
    }

    void HandleModeChanged(Constants.Mode selectedMode, bool isFixedCompassAvailable, CircleData circleData)
    {
        uiManager.UpdateModeUI(selectedMode, isFixedCompassAvailable, circleData);
    }

    void HandleTimeChanged(int remainingSeconds)
    {
        uiManager.UpdateTimerUI(remainingSeconds);
    }

    void HandleTimeExpired()
    {
        drawingManager.StopInput();
        uiManager.ShowResultUI(questionManager.Score);
    }

    void HandleScoreChanged()
    {
        uiManager.UpdateScoreUI(questionManager.Score);
    }

}