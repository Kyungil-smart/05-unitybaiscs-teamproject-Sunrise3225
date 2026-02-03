using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextUI : MonoBehaviour
{
    private float _textMoveSpeed; // 텍스트가 위로 올라가는 속도
    private float _textAlphaSpeed; // 텍스트가 투명하게 변환되는 속도
    private float _textDestroyTime; // 텍스트 파괴 시간
    TextMeshPro _damageText;
    Color alphaColor;
    public int damage;

    private void Start()
    {
        _textMoveSpeed = 2.0f;
        _textAlphaSpeed = 2.0f;
        _textDestroyTime = 2.0f;
        
        _damageText = GetComponent<TextMeshPro>();
        alphaColor = _damageText.color;
        _damageText.text = damage.ToString();
        Invoke("DestroyText", _textAlphaSpeed);
    }

    private void Update()
    {
        transform.Translate(new Vector3(0, _textAlphaSpeed * Time.deltaTime, 0));
        
        alphaColor.a = Mathf.Lerp(alphaColor.a, 0, _textAlphaSpeed * Time.deltaTime);
        _damageText.color = alphaColor;
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}
