using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterMovement : MonoBehaviour
{
    public Transform _cameraView;
    
    [SerializeField] private float _moveSpeed;
    
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _pitchMin;
    [SerializeField] private float _pitchMax;

    public float GroundDistance = 0.1f;
    private Rigidbody _rigidbody;
    private bool _isGrounded;
    private float _pitch;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Rotation();
        Move();
    }

    private void Rotation()
    {
        float x = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float y = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        
        transform.Rotate(Vector3.up, x);

        _pitch -= y;
        _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
        
        _cameraView.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = (transform.right * x + transform.forward * z).normalized;
        transform.position += movement * (_moveSpeed * Time.deltaTime);
    }
}