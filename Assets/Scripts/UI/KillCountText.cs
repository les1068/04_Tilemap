using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCountText : MonoBehaviour
{
    TextMeshProUGUI killCount;

    private void Awake()
    {
        killCount = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onKillCountChange += OnKillCountChange;
    }

    private void OnKillCountChange(int count)
    {
        killCount.text = count.ToString();
    }
}
