using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    const int QuestionsPerStage = 4;
    const float FirstBoxX = -6f;
    const float BoxSpacing = 4f;
    const float BoxY = 4f;
    const float AnswerTolerance = 0.005f;
    const int MaxGenerationAttempts = 1000;

    [SerializeField] GameObject boxPrefab;

    [SerializeField] private int stageLevel = 1;

    readonly List<BoxManager> boxes = new();
    readonly List<Fraction> questions = new();
    readonly bool[] solved = new bool[QuestionsPerStage];

    void Start()
    {
        StartStage(spawnFromRight: false);
    }

    void StartStage(bool spawnFromRight)
    {
        List<Fraction> nextQuestions = CreateQuestionsForStage(stageLevel);
        ApplyQuestions(nextQuestions, spawnFromRight);
    }

    List<Fraction> CreateQuestionsForStage(int targetStageLevel)
    {
        int[] levels = StageRules.GetQuestionLevels(targetStageLevel);

        if (levels.Length != QuestionsPerStage)
        {
            throw new InvalidOperationException(
                $"問題数が一致しません。期待値: {QuestionsPerStage}、実際: {levels.Length}"
            );
        }

        List<Fraction> result = new(QuestionsPerStage);
        List<float> previousValues = new(QuestionsPerStage);

        foreach (int level in levels)
        {
            bool generated = false;

            for (int attempt = 0; attempt < MaxGenerationAttempts; attempt++)
            {
                Fraction question = QuestionGenerator.CreateTargetNumber(level);
                float value = question.ToFloat();

                if (float.IsNaN(value) || float.IsInfinity(value)) continue;

                if (value <= 0.1f || value >= 10f) continue;

                if (previousValues.Contains(value)) continue;

                result.Add(question);
                previousValues.Add(value);
                generated = true;
                break;
            }

            if (!generated)
            {
                throw new InvalidOperationException(
                    $"問題を生成できませんでした。" +
                    $" ステージ: {targetStageLevel}" +
                    $" 難易度: {level}" +
                    $" 試行上限: {MaxGenerationAttempts}"
                );
            }
        }

        return result;
    }

    void ApplyQuestions(List<Fraction> nextQuestions, bool spawnFromRight)
    {
        questions.Clear();
        questions.AddRange(nextQuestions);

        Array.Clear(solved, 0, solved.Length);
        boxes.Clear();

        for (int i = 0; i < QuestionsPerStage; i++)
        {
            float targetX = FirstBoxX + BoxSpacing * i;
            float spawnX = spawnFromRight
                ? targetX + Constants.BoxMoveDistance
                : targetX;

            GameObject boxObject = Instantiate(
                boxPrefab,
                new Vector3(spawnX, BoxY, 0f),
                Quaternion.identity
            );

            BoxManager box = boxObject.GetComponent<BoxManager>();
            box.Init(questions[i]);
            boxes.Add(box);

            if (spawnFromRight)
            {
                box.ArriveBox();
            }
        }
    }

    public int GetTargetMoves()
    {
        return StageRules.GetTargetMovesForStage(stageLevel);
    }

    public (bool solvedAny, bool stageCompleted) CheckAnswer(float length)
    {
        bool solvedAny = false;
        for (int i = 0; i < QuestionsPerStage; i++)
        {
            if (solved[i] || Math.Abs(questions[i].ToFloat() - length) > AnswerTolerance) continue;

            Debug.Log($"{i}番目の長さが完成");
            solvedAny = true;
            solved[i] = true;
            boxes[i].CloseBox();
        }

        bool stageCompleted = solvedAny && AllQuestionsSolved();

        return (solvedAny, stageCompleted);
    }

    bool AllQuestionsSolved()
    {
        foreach (bool isSolved in solved)
        {
            if (!isSolved) return false;
        }

        return true;
    }
    
    public bool TryAdvanceStage()
    {
        int nextStageLevel = stageLevel + 1;
        List<Fraction> nextQuestions;

        try
        {
            nextQuestions = CreateQuestionsForStage(nextStageLevel);
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

        foreach (BoxManager box in boxes)
        {
            if (box != null)
            {
                box.RemoveBox();
            }
        }

        stageLevel = nextStageLevel;
        ApplyQuestions(nextQuestions, spawnFromRight: true);

        return true;
    }
}
