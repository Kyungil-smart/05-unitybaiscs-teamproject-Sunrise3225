using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health;
    
    [SerializeField] [Range(0, 10)] private float _explosionRange;
    [SerializeField] private int _explosionDamage;
    
    private void Update()
    {
        if (_health <= 0)
        {
            ActiveBomb();
        }
    }
    
    private void ActiveBomb()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRange);

        foreach (Collider hit in hitColliders)
        {
            TestMons monster = hit.GetComponent<TestMons>();

            if (monster != null)
            {
                monster.TakeDamage(_explosionDamage);
            }
        }
        
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
    }
}
