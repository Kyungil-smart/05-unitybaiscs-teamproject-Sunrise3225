using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterMovement : MonoBehaviour
{
    // public Transform _cameraView;
    
    [SerializeField] private float _moveSpeed;
    
    // [SerializeField] private float _mouseSensitivity;
    // [SerializeField] private float _pitchMin;
    // [SerializeField] private float _pitchMax;
    
    private Rigidbody _rigidbody;
    private CharacterController _characterController;

    private Vector3 movement;

    private bool _isWall;
    // private float _pitch;

    private Coroutine _fastMoveCoroutine; // FastMoveItem 코루틴 체크용
    private Coroutine _slowMoveCoroutine; // SlowMoveItem 코루틴 체크용
    private float _originalMoveSpeed;     // SlowMoveItem용 백업 스피드
    private float _fastBuffValue;         // FastMoveItem 버그 해결을 위한 변수
    private float _slowDebuffValue;       // SlowMoveItem 버그 해결을 위한 변수
    
    // 달리기 기능
    [Header("Sprint")]
    [SerializeField] private float _walkSpeed = 5f;          // 기본 걷기 속도(기존 _moveSpeed 대신 기준값)
    [SerializeField] private float _sprintMultiplier = 1.2f; // 달리기 배수
    [SerializeField] private float _sprintDuration = 2.5f;   // 2~3초
    [SerializeField] private float _sprintCooldown = 10f;    // 10초

    private float _sprintRemain;
    private float _sprintCooldownRemain;
    private bool _isSprinting;
    private float _sprintMul = 1f;

    // UI 프로퍼티
    // Speed
    public float MoveSpeed { get { return _moveSpeed;} set => _moveSpeed = value; }

    private void Awake()
    {
        _originalMoveSpeed = _walkSpeed;   // SlowMoveItem용 원래 스피드 백업 
    }

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Rotation();
        TickSprint();
        Move();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + movement * _moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _isWall = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _isWall = false;
        }
    }

    // private void Rotation()
    // {
    //     float x = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
    //     float y = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;
    //     
    //     transform.Rotate(Vector3.up, x);
// 
    //     _pitch -= y;
    //     _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
    //     
    //     _cameraView.localRotation = Quaternion.Euler(_pitch, 0, 0);
    // }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        movement = (transform.right * x + transform.forward * z).normalized;

        if (_isWall && !_characterController._isGrounded)
        {
            movement *= 0.05f;
        }
        // transform.position += movement * (_moveSpeed * Time.deltaTime);
    }

    #region FastBuffItem
    public void ApplyFastMove(float value, float duration)
    {
        if (_fastMoveCoroutine != null) StopCoroutine(_fastMoveCoroutine);
        _fastMoveCoroutine = StartCoroutine(FastMoveCoroutine(value, duration));
    }

    private IEnumerator FastMoveCoroutine(float value, float duration)
    {
        _fastBuffValue = value;
        UpdateMoveSpeed();
        yield return new WaitForSeconds(duration);
        _fastBuffValue = 0;
        UpdateMoveSpeed();
        _fastMoveCoroutine = null;
    }
    #endregion

    #region SlowDebuffItem
    public void ApplySlowMove(float value, float duration)
    {
        if (_slowMoveCoroutine != null) StopCoroutine(_slowMoveCoroutine);
        _slowMoveCoroutine = StartCoroutine(SlowMoveCoroutine(value, duration));
    }

    private IEnumerator SlowMoveCoroutine(float value, float duration)
    {
        _slowDebuffValue = value;
        UpdateMoveSpeed();
        yield return new WaitForSeconds(duration);
        _slowDebuffValue = 0;
        UpdateMoveSpeed();
        _slowMoveCoroutine = null;
    }
    #endregion

    #region UpdateMoveSpeed
    private void UpdateMoveSpeed()
    {
        MoveSpeed = Mathf.Max((_originalMoveSpeed + _fastBuffValue - _slowDebuffValue) * _sprintMul, 0f);
    }
    #endregion
    
    private void TickSprint()
    {
        // 쿨타임 감소
        if (_sprintCooldownRemain > 0f)
            _sprintCooldownRemain -= Time.deltaTime;

        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isMovingInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f ||
                             Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;

        // 시작 조건: 쉬프트 + 이동 입력 + 쿨타임 0 + 현재 스프린트 아님
        if (!_isSprinting)
        {
            if (shiftHeld && isMovingInput && _sprintCooldownRemain <= 0f)
            {
                _isSprinting = true;
                _sprintRemain = _sprintDuration;
                _sprintMul = _sprintMultiplier;
                UpdateMoveSpeed();
            }
            return;
        }

        // 스프린트 중: 쉬프트 떼거나 이동 입력 없으면 종료
        if (!shiftHeld || !isMovingInput)
        {
            EndSprint();
            return;
        }

        _sprintRemain -= Time.deltaTime;
        if (_sprintRemain <= 0f)
        {
            EndSprint();
            return;
        }
    }

    private void EndSprint()
    {
        _isSprinting = false;
        _sprintRemain = 0f;
        _sprintMul = 1f;
        _sprintCooldownRemain = _sprintCooldown;
        UpdateMoveSpeed();
    }
}