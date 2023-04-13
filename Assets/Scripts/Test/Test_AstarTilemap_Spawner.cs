using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_AstarTilemap_Spawner : Test_Base
{
    protected override void Test1(InputAction.CallbackContext _)
    {
        Slime[] slimes = FindObjectsOfType<Slime>();
        foreach(var slime in slimes)
        {
            slime.OnAttacked();
        }
    }
    protected override void Test2(InputAction.CallbackContext _)
    {
        Slime[] slimes = FindObjectsOfType<Slime>();
        slimes[0].OnAttacked();
    }
}
