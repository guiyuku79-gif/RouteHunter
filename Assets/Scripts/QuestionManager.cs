using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    const int QuestionsPerStage = 4;
    const float FirstBoxX = -7f;
    const float BoxSpacing = 4f;
    const float BoxY = 4f;
    const float SpawnOffsetX = 18f;
    const float AnswerTolerance = 0.005f;

    [SerializeField] GameObject boxPrefab;

    int stageLevel = 1;
    readonly List<BoxManager> boxes = new();
    readonly List<Fraction> questions = new();
    readonly bool[] solved = new bool[QuestionsPerStage];

    void Start()
    {
        StartStage(spawnFromRight: false);
    }

    void StartStage(bool spawnFromRight)
    {
        questions.Clear();
        questions.AddRange(CreateQuestionsForStage());
        Array.Clear(solved, 0, solved.Length);
        boxes.Clear();

        for (int i = 0; i < QuestionsPerStage; i++)
        {
            float targetX = FirstBoxX + BoxSpacing * i;
            float spawnX = spawnFromRight ? targetX + SpawnOffsetX : targetX;
            GameObject boxObject = Instantiate(boxPrefab, new Vector3(spawnX, BoxY, 0f), Quaternion.identity);
            BoxManager box = boxObject.GetComponent<BoxManager>();
            box.Init(questions[i]);
            boxes.Add(box);

            if (spawnFromRight) box.ArriveBox();
        }
    }

    List<Fraction> CreateQuestionsForStage()
    {
        int[] levels;
        if (stageLevel <= 3) levels = new[] { 1, 2, 3, 4 };
        else levels = new[] { 1, 1, 2, 2 };


        List<Fraction> result = new(QuestionsPerStage);
        List<float> previousValues = new(QuestionsPerStage);

        foreach (int level in levels)
        {
            Fraction question;
            float value;
            do
            {
                question = Question.CreateTargetNumber(level);
                value = question.ToFloat();
            }
            while (value <= 0.1f || value >= 12f || previousValues.Contains(value));

            result.Add(question);
            previousValues.Add(value);
        }

        return result;
    }

    public void CheckAnswer(float length)
    {
        for (int i = 0; i < QuestionsPerStage; i++)
        {
            if (solved[i] || Math.Abs(questions[i].ToFloat() - length) > AnswerTolerance) continue;

            Debug.Log($"{i}番目の長さが完成");
            solved[i] = true;
            boxes[i].CloseBox();
        }

        if (AllQuestionsSolved()) AdvanceStage();
    }

    bool AllQuestionsSolved()
    {
        foreach (bool isSolved in solved)
        {
            if (!isSolved) return false;
        }

        return true;
    }

    void AdvanceStage()
    {
        foreach (BoxManager box in boxes)
        {
            if (box != null) box.RemoveBox();
        }

        stageLevel++;
        StartStage(spawnFromRight: true);
    }
}
