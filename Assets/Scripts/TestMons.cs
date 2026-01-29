using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMons : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health;
    private MonsterSpawn _monsterSpawn;

    public void Init(MonsterSpawn monsterSpawn)
    {
        _monsterSpawn = monsterSpawn;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        _monsterSpawn.OnMonsterSpawn();
        Destroy(gameObject);
    }
}
