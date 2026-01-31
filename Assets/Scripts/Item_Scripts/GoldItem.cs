using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GoldItem : MonoBehaviour
{
    [Tooltip("°ñµå È¹µæ·®")]
    [SerializeField] private int _moneyValue;
    [SerializeField] private ItemType itemType = ItemType.GoldItem;
    public ItemType ItemType => itemType;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController gold = other.GetComponent<CharacterController>();

        if (gold == null) return;

        gold.GoldPlus(_moneyValue);
        Destroy(gameObject);
    }
}
