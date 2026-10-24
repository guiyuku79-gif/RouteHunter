using System.Collections;
using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] QuestionManager questionManager;
    [SerializeField] DrawingManager drawingManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] LineEffectManager lineEffectManager;

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
    void HandleLineCompleted(int lineId, LineData lineData)
    {
        if (stageTransitionPending) return;

        float length = Vector2.Distance(lineData.Start, lineData.End);
        var result = questionManager.CheckAnswer(length);

        HandleScoreChanged();

        // 最後の正解も、移行に失敗した場合にUndoできないようにする。
        if (result.solvedAny)
        {
            drawingManager.LockUndoThroughCurrentDrawing();

            drawingManager.RemoveSolvedLine(lineId);
            lineEffectManager.EffectCompletedLine(lineData, result.index);
        }

        if (result.stageCompleted)
        {
            stageTransitionPending = true;
            AdvanceToNextStage();
        }
    }

    void AdvanceToNextStage()
    {
        int extraTime = Mathf.Max(0, (questionManager.GetTargetMoves() - drawingManager.CurrentMoves) * 10);

        gameStateManager.AddExtraTime(extraTime);
        uiManager.UpdateExtraTime(gameStateManager.timer, extraTime);

        stageTransitionPending = false;
        StartCoroutine(WaitForNewStage());
    }
    IEnumerator WaitForNewStage()
    {
        yield return new WaitForSeconds(1.2f);
        questionManager.TryAdvanceStage();
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