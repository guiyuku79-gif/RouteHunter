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
        if (!questionManager.TryAdvanceStage())
        {
            return;
        }

        stageTransitionPending = false;

        uiManager.MoveConveyorUI();
        drawingManager.BeginStage();
    }

    void HandleDrawingStateChanged(int currentMoves, bool canUndo)
    {
        uiManager.UpdateMoveUI(currentMoves, questionManager.GetTargetMoves(), canUndo);
    }

    void HandleModeChanged(Constants.Mode selectedMode)
    {
        uiManager.UpdateModeUI(selectedMode);
    }

    void HandleTimeChanged(int remainingSeconds)
    {
        uiManager.UpdateTimerUI(remainingSeconds);
    }

    void HandleTimeExpired()
    {
        drawingManager.StopInput();
    }

}