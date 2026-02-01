using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimResolver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _rayStartPoint;   // = CharacterController의 _rayStartPoint
    [SerializeField] private Transform _rayEndPoint;     // = CharacterController의 _rayEndPoint

    [Header("Ray Settings")]
    [SerializeField] private float _maxDistance = 100f;

    // B가 "벽 표면"으로 이동하려면 Wall을 포함해야 함
    // (바닥까지 포함하고 싶으면 Floor도 포함)
    [SerializeField] private LayerMask _hitMask;

    public bool HasHit { get; private set; }
    public RaycastHit HitInfo { get; private set; }

    private void Awake()
    {
        if (_rayStartPoint == null) _rayStartPoint = transform;
        if (_rayEndPoint == null) _rayEndPoint = transform;
    }

    public void TickResolve()
    {
        HasHit = false;

        if (_rayStartPoint == null || _rayEndPoint == null) return;

        Vector3 dir = GetDirection(_rayStartPoint, _rayEndPoint);
        Ray ray = new Ray(_rayStartPoint.position, dir);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _maxDistance, _hitMask))
        {
            HasHit = true;
            HitInfo = hit;
        }
    }

    private Vector3 GetDirection(Transform start, Transform end)
    {
        Vector3 diff = (end.position - start.position);
        if (diff.sqrMagnitude <= 0.000001f) return start.forward;
        return diff.normalized;
    }
}