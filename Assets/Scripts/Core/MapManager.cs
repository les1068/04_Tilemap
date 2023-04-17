using System.Collections;
using System.Collections.Generic;
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
    readonly Vector2 totalOrigin = new Vector2(
        -mapWidthLenght * WidthCount * 0.5f, -mapHeightLenght * HeightCount * 0.5f);

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
        Loaded          // 로딩 완료됨
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
                    result = false; // 하나라도 언로드가 아니면 false
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 로딩을 시도할 목록
    /// </summary>
    List<int> loadWork = new List<int>();

    /// <summary>
    /// 로딩 시도가 완료된 목록
    /// </summary>
    List<int> loadWorkComplete = new List<int>();

    /// <summary>
    /// 로딩 해제할 목록
    /// </summary>
    List<int> unloadWork = new List<int>();

    /// <summary>
    /// 로딩 해제가 완료된 목록
    /// </summary>
    List<int> unloadWorkComplete = new List<int>();

    private void Update()
    {
        foreach (var index in loadWorkComplete)      // 완료된 것은 loadWork에서 제거
        {
            loadWork.Remove(index);
        }
        loadWorkComplete.Clear();
        foreach (var index in loadWork)             // 남아있는 loadWork를 로딩 시도
        {
            AsyncSceneLoad(index);
        }

        foreach (var index in unloadWorkComplete)  // 완료된 것은 unloadWork에서 제거
        {
            unloadWork.Remove(index);
        }
        unloadWorkComplete.Clear();
        foreach (var index in unloadWork)          // 남아있는 unloadWork를 로딩 해제 시도
        {
            AsyncSceneUnload(index);
        }
    }

    /// <summary>
    /// 싱글톤이 처음 만들어졌을 때만 실행되는 함수(씬 로드 완료된 시점에서 실행)
    /// </summary>
    public void PreInitialize()
    {
        sceneNames = new string[HeightCount * WidthCount];              // 배열 확보
        sceneLoadState = new SceneLoadState[HeightCount * WidthCount];
        for (int y = 0; y < HeightCount; y++)
        {
            for (int x = 0; x < WidthCount; x++)
            {
                int index = GetIndex(x, y);
                sceneNames[index] = $"{SceneNameBase}_{x}_{y}";         // 배열 채워 넣기
                sceneLoadState[index] = SceneLoadState.Unload;
            }
        }
    }

    /// <summary>
    /// 씬로드가 될 때마다 실행되는 함수(이 싱글톤 오브젝트가 로드 될 때만)
    /// </summary>
    public void Initialize()
    {
        for (int i = 0; i < sceneLoadState.Length; i++)
        {
            sceneLoadState[i] = SceneLoadState.Unload;
        }

        // 플레이어 기준으로 플레이어 주변의 맵만 로딩하기
    }

    /// <summary>
    /// x,y좌표를 인덱스로 변경해주는 함수
    /// </summary>
    /// <param name="x">x좌표</param>
    /// <param name="y">y좌표</param>
    /// <returns>좌표에 해당하는 인덱스</returns>
    int GetIndex(int x, int y)
    {
        return x + WidthCount * y;
    }

    /// <summary>
    /// 씬을 비동기로 로딩할 것을 요청하는 함수
    /// </summary>
    /// <param name="x">x좌표(맵의 위치)</param>
    /// <param name="y">y좌표(맵의 위치)</param>
    void RequestAsyncSceneLoad(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadState[index] == SceneLoadState.Unload)
        {
            loadWork.Add(index);    // 로딩이 안된 맵이면 loadWork에 추가
        }
    }

    /// <summary>
    /// 씬을 비동기로 로딩 해제할 것을 요청하는 함수
    /// </summary>
    /// <param name="x">x좌표(맵의 위치)</param>
    /// <param name="y">y좌표(맵의 위치)</param>
    void RequestAsyncSceneUnload(int x, int y)
    {
        int index = GetIndex(x, y);
        if (sceneLoadState[index] == SceneLoadState.Loaded)
        {
            unloadWork.Add(index);  // 로딩이 되어있는 맵이면 unloadWork에 추가
        }

        // 슬라임을 풀로 되돌려서 삭제 되지 않게 만들기
        Scene scene = SceneManager.GetSceneByName(sceneNames[index]);   // 해당 씬 가져오기
        if (scene.isLoaded) // 로드 되어 있으면
        {
            GameObject[] sceneObjs = scene.GetRootGameObjects();    // 씬의 루트 오브젝트 전부 가져오기
            if (sceneObjs != null && sceneObjs.Length > 0)          // 오브젝트가 하나라도 있으면
            {
                Slime[] slimes = sceneObjs[0].GetComponentsInChildren<Slime>(); // 그 안에서 슬라임 전부 찾기
                foreach (var slime in slimes)
                {
                    slime.ReturnToPool();                           // 슬라임을 모두 풀로 되돌리기
                }
            }
        }
    }

    /// <summary>
    /// 씬을 비동기로 로딩하는 함수
    /// </summary>
    /// <param name="index">로딩을 할 씬의 인덱스(내가 설정한 인덱스)</param>
    void AsyncSceneLoad(int index)
    {
        if (sceneLoadState[index] == SceneLoadState.Unload) // 언로드 상태일 때만 로딩 시도
        {
            sceneLoadState[index] = SceneLoadState.PendingLoad; // 로딩 진행중이라고 표시

            // 비동기로 로딩 시작
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneNames[index], LoadSceneMode.Additive);
            // 로딩이 끝나면 Loaded 상태로 변경하는 람다함수를 델리게이트에 추가
            async.completed += (_) =>
            {
                sceneLoadState[index] = SceneLoadState.Loaded;  // 로드 상태로 변경
                loadWorkComplete.Add(index);                    // 로딩 완료 목록에 추가
            };
        }
    }

    /// <summary>
    /// 씬을 비동기로 로딩해제하는 함수
    /// </summary>
    /// <param name="index">로딩 해제를 할 씬의 인덱스(내가 설정한 인덱스)</param>
    void AsyncSceneUnload(int index)
    {
        if (sceneLoadState[index] == SceneLoadState.Loaded)         // 로딩 완료된 상태일 때만
        {
            sceneLoadState[index] = SceneLoadState.PendingUnload;   // 언로드 진행중이라고 표시
            // 비동기로 언로드 시작
            AsyncOperation async = SceneManager.UnloadSceneAsync(sceneNames[index]);
            // 언로드가 끝나면 Unload 상태로 변경하는 람다함수를 델리게이트에 추가
            async.completed += (_) =>
            {
                sceneLoadState[index] = SceneLoadState.Unload;      // 상태를 Unload로 변경
                unloadWorkComplete.Add(index);                      // 로딩 해제 완료 목록에 추가
            };
        }
    }

    void worldToGird()
    {
        Player player;
        //player.transform.position = totalOrigin;
        
    }
    // 테스트용 함수 -------------------------------------------------------------------------------
#if UNITY_EDITOR
    public void Test_LoadScene(int x, int y)
    {
        RequestAsyncSceneLoad(x, y);
    }

    public void Test_UnloadScene(int x, int y)
    {
        RequestAsyncSceneUnload(x, y);
    }
#endif
}
