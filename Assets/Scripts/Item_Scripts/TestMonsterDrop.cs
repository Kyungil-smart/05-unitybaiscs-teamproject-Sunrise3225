using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonsterDrop : MonoBehaviour
{
    [SerializeField] private DropItem _dropItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dead();
        }
    }

    private void Dead()
    {
        _dropItem.MakeDropItem();
        Destroy(gameObject);
    }
}
