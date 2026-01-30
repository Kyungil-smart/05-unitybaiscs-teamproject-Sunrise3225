using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

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
     
     private Ray _ray;

     [SerializeField] private int _playerLife;
     public bool _isGrounded;
     private float _currentHp;                                // 현재 체력
     public bool IsDead = false;                          // 생존 여부
     
     // UI 프로퍼티
     // HP
     public float CurrentHp { get {return _currentHp; } }
     public int MaxHp { get { return _maxHp; } set => _maxHp = value; }
     // Magazine
     public int CurrentMagazine { get {return _currentMagazine; } }
     public int MaxMagazine { get { return _maxMagazine; } set => _maxMagazine = value; }
     // Player Life
     public int PlayerLife { get {return _playerLife; } }
    public int Attack { get => _attackDamage; set => _attackDamage = value; }
    public int Money { get => _money; set => _money = value; }

     private void Awake()
     {
          Init();
     }

     private void Start()
     {
         CursorLock(true);
         _rigidbody = GetComponent<Rigidbody>();
     }

     private void Update()
     {
         DetectTarget();
         
         Vector3 rayStartPos = transform.position + Vector3.up * 0.1f;
         _isGrounded = Physics.Raycast(rayStartPos, Vector3.down, 0.2f, GroundLayer);
         
         // Debug.DrawRay(rayStartPos, Vector3.down * 0.2f, _isGrounded ? Color.green : Color.red);
         
         if (Input.GetMouseButton(0)) Fire();
         
         if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();
         // RefreshMagazineUI();
     }

     private void Init()
     {
         _currentMagazine = _maxMagazine;
         _currentHp = _maxHp;
         _cameraController = GetComponent<CameraController>();
     }
     
     private void RefreshMagazineUI()     // 탄창 UI 인데 일단 TMPro 아까 겹치면 터질수도있을거 같다해서 안썼습니다.
     {
     }

     private void DetectTarget()
     {
         _ray = _cameraController._camera.ScreenPointToRay(Input.mousePosition);
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
         _currentMagazine--;
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
     
     public void PlayerTakeDamage(float damage)
     {
         _currentHp -= damage;
         
         if (_currentHp <= 0)
         {
             _playerLife--;
             _currentHp = 100; 
         }
         if (_playerLife <= 0)
         {
             IsDead = true;
             GameOver();
         }
     }

     private void Jump()
     {
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

    private void OnDrawGizmos()
     {
         Gizmos.color = Color.red;
         Gizmos.DrawRay(_ray.origin, _ray.direction * _attackRange);
     }
}
