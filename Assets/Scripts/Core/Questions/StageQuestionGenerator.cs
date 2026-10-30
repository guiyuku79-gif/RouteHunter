using System;
using System.Collections.Generic;

public static class StageQuestionGenerator
{
    const int QuestionsPerStage = 4;
    const int MaxGenerationAttempts = 1000;

    public static List<Fraction> Generate(int stageLevel, IReadOnlyList<Fraction> previousQuestions = null)
    {
        int[] levels = StageRules.GetQuestionLevels(stageLevel);

        if (levels.Length != QuestionsPerStage)
        {
            throw new InvalidOperationException(
                $"問題数が一致しません。期待値: {QuestionsPerStage}、実際: {levels.Length}"
            );
        }

        List<Fraction> result = new(QuestionsPerStage);
        List<float> previousValues = new(QuestionsPerStage);

        // 直前のフェーズと同じ値は、表記が異なる場合も候補から除外する。
        if (previousQuestions != null)
        {
            foreach (Fraction question in previousQuestions)
            {
                previousValues.Add(question.ToFloat());
            }
        }

        foreach (int level in levels)
        {
            bool generated = false;

            for (int attempt = 0; attempt < MaxGenerationAttempts; attempt++)
            {
                Fraction question = QuestionGenerator.CreateTargetNumber(level);
                float value = question.ToFloat();

                if (float.IsNaN(value) || float.IsInfinity(value)) continue;

                if (value <= 0.1f || value >= 10f) continue;

                bool overlaps = previousValues.Exists(previous => Math.Abs(previous - value) <= 2f * QuestionProgress.AnswerTolerance);

                if (overlaps) continue;

                result.Add(question);
                previousValues.Add(value);
                generated = true;
                break;
            }

            if (!generated)
            {
                throw new InvalidOperationException(
                    $"問題を生成できませんでした。" +
                    $" ステージ: {stageLevel}" +
                    $" 難易度: {level}" +
                    $" 試行上限: {MaxGenerationAttempts}"
                );
            }
        }

        return result;
    }
}
