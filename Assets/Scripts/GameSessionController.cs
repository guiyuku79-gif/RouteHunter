using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] QuestionManager questionManager;
    [SerializeField] DrawingManager drawingManager;
    [SerializeField] UIManager uiManager;

    void OnEnable()
    {
        drawingManager.LineCompleted += HandleLineCompleted;
        drawingManager.DrawingStateChanged += HandleDrawingStateChanged;
        drawingManager.ModeChanged += HandleModeChanged;
    }

    void OnDisable()
    {
        if (drawingManager != null)
        {
            drawingManager.LineCompleted -= HandleLineCompleted;
            drawingManager.DrawingStateChanged -= HandleDrawingStateChanged;
            drawingManager.ModeChanged -= HandleModeChanged;
        }
    }
    void HandleLineCompleted(float length)
    {
        var result = questionManager.CheckAnswer(length);

        if (result.stageCompleted)
        {
            AdvanceToNextStage();
            return;
        }

        if (result.solvedAny)
        {
            drawingManager.LockUndoThroughCurrentDrawing();
        }
    }

    void AdvanceToNextStage()
    {
        uiManager.MoveConveyorUI();
        questionManager.AdvanceStage();
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

}