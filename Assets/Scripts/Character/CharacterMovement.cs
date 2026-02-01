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

    // UI 프로퍼티
    // Speed
    public float MoveSpeed { get { return _moveSpeed;} set => _moveSpeed = value; }

    private void Awake()
    {
        _originalMoveSpeed = MoveSpeed;   // SlowMoveItem용 원래 스피드 백업 
    }

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Rotation();
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
        if (_fastMoveCoroutine != null)
        {
            StopCoroutine(_fastMoveCoroutine);
            MoveSpeed -= value;
        }
        _fastMoveCoroutine = StartCoroutine(FastMoveCoroutine(value, duration));
    }

    private IEnumerator FastMoveCoroutine(float value, float duration)
    {
        MoveSpeed += value;
        yield return new WaitForSeconds(duration);
        MoveSpeed -= value;
        _fastMoveCoroutine = null;
    }
    #endregion

    #region SlowDebuffItem
    public void ApplySlowMove(float value, float duration)
    {
        if (_slowMoveCoroutine != null)
            StopCoroutine(_slowMoveCoroutine);

        _slowMoveCoroutine = StartCoroutine(SlowMoveCoroutine(value, duration));
    }

    private IEnumerator SlowMoveCoroutine(float value, float duration)
    {
        MoveSpeed = Mathf.Max(_originalMoveSpeed - value, 0f);
        yield return new WaitForSeconds(duration);
        MoveSpeed = _originalMoveSpeed;
        _slowMoveCoroutine = null;
    }
    #endregion
}