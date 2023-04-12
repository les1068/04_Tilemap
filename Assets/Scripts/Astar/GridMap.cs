using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

// 그리드 좌표 (0,0) = 월드 좌표 (0.5f, 0.5f, 0);
// 그리드 좌표의 한칸의 간격은 1
public class GridMap 
{
    Node[] nodes;  // 이 맵에 있는 모든 노드

    int width;     // 맵의 가로 길이
    int height;    // 맵의 세로 길이

    Vector2Int origin; // 맵 원점의 그리드 좌표

    Tilemap background; // 배경용 타일맵 

    Vector2Int[] movablepositions; // 이 맵에서 이동 가능한 위치(그리드 좌표)

    public const int Error_Not_Vaild_Position = -1;  // 위치 입력이 잘못되었다는 것을 표시하기 위한 함수

    /// <summary>
    /// 크기를 기반으로 타일맵을 생성하는 생성자
    /// </summary>
    /// <param name="width">가로크기</param>
    /// <param name="height">세로크기</param>
    public GridMap(int width, int height)
    {
        // world의 (0,0,0)에 만든다고 가정
        this.width = width;
        this.height = height;

        // C#의 다차원배열은 함수호출 형식으로 처리가 되기 때문에 속도가 느리다.
        //Node[,] test = new Node[height, width];
        //test[2,1]
        nodes = new Node[height * width];
        movablepositions = new Vector2Int[height * width];
        
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                int index = GridToIndex(x, y);
                nodes[index] = new Node(x,y);
                movablepositions[index] = new Vector2Int(x,y);
            }
        }    
    }

    /// <summary>
    /// 타일맵을 기반으로 맵을 생성하는 생성자
    /// </summary>
    /// <param name="background">배경용 타일맵(가장 큰 타일맵)</param>
    /// <param name="obstacle">장애물 표시용 타일맵</param>
    public GridMap(Tilemap background, Tilemap obstacle)
    {
        width = background.size.x;   // 백그라운드 크기를 기반으로 가로 세로 설정
        height = background.size.y;

        // nodes 생성하기
        nodes = new Node[height * width];  // 배열 생성

        origin = (Vector2Int)background.origin;  // 배열입장에서 0,0인 그리드 위치를 저장하기


        // 코드 짧게 보이게 하기 위한 임시변수
        Vector2Int min = new(background.cellBounds.xMin, background.cellBounds.yMin);
        Vector2Int max = new(background.cellBounds.xMax, background.cellBounds.yMax);

        List<Vector2Int> movable = new List<Vector2Int>(height * width);
        // 백그라운드 내부를 한칸씩 순회하기
        for (int y = min.y; y < max.y; y++)
        {
            for (int x = min.x; x < max.x; x++)
            {
                int index = GridToIndex(x, y);  // 그리드 좌표를 기반으로 인덱스 값 가져오기

                Node.GridType tileType = Node.GridType.Plain;  // 기본 타일 타입 설정
                TileBase tile = obstacle.GetTile(new(x, y));  // 장애물 타일 가져오기 시도
                if(tile != null)  // 장애물 타일이 있으면
                {
                    tileType = Node.GridType.Wall;  // 타일의 타입을 벽으로 설정
                }
                else
                {
                    movable.Add(new Vector2Int(x, y));
                }
                nodes[index] = new Node(x, y, tileType);  // 해당 인덱스에 노드 생성해서 배열에 넣기
            }
        }
        movablepositions = movable.ToArray();
        this.background = background;  // 백그라운드 저장
    }

    public Node GetNode(int x, int y) // 특정 위치에 있는 노드를 돌려주는 함수 / return x,y에 있는 노드
    {
        int index = GridToIndex(x, y);
        Node result = null;
        if( index != Error_Not_Vaild_Position)
        {
            result = nodes[index];
        }
        return result;
    }
    public Node GetNode(Vector2Int gridPos) // 특정 위치에 있는 노드를 돌려주는 함수 / gridPos 그리드 좌표
    {
        return GetNode(gridPos.x, gridPos.y);
    }
    public Node GetNode(Vector3 worldPos) // 특정 월드에 있는 노드를 돌려주는 함수 / gridPos 그리드 좌표
    {
        return GetNode(WorldToGrid(worldPos));
    }
    public void ClearData()  // 모든 노드의 A* 계산용 데이터를 초기화 하는 함수
    {
        foreach(var node in nodes)
        {
            node.ClearData();
        }
    }

    // 유틸리티 함수-----------------------------------------
    public Vector2Int WorldToGrid(Vector3 worldPos) // 월드 좌표를 그리드 좌표로 변경해주는 함수 / worldPos : 월드 좌표, return : 변환된 그리드 좌표
    {
        if(background != null)
        {
            return (Vector2Int)background.WorldToCell(worldPos);
        }
        return new Vector2Int((int)worldPos.x, (int)worldPos.y);
    }

    public Vector2 GridToWorld(Vector2Int gridPos) // 그리드 좌표를 월드 좌표로 변경해주는 함수 / girdPos : 그리드 좌표, return : 변환된 월드 좌표
    {
        if(background != null)
        {
            return background.CellToWorld((Vector3Int)gridPos) + new Vector3(0.5f, 0.5f);
        }
        return new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f);
    }

    private int GridToIndex(int x, int y) // 그리드 좌표를 index로 변경해주는 함수 / x : x좌표, y : y좌표 , return : 변환된 인덱스 값
    {
        // 왼쪽 위가 원점일 때
        // index = x + y * width

        // 왼쪽 아래가 원점일 때
        // index = x + (height - y - 1) * width

        int index = Error_Not_Vaild_Position;
        if(IsValidPosition(x, y))
        {
            index = (x - origin.x) + ((height - 1) - (y - origin.y)) * width;
        }
        return index;
    }

    public bool IsValidPosition(int x, int y)
    {// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수 / x : x좌표, y : y좌표, return : 맵 내무면 true, 맵 밖이면 flase
        return x >= origin.x && x < (width + origin.x) && y >= origin.y && y < (height + origin.y);
    }
    public bool IsValidPosition(Vector2Int gridPos)
    {// 입력받은 위치가 맵 내부인지 아닌지 확인하는 함수 / x : x좌표, y : y좌표, return : 맵 내무면 true, 맵 밖이면 flase
        return IsValidPosition(gridPos.x, gridPos.y);
    }
    public bool IsWall(int x, int y)  // 입력받은 위치가 벽인지 아닌지 확인하는 함수
    {
        Node node = GetNode(x, y);
        return node != null && node.gridType == Node.GridType.Wall;
    }
    public bool IsWall(Vector2Int gridPos)
    {
        return IsWall(gridPos.x,gridPos.y); 
    }
    public bool IsMonster(int x, int y)
    {
        Node node = GetNode(x, y);
        return node != null && node.gridType == Node.GridType.Monster;
    }
    public bool IsMonster(Vector2Int gridPos)
    {
        return IsMonster(gridPos.x, gridPos.y);
    }
    /// <summary>
    /// 랜덤으로 이동가능한 지역 뽑기
    /// </summary>
    /// <returns>이동 가능한 위치 중 하나(랜덤)</returns>
    public Vector2Int GetRandomMovavlePosition()
    {
        int index = Random.Range(0, movablepositions.Length);
        return movablepositions[index];
    }
}
