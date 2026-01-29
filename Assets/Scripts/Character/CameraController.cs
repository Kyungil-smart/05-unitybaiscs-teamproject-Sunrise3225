using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraViewPoint;
    [SerializeField] [Range(0, 0.1f)] private float _cameraSpeed;
    [SerializeField] private LayerMask _cameraTargetLayer;

    private Ray _ray;
    private float _rayDistance;
    private Camera _camera;
    private float _maxCameraDistance;
    private float _cameraTargetDistance;
    private float _currentCameraDistance;


    private void Awake()
    {
        Init();
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

    private void SetCameraPosition()
    {
        Vector3 dir = GetDirectionToCamera();
        
        _currentCameraDistance = Mathf.Lerp(_currentCameraDistance, _cameraTargetDistance, _cameraSpeed);
        
        _camera.transform.position = transform.position + (dir * _currentCameraDistance);
        _camera.transform.LookAt(_cameraViewPoint.position);
    }

    private void HandleCamera()
    {
        Vector3 dir = GetDirectionToCamera();
        _ray = new Ray(transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(_ray, out hit, _rayDistance, _cameraTargetLayer))
        {
            _cameraTargetDistance = hit.distance;
        }
        else
        {
            _cameraTargetDistance = _maxCameraDistance;
        }
    }

    private Vector3 GetDirectionToCamera()
    {
        return (_cameraViewPoint.position - transform.position).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_ray.origin, _ray.direction * _rayDistance);
    }
}
