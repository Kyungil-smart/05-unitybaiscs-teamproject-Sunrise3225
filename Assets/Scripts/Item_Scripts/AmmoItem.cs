using UnityEngine;
using static Define;

public class AmmoItem : MonoBehaviour
{
    [Tooltip("탄창수 회복량")]
    [SerializeField] private int _ammoValue;
    [SerializeField] private ItemType itemType = ItemType.AmmoItem;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CharacterController weapon = other.GetComponent<CharacterController>();

        if (weapon == null) return;

        weapon.RefillAmmo(_ammoValue);
        Destroy(gameObject);
    }
}
