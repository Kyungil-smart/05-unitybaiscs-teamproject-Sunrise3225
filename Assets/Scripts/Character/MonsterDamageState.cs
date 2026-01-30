using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageState : MonoBehaviour
{
    public GameObject hudDamageText;
    public Transform hudPos;

    public void TakeDamage(int damage)
    {
        GameObject damageText = Instantiate(hudDamageText);
        damageText.transform.position = hudPos.position;
        damageText.GetComponent<DamageTextUI>().damage = damage;
    }
}
