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
        questionManager.StageCompleted += HandleStageCompleted;
    }

    void OnDisable()
    {
        if (drawingManager != null)
        {
            drawingManager.LineCompleted -= HandleLineCompleted;
            drawingManager.DrawingStateChanged -= HandleDrawingStateChanged;
            drawingManager.ModeChanged -= HandleModeChanged;
        }

        if (questionManager != null)
        {
            questionManager.StageCompleted -= HandleStageCompleted;
        }
    }
    void HandleLineCompleted(float length)
    {
        int stageBeforeAnswer = questionManager.stageLevel;

        bool isCompleted = questionManager.CheckAnswer(length);

        if (questionManager.stageLevel != stageBeforeAnswer)
        {
            return;
        }

        if (isCompleted)
        {
            drawingManager.LockUndoThroughCurrentDrawing();
        }
    }

    void HandleStageCompleted()
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