using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Freeze1SecItem : MonoBehaviour
{
    private MineDropSystem _dropSystem;
    [SerializeField] private AudioClip _mineSound;

    [SerializeField] private CharacterMovement _player;

    [SerializeField] private ItemType itemType = ItemType.FreezeItem;
    public ItemType ItemType => itemType;

    private void Awake()
    {
        _dropSystem = FindObjectOfType<MineDropSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<CharacterMovement>();
        if (player == null || !player.enabled) return;

        // Grenade3Short
        AudioSource.PlayClipAtPoint(_mineSound, transform.position, 0.2f);
        // WFX_SmokeGrenade Blue
        EffectManager.Instance.SpawnEffect(EffectManager.EffectType.Mine, transform.position, transform.rotation, transform);
        StartCoroutine(Freeze1SecMove(player));
        
    }

    private IEnumerator Freeze1SecMove(CharacterMovement player)
    {
        player.enabled = false;
        yield return new WaitForSeconds(2f);
        player.enabled = true;
        _dropSystem.ReturnMine(gameObject);
    }
}
