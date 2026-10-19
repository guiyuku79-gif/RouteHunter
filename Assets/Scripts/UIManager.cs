using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] SpriteRenderer compassButton;
    [SerializeField] SpriteRenderer lineButton;
    [SerializeField] SpriteRenderer fixedCompassButton;
    [SerializeField] SpriteRenderer undoButton;
    [SerializeField] TextMeshProUGUI moveUI;
    [SerializeField] GameObject beltConveyor;

    public void UpdateModeUI(Constants.Mode selectedMode)
    {
        lineButton.color =
            selectedMode == Constants.Mode.Line
                ? Color.gray
                : Color.white;

        compassButton.color =
            selectedMode == Constants.Mode.Compass
                ? Color.gray
                : Color.white;

        fixedCompassButton.color =
            selectedMode == Constants.Mode.FixedCompass
                ? Color.gray
                : Color.white;
    }

    public void UpdateMoveUI(int currentMoves, int targetMoves, bool canUndo)
    {
        //手数の表示
        moveUI.text = $"{currentMoves} moves\ntargetMoves {targetMoves}";
        //戻るボタンの表示
        if (canUndo) undoButton.color = Color.white;
        else undoButton.color = Color.gray;
    }

    public void MoveConveyorUI()
    {
        StartCoroutine(MoveBeltConveyorLeft());
    }
    IEnumerator MoveBeltConveyorLeft()
    {
        Vector3 start = beltConveyor.transform.position;
        Vector3 target = start + Vector3.left.normalized * Constants.BoxMoveDistance;
        float elapsed = 0f;

        while (elapsed < Constants.BoxMoveDuration)
        {
            elapsed += Time.deltaTime;
            beltConveyor.transform.position = Vector3.Lerp(start, target, elapsed / Constants.BoxMoveDuration);
            yield return null;
        }

        beltConveyor.transform.position = start;
    }
}
