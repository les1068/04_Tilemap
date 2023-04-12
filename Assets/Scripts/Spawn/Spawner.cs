using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Spawner : MonoBehaviour
{
    public float interval = 1.0f;  // 슬라임 스폰 간격
     
    public int capacity = 2;       // 동시에 유지되는 최대 슬라임 수

    public Vector2 size;           // 스포너의 크기

    float elapsed = 0.0f;          // 현재 스폰된 슬람이의 수
    int count = 0;

    List<Node> spawnAreaList;      // 스폰 영역 중에 벽이 아닌 지역

    SpawnerManger manager;
    private void Start()
    {
        manager = GetComponentInParent<SpawnerManger>();
        spawnAreaList = manager.CalcSpawnArea(this);
    }
    private void Update()
    {
        if (count < capacity)
        {
            elapsed += Time.deltaTime;
            if (elapsed > interval)
            {
                Spawn();
                elapsed = 0.0f;
            }

        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 basePos = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y));

        Vector3 p0 = basePos;
        Vector3 p1 = basePos + Vector3.right * size.x;
        Vector3 p2 = basePos + new Vector3(size.x, size.y);
        Vector3 p3 = basePos + Vector3.up * size.y;

        Handles.color = Color.yellow;
        Handles.DrawLine(p0, p1, 5);
        Handles.DrawLine(p1, p2, 5);
        Handles.DrawLine(p2, p3, 5);
        Handles.DrawLine(p3, p0, 5);
    }
#endif

    /// <summary>
    /// 몬스터를 한마리 스폰하는 함수
    /// </summary>
    void Spawn()
    {
        if (GetSpawnPosition(out Vector3 spawnPos))
        {
            Slime slime = Factory.Inst.GetSlime();
            count++;
            Transform oldParent = slime.transform.parent;
            slime.onDie += () =>
            {
                slime.transform.SetParent(oldParent);
                count--;
            };
            slime.transform.SetParent(transform);
            manager.SlimeInitialize(slime, spawnPos);
        }
    }

    /// <summary>
    /// 스폰할 위치를 구하는 함수
    /// </summary>
    /// <param name="spawnPos">스폰할 위치를 찾았을 때 스폰 가능한 위치 중 하나를 돌려줌. 출력용 파라메터</param>
    /// <returns>true면 찾은것, false면 스폰할 자리가 없는 것</returns>
    bool GetSpawnPosition(out Vector3 spawnPos)
    {
        bool result = false;
        List<Node> spawns = new List<Node>();
        foreach(var node in spawnAreaList)           // spawnAreaList에서 몬스터가 없는 노드 구하기
        {
            if(node.gridType != Node.GridType.Monster)
            {
                spawns.Add(node);
            }
        }
        if(spawns.Count > 0)
        {
            // 스폰 가능한 지역이 최소 하나는 있었다. => 그중에서 하나를 랜덤으로 고르기
            int index = Random.Range(0, spawns.Count);
            Node target = spawns[index];
            spawnPos = manager.GridToWorld(target.x, target.y);
            result = true;
        }
        else
        {
            spawnPos = Vector3.zero;   // 스폰 가능한 위치가 없다.
        }
        return result;
    }
}
