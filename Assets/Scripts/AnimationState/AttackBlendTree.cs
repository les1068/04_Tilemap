using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AttackBlendTree 상태에 추가한 스트립트
public class AttackBlendTree : StateMachineBehaviour
{
   
    Player player;  // 미리 찾아놓을 플레이어

    private void Awake()
    {
        // 플레이어 찾기
        player = FindObjectOfType<Player>();
    }

    // OnStateExit는 트랜지션이 끝날 때나 상태 머신이 이 상태를 끝낼 때 호출된다.
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log($"StateExit : {player.gameObject.name}");
        player.RestoreInputDir();   // 플레이어의 이동 방향 복원시키기
    }
}
