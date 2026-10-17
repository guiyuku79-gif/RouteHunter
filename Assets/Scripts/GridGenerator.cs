using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] Vector2 gridStartPos;
    [SerializeField] Vector2 gridNumber;
    [SerializeField] float gridSize;
    [SerializeField] Color32 gridColor;
    [SerializeField] float gridWidth;

    [SerializeField] LineRenderer gridPrefab;

    public (List<Vector2> intersections,List<LineData> lines) GenerateGrid()
    {
        List<Vector2> gridIntersection = new List<Vector2>();
        List<LineData> lines = new List<LineData>();
        for (int i = 0; i < gridNumber.y + 1; i++)
        {
            CreateOneGrid(new Vector2(0, i * gridSize), new Vector2(gridNumber.x * gridSize, i * gridSize));
            lines.Add(new LineData(new Vector2(0, i * gridSize), new Vector2(gridNumber.x * gridSize, i * gridSize)));
        }
        for (int i = 0; i < gridNumber.x + 1; i++)
        {
            CreateOneGrid(new Vector2(i * gridSize, 0), new Vector2(i * gridSize, gridNumber.y * gridSize));
            lines.Add(new LineData(new Vector2(i * gridSize, 0), new Vector2(i * gridSize, gridNumber.y * gridSize)));
        }
        for (int i = 0; i < gridNumber.y + 1; i++)
        {
            for (int j = 0; j < gridNumber.x + 1; j++)
            {
                gridIntersection.Add(new Vector2(j * gridSize, i + gridSize) + gridStartPos);
            }
        }

        return (gridIntersection,lines);
    }

    void CreateOneGrid(Vector2 startPos, Vector2 endPos)
    {
        LineRenderer Line = Instantiate(gridPrefab);
        Line.positionCount = 2;
        Line.SetPosition(0, startPos + gridStartPos);
        Line.SetPosition(1, endPos + gridStartPos);
        Line.startColor = gridColor;
        Line.endColor = gridColor;
        Line.startWidth = gridWidth;
        Line.endWidth = gridWidth;
    }

}
