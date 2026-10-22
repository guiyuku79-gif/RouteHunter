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
        StartStage(spawnFromRight: false);
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

    public (bool solvedAny, bool stageCompleted) CheckAnswer(float length)
    {
        var result = progress.CheckAnswer(length);

        foreach (int index in result.solvedIndices)
        {
            Debug.Log($"{index}番目の長さが完成");
            questionView.ShowSolved(index);

            score += 1;
        }

        return (
            result.solvedIndices.Count > 0,
            result.stageCompleted
        );
    }

    public bool TryAdvanceStage()
    {
        int nextStageLevel = stageLevel + 1;
        List<Fraction> nextQuestions;

        try
        {
            nextQuestions = StageQuestionGenerator.Generate(nextStageLevel);
        }
        catch (InvalidOperationException exception)
        {
            Debug.LogError($"次のステージの生成に失敗しました\n{exception.Message}");
            return false;
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Debug.LogError(
                $"出題設定を確認してください。\n{exception.Message}",
                this
                );
            return false;
        }

        stageLevel = nextStageLevel;
        ApplyQuestions(nextQuestions, spawnFromRight: true);

        return true;
    }
}
