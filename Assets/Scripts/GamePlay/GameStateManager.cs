using System;
using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int maxTime = 300;

    public int timer;

    private float timeCount;

    private bool isTimerMoved = false;

    public event Action<int> TimeChanged;
    public event Action TimeExpired;

    public void StartTimer()
    {
        timer = maxTime;
        isTimerMoved = true;
        TimeChanged?.Invoke(timer);

    }

    void Update()
    {
        if (!isTimerMoved) return;

        timeCount += Time.deltaTime;
        if (timeCount >= 1f)
        {
            timeCount = 0;
            timer--;
            TimeChanged?.Invoke(timer);
        }

        if (timer <= 0f)
        {
            TimeExpired?.Invoke();
            isTimerMoved = false;
        }


    }


    public void AddExtraTime(int extraTime)
    {
        StartCoroutine(WaitWhileMove(extraTime));
    }

    IEnumerator WaitWhileMove(int extraTime)
    {
        isTimerMoved = false;
        yield return new WaitForSeconds(Constants.BoxMoveDuration + 1f);

        timer += extraTime;
        isTimerMoved = true;
    }
}