using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class RoadTile : Tile
{
    // 주변 어느 위치에 RoadTile이 있는지 표시해주는 enum
    [Flags]  // 이 enum은 비트플래그로 사용하겠다라는 어트리뷰트
    enum AdjTilePosition : byte         // 이 enum은 byte(8bit) 크기를 가진다.
    {
        None = 0,         // 0000 0000
        North = 1,        // 0000 0001  // 북쪽
        East = 2,         // 0000 0010  // 동쪽
        South = 4,        // 0000 0100  // 남쪽
        West = 8,         // 0000 1000  // 서쪽
        All = North | East| South| West  // 0000 1111
    }

    public Sprite[] sprites;


    // 타일이 그려질 때 자동으로 호출이 되는 함수.
    // position : 타일의 위치 (그리드의 좌표값) / tilemap : 이 타일이 그려지는 타일맵
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)  
    {
        for(int y = -1; y<2; y++)
        {
            for(int x = -1; x<2; x++)
            {
                Vector3Int location = new(position.x+x, position.y+y,position.z);  // 나와 주변타일의 위치
                if(HasThisTile(tilemap, location))   // 같은 종류의 타일이면
                {
                    tilemap.RefreshTile(location);   // 갱신 시키기(보이는 이미지 변경)
                }
            }
        }
    }


    // 타일이 실제로 어떤 스프라이트를 그리는지 결정하는 함수 (tileData에 그려질 타일의 정보를 넘겨준다.)
    // position : 타일 데이터를 가져올 타일의 위치 / tilemap : 타일 데이터를 가져올 타일맵 / tileData : 가져온 타일 데이터의 참조(읽기, 쓰기 가능)
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) 
    {
        AdjTilePosition mask = AdjTilePosition.None;

        // mask에 주변 타일 정보 기록
        mask |= HasThisTile(tilemap, position + new Vector3Int(0, 1, 0)) ? AdjTilePosition.North : 0;
        mask |= HasThisTile(tilemap, position + new Vector3Int(1, 0, 0)) ? AdjTilePosition.East : 0;
        mask |= HasThisTile(tilemap, position + new Vector3Int(0, -1, 0)) ? AdjTilePosition.South : 0;
        mask |= HasThisTile(tilemap, position + new Vector3Int(-1, 0 ,0)) ? AdjTilePosition.West : 0;

        /*if(HasThisTile(tilemap, position + new Vector3Int(0, 1, 0)))  // 위랑 같은 코드
        {
            mask = mask | AdjTilePosition.North;
        }*/
        int index = GetIndex(mask);  // 어떤 스프라이트를 그릴 것인지 결정

        if(index > -1)
        {
            tileData.sprite = sprites[index];  // 스프라이트 적용
            tileData.color = Color.white;      // 색상을 기본인 흰색으로 설정
            Matrix4x4 m = tileData.transform;
            m.SetTRS(Vector3.zero, GetRotation(mask), Vector3.one);  // 회전 적용하기
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;        // 다른 타일이 회전을 못시키게 만들기
            tileData.colliderType = ColliderType.None;       // 컬라이더 없는 것으로 처리
        }
        else
        {
            Debug.LogError($"잘못된 인덱스:{index},mask={mask}");
        }
    }

    private int GetIndex(AdjTilePosition mask)
    {
        return 0;
    }
    private Quaternion GetRotation(AdjTilePosition mask)
    {
        return Quaternion.identity;
    }

    // 타일맵에서 지정된 위치에 있는 타일이 같은 종류의 타일인지 확읺아는 함수
    // tilemap : 확인할 타일맵 / position : 확일할 위치 / true면 같은 종류의 타일. false면 다른 종류의 타일
    bool HasThisTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;   // 타일은 1개. 타일맵은 타일의 정보를 참조해서 보여주기 때문에 이 코드가 가능
    }
}
