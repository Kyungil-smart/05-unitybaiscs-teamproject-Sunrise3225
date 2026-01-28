using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPrefab;
    [SerializeField] private List<Vector3> _spawnPositions = new List<Vector3>();
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnInterval;
    [SerializeField] private int _maxSpawnCount;
    
    private Vector3 _spawnPosition;
    private int _randomSelect; // 생성 위치를 랜덤으로 설정하기 위한 정수형 변수
    private int _spawnCount;
    

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        RandomSpawn();
    }

    private void OnDisable()
    {
        StopCoroutine(Spawn());
    }

    private void RandomSpawn()
    {
        _randomSelect = Random.Range(0, _spawnPositions.Count);
        _spawnPosition = _spawnPositions[_randomSelect];
    }

    private IEnumerator Spawn()
    {
        while (_spawnCount < _maxSpawnCount)
        {
            Instantiate(_spawnPrefab, _spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _spawnPositions.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_spawnPositions[i],0.5f);
        }
    }
}