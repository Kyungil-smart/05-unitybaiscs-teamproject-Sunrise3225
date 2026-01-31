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
    // 보스 출현 테스트
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Transform _bossSpawnPos;
    
    // 코루틴 접근 필드
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
    
    // 첫 웨이브가 시작되기까지 콜라이더와 상호작용해야하는 시간을 설정하기 위한 필드
    [SerializeField] private float _waveStartTime;
    private float _timeCount;
    
    // 콜라이더 충돌 확인하기 위한 필드
    private CharacterController _player;
    private SphereCollider _collider;
    private bool _isPlayerInside;
    private bool _onMonsterWave;
    private bool _isFirstWave = true;
    
    // Kill을 스포너 기준으로 계산해주는 프로퍼티
    private int _totalSpawnedCount;
    public int TotalSpawnedCount
    {
        get { return _totalSpawnedCount; }
        private set { _totalSpawnedCount = value; }
    }
    public int KillCount
    {
        get { return TotalSpawnedCount - AliveMonsterCount; }
    }
    
    // 현재 씬에 남아있는 몬스터 수를 확인하기 위한 필드
    private int _aliveMonsterCount;
    public int AliveMonsterCount
    {
        get => _aliveMonsterCount;
        set
        {
            _aliveMonsterCount = value;
        }
    }

    // 최대 웨이브 수를 결정하는 필드
    [SerializeField] private int _maxWaveCount;
    private int _waveCount;
    public int WaveCount
    {
        get => _waveCount;
        set
        {
            _waveCount = value;
        }
    }

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        // 보스 출현 테스트
        GameObject go = Instantiate(_bossPrefab, _bossSpawnPos.position, _bossSpawnPos.rotation);
        BossMonster boss = go.GetComponent<BossMonster>();
        if (boss == null)
        {
            Debug.Log("[MonsterSpawn] BossPrefab에 BossMonster가 없습니다");
            return;
        }
        boss.Init(this);
        StartCoroutine(boss.CoSpawnIntro());
    }

    private void Update()
    {
        if (_isPlayerInside)
        {
            _timeCount += Time.deltaTime;
            if (_timeCount >= _waveStartTime)
            {
                if (_isFirstWave)
                {
                    WaveStart();
                    _isFirstWave = false;
                }
                
                _collider.enabled = false;
                _isPlayerInside = false;
                _onMonsterWave = true;
            }
        }
        
        RandomSpawn();
        
    }

    private void Init()
    {
        RandomSpawn();
        _collider = GetComponent<SphereCollider>();
    }
    
    private void RandomSpawn()
    {
        _randPositionSelect = Random.Range(0, _spawnPositions.Count);
        _spawnPosition = _spawnPositions[_randPositionSelect];
        
        _randMonsterSelect = Random.Range(0, _spawnPrefabs.Count);
        _spawnPrefab = _spawnPrefabs[_randMonsterSelect];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _player = other.GetComponentInParent<CharacterController>();

        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInside = true;
            _timeCount = 0f;
            Debug.Log($"{_player}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = null;
            _isPlayerInside = false;
            _timeCount = 0f;
        }
    }

    public void WaveStart()
    {
        StartCoroutine(Spawn());
    }
    
    public void WaveStop()
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
        
            _onMonsterWave = false;
            _collider.enabled = true;
            _maxSpawnCount = (int)(_maxSpawnCount * 1.5f);
            _spawnCount = 0;
            yield return new WaitUntil(() => _onMonsterWave);
        }
    }

    private void SpawnMonster()
    {
        GameObject monster = Instantiate(_spawnPrefab, _spawnPosition, Quaternion.identity);
        _aliveMonsterCount++;
        TotalSpawnedCount++;

        monster.GetComponent<MonsterController>().Init(this);
    }

    public void OnMonsterSpawn()
    {
        _aliveMonsterCount--;
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