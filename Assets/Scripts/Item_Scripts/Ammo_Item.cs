using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo_Item : MonoBehaviour
{
    [SerializeField] private int _ammoValue;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;

    //    무기정보 weapon = other.GetComponent<무기정보>();

    //    if (weapon == null) return;

    //    weapon.현재탄창수 += _ammoValue;

    //    if (weapon.현재탄창수 > weapon.최대탄창수)
    //    {
    //        weapon.현재탄창수 = weapon.최대탄창수;
    //    }

    //    Destroy(gameObject);
    //}
}
