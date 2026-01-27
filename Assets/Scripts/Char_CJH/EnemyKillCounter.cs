using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    private float _killCount;

    private void KillCounter()
    {
        _killCount++;
        Debug.Log($"처치 수 = {_killCount}");
    }
}