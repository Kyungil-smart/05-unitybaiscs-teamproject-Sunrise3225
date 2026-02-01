using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SlowMoveItem : MonoBehaviour
{
    [Tooltip("이동속도 디버프 수치를 넣어주세요.")]
    [SerializeField] private float _slowMoveVelue;
    [Tooltip("디버프 지속시간을 넣어주세요.")]
    [SerializeField] private float _debuffTime;

    [SerializeField] private ItemType itemType = ItemType.SlowItem;
    public ItemType ItemType => itemType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterMovement player = other.GetComponent<CharacterMovement>();
        if (player == null) return;

        player.ApplySlowMove(_slowMoveVelue, _debuffTime);

        Destroy(gameObject);
    }
}