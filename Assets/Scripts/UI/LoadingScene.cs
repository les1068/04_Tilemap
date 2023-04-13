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
    /// 로딩 바의 value가 목표로 하는 값
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

        loadingTextCoroutine = LoadingTestProgress();
        StartCoroutine(loadingTextCoroutine);
        StartCoroutine(LoadScene());
    }
    private void Update()
    {
        if (slider.value < loadRatio)
        {
            slider.value += (Time.deltaTime * loadingBarSpeed);
        }
        else
        {
            slider.value = loadRatio;
        }
    }
    private void Press(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(loadingComplete)
        {
            async.allowSceneActivation = true;
        }
    }
    IEnumerator LoadScene()
    {
        slider.value = 0.0f;
        loadRatio = 0.0f;
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;

        while (loadRatio < 1.0f)
        {
            loadRatio = async.progress + 0.1f;
            yield return null;
        }
        yield return new WaitForSeconds((loadRatio - slider.value) / loadingBarSpeed);

        StopCoroutine(loadingTextCoroutine);
        loadingComplete = true;
        loadingText.text = "Loading\nComplete.";
    }
    IEnumerator LoadingTestProgress()
    {
        yield return null;
    }
}
