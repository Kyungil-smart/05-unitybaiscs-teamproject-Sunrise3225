using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    private float _killCount;

    // TODO 적 처치 시 KillCounter 메서드 추가해 주세요.
    public void KillCounter()
    {
        _killCount++;
        Debug.Log($"처치 수 = {_killCount}");
    }
}