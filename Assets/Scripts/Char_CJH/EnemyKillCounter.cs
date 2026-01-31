using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    [SerializeField] private MonsterSpawn _monsterSpawn;

    public int Counter
    {
        get
        {
            if (_monsterSpawn == null)
                return 0; // 웨이브 시작 전이면 0 표시

            return _monsterSpawn.KillCount;
        }
    }
}
