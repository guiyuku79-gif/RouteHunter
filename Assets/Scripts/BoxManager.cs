using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoxManager : MonoBehaviour
{
    private Fraction fraction;
    [SerializeField] GameObject letterPrefab;
    [SerializeField] LineRenderer linePrefab;

    [SerializeField] Sprite closedBox;

    public void Init(Fraction fraction)
    {
        this.fraction = fraction;
        DrawFraction();
    }

    void DrawFraction()
    {
        var len = fraction.FractionToString();
        if (len.denominator == "1")
        {
            DrawPolynomial(len.numerator, 0);
        }
        else
        {
            DrawPolynomial(len.numerator, 0.13f);
            DrawPolynomial(len.denominator, -0.13f);
            float barLen = Mathf.Max(len.numerator.Length, len.denominator.Length);
            LineRenderer lineRenderer = Instantiate(linePrefab, transform, false);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(-barLen * 0.125f / 2, 0, 0));
            lineRenderer.SetPosition(1, new Vector3(barLen * 0.125f / 2, 0, 0));
        }
    }

    void DrawPolynomial(string str, float height)
    {
        bool isRooted = false;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i].ToString() == "+" || str[i].ToString() == "-") isRooted = false;

            GameObject gameObject = Instantiate(letterPrefab);
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.localPosition = new Vector3(0.125f * i - (str.Count() - 1) / 2 * 0.125f, height, 0);
            gameObject.GetComponent<LetterController>().Init(str[i], isRooted);

            if (str[i].ToString() == "√") isRooted = true;
        }
    }

    public void CloseBox()
    {
        GetComponent<SpriteRenderer>().sprite = closedBox;
    }
}
