using UnityEngine;
using System.Collections;
using static Define;

public class FastMoveItem : MonoBehaviour
{
    [Tooltip("이동속도 버프 수치를 넣어주세요.")]
    [SerializeField] private float _fastMoveVelue;
    [Tooltip("버프 지속시간을 넣어주세요.")]
    [SerializeField] private float _buffTime;

    [SerializeField] private ItemType itemType = ItemType.FastItem;
    public ItemType ItemType => itemType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterMovement player = other.GetComponent<CharacterMovement>();
        if (player == null) return;

        player.ApplyFastMove(_fastMoveVelue, _buffTime);

        Destroy(gameObject);
    }
}