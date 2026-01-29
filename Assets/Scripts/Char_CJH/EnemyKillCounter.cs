using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    private int _killCount;

    public int Counter { get { return _killCount; } }
    
    // TODO 적 처치 시 AddKill 메서드 추가해 주세요.
    public void AddKill()
    {
        _killCount++;
        Debug.Log($"처치 수 = {_killCount}");
    }
}