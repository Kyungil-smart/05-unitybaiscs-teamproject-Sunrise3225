using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze1SecItem : MonoBehaviour
{
    [Tooltip("이동기능 스크립트를 추가해 주세요.")]
    [SerializeField] private CharacterMovement _player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_player.enabled == true)
        {
            StartCoroutine(Freeze1SecMove());
        }
    }

    private IEnumerator Freeze1SecMove()
    {
        _player.enabled = false;
        yield return new WaitForSeconds(1f);
        _player.enabled = true;
        Destroy(gameObject);
    }
}
