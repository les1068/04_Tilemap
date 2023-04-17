using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    /// <summary>
    /// 다음에 로딩할 씬의 이름
    /// </summary>
    public string nextSceneName = "14_LoadingSample";

    /// <summary>
    /// 비동기 명령 처리용
    /// </summary>
    AsyncOperation async;

    /// <summary>
    /// 글자 변화용 코루틴
    /// </summary>
    IEnumerator loadingTextCoroutine;

    /// <summary>
    /// 로딩 바의 value가 목표로 하는 값(0.0~1.0)
    /// </summary>
    float loadRatio = 0.0f;

    /// <summary>
    /// 로딩바가 증가하는 속도
    /// </summary>
    public float loadingBarSpeed = 1.0f;

    /// <summary>
    /// 로딩이 완료되었음을 표시하는 변수(true일 때 로딩 완료)
    /// </summary>
    bool loadingComplete = false;

    // 컴포넌트
    PlayerInputActions inputActions;
    Slider slider;
    TextMeshProUGUI loadingText;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.AnyKey.performed += Press;
        inputActions.UI.Click.performed += Press;
    }
    private void OnDisable()
    {
        inputActions.UI.Click.performed -= Press;
        inputActions.UI.AnyKey.performed -= Press;
        inputActions.UI.Disable();
    }
    private void Start()
    {
        slider = FindObjectOfType<Slider>();
        loadingText = FindObjectOfType<TextMeshProUGUI>();

        loadingTextCoroutine = LoadingTestProgress();  // 코루틴 정시 시키기 위해 저장해 놓기
        StartCoroutine(loadingTextCoroutine);          // 글자 변경 코루틴 시작
        StartCoroutine(LoadScene());                   // 로딩바 움직이는 코루틴 시작
    }
    private void Update()
    {
        if (slider.value < loadRatio)   // slider.value를 loadRatio까지 무조건 증가시키기
        {
            slider.value += (Time.deltaTime * loadingBarSpeed);  // 넘쳐도 slider.value의 최대값은 1
        }
    }
    private void Press(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(loadingComplete)    // 로딩이 완료되었으면
        {
            async.allowSceneActivation = true;  // 씬 활성화 시킬 수 있게 만들기
        }
    }

    /// <summary>
    /// 비동기로 씬을 로딩하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadScene()
    {
        slider.value = 0.0f;  // 슬라이더 초기화
        loadRatio = 0.0f;     // 목표값 초기화
        async = SceneManager.LoadSceneAsync(nextSceneName);  // 비동기 씬 로딩 시작
        async.allowSceneActivation = false;                  // 자동으로 씬 활성화 금지

        /*Color start = Color.black;
        Color end = Color.white;
        Image image = slider.transform.GetChild(1).GetChild(0).GetComponent<Image>();*/

        while (loadRatio < 1.0f)        // loadRation가 1이상이 될때까지 반복
        {
            loadRatio = async.progress + 0.1f;  // 씬이 로딩 완료되었을 때 loadRatio는 1이 된다.

            //image.color = Color.Lerp(start, end, loadRatio);

            yield return null;                  // 다음 프레임까지 대기
        }

        // slider.value가 loadRatio로 올라갈때 까지 대기
        yield return new WaitForSeconds((loadRatio - slider.value) / loadingBarSpeed);

        StopCoroutine(loadingTextCoroutine);     // 글자 변경하는 코루틴 실행
        loadingComplete = true;                  // 로딩 완료로 표시해서 입력받을 수 있게 하기
        loadingText.text = "Loading\nComplete."; // 글자 최종 변경
    }

    /// <summary>
    /// Loading 글자 뒤에 점을 찍기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingTestProgress()
    {
        float watiTime = 0.2f;   // 글자가 변경되는 간격
        WaitForSeconds wait = new WaitForSeconds(watiTime);  // 한번만 new를 하기 위해 미리 만들기

        // 문자열 결합이 비효율 적이라 미리 만들어 놓기
        string[] texts = { "Loading", "Loading .", "Loading ..", "Loading ...", "Loading ....", "Loading ....." };
        int index = 0;   // texts 중에 몇번째 글자를 출력할지 결정할 인덱스

        // complete되서 끝나기 전까지는 무한으로 돌리기
        while (true)
        {
            loadingText.text = texts[index];  // 글자 바꾸고
            index++;                          // 인덱스 증가시키고
            index %= texts.Length;            // 인덱스 범위 벗어나면 다시 처음으로

            yield return wait;                // 대기
        }
    }
}
