using System.Linq;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    private Fraction fraction;
    [SerializeField] GameObject letterPrefab;

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
            bool isRooted = false;
            for (int i = 0; i < len.numerator.Length; i++)
            {
                if (len.numerator[i].ToString() == "+" || len.numerator[i].ToString() == "-") isRooted = false;

                GameObject gameObject = Instantiate(letterPrefab);
                gameObject.transform.SetParent(this.transform);
                gameObject.transform.localPosition = new Vector3(0.125f * i - (len.numerator.Count() - 1) / 2 * 0.125f, 0, 0);
                gameObject.GetComponent<LetterController>().Init(len.numerator[i], isRooted);

                if (len.numerator[i].ToString() == "√") isRooted = true;
            }
        }
    }
}
