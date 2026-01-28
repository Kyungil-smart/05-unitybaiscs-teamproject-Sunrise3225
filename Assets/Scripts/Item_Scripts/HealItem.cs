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

    //    CharacterController player = other.GetComponent<CharacterController>();

    //    if (player == null) return;

    //    player._currentHp += _healingValue;

    //    if (player.플레이어채력 > player.플레이어최대채력)
    //    {
    //        player.플레이어채력 = player.플레이어최대채력;
    //    }

    //    Destroy(gameObject);
    //}
}
