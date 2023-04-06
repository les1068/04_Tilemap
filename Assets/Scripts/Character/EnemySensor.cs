using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public Action<Slime> onEnemyEnter;  // 슬라임이 트리거 영역 안에 들어오면 실행되는 델리게이트
    public Action<Slime> onEnemyExit;   // 슬라임이 트리거 영역 밖으로 나가면 실행되는 델리게이트
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Slime slime = collision.GetComponent<Slime>();
        if(slime != null)                  // 들어오는 것이 슬라임일 때만
        {
            onEnemyEnter?.Invoke(slime);   // 델리게이트 실행
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Slime slime = collision.GetComponent<Slime>();
        if (slime != null)                 // 나가는 것은 슬라임일 때만
        {
            onEnemyExit?.Invoke(slime);    // 델리게이트 실행
        }

    }
}
