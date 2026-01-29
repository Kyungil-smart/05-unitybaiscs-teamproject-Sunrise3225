using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class BossMonster : MonsterController
{
    public bool randomPattern = false;
    public List<BossSkillType> pattern = new List<BossSkillType>()
    {
        BossSkillType.ApproachMelee,
        BossSkillType.JumpAttack,
        BossSkillType.SlamAttack,
    };
    [Header("Skill Timing")]
    public float skillGap = 0.35f;
    // 1번 스킬
    public float approachMaxTime = 5.0f; // 이 시간에 못 붙으면 다음 스킬로 넘어감
    // 3번 스킬
    public float slamWindup = 1.0f;
    public float slamRadius = 3.5f;
    public float slamDamageMul = 1.5f;

    Coroutine _coPattern;
    int _patternIndex = 0;
    bool _doingSkill = false;

    protected override bool CanAutoAttack() => false;

    protected override void InitStats()
    {
        PatrolSpeed = 2.5f;
        ChaseSpeed = 4.2f;
        Attack = 50f;
        Hp = 300f;
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

    public override bool Init()
    {
        base.Init();
        transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        return true;
    }
    protected override string GetAttackTrigger(bool useRight)
    {
        return useRight ? "MeleeAttack" : "MeleeAttack";
    }
    public override void UpdateAttack()
    {
        monsterState = MonsterState.AttackBegin;

        bool useRight = NextRight;
        _attackRoot = useRight ? attackRoot_R : attackRoot_L;
        NextRight = !NextRight;

        agent.isStopped = true;
        Anim.applyRootMotion = false;

        Anim.SetTrigger(GetAttackTrigger(false));
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
        switch (skill)
        {
            case BossSkillType.ApproachMelee:
                yield return Skill_ApproachMelee();
                break;
            case BossSkillType.SlamAttack:
                yield return Skill_SlamAttack();
                break;
        }
    }
    #endregion

    IEnumerator Skill_ApproachMelee()
    {
        // 접근하다가 붙으면 기본 공격 트리거, 못붙으면 스킵
        float time = 0f;

        monsterState = MonsterState.Chase;
        agent.isStopped = false;
        agent.speed = ChaseSpeed;

        while (time < approachMaxTime && !IsDead)
        {
            if (Player == null || Player.IsDead) yield break;

            agent.SetDestination(Player.transform.position);

            float dir = Vector3.Distance(Player.transform.position, transform.position);
            if (dir <= AttackDistance)
                break;

            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // 붙었으면 공격 한번
        if (Player != null && !Player.IsDead &&
            Vector3.Distance(Player.transform.position, transform.position) <= AttackDistance)
        {
            UpdateAttack();

            float wait = 0f;
            while (!IsDead && wait < 2f && monsterState != MonsterState.Chase)
            {
                wait += Time.deltaTime;
                yield return null;
            }
        }
    }
    
    IEnumerator Skill_SlamAttack()
    {
        // 3번 : 제자리 내려찍기
        monsterState = MonsterState.AttackBegin;
        agent.isStopped = true;

        Anim.SetTrigger("Slam");

        yield return new WaitForSeconds(slamWindup);

        DoAoeDamage(transform.position, slamRadius, Attack * slamDamageMul);

        yield return new WaitForSeconds(2.2f);
        monsterState = MonsterState.Chase;
        agent.isStopped = false;
    }

    void DoAoeDamage(Vector3 center, float radius, float damage)
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
}
