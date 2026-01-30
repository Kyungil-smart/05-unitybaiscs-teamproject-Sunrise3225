using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    [Tooltip("탄창수 회복량")]
    [SerializeField] private int _ammoValue;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController weapon = other.GetComponent<CharacterController>();

        if (weapon == null) return;

        weapon.RefillAmmo(_ammoValue);
        Destroy(gameObject);
    }
}
