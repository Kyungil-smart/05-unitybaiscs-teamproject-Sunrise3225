using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _aimCamera;                   // 보통 MainCamera
    [SerializeField] private AimResolver _rayResolver;            // 레이 히트 제공

    [Header("Crosshair UI")]
    [SerializeField] private RectTransform _crosshairA;           // 중앙 고정
    [SerializeField] private RectTransform _crosshairB;           // 레이 기준 이동
    [SerializeField] private Image _crosshairAImage;
    [SerializeField] private Image _crosshairBImage;

    [Header("Meet Settings")]
    [SerializeField] private float _meetThresholdPx = 8f;
    [SerializeField] private bool _hideBWhenNoHit = false;

    [Header("Color")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _readyColor = Color.green;

    [Header("Lerp")]
    [SerializeField] private float _followSpeed = 20f;
    [SerializeField] private bool _useUnscaledTime = true;

    private Vector3 _bCurrentScreenPos;
    private bool _bPosInitialized;

    private void Awake()
    {
        if (_aimCamera == null) _aimCamera = Camera.main;
    }

    private void Update()
    {
        if (_rayResolver == null || _crosshairA == null || _crosshairB == null) return;

        _rayResolver.TickResolve();

        Vector3 targetScreenPos;
        bool bOk = TryGetBTargetScreenPos(out targetScreenPos);

        if (bOk)
        {
            // 목표 위치로 "부드럽게" 이동
            SetBTargetSmooth(targetScreenPos);
        }

        bool ready = bOk && IsMeet();
        SetCrosshairColor(ready);
    }

    private bool TryGetBTargetScreenPos(out Vector3 targetScreenPos)
    {
        targetScreenPos = Vector3.zero;

        if (!_rayResolver.HasHit)
        {
            if (_hideBWhenNoHit)
            {
                _crosshairB.gameObject.SetActive(false);
                return false;
            }

            _crosshairB.gameObject.SetActive(true);
            // 히트가 없을 때도 B를 유지하고 싶으면 "현재 위치 유지"로 처리
            // (즉, target을 내보내지 않음)
            return false;
        }

        _crosshairB.gameObject.SetActive(true);

        Vector3 sp = _aimCamera.WorldToScreenPoint(_rayResolver.HitInfo.point);
        if (sp.z < 0f)
        {
            _crosshairB.gameObject.SetActive(false);
            return false;
        }

        targetScreenPos = new Vector3(sp.x, sp.y, 0f);
        return true;
    }

    private bool IsMeet()
    {
        Vector2 a = _crosshairA.position;
        Vector2 b = _crosshairB.position;
        return Vector2.Distance(a, b) <= _meetThresholdPx;
    }

    private void SetCrosshairColor(bool ready)
    {
        if (_crosshairAImage != null) _crosshairAImage.color = ready ? _readyColor : _normalColor;
        if (_crosshairBImage != null) _crosshairBImage.color = ready ? _readyColor : _normalColor;
    }

    private void SetBTargetSmooth(Vector3 targetScreenPos)
    {
        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (!_bPosInitialized)
        {
            _bCurrentScreenPos = targetScreenPos;
            _bPosInitialized = true;
        }

        float t = 1f - Mathf.Exp(-_followSpeed * dt);

        _bCurrentScreenPos = Vector3.Lerp(_bCurrentScreenPos, targetScreenPos, t);
        _crosshairB.position = new Vector3(_bCurrentScreenPos.x, _bCurrentScreenPos.y, 0f);
    }
}