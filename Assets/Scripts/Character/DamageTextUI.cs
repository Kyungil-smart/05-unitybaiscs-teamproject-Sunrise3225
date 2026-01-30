using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextUI : MonoBehaviour
{
    private float _moveSpeed;
    private float _alphaSpeed;
    private float _destroyTime;
    TextMeshPro _damageText;
    Color alphaColor;
    public int damage;

    private void Start()
    {
        _moveSpeed = 2.0f;
        _alphaSpeed = 2.0f;
        _destroyTime = 2.0f;
        
        _damageText = GetComponent<TextMeshPro>();
        alphaColor = _damageText.color;
        _damageText.text = damage.ToString();
        Invoke("DestroyText", _destroyTime);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, _moveSpeed * Time.deltaTime, 0));
        
        alphaColor.a = Mathf.Lerp(alphaColor.a, 0, _alphaSpeed * Time.deltaTime);
        _damageText.color = alphaColor;
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}
