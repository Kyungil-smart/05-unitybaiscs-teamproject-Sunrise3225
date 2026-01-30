using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : MonoBehaviour
{
    [Tooltip("°ñµå È¹µæ·®")]
    [SerializeField] private int _moneyValue;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController gold = other.GetComponent<CharacterController>();

        if (gold == null) return;

        gold.RefillAmmo(_moneyValue);
        Destroy(gameObject);
    }
}
