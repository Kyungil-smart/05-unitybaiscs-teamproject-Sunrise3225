using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonsterDrop : MonoBehaviour
{
    [SerializeField] private DropItem _dropItem;
    [SerializeField] private DropItem _dropItem2;
    [SerializeField] private DropItem _dropItem1;

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
        _dropItem2.MakeDropItem();
        _dropItem1.MakeDropItem();
        Destroy(gameObject);
    }
}
