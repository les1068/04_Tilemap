using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 그리드 좌표 (0,0) = 월드 좌표 (0.5f, 0.5f, 0);
// 그리드 좌표의 한칸의 간격은 1
public class GridMap 
{
    Node[] nodes;  // 이 맵에 있는 모든 노드

    int width;     // 맵의 가로 길이
    int height;    // 맵의 세로 길이

    public const int Error_Not_Vaild_Position = -1;  // 위치 입력이 잘못되었따는 것을 표시하기 위한 함수

    public GridMap(int width, int height)
    {
        // world의 (0,0,0)에 만든다고 가정
        this.width = width;
        this.height = height;

        // C#의 다차원배열은 함수호출 형식으로 처리가 되기 때문에 속도가 느리다.
        //Node[,] test = new Node[height, width];
        //test[2,1]
        nodes = new Node[height * width];
        
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                int index = GridToIndex(x, y);
                nodes[index] = new Node(x,y);
            }
        }    
    }
    public Node GetGird(int x, int y) // 특정 위치에 있는 노드를 돌려주는 함수 / x : x좌표, y : y좌표 / return x,y에 있는 노드
    {
        int index = GridToIndex(x, y);
        Node result = null;
        if( index != Error_Not_Vaild_Position)
        {
            result = nodes[index];
        }
        return result;
    }
    public Node GetGrid(Vector2Int gridPos)
    {
        return null;
    }
    public Node GetGrid(Vector3 worldPos)
    {
        return null;
    }

    // 유틸리티 함수-----------------------------------------
    public Vector2Int WorldToGrid(Vector3 worldPos) // 월드 좌표를 그리드 좌표로 변경해주는 함수 / worldPos : 월드 좌표, return : 변환된 그리드 좌표
    {
        return Vector2Int.zero;
    }

    public Vector2 GriodToWorld(Vector2Int girdPos) // 그리드 좌표를 월드 좌표로 변경해주는 함수 / girdPos : 그리드 좌표, return : 변환된 월드 좌표
    {
        return Vector2.zero;
    }

    private int GridToIndex(int x, int y) // 그리드 좌표를 index로 변경해주는 함수 / x : x좌표, y : y좌표 , return : 변환된 인덱스 값
    {
        // 왼쪽 위가 원점일 때
        // index = x + y * width

        // 왼쪽 아래가 원점일 때
        // index = x + (height - y - 1) * widthS

        int index = Error_Not_Vaild_Position;
        if(IsValidPosition(x, y))
        {
            index = x + (height - y - 1) * width;
        }
        return index;
    }

    public bool IsValidPosition(int x, int y)
    {// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수 / x : x좌표, y : y좌표, return : 맵 내무면 true, 맵 밖이면 flase
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public bool IsValidPosition(Vector2Int gridPos)
    {// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수 / x : x좌표, y : y좌표, return : 맵 내무면 true, 맵 밖이면 flase
        return IsValidPosition(gridPos.x, gridPos.y);
    }
    public bool IsWall(int x, int y)
    {
        return false;
    }
    public bool IsWall(Vector2Int gridPos)
    {
        return false;
    }
    public bool IsMonster(int x, int y)
    {
        return false;
    }
    public bool IsMonster(Vector2Int gridPos)
    {
        return false;
    }
}
