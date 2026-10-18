using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] Color32 gridColor;
    [SerializeField] float gridWidth;

    [SerializeField] LineRenderer gridPrefab;

    readonly List<LineRenderer> gridRenderers = new();

    public (List<Vector2> intersections, List<LineData> lines) GenerateGrid()
    {
        ClearGrid();

        List<Vector2> gridIntersection = new List<Vector2>();
        List<LineData> lines = new List<LineData>();

        for (int i = 0; i <= Constants.GridNumber.y; i++)
        {
            Vector2 start = GetGridWorldPos(0, i);
            Vector2 end = GetGridWorldPos(Constants.GridNumber.x, i);

            lines.Add(new LineData(start, end));
            CreateOneGrid(start, end);
        }
        for (int i = 0; i <= Constants.GridNumber.x; i++)
        {
            Vector2 start = GetGridWorldPos(i, 0);
            Vector2 end = GetGridWorldPos(i, Constants.GridNumber.y);

            lines.Add(new LineData(start, end));
            CreateOneGrid(start, end);
        }
        for (int y = 0; y <= Constants.GridNumber.y; y++)
        {
            for (int x = 0; x <= Constants.GridNumber.x; x++)
            {
                gridIntersection.Add(GetGridWorldPos(x, y));
            }
        }

        return (gridIntersection, lines);
    }


    //格子の番号をゲーム内の座標にする
    Vector2 GetGridWorldPos(int x, int y)
    {
        return Constants.GridStartPos + new Vector2(x * Constants.GridSize, y * Constants.GridSize);
    }

    void CreateOneGrid(Vector2 startPos, Vector2 endPos)
    {
        LineRenderer line = Instantiate(gridPrefab);
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
        line.startColor = gridColor;
        line.endColor = gridColor;
        line.startWidth = gridWidth;
        line.endWidth = gridWidth;

        gridRenderers.Add(line);
    }

    void ClearGrid()
    {
        foreach (LineRenderer line in gridRenderers)
        {
            if (line == null) continue;

            Destroy(line.gameObject);
        }
        gridRenderers.Clear();
    }
}
