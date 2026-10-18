using UnityEngine;

public class GameSessionController : MonoBehaviour
{
    [SerializeField] QuestionManager questionManager;
    [SerializeField] DrawingManager drawingManager;
    [SerializeField] UIManager uiManager;

    void OnEnable()
    {
        questionManager.StageCompleted += HandleStageCompleted;
    }

    void OnDisable()
    {
        if (questionManager != null)
        {
            questionManager.StageCompleted -= HandleStageCompleted;
        }
    }

    void HandleStageCompleted()
    {
        uiManager.MoveConveyorUI();
        questionManager.AdvanceStage();
        drawingManager.BeginStage();
    }

}