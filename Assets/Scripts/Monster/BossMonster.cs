using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BossSkill_ApproachMelee;
using static Define;

public class BossMonster : MonsterController
{
    public bool randomPattern = false;
    public List<BossSkillType> pattern = new List<BossSkillType>()
    {
        BossSkillType.ApproachMelee,
        BossSkillType.SlamAttack,
        BossSkillType.RushAttack,
    };

    [Header("Skill Timing")]
    public float skillGap = 0.35f;
    [Header("Approach Melee Skill Setting")]
    public float approachMaxTime = 5.0f; // 이 시간에 못 붙으면 다음 스킬로 넘어감
    [Header("Slam Skill Setting")]
    public float slamWindup = 1.3f;
    public float slamRadius = 20f;
    public float slamDamageMul = 1.5f;
    [Header("RushAttack")]
    public float rushWindup = 1f;
    public float rushDistance = 40f;
    public float rushSpeed = 30f;
    public float rushDamageMul = 3f;
    public float launchForce = 0.1f;

    Coroutine _coPattern;
    int _patternIndex = 0;
    bool _doingSkill = false;

    // 스킬 보관
    Dictionary<BossSkillType, BossSkill> _skills;

    protected override bool CanAutoAttack() => false;

    protected override void InitStats()
    {
        PatrolSpeed = 2.5f;
        ChaseSpeed = 4.2f;
        Attack = 50f;
        Hp = 5000f;
        AttackDistance = 3.0f;
    }

    private void Start()
    {
        if (_coPattern != null)
            StopCoroutine(_coPattern);

        _coPattern = StartCoroutine(CoPatternLoop());
    }
    private void OnDestroy()
    {
        if (_coPattern != null)
        {
            StopCoroutine(_coPattern);
            _coPattern = null;
        }
    }

    public override bool Init(MonsterSpawn monsterSpawn)
    {
        base.Init(monsterSpawn);
        objectType = ObjectType.Boss;
        transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        BuildSkills();
        return true;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Slam 범위
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, slamRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, slamRadius);

        // Windup 동안 "현재 범위" 확인용 텍스트
        UnityEditor.Handles.color = new Color(1f, 0.8f, 0.2f, 1f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.0f,
            $"SLAM R={slamRadius:0.0}  Windup={slamWindup:0.0}s  Dmg={Attack * slamDamageMul:0}");
    }
#endif

    void BuildSkills()
    {
        if (_skills != null) return;

        _skills = new Dictionary<BossSkillType, BossSkill>(4)
        {
            {BossSkillType.ApproachMelee, new BossSkill_ApproachMelee() },
            {BossSkillType.SlamAttack, new BossSkill_SlamAttack() },
            {BossSkillType.RushAttack, new BossSkill_RushAttack() },
        };
    }
    protected override string GetAttackTrigger(bool useRight)
    {
        return base.GetAttackTrigger(useRight);
    }
    public override void UpdateAttack()
    {
        base.UpdateAttack();
    }

    #region Skill Sequence
    IEnumerator CoPatternLoop()
    {
        while (!IsDead)
        {
            if (_doingSkill)
            {
                yield return null;
                continue;
            }

            if (Player == null || !Player.isActiveAndEnabled || Player.IsDead)
            {
                yield return null;
                continue;
            }
            _doingSkill = true;

            BossSkillType next = GetNextSkill();
            yield return ExeCuteSkill(next);

            _doingSkill = false;
            yield return new WaitForSeconds(skillGap);

        }
    }
    BossSkillType GetNextSkill()
    {
        if (pattern == null || pattern.Count == 0)
            return BossSkillType.ApproachMelee;

        if (!randomPattern)
        {
            BossSkillType skill = pattern[_patternIndex % pattern.Count];
            _patternIndex++;
            return skill;
        }
        else
        {
            int index = Random.Range(0, pattern.Count);
            return pattern[index];
        }
    }
    IEnumerator ExeCuteSkill(BossSkillType skill)
    {
        if (_skills == null) BuildSkills();

        if (_skills.TryGetValue(skill, out BossSkill bossSkill) == false || bossSkill == null)
            yield break;

        yield return bossSkill.Execute(this);
    }
    #endregion

    public void DoAoeDamage(Vector3 center, float radius, float damage)
    {
        Collider[] cols = Physics.OverlapSphere(center, radius, IsTarget);
        for (int i = 0; i < cols.Length; i++)
        {
            CharacterController cc = cols[i].GetComponentInParent<CharacterController>();
            if (cc != null && cc.isActiveAndEnabled && !cc.IsDead)
            {
                cc.PlayerTakeDamage(damage);
                return;
            }
        }
    }

    public void DoRushDamage(Vector3 start, Vector3 direction, float distance, float damage)
    {
        if (Physics.Raycast(start + Vector3.up, direction, out RaycastHit hit, distance))
        {
            CharacterController cc = hit.collider.GetComponentInParent<CharacterController>();
            if (cc != null && cc.isActiveAndEnabled && !cc.IsDead)
            {
                cc.PlayerTakeDamage(damage);
                Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 force = (Vector3.up * 0.2f + direction).normalized * launchForce;
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }
        }
    }
}