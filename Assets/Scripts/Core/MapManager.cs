using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    /// <summary>
    /// 서브맵의 세로 갯수
    /// </summary>
    const int HeightCount = 3;
    /// <summary>
    /// 서브맵의 가로 갯수
    /// </summary>
    const int WidthCount = 3;

    /// <summary>
    /// 서브맵 하나의 세로 길이
    /// </summary>
    const float mapHeightLenght = 20.0f;
    /// <summary>
    /// 서브맵 하나의 가로 길이
    /// </summary>
    const float mapWidthLenght = 20.0f;

    /// <summary>
    /// 전체 맵의 왼쪽 아래 끝 지점
    /// </summary>
    readonly Vector2 totalOrigin = new Vector2(-mapWidthLenght * WidthCount * 0.5f, -mapHeightLenght * HeightCount * 0.5f);
    
    /// <summary>
    /// 씬 이름 조합용 기본 이름
    /// </summary>
    const string SceneNameBase = "Seemless";

    /// <summary>
    /// 조합이 완료된 모든 씬의 이름 배열
    /// </summary>
    string[] sceneNames;

    /// <summary>
    /// 씬의 로딩 상태를 나타낼 Enum
    /// </summary>
    enum SceneLoadState : byte
    {
        Unload = 0,     // 로딩 안되어있음
        Pendingunload,  // 로딩 해제 진행중
        pendingLoad,    // 로딩 진행중
        Load,           // 로딩 완료됨
    }
    /// <summary>
    /// 모든 씬의 로딩 상태
    /// </summary>
    SceneLoadState[] sceneLoadState;
    
    /// <summary>
    /// 모든 씬이 언로드 되었음을 확인하기 위한 프로퍼티. 모든씬이 언로드 되었으면 true, 아니면 false
    /// </summary>
    public bool IsUnloadAll
    {
        get
        {
            bool result = true;
            foreach(var state in sceneLoadState)
            {
                if(state != SceneLoadState.Unload)
                {
                    result = false;   // 하나라도 언로드가 아니면 false
                    break;
                }
            }
            return result;
        }
    }

    public void Initialize()
    {

    }
    public void PreInitialize()
    {

    }
}
