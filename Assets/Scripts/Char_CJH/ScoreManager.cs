using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int _pointsPerKill = 10;
    private int _score;

    public int Score
    {
        get{return _score;}
    }
    
    public void AddKillScore()
    {
        _score += _pointsPerKill;
        Debug.Log($"점수 = {_score}");
    }
}
