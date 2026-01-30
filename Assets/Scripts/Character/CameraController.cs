using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraViewPoint;
    [SerializeField] [Range(0, 0.1f)] private float _cameraSpeed;
    [SerializeField] private LayerMask _cameraTargetLayer;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _pitchMin;
    [SerializeField] private float _pitchMax;
    
    private float _pitch;
    private Ray _ray;
    private float _rayDistance;
    public Camera _camera;
    private float _maxCameraDistance;
    private float _cameraTargetDistance;
    private float _currentCameraDistance;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Rotation();
    }

    private void LateUpdate()
    {
        HandleCamera();
        SetCameraPosition();
    }

    private void Init()
    {
        _camera = Camera.main;
        
        _maxCameraDistance = Vector3.Distance(transform.position, _cameraViewPoint.position);
        _rayDistance = _maxCameraDistance;
        _currentCameraDistance = _maxCameraDistance;
        _cameraTargetDistance = _maxCameraDistance;
    }
    
    private void Rotation()
    {
        float x = Input.GetAxisRaw("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float y = Input.GetAxisRaw("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        
        transform.Rotate(Vector3.up, x);

        _pitch -= y;
        _pitch = Mathf.Clamp(_pitch, _pitchMin, _pitchMax);
        
        _cameraViewPoint.localRotation = Quaternion.Euler(_pitch, 0, 0);
    }

    private void SetCameraPosition()
    {
        Vector3 dir = -_cameraViewPoint.forward;
        
        _currentCameraDistance = Mathf.Lerp(_currentCameraDistance, _cameraTargetDistance, _cameraSpeed);
        
        _camera.transform.position = _cameraViewPoint.position + (dir * _currentCameraDistance);
        _camera.transform.LookAt(_cameraViewPoint.position);
    }

    private void HandleCamera()
    {
        Vector3 dir = -_cameraViewPoint.forward;
        _ray = new Ray(_cameraViewPoint.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(_ray, out hit, _maxCameraDistance, _cameraTargetLayer))
        {
            _cameraTargetDistance = hit.distance;
        }
        else
        {
            _cameraTargetDistance = _maxCameraDistance;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_ray.origin, _ray.direction * _rayDistance);
    }
}
