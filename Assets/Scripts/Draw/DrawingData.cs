using System.Collections.Generic;
using UnityEngine;

public class DrawingData
{
    // 今までに描いた図形
    Dictionary<int, LineData> lineDataList = new();
    Dictionary<int, CircleData> circleDataList = new();

    //線そのものを管理
    Dictionary<int, LineRenderer> lines = new();

    Dictionary<int, GameObject> circleCenters = new();

    // 交点計算と交点一覧の管理
    GetIntersection getIntersection = new();

}