using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> _spawnPrefabs = new List<GameObject>();
    [SerializeField] private List<Vector3> _spawnPositions = new List<Vector3>();
    
    // 코루틴 접근 필드
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnInterval;
    
    // 생성 위치를 랜덤으로 설정하기 위한 인덱스
    private int _randPositionSelect; 
    private Vector3 _spawnPosition;
    
    // 생성 몬스터를 랜덤으로 설정하기 위한 인덱스
    private int _randMonsterSelect; 
    private GameObject _spawnPrefab;
    
    // 최대 생성 몬스터를 설정하기 위한 필드
    [SerializeField] private int _maxSpawnCount;
    private int _spawnCount;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        RandomSpawn();
    }

    private void Init()
    {
        RandomSpawn();
    }
    
    private void RandomSpawn()
    {
        _randPositionSelect = Random.Range(0, _spawnPositions.Count);
        _spawnPosition = _spawnPositions[_randPositionSelect];
        
        _randMonsterSelect = Random.Range(0, _spawnPrefabs.Count);
        _spawnPrefab = _spawnPrefabs[_randMonsterSelect];
    }

    private IEnumerator Spawn()
    {
        while (_spawnCount < _maxSpawnCount)
        {
            Instantiate(_spawnPrefab, _spawnPosition, Quaternion.identity);
            _spawnCount++;
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