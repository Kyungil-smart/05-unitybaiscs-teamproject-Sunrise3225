using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MonsterDamageState : MonoBehaviour
{
    public GameObject hudDamageText;
    public Transform hudPos;
    private Camera _camera;
    private MonsterController _monsterController; // 몬스터 컨트롤러 참조

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if(_monsterController != null)
           _monsterController.OnTakeDamage.AddListener(ShowDamageText);
    }

    private void OnDisable()
    {
        if (_monsterController != null)
            _monsterController.OnTakeDamage.RemoveListener(ShowDamageText);
    }

    private void Init()
    {
        _camera = Camera.main;
        _monsterController = GetComponent<MonsterController>();
    }
    
    private void ShowDamageText(int damage)
    {
        GameObject damageText = Instantiate(hudDamageText);
        damageText.transform.position = hudPos.position;
        damageText.transform.forward = _camera.transform.forward;
        damageText.GetComponent<DamageTextUI>().damage = damage;
    }
}
