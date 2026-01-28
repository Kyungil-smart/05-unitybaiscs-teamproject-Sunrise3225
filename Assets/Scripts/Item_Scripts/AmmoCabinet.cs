using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCabinet : MonoBehaviour
{
    [Tooltip("해당 오브젝트의 SphereCollider를 드래그&드롭 해주세요.")]
    [SerializeField] private SphereCollider _sphrerCollider;
    private bool _canAmmoCharge = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_canAmmoCharge == true)
            {
                AmmoCharge();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.CompareTag("Player"))
        {
            _canAmmoCharge = true;
        }
            Debug.Log($"{other.CompareTag("Player")}가 {_canAmmoCharge}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.CompareTag("Player"))
        {
            _canAmmoCharge = false;
        }
            Debug.Log($"{other.CompareTag("Player")}가 {_canAmmoCharge}");
    }

    private void AmmoCharge()
    {
        //무기정보 weapon = other.GetComponent<무기정보>();

        //if (weapon == null) return;

        //weapon.현재탄창수 = weapon.최대탄창수;

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, _sphrerCollider.radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _sphrerCollider.radius);
    }
}
