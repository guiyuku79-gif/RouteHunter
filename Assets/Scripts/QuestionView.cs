using System.Collections.Generic;
using UnityEngine;

public class QuestionView : MonoBehaviour
{
    const float FirstBoxX = -6f;
    const float BoxSpacing = 4f;
    const float BoxY = 4f;

    [SerializeField] GameObject boxPrefab;

    private readonly List<BoxManager> boxes = new();

    public void ShowQuestions(IReadOnlyList<Fraction> questions, bool spawnFromRight)
    {
        foreach (BoxManager box in boxes)
        {
            box.RemoveBox();
        }

        boxes.Clear();

        for(int i = 0; i < questions.Count; i++)
        {
            float targetX = FirstBoxX + BoxSpacing * i;

            float spawnX = spawnFromRight ? targetX + Constants.BoxMoveDistance : targetX;

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

    public void ShowSolved(int questionIndex)
    {
        boxes[questionIndex].CloseBox();
    }
}