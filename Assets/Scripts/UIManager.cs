using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] SpriteRenderer compassButton;
    [SerializeField] SpriteRenderer lineButton;
    [SerializeField] SpriteRenderer fixedCompassButton;
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

    public void updateMoveUI(int currentMoves, int[] levels)
    {
        int sum = 0;
        foreach (int level in levels) sum += level;
        moveUI.text = $"{currentMoves} moves\ntargetMoves {sum}";
    }
}
