using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    int stageLebel;
    List<Fraction> questions;
    List<bool> isSolved;

    [SerializeField] GameObject boxPrefab;

    void Start()
    {
        CreateNewQuestions();
    }
    void CreateNewQuestions()
    {
        if (stageLebel <= 3) questions = CreateFourQuestions(new List<int> { 2, 3, 4, 5 });
        else questions = CreateFourQuestions(new List<int> { 1, 1, 1, 1 });
        for (int i = 0; i < 4; i++)
        {
            GameObject gameObject = Instantiate(boxPrefab);
            gameObject.transform.position = new Vector3(-7f + 4 * i, 4f, 0);
            gameObject.GetComponent<BoxManager>().Init(questions[i]);
        }
        isSolved = new List<bool> { false, false, false, false };
    }

    List<Fraction> CreateFourQuestions(List<int> levels)
    {
        List<Fraction> results = new();
        List<float> privious = new();
        for (int i = 0; i < levels.Count; i++)
        {
            bool isProblemed = true;
            Fraction fraction = Question.CreateTargetNumber(1);
            while (isProblemed)
            {
                isProblemed = false;
                fraction = Question.CreateTargetNumber(levels[i]);
                if (fraction.ToFloat() <= 0) isProblemed = true;
                if (fraction.ToFloat() >= 12) isProblemed = true;
                if (privious.Contains(fraction.ToFloat())) isProblemed = true;
            }
            results.Add(fraction);
            privious.Add(fraction.ToFloat());
        }
        foreach (int level in levels)
        {
            results.Add(Question.CreateTargetNumber(level));
        }
        return results;
    }

    public void CheckAnswer(float length)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Math.Abs(questions[i].ToFloat() - length) <= 0.005f)
            {
                Debug.Log($"{i}番目の長さが完成");
                isSolved[i] = true;
            }
        }
    }
}
