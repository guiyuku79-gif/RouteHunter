using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    int stageLebel;
    List<Fraction> questions;
    List<bool> isSolved;

    void Start()
    {
        CreateNewQuestions();
    }
    void CreateNewQuestions()
    {
        if (stageLebel <= 3) questions = CreateFourQuestions(new List<int> { 1, 1, 2, 2 });
        else questions = CreateFourQuestions(new List<int> { 1, 1, 1, 1 });
        foreach (Fraction fraction in questions)
        {
            Debug.Log(fraction.FractionToString());
        }
        isSolved = new List<bool> { false, false, false, false };
    }

    List<Fraction> CreateFourQuestions(List<int> levels)
    {
        List<Fraction> results = new();
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
            if (Math.Abs(questions[i].ToFloat() - length) <= 0.001f)
            {
                Debug.Log($"{i}番目の長さが完成");
                isSolved[i] = true;
            }
        }
    }
}
