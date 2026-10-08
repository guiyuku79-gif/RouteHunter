using TMPro;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] SpriteRenderer root;

    public void Init(char letter, bool isRooted)
    {
        numberText.text = letter.ToString();
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
