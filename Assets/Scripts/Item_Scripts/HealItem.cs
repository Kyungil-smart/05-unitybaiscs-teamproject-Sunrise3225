using UnityEngine;
using static Define;

public class HealItem : MonoBehaviour
{
    [Tooltip("체력 회복량")]
    [SerializeField] private int _healingValue;
    [SerializeField] private ItemType itemType = ItemType.HealItem;
    public ItemType ItemType => itemType;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController player = other.GetComponent<CharacterController>();

        if (player == null) return;

        player.Heal(_healingValue);
        Destroy(gameObject);
    }
}
