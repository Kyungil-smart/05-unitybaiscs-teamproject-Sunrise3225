using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

     public float GroundDistance = 0.5f;
     private Rigidbody _rigidbody;
     private IDamageable _targetDamageable;
     private Transform _targetTransform;
     private CameraController _cameraController;

     [SerializeField] private Transform _rayStartPoint;
     [SerializeField] private Transform _rayEndPoint;
     private Ray _ray;

     [SerializeField] private int _playerLife;
     public bool _isGrounded;
     private float _currentHp;                                // 현재 체력
     public bool IsDead = false;                          // 생존 여부
     
     [SerializeField] private bool _useAnimationTimingFire = true;
     
     [SerializeField] private GameObject _shopPanel;
     [SerializeField] private GameObject _pauseUI;
     private bool _onShopPanel;
     private bool _isPaused;

    [SerializeField] private onDamageColor _damageColor; // Hit Flash(맞으면 붉은색)

    // UI 프로퍼티
    // HP
    public float CurrentHp { get {return _currentHp; } }
     public int MaxHp { get { return _maxHp; } set => _maxHp = value; }
     // Magazine
     public int CurrentMagazine { get {return _currentMagazine; } }
     public int MaxMagazine { get { return _maxMagazine; } set => _maxMagazine = value; }
     // Player Life
     public int PlayerLife { get {return _playerLife; } }
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

         if (_rayEndPoint == null)
             _rayEndPoint = transform;
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
                 _pauseUI.SetActive(true);
                 Time.timeScale = 0f;
             }
             else
             {
                 _isPaused = false;
                 _pauseUI.SetActive(false);
                 Time.timeScale = 1f;
             }
         }
         
         if (Input.GetKeyDown(KeyCode.K))
         {
             _shopPanel.SetActive(true);
             _onShopPanel = true;
             _cameraController._canRotate = false;
             CursorLock(false);
         }

         if (_onShopPanel && Input.GetKeyDown(KeyCode.Escape))
         {
             _shopPanel.SetActive(false);
             _onShopPanel = false;
             _cameraController._canRotate = true;
             CursorLock(true);
         }

         
         
         DetectTarget();
         
         Vector3 rayStartPos = transform.position + Vector3.up * 0.1f;
         _isGrounded = Physics.Raycast(rayStartPos, Vector3.down, 0.2f, GroundLayer);
         
         // Debug.DrawRay(rayStartPos, Vector3.down * 0.2f, _isGrounded ? Color.green : Color.red);
         
         if (!_useAnimationTimingFire)
         {
             if (Input.GetMouseButtonDown(0) && !_onShopPanel) Fire();
         }
         // if (Input.GetMouseButtonDown(0)) Fire();
         
         if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();
         // RefreshMagazineUI();
     }

     private void Init()
     {
         _maxHp = Mathf.Max(0, _maxHp);
         _maxMagazine = Mathf.Max(0, _maxMagazine);
         _playerLife = Mathf.Max(0, _playerLife);

         _currentMagazine = _maxMagazine;
         _currentHp = _maxHp;
         
         _money = Mathf.Max(0, _money);
         _shopPanel.SetActive(false);
         _pauseUI.SetActive(false);
     }
     
     private void DetectTarget()
     {
         if (_rayStartPoint == null || _rayEndPoint == null)
         {
             _targetDamageable = null;
             _targetTransform = null;
             return;
         }
         
         Vector3 dir = GetDirection(_rayStartPoint, _rayEndPoint);
         _ray = new Ray(_rayStartPoint.position, dir);
         
         RaycastHit hit;

         if (Physics.Raycast(_ray, out hit, _attackRange, _attackTargetLayer))
         {
             if (_targetTransform == hit.transform) return;
             _targetTransform = hit.transform;
             _targetDamageable = hit.transform.GetComponent<IDamageable>();
             Debug.Log("타겟 발견");
         }
         else
         {
             _targetDamageable = null;
             _targetTransform = null;
         }
     }
     
     private void Fire()
     {
         if (_currentMagazine <= 0)
         {
             _currentMagazine = 0;
             return;
         }
         
         _currentMagazine = Mathf.Max(0, _currentMagazine - 1);
         //Debug.Log(_currentMagazine);
         
         if (_targetDamageable == null) return;
         
         _targetDamageable.TakeDamage(_attackDamage); 
     }

     private void CursorLock(bool isLocked)
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
             // 라이프 0 밑으로 떨어지지 않게 고정
             _playerLife = Mathf.Max(0, _playerLife - 1);

             if (_playerLife > 0)
             {
                 // 리스폰 체력: 100 고정 대신 maxHp로 복구(일관성)
                 _currentHp = _maxHp;
             }
         }

         if (_playerLife <= 0)
         {
             IsDead = true;
             GameOver();
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
        Fire();
    }
}
