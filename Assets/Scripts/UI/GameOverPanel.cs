using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public float alphaChangeSpeed = 1.0f;
    CanvasGroup canvasGroup;
    TextMeshProUGUI playTime;
    TextMeshProUGUI killCount;

    Button restart;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Transform child = transform.GetChild(1);
        playTime = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        killCount = child.GetComponent<TextMeshProUGUI>();  
        restart = GetComponentInChildren<Button>();

        restart.onClick.AddListener(OnRestartClick);
    }
    private void Start()
    {
        StopAllCoroutines();
        Player player = GameManager.Inst.Player;
        player.onDie += OnPlayerDie;
    }

    private void OnPlayerDie(float totalPlayTime, int totalKillCount)
    {
        playTime.text = $"Total Play Time\n\r< {totalPlayTime:F1} Sec >";
        killCount.text = $"Total Kill Count\n\r< {totalKillCount} Kill >";

        StartCoroutine(StartAlphaChange());
    }
    IEnumerator StartAlphaChange()
    {
        while(canvasGroup.alpha < 1.0f)
        {
            canvasGroup.alpha += Time.deltaTime * alphaChangeSpeed;
            yield return null;
        }
    }
    private void OnRestartClick()
    {
        StartCoroutine(WaitUnloadAll());   
    }
    IEnumerator WaitUnloadAll()
    {
        MapManager mapManager = GameManager.Inst.MapManager;
        while (!mapManager.IsUnloadAll)
        {
            yield return null;
        }

        SceneManager.LoadScene("LoadingScene");
    }
}
