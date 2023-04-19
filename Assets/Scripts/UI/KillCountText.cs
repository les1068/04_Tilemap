using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCountText : MonoBehaviour
{
    TextMeshProUGUI killCount;
    public float speed = 1.0f;
    float targetValue = 1.0f;
    float currentValue;


    private void Awake()
    {
        killCount = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onKillCountChange += OnKillCountChange;
        targetValue = 0;
        currentValue = 0;
    }
    private void Update()
    {
        currentValue += Time.deltaTime * speed;
        if(currentValue>targetValue)
        {
            currentValue = targetValue;
        }
        int temp = (int)currentValue;
        killCount.text = temp.ToString();
    }
    private void OnKillCountChange(int count)
    {
        //killCount.text = count.ToString();
        targetValue = count;
    }
}
