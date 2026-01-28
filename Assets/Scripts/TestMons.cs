using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMons : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health;
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
    }

    public void LockOn(bool isLockOn)
    {
        
    }
}
