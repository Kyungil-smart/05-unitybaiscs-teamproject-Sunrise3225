using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public enum EffectType { Hit, Explosion, Healing, Common, Blood}

    private static EffectManager _instance;
    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EffectManager>();
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class EffectSettings
    {
        [Tooltip("사용하실 이펙트 타입을 설정해주세요.")]
        public EffectType effectType;
        [Tooltip("이펙트 오브젝트를 드래그&드롭 해주세요.")]
        public GameObject effectObject;
        [Tooltip("풀 사이즈를 미리 입력해주세요.")]
        public int poolSize;
    }


    [SerializeField] private List<EffectSettings> _effectList;
    private Dictionary<EffectType, Queue<GameObject>> _effectPools;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        _effectPools = new Dictionary<EffectType, Queue<GameObject>>();

        foreach (var settingData in _effectList)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < settingData.poolSize; i++)
            {
                GameObject effectObj = Instantiate(settingData.effectObject);
                effectObj.transform.SetParent(transform);
                effectObj.SetActive(false);
                queue.Enqueue(effectObj);
            }
            _effectPools.Add(settingData.effectType, queue);
        }
    }

    /// <summary>
    /// 지정한 타입의 이펙트를 풀에서 꺼내어 생성합니다.
    /// 풀에 여유가 없으면 새로 생성합니다.
    /// </summary>
    /// <param name="effectType">EffectManager.EffectType.(이펙트 타입)</param>
    public GameObject SpawnEffect(EffectType effectType, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!_effectPools.ContainsKey(effectType)) return null;

        GameObject effect;

        if (_effectPools[effectType].Count > 0)
        {
            effect = _effectPools[effectType].Dequeue();
        }
        else
        {
            EffectSettings setting = _effectList.Find(x => x.effectType == effectType);
            effect = Instantiate(setting.effectObject);
        }

        effect.transform.position = position;
        effect.transform.rotation = rotation;
        if (parent != null)
            effect.transform.SetParent(parent);

        effect.SetActive(true);

        StartCoroutine(CoReturnEffect(effectType, effect, 2.5f));
        return effect;
    }

    public void ReturnEffect(EffectType effectType, GameObject effect)
    {
        if (!_effectPools.ContainsKey(effectType)) return;
        if (effect == null) return;

        effect.transform.SetParent(transform);
        effect.SetActive(false);
        _effectPools[effectType].Enqueue(effect);
    }

    IEnumerator CoReturnEffect(EffectType type, GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (effect != null)
            ReturnEffect(type, effect);
    }
}
