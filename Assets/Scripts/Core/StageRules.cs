using System;

public static class StageRules
{
    public static int[] GetQuestionLevels(int stageLevel)
    {
        if (stageLevel <= 1)
        {
            return new[] { 1, 1, 1, 1 };
        }

        if (stageLevel <= 3)
        {
            return new[] { 1, 2, 2, 3 };
        }

        if (stageLevel <= 4)
        {
            return new[] { 2, 3, 3, 4 };
        }

        if (stageLevel <= 6)
        {
            return new[] { 2, 4, 4, 5 };
        }

        if (stageLevel <= 7)
        {
            return new[] { 3, 4, 5, 7 };
        }

        return new[] { 7, 7, 7, 7 };
    }

    public static int GetTargetMovesForQuestion(int questionLevel)
    {
        switch (questionLevel)
        {
            case 1: return 2;
            case 2: return 3;
            case 3: return 3;
            case 4: return 4;
            case 5: return 5;
            case 6: return 7;
            case 7: return 4;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(questionLevel),
                    questionLevel,
                    "目標手数が定義されていない難易度です。"
                );
        }
    }

    public static int GetTargetMovesForStage(int stageLevel)
    {
        int total = 0;

        foreach (int questionLevel in GetQuestionLevels(stageLevel))
        {
            total += GetTargetMovesForQuestion(questionLevel);
        }

        return total;
    }
}