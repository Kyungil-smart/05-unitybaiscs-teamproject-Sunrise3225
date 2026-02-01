using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health;
    
    [SerializeField] [Range(0, 10)] private float _explosionRange;
    [SerializeField] private int _explosionDamage;
    public AudioClip bombClip;
    AudioSource _audioPlayer;
    private void Awake()
    {
        _audioPlayer = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (_health <= 0)
        {
            ActiveBomb();
        }
    }
    
    private void ActiveBomb()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRange);

        foreach (Collider hit in hitColliders)
        {
            MonsterController monster = hit.GetComponent<MonsterController>();

            if (monster != null)
            {
                monster.TakeDamage(_explosionDamage);
            }
        }
        Vector3 fxPos = transform.position + Vector3.up * 0.2f;
        Quaternion fxRot = Quaternion.identity;
        EffectManager.Instance.SpawnEffect(EffectManager.EffectType.Explosion, fxPos, fxRot, null);

        if (bombClip != null) 
            _audioPlayer.PlayOneShot(bombClip, volumeScale: 1f);

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
    }
}
