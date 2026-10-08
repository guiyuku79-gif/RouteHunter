using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxManager : MonoBehaviour
{
    const float CharacterSpacing = 0.125f;
    const float BoxTravelDistance = 18f;
    const float BoxTravelDuration = 1.5f;

    [SerializeField] GameObject letterPrefab;
    [FormerlySerializedAs("linePrefab")]
    [SerializeField] LineRenderer fractionBarPrefab;
    [SerializeField] Sprite closedBox;

    public void Init(Fraction fraction)
    {
        DrawFraction(fraction);
    }

    void DrawFraction(Fraction fraction)
    {
        var parts = fraction.FractionToString();
        if (parts.denominator == "1")
        {
            DrawPolynomial(parts.numerator, 0f);
            return;
        }

        DrawPolynomial(parts.numerator, 0.13f);
        DrawPolynomial(parts.denominator, -0.13f);
        DrawFractionBar(parts.numerator.Length, parts.denominator.Length);
    }

    void DrawFractionBar(int numeratorLength, int denominatorLength)
    {
        float barLength = Mathf.Max(numeratorLength, denominatorLength);
        LineRenderer bar = Instantiate(fractionBarPrefab, transform, false);
        bar.positionCount = 2;
        bar.SetPosition(0, new Vector3(-barLength * CharacterSpacing / 2f, 0f, 0f));
        bar.SetPosition(1, new Vector3(barLength * CharacterSpacing / 2f, 0f, 0f));
    }

    void DrawPolynomial(string text, float height)
    {
        bool isRooted = false;
        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];
            if (character == '+' || character == '-') isRooted = false;

            GameObject letter = Instantiate(letterPrefab, transform, false);
            float x = CharacterSpacing * i - (text.Length - 1) / 2 * CharacterSpacing;
            letter.transform.localPosition = new Vector3(x, height, 0f);
            letter.GetComponent<LetterController>().Init(character, isRooted);

            if (character == '√') isRooted = true;
        }
    }

    public void CloseBox()
    {
        GetComponent<SpriteRenderer>().sprite = closedBox;
    }

    public void RemoveBox()
    {
        StartCoroutine(MoveBox(Vector3.left, destroyWhenFinished: true));
    }

    public void ArriveBox()
    {
        StartCoroutine(MoveBox(Vector3.left, destroyWhenFinished: false));
    }

    IEnumerator MoveBox(Vector3 direction, bool destroyWhenFinished)
    {
        Vector3 start = transform.localPosition;
        Vector3 target = start + direction.normalized * BoxTravelDistance;
        float elapsed = 0f;

        while (elapsed < BoxTravelDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, target, elapsed / BoxTravelDuration);
            yield return null;
        }

        transform.localPosition = target;
        if (destroyWhenFinished) Destroy(gameObject);
    }
}
