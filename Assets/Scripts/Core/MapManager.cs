using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        PendingUnload,  // 로딩 해제 진행중
        PendingLoad,    // 로딩 진행중
        Loaded,           // 로딩 완료됨
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
            foreach (var state in sceneLoadState)
            {
                if (state != SceneLoadState.Unload)
                {
                    result = false;   // 하나라도 언로드가 아니면 false
                    break;
                }
            }
            return result;
        }
    }

    public void PreInitialize()
    {
        sceneNames = new string[HeightCount * WidthCount];
        sceneLoadState = new SceneLoadState[HeightCount * WidthCount];
        for (int y = 0; y < HeightCount; y++)
        {
            for (int x = 0; x < WidthCount; x++)
            {
                int index = GetIndex(x, y);
                sceneNames[index] = $"{SceneNameBase}_{x}_{y}";
                sceneLoadState[index] = SceneLoadState.Unload;
            }
        }
    }
    public void Initialize()
    {
        for (int i = 0; i < sceneLoadState.Length; i++)
        {
            sceneLoadState[i] = SceneLoadState.Unload;
        }
        // 플레이어 기준으로 플레이어 주변의 맵만 로딩하기
    }
    int GetIndex(int x, int y)
    {
        return x + WidthCount * y;
    }
    void RequestAsyncSceneLoad(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadState[index] == SceneLoadState.Unload) // 언로드 상태일 때만 로딩 시도
        {
            // 비동기로 로딩 시작
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[index], LoadSceneMode.Additive);
            // 로딩이 끝나면 Loaded 상태로 변경하는 람다함수를 델리게이트에 추가
            async.completed += (_) => sceneLoadState[index] = SceneLoadState.Loaded;
            sceneLoadState[index] = SceneLoadState.PendingLoad; // 로딩 진행중이라고 표시
        }
    }

    void RequestAsyncSceneUnload(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadState[index] == SceneLoadState.Loaded)
        {
            // 슬라임을 풀로 되돌려서 삭제 되지 않게 만들기
            Scene scene = SceneManager.GetSceneByName(sceneNames[index]);
            GameObject[] sceneObjs = scene.GetRootGameObjects();
            if (sceneObjs != null && sceneObjs.Length > 0)
            {
                Slime[] slimes = sceneObjs[0].GetComponentsInChildren<Slime>();
                foreach (var slime in slimes)
                {
                    slime.ReturnToPool();
                }
            }

            // 비동기로 언로드 시작
            AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[index]);
            // 언로드가 끝나면 Unload 상태로 변경하는 람다함수를 델리게이트에 추가
            async.completed += (_) => sceneLoadState[index] = SceneLoadState.Unload;
            sceneLoadState[index] = SceneLoadState.PendingUnload;   // 언로드 진행중이라고 표시
        }
    }
#if UNITY_EDITOR
    // 테스트용 함수------
    public void Test_LoadScene(int x, int y)
    {
        RequestAsyncSceneLoad(x, y);
    }
    public void Test_UnLoadScene(int x, int y)
    {
        RequestAsyncSceneUnload(x, y);
    }
#endif
}
