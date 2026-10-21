using System;
using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int maxTime = 300;

    public int timer;

    public event Action<int> TimeChanged;
    public event Action TimeExpired;

    public void StartTimer()
    {
        timer = maxTime;
        TimeChanged?.Invoke(timer);

        StartCoroutine(ChangeTime());
    }

    IEnumerator ChangeTime()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);

            timer--;
            TimeChanged?.Invoke(timer);
        }

        TimeExpired?.Invoke();
    }
}