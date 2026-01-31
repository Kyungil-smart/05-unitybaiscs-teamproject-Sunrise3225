using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GoldItem : MonoBehaviour
{
    [SerializeField] private ItemType itemType = ItemType.GoldItem;
    public ItemType ItemType => itemType;

}
