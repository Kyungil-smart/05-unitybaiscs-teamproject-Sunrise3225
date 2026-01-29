using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutoReturn : MonoBehaviour
{
    [Tooltip("반환할 이펙트 타입을 설정해주세요.")]
    [SerializeField] private EffectManager.EffectType effectType;
    [Tooltip("이펙트 지속시간을 설정해주세요.")]
    [SerializeField] private float DurationTime;
    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), DurationTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnToPool()
    {
        EffectManager.Instance.ReturnEffect(effectType, gameObject);
    }
}
