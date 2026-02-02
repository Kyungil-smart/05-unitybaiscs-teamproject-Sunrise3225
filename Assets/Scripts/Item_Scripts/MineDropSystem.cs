using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MineDropSystem : MonoBehaviour
{
    [Tooltip("마인프리펩을 드래그&드롭 해주세요.")]
    [SerializeField] private GameObject _mine;
    [Tooltip("마인의 풀사이즈를 미리 설정해주세요.")]
    [SerializeField] private int poolSize;
    [Tooltip("플레이어 기준 드롭 최대거리를 입력해주세요.")]
    [SerializeField] private float _dropMaxRange;
    [Tooltip("플레이어 기준 드롭 최소거리를 입력해주세요.")]
    [SerializeField] private float _dropMinRange;

    private Queue<GameObject> _mineQPool = new Queue<GameObject>();

    private void Awake()
    {
        Init();
        SpawnAllMines();
    }

    private void Update()
    {
    }

    private void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject _mineObj = Instantiate(_mine);
            _mineObj.SetActive(false);
            _mineQPool.Enqueue(_mineObj);
        }
    }

    private void SpawnAllMines()
    {
        int count = _mineQPool.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject mine = GetMine();
            mine.transform.position = GetRandomPositionAroundDropper();
        }
    }

    private Vector3 GetRandomPositionAroundDropper()
    {
        Vector2 randCircle = Random.insideUnitCircle.normalized * Random.Range(_dropMinRange, _dropMaxRange);
        Vector3 pos = transform.position + new Vector3(randCircle.x, 0, randCircle.y);

        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            pos = hit.position;

        return pos;
    }

    public GameObject GetMine()
    {
        if (_mineQPool.Count > 0)
        {
            GameObject mineObj = _mineQPool.Dequeue();
            mineObj.SetActive(true);
            return mineObj;
        }
        else
        {
            return null;
        }
    }

    public void ReturnMine(GameObject mineObj)
    {
        mineObj.SetActive(false);
        _mineQPool.Enqueue(mineObj);

        GameObject newMine = GetMine();
        if (newMine == null) return;

        mineObj.transform.position = GetRandomPositionAroundDropper();
    }
}
