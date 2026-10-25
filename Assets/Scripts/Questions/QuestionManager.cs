using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] QuestionView questionView;
    [SerializeField] private int stageLevel = 1;

    private int score = 0;
    public int Score => score;

    private readonly QuestionProgress progress = new();

    public void Initialize()
    {
        StartStage(spawnFromRight: true);
    }

    void StartStage(bool spawnFromRight)
    {
        List<Fraction> nextQuestions = StageQuestionGenerator.Generate(stageLevel);
        ApplyQuestions(nextQuestions, spawnFromRight);
    }

    void ApplyQuestions(List<Fraction> nextQuestions, bool spawnFromRight)
    {
        progress.SetQuestions(nextQuestions);
        questionView.ShowQuestions(nextQuestions, spawnFromRight);

    }

    public int GetTargetMoves()
    {
        return StageRules.GetTargetMovesForStage(stageLevel);
    }

    public (bool solvedAny, bool stageCompleted, int index) CheckAnswer(float length)
    {
        var result = progress.CheckAnswer(length);

        int completedIndex = -1;
        foreach (int index in result.solvedIndices)
        {
            Debug.Log($"{index}番目の長さが完成");
            questionView.ShowSolved(index);
            completedIndex = index;
            score += 1;
        }

        return (
            result.solvedIndices.Count > 0,
            result.stageCompleted,
            completedIndex
        );
    }

    public int TryAdvanceStage()
    {
        int nextStageLevel = stageLevel + 1;
        List<Fraction> nextQuestions = StageQuestionGenerator.Generate(nextStageLevel);

        stageLevel = nextStageLevel;
        ApplyQuestions(nextQuestions, spawnFromRight: true);

        return StageRules.GetTargetMovesForStage(stageLevel - 1);
    }

    public void RemoveBoxesForGameClear()
    {
        questionView.RemoveBoxes();
    }
}
