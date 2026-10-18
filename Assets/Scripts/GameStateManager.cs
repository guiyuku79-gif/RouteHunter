using UnityEngine;
using TMPro;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int maxTime = 300;
    [SerializeField] private TextMeshProUGUI timerUI;

    public int timer;
    void Start()
    {
        timer = maxTime;
        StartCoroutine(ChangeTime());
    }

    IEnumerator ChangeTime()
    {
        while (true)
        {
            timer--;
            if (timer < 0)
            {
                timer = 0;
                break;
            }
            int minute = timer / 60;
            int second = timer % 60;
            timerUI.text = minute.ToString("00") + ":" + second.ToString("00");
            yield return new WaitForSeconds(1f);
        }

    }
}
