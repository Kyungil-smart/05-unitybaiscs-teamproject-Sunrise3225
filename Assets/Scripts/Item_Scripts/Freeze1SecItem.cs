using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze1SecItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<CharacterMovement>();
        if (player == null || !player.enabled) return;
        
            StartCoroutine(Freeze1SecMove(player));
        
    }

    private IEnumerator Freeze1SecMove(CharacterMovement player)
    {
        player.enabled = false;
        yield return new WaitForSeconds(2f);
        player.enabled = true;
        Destroy(gameObject);
    }
}
