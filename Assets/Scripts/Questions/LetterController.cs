using TMPro;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] SpriteRenderer root;

    public void Init(char letter, bool isRooted)
    {
        if (letter.ToString() == "f")
        {
            transform.position += Vector3.right * 0.12f + Vector3.up * 0.12f;
            numberText.text = $"<size=50%>4</size>";
        }
        else
        {
            numberText.text = letter.ToString();
        }

        if (isRooted)
        {
            root.enabled = true;
        }
        else
        {
            root.enabled = false;
        }
    }
}
