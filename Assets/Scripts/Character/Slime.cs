using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slime : PoolObject
{
    bool isActivete = false;  // 슬라임이 활동 중인지 아닌지 표시하는 변수
    Vector2Int Position => map.WorldToGrid(transform.position);  // 위치 확인용 프로퍼티(그리드 좌표)
    Transform pool = null;
    public Transform Pool
    {
        get => pool;            // 읽기는 마음대로
        set
        {
            if(pool == null)    // 쓰기는 딱 한번만 가능
            {
                pool = value;
            }
        }
    }
    public Action onDie;  // 죽었을 때 실행될 델리게이트 (보너스용)


    public float moveSpeed = 2.0f;      // 이동속도
    GridMap map;                        // 이 슬라임이 있는 그리드 맵
    List<Vector2Int> path;              // 슬라임이 이동할 경로 
    float pathWaitTime = 0.0f;          // 다른 슬라임에 의해 경로가 막혔을 때 기다리는 시간
    const float MaxPathWaitTime = 1.0f; // 경로가 막혔을 때 최대로 기다리는 시간
    PathLine pathLine;                  // 슬라임이 이동할 경로를 그리는 클래스
    public PathLine PathLine => pathLine; // 경로 그리는 클래스 접근용 프로퍼티

    /// <summary>
    /// 이 슬라임이 현재 위치하고 있는 노드
    /// </summary>
    Node current;
    Node Current
    {
        get => current;
        set
        {
            if (current != value)
            {
                if (current != null)
                {
                    current.gridType = Node.GridType.Plain; // 이전 노드를 plain으로 되돌리기
                }
                current = value;
                current.gridType = Node.GridType.Monster;   // 새 current를 Monster로 설정

                spriteRenderer.sortingOrder = -current.y;   // 겹쳤을 때 아래쪽 슬라임이 위에 그려지도록 설정
            }
        }
    }
    Action OnGoalArrive;            // 목적지 도착했을 때 실행되는 델리게이트


    public float phaseDuration = 0.5f;     // 페이즈 전체 진행 시간
    public float dissolveDuration = 1.0f;  // dissolve 전체 진행 시간
    const float Outline_Thickness = 0.005f;// 외각선 두께용 상수 


    Action onPhaseEnd;       // Phase가 끝날 때 실행되는 델리게이트
    Action onDissolveEnd;    // Dissolve가 끝날 때 실행되는 델리게이트(기본적으로 Die 함수가 연결되어있음)

    Material mainMaterial;   // 이 게임 오브젝트의 Material     

    // 컴포넌트들
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        pathLine = GetComponentInChildren<PathLine>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        mainMaterial = spriteRenderer.material;

        onPhaseEnd += () =>
        {
            isActivete = true;     // 페이즈가 끝나면 isActivate를 활성화
            PathLine.gameObject.SetActive(true);
        };
        onDissolveEnd += Die;      // Dissolve가 끝나면 죽게 만들기

        OnGoalArrive += () =>
        {
            //SetDestination(map.GetRandomMovavlePosition());  // 현재 위치가 다시 나와도 상관없음

            // 현재 위치가 다시 안나왔으면 좋겠을 때
            Vector2Int pos;
            do
            {
                pos = map.GetRandomMovavlePosition();
            } while (pos == Position);  // 랜덤으로 가져온 위치가 현재 위치랑 다른 위치일 때 까지 반복

            SetDestination(pos);        // 지정된 위치로 이동하기
        };
    }
    private void OnEnable()
    {
        isActivete = false;
        ResetShaderProperties();        // 스폰 될 때 쉐이더 프로퍼티 초기화
        StartCoroutine(StartPhase());   // 쉐이더 시작
    }
    private void Update()
    {
        MoveUpdate();
    }

    private void ResetShaderProperties() // 쉐이더 프로포티 초기화 함수
    {
        mainMaterial.SetFloat("_OutlineThickbess", 0.0f);    // 아웃라인 안보이게 하기

        mainMaterial.SetFloat("_PhaseSplit", 1.0f);          // 페이즈 선 위치 초기화
        mainMaterial.SetFloat("_PhaseThickness", 0.1f);      // 페이즈 선 두께 설정해서 선 보이게 만들기

        mainMaterial.SetFloat("_DissolveFade", 1.0f);        // Dissolvefade 상태 초기화

    }

    IEnumerator StartPhase()        // 스폰될 때 실핼될 코루틴 phase 동작 시키기
    {
        float timeElipsed = 0.0f;                            // 진행 시간 초기화
        float phaseNormalize = 1.0f / phaseDuration;         // split 값을 0~1사이로 정규화하기 위해 미리 계산(나누기 횟수 줄이기 위한 용도)

        while (timeElipsed < phaseDuration)                  // 진행시간이 목표시간에 도달할 때까지 진행
        {
            timeElipsed += Time.deltaTime;                   // 진행시간 계속 증가

            mainMaterial.SetFloat("_PhaseSplit", 1 - (timeElipsed * phaseNormalize));  // split 값 감소

            yield return null;                               // 다음 프레임까지 대기
        }
        mainMaterial.SetFloat("_PhaseThickness", 0.0f);      // 페이즈 선 안보이게 하기
        mainMaterial.SetFloat("_PhaseSplit", 0.0f);          // 다 진행된 상태로 설정

        onPhaseEnd?.Invoke();                                // 페이즈가 끝났음을 알림
    }
    IEnumerator StartDissolve()   // 플레이어에게 공격 당했을 때 실행될 코루틴
    {
        float timeElipsed = 0.0f;                            // 진행 시간 초기화
        float dissolveNormalize = 1.0f / dissolveDuration;   // fade 값을 0~1사이로 정규화하기 위해 미리 계산(나누기 횟수 줄이기 위한 용도)

        while (timeElipsed < dissolveDuration)               // 진행시간이 목표시간에 도달할 때까지 진행
        {
            timeElipsed += Time.deltaTime;                   // 진행시간 계속 증가

            mainMaterial.SetFloat("_DissolveFade", 1 - (timeElipsed * dissolveNormalize));  // fade 값 감소

            yield return null;                               // 다음 프레임까지 대기
        }
        mainMaterial.SetFloat("_DissolveFade", 0.0f);        // 다 진행된 상태로 설정

        onDissolveEnd?.Invoke();                             // dissolveEnd가 끝났음을 알림
    }

    public void ShowOutline(bool isShow = true)  // 아웃라인 표시용 함수 / isShow : true면 아웃라인을 표시, false면 아웃라인 끄기
    {
        if (isShow)
        {
            mainMaterial.SetFloat("_OutlineThickness", Outline_Thickness);
        }
        else
        {
            mainMaterial.SetFloat("_OutlineThickness", 0.0f);
        }
    }

    public void OnAttacked()   // 플레이어에게 공격 당하면 실행되는 함수
    {
        if (isActivete)
        {
            isActivete = false;               // 활성화 끄기
            StartCoroutine(StartDissolve());  // Dissolve Shader 켜기
        }
    }
    void Die() // 사망 처리용 함수. Dissolve가 끝날 때 실행됨.
    {
        onDie?.Invoke();
        onDie = null;
        
        ReturnToPool();
    }

    /// <summary>
    /// 슬라임을 풀로 되돌리는 작업
    /// </summary>
    public void ReturnToPool()
    {
        path.Clear();         // 경로를 다 비우기
        PathLine.ClearPath(); // 라인랜더러 초기화 하고 오브젝트 비활성화

        transform.SetParent(Pool);  // 부모를 풀로 돌리기

        gameObject.SetActive(false);
    }
    /// <summary>
    /// 슬라임 초기화용 함수
    /// </summary>
    /// <param name="gridmap">그리드 맵</param>
    /// <param name="pos">시작 위치의 그리드 좌표</param>
    public void Initialize(GridMap gridmap, Vector3 pos)
    {
        map = gridmap;  // 맵 저장
        transform.position = map.GridToWorld(map.WorldToGrid(pos));  // 시작 위치에 배치
        Current = map.GetNode(pos);
    }

    /// <summary>
    /// 목적지를 지정하는 함수
    /// </summary>
    /// <param name="goal">목적지의 그리드 좌표</param>
    public void SetDestination(Vector2Int goal)
    {
        path = Astar.PathFind(map, Position, goal);  // 길찾기해서 경로 저장하기
        pathLine.DrawPath(map, path);                // 경로 따라서 그리기

    }

    /// <summary>
    /// Update에서 실행되는 함수. 이동 처리.
    /// </summary>
    private void MoveUpdate()
    {
        if (isActivete)  // 활성화 상태일 때만 움직이기
        {
            // path가 있고 path의 갯수가 0보다 크고, 대기시간이 최대 대기 시간보다 작을 때
            if (path != null && path.Count > 0 && pathWaitTime < MaxPathWaitTime)
            {
                Vector2Int destGrid = path[0];   // path의 [0]번째를 중간 목적지로 설정

                // destGrid에 몬스터가 없거나 destGird가 current가 일때(내 위치)
                if (!map.IsMonster(destGrid) || map.GetNode(destGrid) == Current)
                {
                    Vector3 dest = map.GridToWorld(destGrid); // 중간 목적지의 월드 좌표 계산
                    Vector3 dir = dest - transform.position;  // 방향 결정

                    if (dir.sqrMagnitude < 0.001f)   // 남은 거리 확인
                    {
                        // 거의 도착한 상태
                        transform.position = dest;   // 중간 도착지점으로 위치옮기기   
                        path.RemoveAt(0);            // path의 0번째 제거
                    }
                    else
                    {
                        // 아직 거리가 남아있는 상태
                        transform.Translate(Time.deltaTime * moveSpeed * dir.normalized);  // 중간 지점까지 계속 이동
                        Current = map.GetNode(transform.position);   // 현재 노드 변경 시도
                    }
                    pathWaitTime = 0.0f; // 조금이라도 움지이면 대기시간 초기화

                }
                else
                {
                    pathWaitTime += Time.deltaTime; // 기다리는 시간 누적시키기
                }
            }
            else
            {
                // path 따라서 도착
                pathWaitTime = 0.0f;     // 기다린 시간 초기화
                OnGoalArrive?.Invoke();  // 도착했다고 알람 보내기
            }
        }
    }
}

