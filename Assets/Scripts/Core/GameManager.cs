using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 게임에 존재하는 플레이어
    /// </summary>
    Player player;
    public Player Player => player;

    /// <summary>
    /// 게임의 전체 맵을 관리하는 매니저
    /// </summary>
    MapManager mapManager;
    public MapManager MapManager => mapManager;

    //int mainSceneIndex = -1;
    protected override void PreInitialize()
    {
        base.PreInitialize();

        //Scene mainScene = SceneManager.GetActiveScene();  // 게임 매니져가 있는 씬
        //mainSceneIndex = mainScene.buildIndex;            // 게임 매니져가 있던 씬의 인덱스 저장

        mapManager = GetComponent<MapManager>();
        mapManager.PreInitialize();
    }
    protected override void Initialize()
    {
        base.Initialize();
        player = FindObjectOfType<Player>();
        mapManager.Initialize();
    }

}
