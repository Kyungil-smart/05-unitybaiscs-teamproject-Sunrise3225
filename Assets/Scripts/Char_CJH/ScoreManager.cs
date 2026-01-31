using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _pointsPerKill = 10;
    [SerializeField] private EnemyKillCounter _enemyKillCounter;

    public int Score
    {
        get
        {
            if (_enemyKillCounter == null) return 0;
            return _enemyKillCounter.Counter * _pointsPerKill;
        }
    }
}