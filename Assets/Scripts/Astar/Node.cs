using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>   // Node 클래스로 비교가 가능하게끔 IComparable 상속
{
    // 축 기준 : 왼쪽 아래가 워점
    // 축 방향 : 오른쪽 x+, 위쪽 y+
    public int x;   // 그리드 맵의 X좌표
    public int y;   // 그리드 맵의 Y좌표

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

    public Node(int x, int y, GridType gridType = GridType.Plain) // Node 생성자 / x : x좌표, y : y좌표, gridType : 노드의 지형타입. 기본 값은 평지
    {
        this.x = x;
        this.y = y;
        this.gridType = gridType;

        ClearData();
    }
    public void ClearData() // Astar 경로 계산용 데이터 초기화
    {
        G = float.MaxValue;
        H = float.MaxValue;
        parent = null;
    }

    public int CompareTo(Node other)
    {// 크기 비교를 하기 위해서 추가된 함수 IComparable를 상속받았기 때문에 반드시 구현해야 한다. / oter 비교 대상

        // 리턴이 0보다 작다  : 내가 작다. (this < other) 
        // 리턴이 0이다      : this와 other가 크기(순서)가 같다.
        // 리턴이 0보다 크다  : 내가 크다. (this > other) 

        if (other == null)  // other가 null이면 무조건 내가 크다.
        {
            return 1;
        }
        return F.CompareTo(other.F); // F값을 기준으로 크기를 정하기
    }


    // == 연산자를 오버라이드 하기 위해 필요한 함수들----------------------------------
    public override bool Equals(object obj)
    {
        // obj가 Node 타입이고 임시적으로 other라고 부르겠다.
        // this와 other의 x가 같고 y도 같다.
        return obj is Node other && this.x == other.x && this.y == other.y;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(this.x, this.y);  // x와 y를 기준으로 해시코드 생성
    }

    // == 연산자는 Node와 Vector2Int로 비교
    public static bool operator == (Node left, Vector2Int right)
    {
        return left.x == right.x && left.y == right.y; // x와 y 둘 다 같으면 같은 것으로 처리
    }

    // != 연산자는 Node와 Vector2Int로 비교
    public static bool operator != (Node left, Vector2Int right)
    {
        return left.x != right.x || left.y != right.y;  // x와 y중 하나만 달라도 다른 것으로 처리
    }
    
}
