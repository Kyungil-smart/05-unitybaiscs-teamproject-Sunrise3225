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
        Vector3 pos = transform.position;
        _dropItem.MakeDropItem(pos);
        Destroy(gameObject);
    }
}
