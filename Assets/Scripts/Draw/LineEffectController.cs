using System.Collections;
using UnityEngine;

public class LineEffectController : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }
    public void Init(LineData lineData, int boxIndex)
    {
        StartCoroutine(MoveToBox(lineData, boxIndex, 0.7f, 0.3f, 15));
    }

    IEnumerator MoveToBox(LineData lineData, int boxIndex, float Movetime, float closeTime, float division)
    {
        var wait = new WaitForSeconds(Movetime / division);

        Vector2 centerLinePos = (lineData.Start + lineData.End) / 2;

        float halfLength = Vector2.Distance(lineData.Start, lineData.End) / 2;
        Vector2 targetPos = new Vector2(Constants.FirstBoxX + boxIndex * Constants.BoxSpacing,
        Constants.BoxY + halfLength);

        float angle = Mathf.Atan2(lineData.End.y - lineData.Start.y, lineData.End.x - lineData.Start.x);

        //boxまでの移動
        for (int i = 1; i < division + 1; i++)
        {
            Vector2 newCenter = centerLinePos + (targetPos - centerLinePos) * i / division;

            float newAngle;
            if (angle > 0)
            {
                newAngle = angle + (Mathf.PI / 2 - angle) * i / division;
            }
            else
            {
                newAngle = angle + (-Mathf.PI / 2 - angle) * i / division;
            }

            Vector2 direction = new Vector2(
                Mathf.Cos(newAngle),
                Mathf.Sin(newAngle)
            ) * halfLength;

            lineRenderer.SetPosition(0, newCenter + direction);
            lineRenderer.SetPosition(1, newCenter - direction);
            yield return wait;
        }

        wait = new WaitForSeconds(closeTime / division);

        Vector2 startPos = new Vector2(Constants.FirstBoxX + boxIndex * Constants.BoxSpacing,
        Constants.BoxY);
        Vector2 endPos = new Vector2(Constants.FirstBoxX + boxIndex * Constants.BoxSpacing,
        Constants.BoxY + halfLength * 2);
        //boxに格納
        for (int i = 1; i < division + 1; i++)
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos + (startPos - endPos) * i / division);
            yield return wait;
        }

        Destroy(this.gameObject);
    }
}
