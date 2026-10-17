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

    public void UpdateMoveUI(int currentMoves, int[] levels, bool canUndo)
    {
        //手数の表示
        int sum = 0;
        foreach (int level in levels) sum += level;
        moveUI.text = $"{currentMoves} moves\ntargetMoves {sum}";
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
