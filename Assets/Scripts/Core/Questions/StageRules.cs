using System;

public static class StageRules
{
    public static int[] GetQuestionLevels(int stageLevel)
    {
        if (stageLevel == 1) return new[] { 1, 1, 2, 2 };
        if (stageLevel == 2) return new[] { 1, 2, 3, 3 };
        if (stageLevel == 3) return new[] { 2, 2, 3, 4 };
        if (stageLevel == 4) return new[] { 2, 4, 4, 5 };
        if (stageLevel == 5) return new[] { 4, 4, 5, 6 };
        if (stageLevel == 6) return new[] { 4, 5, 6, 7 };
        if (stageLevel == 7) return new[] { 4, 5, 6, 7 };
        if (stageLevel == 8) return new[] { 5, 7, 7, 8 };
        if (stageLevel == 9) return new[] { 5, 7, 8, 9 };
        if (stageLevel == 10) return new[] { 5, 7, 9, 9 };
        if (stageLevel == 11) return new[] { 1, 1, 1, 1 };
        if (stageLevel == 12) return new[] { 7, 7, 9, 10 };
        if (stageLevel == 13) return new[] { 7, 7, 9, 10 };


        // if (stageLevel <= 1) return new[] { 1, 1, 1, 1 };
        // if (stageLevel <= 2) return new[] { 2, 2, 2, 2 };
        // if (stageLevel <= 3) return new[] { 3, 3, 3, 3 };
        // if (stageLevel <= 4) return new[] { 4, 4, 4, 4 };
        // if (stageLevel <= 5) return new[] { 5, 5, 5, 5 };
        // if (stageLevel <= 6) return new[] { 6, 6, 6, 6 };
        // if (stageLevel <= 7) return new[] { 7, 7, 7, 7 };
        // if (stageLevel <= 8) return new[] { 8, 8, 8, 8 };


        return new[] { 10, 10, 10, 10 };
    }

    public static int GetTargetMovesForQuestion(int questionLevel)
    {
        switch (questionLevel)
        {
            case 1: return 2;
            case 2: return 2;
            case 3: return 2;
            case 4: return 3;
            case 5: return 4;
            case 6: return 3;
            case 7: return 3;
            case 8: return 7;
            case 9: return 7;
            case 10: return 7;

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