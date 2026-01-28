using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : MonoBehaviour
{
    private int _health;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
        }
    }
    
    [SerializeField] [Range(0, 10)] private float _explosionRange;
    private bool _isExploded = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActiveBomb();
        }

        if (_isExploded)
        {
            Destroy(gameObject);
        }
    }
    
    private void ActiveBomb()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRange);

        foreach (Collider hit in hitColliders)
        {
            MonsterController monster = hit.GetComponent<MonsterController>();

            if (monster != null)
            {
                Debug.Log($"{monster.gameObject.name} 데미지 입힘");
            }
        }
        
        _isExploded = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }
}
