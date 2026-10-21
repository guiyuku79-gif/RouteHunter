using UnityEngine;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject compassButton;
    [SerializeField] GameObject lineButton;
    [SerializeField] GameObject fixedCompassButton;
    [SerializeField] GameObject selectedSign;

    [SerializeField] SpriteRenderer compassButtonSprite;
    [SerializeField] SpriteRenderer lineButtonSprite;
    [SerializeField] SpriteRenderer fixedCompassButtonSprite;
    [SerializeField] SpriteRenderer undoButton;
    [SerializeField] TextMeshProUGUI moveUI;
    [SerializeField] GameObject beltConveyor;
    [SerializeField] TextMeshProUGUI timerUI;


    public void UpdateModeUI(Constants.Mode selectedMode, bool isFixedCompassAvailable)
    {
        fixedCompassButtonSprite.color =
            isFixedCompassAvailable
                ? Color.white
                : Color.gray;


        if (selectedMode == Constants.Mode.Line)
        {
            selectedSign.transform.position = lineButton.transform.position;
        }
        if (selectedMode == Constants.Mode.Compass)
        {
            selectedSign.transform.position = compassButton.transform.position;
        }
        if (selectedMode == Constants.Mode.FixedCompass)
        {
            selectedSign.transform.position = fixedCompassButton.transform.position;
        }

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

    public void UpdateTimerUI(int remainingSeconds)
    {
        int minute = remainingSeconds / 60;
        int second = remainingSeconds % 60;

        timerUI.text = $"{minute:00}:{second:00}";
    }
}
