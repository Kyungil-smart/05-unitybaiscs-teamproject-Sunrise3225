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
}