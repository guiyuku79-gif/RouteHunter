using System;
using System.Collections.Generic;

public class QuestionProgress
{
    public const float AnswerTolerance = 0.005f;

    private readonly List<Fraction> questions = new();
    private bool[] solved = Array.Empty<bool>();

    public void SetQuestions(IReadOnlyList<Fraction> newQuestions)
    {
        questions.Clear();

        for (int i = 0; i < newQuestions.Count; i++)
        {
            questions.Add(newQuestions[i]);
        }
        solved = new bool[questions.Count];
    }

    public (List<int> solvedIndices, bool stageCompleted) CheckAnswer(float length)
    {
        List<int> solvedIndices = new();

        for (int i = 0; i < questions.Count; i++)
        {
            if (solved[i]) continue;

            if (Math.Abs(questions[i].ToFloat() - length) > AnswerTolerance) continue;

            solved[i] = true;
            solvedIndices.Add(i);
            break;
        }

        bool stageCompleted = solvedIndices.Count > 0 && AllQuestionsSolved();

        return (solvedIndices, stageCompleted);

    }

    private bool AllQuestionsSolved()
    {
        foreach (bool isSolved in solved)
        {
            if (!isSolved) return false;
        }
        return true;
    }
}