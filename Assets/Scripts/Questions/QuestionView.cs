using System.Collections.Generic;
using UnityEngine;

public class QuestionView : MonoBehaviour
{


    [SerializeField] GameObject boxPrefab;

    private readonly List<BoxManager> boxes = new();

    public void ShowQuestions(IReadOnlyList<Fraction> questions, bool spawnFromRight)
    {
        foreach (BoxManager box in boxes)
        {
            box.RemoveBox();
        }

        boxes.Clear();

        for (int i = 0; i < questions.Count; i++)
        {
            float targetX = Constants.FirstBoxX + Constants.BoxSpacing * i;

            float spawnX = spawnFromRight ? targetX + Constants.BoxMoveDistance : targetX;

            GameObject boxObject = Instantiate(
                boxPrefab,
                new Vector3(spawnX, Constants.BoxY - 0.4f, 0f),
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