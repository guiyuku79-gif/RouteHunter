using UnityEngine;

public class LineEffectManager : MonoBehaviour
{
    [SerializeField] GameObject lineEffectPrefab;
    public void EffectCompletedLine(LineData lineData, int boxNumber)
    {
        GameObject gameObject = Instantiate(lineEffectPrefab);
        gameObject.GetComponent<LineEffectController>().Init(lineData,boxNumber);
    }
}
