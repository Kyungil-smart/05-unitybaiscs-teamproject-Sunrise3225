using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour
{
    [Tooltip("체력 회복량")]
    [SerializeField] private int _healingValue;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController player = other.GetComponent<CharacterController>();

        if (player == null) return;

        player.Heal(_healingValue);
        Destroy(gameObject);
    }
}
