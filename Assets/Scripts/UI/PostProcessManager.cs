using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{

    public float speed = 1.0f;
    float targetValue = 1.0f;
    /// <summary>
    /// 퍼스트 프로세스용 볼륨
    /// </summary>
    Volume postProcessVolum;

    /// <summary>
    /// 볼륨에 들어있을 비네트 효과를 사용하기 위한 클래스
    /// </summary>
    Vignette vignette;

    private void Awake()
    {
        postProcessVolum = GetComponent<Volume>();
        postProcessVolum.profile.TryGet<Vignette>(out vignette);  // 찾기, 없으면 null이 설정되고 있으면 null아님 값
    }
    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;   // 플레이어의 수명 변경 델리게이트에 함수 등록
        vignette.intensity.value = 0;                  // 초기화
    }
    private void Update()
    {
        if (vignette.intensity.value > targetValue)                      // 비네트 정도가 목표치 보다 클때
        {
            //  vignette.intensity.value가 줄어야한다
            vignette.intensity.value -= Time.deltaTime * speed;
            if (vignette.intensity.value < targetValue)                  // 줄였다가 목표치를 넘어 섰을때
            {
                vignette.intensity.value = Mathf.Max(0, targetValue);   // targetValue가 되거나 0으로 설정
            }
            else                                                        // 비네트 정도가 목표치 보다 작을 때
            {
                //  vignette.intensity.value가 늘어야한다
                vignette.intensity.value += Time.deltaTime * speed;
                if (vignette.intensity.value > targetValue)              // 늘렸다가 목표치를 넘어 섰을때
                {
                    vignette.intensity.value = Mathf.Min(1, targetValue);// targetValue가 되거나 1로 설정
                }
            }
        }
    }
    private void OnLifeTimeChange(float ratio)
    {
        //vignette.intensity.value = 1.0f - ratio;       // 수명 변할 때마다 비네트 정도 변경
        targetValue = ratio-1;
    }
}
