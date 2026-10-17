using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] SpriteRenderer compassButton;
    [SerializeField] SpriteRenderer lineButton;
    [SerializeField] SpriteRenderer fixedCompassButton;
    [SerializeField] SpriteRenderer undoButton;
    [SerializeField] TextMeshProUGUI moveUI;

    public void CompassModeUI()
    {
        compassButton.color = Color.gray;
        lineButton.color = Color.white;
        fixedCompassButton.color = Color.white;
    }
    public void LineModeUI()
    {
        compassButton.color = Color.white;
        lineButton.color = Color.gray;
        fixedCompassButton.color = Color.white;
    }
    public void FixedCompassModeUI()
    {
        compassButton.color = Color.white;
        lineButton.color = Color.white;
        fixedCompassButton.color = Color.gray;
    }

    public void UpdateMoveUI(int currentMoves, int[] levels,bool canUndo)
    {
        int sum = 0;
        foreach (int level in levels) sum += level;
        moveUI.text = $"{currentMoves} moves\ntargetMoves {sum}";

        if (canUndo) undoButton.color = Color.white;
        else undoButton.color = Color.gray;
    }


}
