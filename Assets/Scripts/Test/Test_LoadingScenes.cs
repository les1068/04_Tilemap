using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_LoadingScenes : Test_Base
{
    AsyncOperation async;

    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;  // 씬 전환을 즉시 하지않고 대기 시키기

        while( async.progress < 0.9f)
        {
            Debug.Log($"progress :{async.progress}");
            yield return null;
        }
        Debug.Log("Loading Complete.");
    }
    protected override void Test1(InputAction.CallbackContext _)
    {
        //SceneManager.LoadScene(1);   // 동기방식(Synchronous)
        StartCoroutine(LoadScene());
    }
    protected override void Test2(InputAction.CallbackContext _)
    {
        async.allowSceneActivation = true;  // true가 되면 로딩 끝나면 바로 전환
    }
}
