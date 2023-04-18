using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeText : MonoBehaviour
{
    TextMeshProUGUI textUI;
    float maxLifeTime;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        Player player = GameManager.Inst.Player;
        player.onLifeTimeChange += OnLifeTimeChange;
        maxLifeTime = player.maxLifeTime;
        textUI.text = $"{maxLifeTime:f2} Sec";
    }

    private void OnLifeTimeChange(float ratio)
    {
        textUI.text = $"{(maxLifeTime * ratio):f2} Sec";
    }
}
