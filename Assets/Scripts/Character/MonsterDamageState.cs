using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterDamageState : MonoBehaviour
{
    public GameObject hudDamageText;
    public Transform hudPos;
    private Camera _camera;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _camera = Camera.main;
    }
    
    public void TakeDamage(int damage)
    {
        GameObject damageText = Instantiate(hudDamageText);
        damageText.transform.position = hudPos.position;
        damageText.transform.forward = _camera.transform.forward;
        damageText.GetComponent<DamageTextUI>().damage = damage;
    }
}
