using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Tooltip("체력 회복량")]
    [SerializeField] private int _healingValue;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    플레이어능력치 player = other.GetComponent<플레이어능력치>();

    //    if (player == null) return;

    //    player.플레이어채력 += _healingValue;

    //    if (player.플레이어채력 > player.플레이어최대채력)
    //    {
    //        player.플레이어채력 = player.플레이어최대채력;
    //    }

    //    Destroy(gameObject);
    //}
}
