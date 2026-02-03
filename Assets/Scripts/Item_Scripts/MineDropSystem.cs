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
        for (int i = 0; i < 10; i++)
        {
            Vector2 rand = Random.insideUnitCircle;
            if (rand.sqrMagnitude < 0.0001f) continue;

            rand = rand.normalized * Random.Range(_dropMinRange, _dropMaxRange);
            Vector3 pos = transform.position + new Vector3(rand.x, 0, rand.y);

            if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }

        return transform.position;
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

        StartCoroutine(RespawnMineAfterDelay(3f));
    }

    private IEnumerator RespawnMineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject newMine = GetMine();
        if (newMine == null) yield break;

        newMine.transform.position = GetRandomPositionAroundDropper();
    }
}
