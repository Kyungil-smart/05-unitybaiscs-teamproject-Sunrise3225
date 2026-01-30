using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onDamageColor : MonoBehaviour
{
    #region onDamageColor
    [Tooltip("데미지를 받았을 때 색상을 선택하세요.")]
    [SerializeField] private Color _onDamageColor = new Color(1f, 0f, 0f, 1f);
    [Tooltip("데미지를 받았을 때 색상이 변하는 지속시간을 입력하세요.")]
    [SerializeField] private float _colorChangeTime = 0.1f;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private Coroutine _damageCoroutine;

    private static readonly int ColorID = Shader.PropertyToID("_BaseColor");
    #endregion

    private void Awake()
    {
        onDamageColorInit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnDamage();
    }

    #region onDamageColorMethod
    public void onDamageColorInit()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void OnDamage()
    {
        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);

        _damageCoroutine = StartCoroutine(OnDamageCoroutine());
    }

    private IEnumerator OnDamageCoroutine()
    {
        foreach (var r in _renderers)
        {
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(ColorID, _onDamageColor);
            r.SetPropertyBlock(_mpb);
        }

        yield return new WaitForSeconds(_colorChangeTime);

        foreach (var r in _renderers)
        {
            r.SetPropertyBlock(null);
        }

        _damageCoroutine = null;
    }
    #endregion
}
