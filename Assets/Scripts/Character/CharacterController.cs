using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private LayerMask GroundLayer;        // 레이어 "땅" 탐지
    [SerializeField] private int _maxHp = 100;             // 플레이어의 최대체력
    [SerializeField] private float _attackRange;           // 사거리
    [SerializeField] private float _jumpForce;             // 점프력
    [SerializeField] private int _attackDamage;            // 데미지 입히기
    [SerializeField] private LayerMask _attackTargetLayer; // 적 레이어 
    [SerializeField] private int _maxMagazine;             // 탄창 수
    [SerializeField] private int _money;
    private int _currentMagazine;                          // 잔탄 수 

    [SerializeField] private LayerMask _rayHitMask; // 벽 + 적 + 장애물(쏴서 맞을 것들)

    private RaycastHit _lastHit;
    private bool _hasHit;
    private bool _hitEnemy;
    private Vector3 _aimPoint;

    public float GroundDistance = 0.5f;
    private Rigidbody _rigidbody;
    private IDamageable _targetDamageable;
    private Transform _targetTransform;
    private CameraController _cameraController;

    [SerializeField] private Transform _rayStartPoint;
    private Ray _ray;
     
    public bool _isGrounded;
    private float _currentHp;                                // 현재 체력
    public bool IsDead = false;                          // 생존 여부
     
    [SerializeField] private bool _useAnimationTimingFire = true;
     
    [SerializeField] private GameObject _pauseUI;
    public bool _onShopPanel;
    private bool _isPaused; 
    private Camera _cam;


    [SerializeField] private AudioClip _attackSound;

    
    [SerializeField] private onDamageColor _damageColor; // Hit Flash(맞으면 붉은색)

    
    #region for Retry
    
    [SerializeField] private GameObject retryUI;
    
    #endregion

    // UI 프로퍼티
    // HP
    
    public float CurrentHp { get {return _currentHp; } }
    public int MaxHp { get { return _maxHp; } set => _maxHp = value; }
     // Magazine
     public int CurrentMagazine { get {return _currentMagazine; } }
     public int MaxMagazine { get { return _maxMagazine; } set => _maxMagazine = value; }
     public int AttackDamage { get => _attackDamage; set => _attackDamage = value; }
     public int Money { get => _money; set => _money = value; }

     private void Awake()
     {
         _rigidbody = GetComponent<Rigidbody>();
         _cameraController = GetComponent<CameraController>();

        if (_damageColor == null)                         // Hit Flash(맞으면 붉은색)
            _damageColor = GetComponent<onDamageColor>(); // Hit Flash(맞으면 붉은색)

        Init();
         
         if (_rayStartPoint == null)
             _rayStartPoint = transform;

         _cam = Camera.main;
     }

     private void Start()
     {
         CursorLock(true);
         
         if (_rigidbody == null)
             _rigidbody = GetComponent<Rigidbody>();
     }

     private void Update()
     {
         if (IsDead) return;

         if (!_onShopPanel && Input.GetKeyDown(KeyCode.Escape))
         {
             if (!_isPaused)
             {
                 _isPaused = true;
                 if (_pauseUI != null) _pauseUI.SetActive(true);
                 Time.timeScale = 0f;
             }
             else
             {
                 _isPaused = false;
                 if (_pauseUI != null) _pauseUI.SetActive(false);
                 Time.timeScale = 1f;
             }
         }

         if (_isPaused || _onShopPanel) return;
         
         DetectTarget();
         
         Vector3 rayStartPos = transform.position + Vector3.up * 0.1f;
         _isGrounded = Physics.Raycast(rayStartPos, Vector3.down, 0.2f, GroundLayer);
         
         // Debug.DrawRay(rayStartPos, Vector3.down * 0.2f, _isGrounded ? Color.green : Color.red);
         
         if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();
         // RefreshMagazineUI();
     }

     private void Init()
     {
         _maxHp = Mathf.Max(0, _maxHp);
         _maxMagazine = Mathf.Max(0, _maxMagazine);

         _currentMagazine = _maxMagazine;
         _currentHp = _maxHp;
         
         _onShopPanel = false;
         
         _money = Mathf.Max(0, _money);
         if (_pauseUI != null) _pauseUI.SetActive(false);
     }
     
     private void DetectTarget()
     {
        if (_rayStartPoint == null)
        {
            _targetDamageable = null;
            _targetTransform = null;
            _hasHit = false;
            _hitEnemy = false;
            return;
        }
        // 카메라 없으면 return
        if (_cam == null) _cam = Camera.main;
        if (_cam == null) return;

        int mask = (_rayHitMask.value | _attackTargetLayer.value) & ~(1 << gameObject.layer);

        // 카메라 중앙 레이(조준점)
        Ray camRay = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // 카메라가 보는 조준점 결정
        if (Physics.Raycast(camRay, out RaycastHit camHit, _attackRange, mask))
            _aimPoint = camHit.point;
        else
            _aimPoint = camRay.origin + camRay.direction * _attackRange;

        // 시작점에서 조준점으로
        Vector3 start = _rayStartPoint.position;
        Vector3 dir = _aimPoint - start;

        dir.Normalize();
        _ray = new Ray(start, dir);

        _hasHit = Physics.Raycast(_ray, out _lastHit, _attackRange, mask);

        if (_hasHit)
        {
            int hitLayer = _lastHit.collider.gameObject.layer;
            _hitEnemy = ((_attackTargetLayer.value & (1 << hitLayer)) != 0);

            if (_hitEnemy)
            {
                if (_targetTransform != _lastHit.transform)
                {
                    _targetTransform = _lastHit.transform;
                    _targetDamageable = _lastHit.transform.GetComponent<IDamageable>();
                }
            }
            else
            {
                _targetDamageable = null;
                _targetTransform = null;
            }
        }
        else
        {
            _hitEnemy = false;
            _targetDamageable = null;
            _targetTransform = null;
        }
     }
     
     private void Fire() // 애니메이션에서 또 Fire를 사용해서 중복 적용됨
     {
        if (_onShopPanel) return;

        if (_currentMagazine <= 0)
        {
            _currentMagazine = 0;
            return;
        }

        if (_attackSound != null)
        {
            AudioSource.PlayClipAtPoint(_attackSound, transform.position, 3f);
        }

        _currentMagazine = Mathf.Max(0, _currentMagazine - 1);

        DetectTarget();

        if (!_hasHit) return;

        if (_hitEnemy)
        {
            if (_targetDamageable != null)
                _targetDamageable.TakeDamage(_attackDamage);
        }
        else
        {
            Vector3 fxPos = _lastHit.point + _lastHit.normal * 0.02f;
            Quaternion fxRot = Quaternion.LookRotation(_lastHit.normal);
            EffectManager.Instance.SpawnEffect(EffectManager.EffectType.Common, fxPos, fxRot, null);
        }
    }

     public void CursorLock(bool isLocked)
     {
         Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
         Cursor.visible = !isLocked;
     }

     private void GameOver()
     {
         Cursor.lockState = CursorLockMode.None;
         Cursor.visible = true;
     }

     private Vector3 GetDirection(Transform start, Transform end)
     {
         Vector3 diff = (end.position - start.position);

         // start == end면 normalized가 (0,0,0) 나올 수 있으니 보정
         if (diff.sqrMagnitude <= 0.000001f)
             return start.forward;

         return diff.normalized;
     }
     
     public void PlayerTakeDamage(float damage)
     {
         if (IsDead) return;

         // 음수 데미지 들어오면 회복이 되므로(의도 아니면) 하한 0 처리
         damage = Mathf.Max(0f, damage);
         
         if (_damageColor != null) _damageColor.OnDamage(); // Hit Flash(맞으면 붉은색)
         
         _currentHp -= damage;
         _currentHp = Mathf.Max(0f, _currentHp);

         if (_currentHp <= 0f)
         {
             IsDead = true;
             StopAllCoroutines();   // 현재 스크립트에서 돌던 AutoFire 포함 즉시 중단
             GameOver();
             var movement = GetComponent<CharacterMovement>();
             if (movement != null) movement.enabled = false;
             if (_cameraController != null) _cameraController.enabled = false;

            #region for Retry
            if (retryUI != null)
            {
                retryUI.transform.SetParent(null);
                retryUI.SetActive(true);
            }
            #endregion
         }
     }

     private void Jump()
     {
         if (_rigidbody == null) return; 
         _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
     }
     
    #region HealItem
    public void Heal(int healingValue)
    {
        _currentHp += healingValue;
        if (_currentHp > _maxHp)
            _currentHp = _maxHp;
    }
    #endregion

    #region AmmoItem
    public void RefillAmmo(int ammoValue)
    {
        _currentMagazine += ammoValue;
        if (_currentMagazine > _maxMagazine)
            _currentMagazine = _maxMagazine;
    }
    #endregion

    #region GoldItem
    public void GoldPlus(int goldValue)
    {
        _money += goldValue;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_ray.origin, _ray.direction * _attackRange);
    }
    
    public void FireFromTiming()
    {
        if (_useAnimationTimingFire == false) return;
        Fire();
    }
}
