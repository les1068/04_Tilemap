using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Test_AstarTilemap : Test_Base
{
    public Tilemap background;
    public Tilemap obstacle;

    public PathLine pathLine;

    public Transform start;
    public Transform end;

    GridMap map;

    private void Start()
    {
        map = new GridMap(background,obstacle);
    }

    // 마우스 좌클릭으로 시작위치 설정
    // 마우스 우클릭으로 도착위치 설정
    protected override void Test1(InputAction.CallbackContext _)
    {
        Vector2Int startGrid = map.WorldToGrid(start.position);
        Debug.Log($"Start : {startGrid}");
        Vector2Int endGrid = map.WorldToGrid(end.position);
        Debug.Log($"End : {endGrid}");

        List<Vector2Int> path = Astar.PathFind(map, startGrid, endGrid);
        pathLine.DrawPath(map,path);
    }
    private void Test_GetTileMapInfos()
    {
        // 타일맵의 크기
        Debug.Log($"Background: {background.size}");
        Debug.Log($"Obstacle: {obstacle.size}");

        // 타일맵의 워점(그리드 좌표)
        Debug.Log($"Background origin: {background.origin}");
        Debug.Log($"Obstacle origin: {obstacle.origin}");

        // 타일맵 전체 순회하기
        for (int y = background.cellBounds.yMin; y < background.cellBounds.yMax; y++)
        {
            for (int x = background.cellBounds.xMin; x < background.cellBounds.xMax; x++)
            {
                TileBase tile = obstacle.GetTile(new(x, y));   // 특정 위치의 타일 가져오기
                if (tile != null)
                {
                    Debug.Log($"Obstacle Pos : ({x}, {y})");
                }
            }

        }
    }
}
