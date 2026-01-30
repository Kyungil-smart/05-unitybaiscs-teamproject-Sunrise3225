using System.Collections;
using UnityEngine;

public class PlayerDeathRespawnDriver : MonoBehaviour
{
    [SerializeField] private global::CharacterController _characterController;
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private CameraController _cameraController;

    [Header("Respawn")]
    [SerializeField] private float _respawnDelaySeconds = 3f;
    [SerializeField] private float _respawnLiftY = 0.2f;

    [Header("Blink (Visual Only)")]
    [SerializeField] private float _blinkSeconds = 2f;
    [SerializeField] private float _blinkIntervalSeconds = 0.1f;
    [SerializeField] private Renderer[] _renderersToBlink;

    private int _lastLife;
    private bool _sequenceRunning;

    private void Awake()
    {
        if (_characterController == null)
            _characterController = GetComponent<global::CharacterController>();

        if (_characterMovement == null)
            _characterMovement = GetComponent<CharacterMovement>();

        if (_animationController == null)
            _animationController = GetComponent<AnimationController>();
        
        if (_cameraController == null)
            _cameraController = GetComponent<CameraController>();
    }

    private void Start()
    {
        if (_characterController != null)
            _lastLife = _characterController.PlayerLife;
    }

    private void Update()
    {
        if (_characterController == null) return;

        // 게임 오버 상태면 더 이상 처리하지 않음
        if (_characterController.IsDead) return;

        int currentLife = _characterController.PlayerLife;

        // 방금 죽은 순간 감지 (목숨 감소)
        if (!_sequenceRunning && currentLife < _lastLife)
        {
            StartCoroutine(CoDeathThenRespawn());
        }

        _lastLife = currentLife;
    }

    private IEnumerator CoDeathThenRespawn()
    {
        _sequenceRunning = true;

        // 사망 중 이동 / 입력 차단
        SetControllersEnabled(false);

        // 죽음 애니메이션
        if (_animationController != null)
        {
            _animationController.SetGameOver(true);
            _animationController.TriggerDie();
        }

        // 죽음 연출 대기
        yield return new WaitForSeconds(_respawnDelaySeconds);

        // 다시 확인: 이 시점에 게임오버인지?
        bool isGameOver =
            _characterController.IsDead ||
            _characterController.PlayerLife <= 0;

        if (!isGameOver)
        {
            // 리스폰 위치 보정 (Y만 살짝)
            Vector3 pos = transform.position;
            pos.y += _respawnLiftY;
            transform.position = pos;
            
            // 부활: 애니 게임오버 false
            if (_animationController != null)
                _animationController.SetGameOver(false);
            
            // 이동 / 입력 복구
            SetControllersEnabled(true);

            // 깜빡임 연출
            if (_renderersToBlink != null && _renderersToBlink.Length > 0)
                yield return StartCoroutine(CoBlink());
        }
        else
        {
            // 게임 오버면 계속 비활성 상태 유지
            SetControllersEnabled(false);
        }

        _sequenceRunning = false;
    }

    private void SetControllersEnabled(bool enabled)
    {
        if (_characterController != null)
            _characterController.enabled = enabled;

        if (_characterMovement != null)
            _characterMovement.enabled = enabled;
        
        if (_cameraController != null)
            _cameraController.enabled = enabled;
    }

    private IEnumerator CoBlink()
    {
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < _blinkSeconds)
        {
            visible = !visible;
            SetRenderersVisible(visible);

            yield return new WaitForSeconds(_blinkIntervalSeconds);
            elapsed += _blinkIntervalSeconds;
        }

        // 반드시 원상복구
        SetRenderersVisible(true);
    }

    private void SetRenderersVisible(bool visible)
    {
        if (_renderersToBlink == null) return;

        for (int i = 0; i < _renderersToBlink.Length; i++)
        {
            if (_renderersToBlink[i] != null)
                _renderersToBlink[i].enabled = visible;
        }
    }
}