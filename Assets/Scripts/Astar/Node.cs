using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // 축 기준 : 왼쪽 아래가 워점
    // 축 방향 : 오른쪽 x+, 위쪽 y+
    public int X;   // 그리드 맵의 X좌표
    public int Y;   // 그리드 맵의 Y좌표

    public float G; // A* 알고르즘용 G값(시작점에서 이 노드까지 오는데 걸리는 거리)
    public float H; // A* 알고리즘용 H값(이 노드에서 도착점까지의 예상 거리)
    public float F => G + H;  // A* 알고리즘으로 출발점에서 이 노드를 경유했을 때 목적지까지의 예상 거리
    
    public Node parent;  // A* 알고리즘으로 경로를 계산했을 때 앞에 있는 노드
    public enum GridType // 노드의 종류를 나열해 놓은 열거형
    {
        Plain = 0,       // 평지(이동 가능)
        Wall,            // 벽(이동 불가능)
        Monster          // 슬라임(이동 불가능)
    }

    public GridType gridType = GridType.Plain;  // 노드의 종류

}
