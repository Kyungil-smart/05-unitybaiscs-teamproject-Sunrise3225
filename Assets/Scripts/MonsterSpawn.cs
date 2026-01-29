using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawn : MonoBehaviour
{
    // 몬스터 프리팹의 종류와 스폰 위치를 설정하는 리스트
    [SerializeField] private List<GameObject> _spawnPrefabs = new List<GameObject>();
    [SerializeField] private List<Vector3> _spawnPositions = new List<Vector3>();
    
    // 코루틴 접근 필드
    [SerializeField] private float _waveDelay;
    [SerializeField] private float _spawnDelay;
    
    // 생성 위치를 랜덤으로 설정하기 위한 인덱스
    private int _randPositionSelect; 
    private Vector3 _spawnPosition;
    
    // 생성 몬스터를 랜덤으로 설정하기 위한 인덱스
    private int _randMonsterSelect; 
    private GameObject _spawnPrefab;
    
    // 첫 웨이브 최대 생성 몬스터를 설정하기 위한 필드
    // 웨이브마다 몬스터의 수가 1.5배 증가
    [SerializeField] private int _maxSpawnCount;
    private int _spawnCount;

    private int _aliveMonsterCount;

    // 최대 웨이브 수를 결정하는 필드
    [SerializeField] private int _maxWaveCount;
    private int _waveCount;

    private float _timer;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        WaveStart();
    }

    private void Update()
    {
        RandomSpawn();

        if (_maxWaveCount < _waveCount)
        {
            WaveStop();
        }
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

    private void WaveStart()
    {
        StartCoroutine(Spawn());
    }

    private void WaveStop()
    {
        StopCoroutine(Spawn());
    }
    
    private IEnumerator Spawn()
    {
        while (_waveCount < _maxWaveCount)
        {
            _waveCount++;
            
            while (_spawnCount < _maxSpawnCount)
            {
                SpawnMonster();
                _spawnCount++;
                yield return new WaitForSeconds(_spawnDelay);
            }

            yield return new WaitUntil(() => _aliveMonsterCount <= 0);
        
            _maxSpawnCount = (int)(_maxSpawnCount * 1.5f);
            _spawnCount = 0;
            yield return new WaitForSeconds(_waveDelay);
        }
    }

    private void SpawnMonster()
    {
        GameObject monster = Instantiate(_spawnPrefab, _spawnPosition, Quaternion.identity);
        _aliveMonsterCount++;
        Debug.Log($"생성된 몬스터 수 : {_aliveMonsterCount}");

        monster.GetComponent<TestMons>().Init(this);
    }

    public void OnMonsterSpawn()
    {
        _aliveMonsterCount--;
        Debug.Log($" 현재 남은 몬스터 수 : {_aliveMonsterCount}");
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