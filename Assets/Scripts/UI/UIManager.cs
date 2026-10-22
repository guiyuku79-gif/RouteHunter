using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject compassButton;
    [SerializeField] GameObject lineButton;
    [SerializeField] GameObject fixedCompassButton;
    [SerializeField] GameObject selectedSign;

    [SerializeField] SpriteRenderer fixedCompassButtonSprite;
    [SerializeField] SpriteRenderer undoButton;
    [SerializeField] TextMeshProUGUI moveUI;
    [SerializeField] GameObject beltConveyor;
    [SerializeField] TextMeshProUGUI timerUI;
    [SerializeField] TextMeshProUGUI scoreUI;

    [SerializeField] GameObject resultUI;
    [SerializeField] TextMeshProUGUI finalScoreUI;

    [SerializeField] LineRenderer HighlightCircle;

    public void UpdateModeUI(Constants.Mode selectedMode, bool isFixedCompassAvailable, CircleData circleData)
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

        //直近の円をハイライト
        if (selectedMode == Constants.Mode.FixedCompass)
        {
            HighlightCircle.positionCount = 2;
            HighlightCircle.SetPosition(0, circleData.Center);
            HighlightCircle.SetPosition(1, circleData.Center + Vector2.right * circleData.Radius);
        }
        else
        {
            HighlightCircle.positionCount = 0;
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

    public void UpdateScoreUI(int score)
    {
        scoreUI.text = $"Score : {score}";
    }

    public void ShowResultUI(int score)
    {
        resultUI.SetActive(true);
        finalScoreUI.text = $"Score : {score}";
    }
}
