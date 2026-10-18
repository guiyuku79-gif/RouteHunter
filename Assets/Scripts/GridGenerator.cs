using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] Color32 gridColor;
    [SerializeField] float gridWidth;

    [SerializeField] LineRenderer gridPrefab;

    public (List<Vector2> intersections, List<LineData> lines) GenerateGrid()
    {
        List<Vector2> gridIntersection = new List<Vector2>();
        List<LineData> lines = new List<LineData>();
        for (int i = 0; i < Constants.GridNumber.y + 1; i++)
        {
            CreateOneGrid(new Vector2(0, i * Constants.GridSize), new Vector2(Constants.GridNumber.x * Constants.GridSize, i * Constants.GridSize));
            lines.Add(new LineData(new Vector2(0, i * Constants.GridSize), new Vector2(Constants.GridNumber.x * Constants.GridSize, i * Constants.GridSize)));
        }
        for (int i = 0; i < Constants.GridNumber.x + 1; i++)
        {
            CreateOneGrid(new Vector2(i * Constants.GridSize, 0), new Vector2(i * Constants.GridSize, Constants.GridNumber.y * Constants.GridSize));
            lines.Add(new LineData(new Vector2(i * Constants.GridSize, 0), new Vector2(i * Constants.GridSize, Constants.GridNumber.y * Constants.GridSize)));
        }
        for (int i = 0; i < Constants.GridNumber.y + 1; i++)
        {
            for (int j = 0; j < Constants.GridNumber.x + 1; j++)
            {
                gridIntersection.Add(new Vector2(j * Constants.GridSize, i + Constants.GridSize) + Constants.GridStartPos);
            }
        }

        return (gridIntersection, lines);
    }

    void CreateOneGrid(Vector2 startPos, Vector2 endPos)
    {
        LineRenderer Line = Instantiate(gridPrefab);
        Line.positionCount = 2;
        Line.SetPosition(0, startPos + Constants.GridStartPos);
        Line.SetPosition(1, endPos + Constants.GridStartPos);
        Line.startColor = gridColor;
        Line.endColor = gridColor;
        Line.startWidth = gridWidth;
        Line.endWidth = gridWidth;
    }

}
