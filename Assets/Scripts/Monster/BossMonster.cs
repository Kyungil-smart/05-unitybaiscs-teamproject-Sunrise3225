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
    [Header("Intro")]
    public string roarStateName = "BossRoaring";
    public float introCamDistance = 6.0f;
    public float introCamHeight = 2.2f;
    public float camMoveTime = 0.45f;

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
    bool _introPlaying = false;

    // 스킬 보관
    Dictionary<BossSkillType, BossSkill> _skills;

    protected override bool CanAutoAttack() => false;

    protected override void InitStats()
    {
        PatrolSpeed = 2.5f;
        ChaseSpeed = 4.2f;
        Attack = 50f;
        Hp = 200f;
        MaxHp = 200f;
        AttackDistance = 3.0f;
    }

    private void Start()
    {
        //if (_coPattern != null)
        //    StopCoroutine(_coPattern);

        //_coPattern = StartCoroutine(CoPatternLoop());
    }
    private void OnDestroy()
    {
        if (_coPattern != null)
        {
            StopCoroutine(_coPattern);
            _coPattern = null;
        }
    }
    void StartPattern()
    {
        if (_coPattern != null)
            StopCoroutine(_coPattern);
        _coPattern = StartCoroutine(CoPatternLoop());
        Anim.SetFloat("Speed", agent.desiredVelocity.magnitude);
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

    #region Intro Scene
    public IEnumerator CoSpawnIntro(Transform playerTransform = null)
    {
        _introPlaying = true;

        Camera cam = Camera.main;
        if (cam == null)
        {
            _introPlaying = false;
            StartPattern();
            yield break;
        }

        // 카메라 추적 스크립트 끄기(이게 있어야 카메라가 안 되돌아감)
        CameraController camFollow = cam.GetComponent<CameraController>();
        if (camFollow == null) 
            camFollow = cam.GetComponentInParent<CameraController>();

        bool prevCamFollow = (camFollow != null) ? camFollow.enabled : false;
        if (camFollow != null) 
            camFollow.enabled = false;

        Vector3 camStartPos = cam.transform.position;
        Quaternion camStartRot = cam.transform.rotation;

        // 보스 고정(핵심: updatePosition = false)
        bool prevRootMotion = false;
        float prevSpeed = 0f;
        if (Anim != null)
        {
            prevRootMotion = Anim.applyRootMotion;
            prevSpeed = Anim.GetFloat("Speed");
            Anim.applyRootMotion = false;
            Anim.SetFloat("Speed", 0f);
        }

        bool prevStopped = false;
        bool prevUpdatePos = true;
        bool prevUpdateRot = true;

        if (agent != null && agent.enabled)
        {
            prevStopped = agent.isStopped;
            prevUpdatePos = agent.updatePosition;
            prevUpdateRot = agent.updateRotation;

            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            agent.updatePosition = false;   // 이게 있어야 Roaring 중에 안 밀림
            agent.updateRotation = false;
        }

        // 현재 카메라가 있는 방향 기준으로 보스 쪽으로 붙여서, 보스만 바라보게
        Vector3 flatDir = camStartPos - transform.position;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude < 0.0001f) flatDir = -transform.forward;
        flatDir.Normalize();

        Vector3 lookTarget = transform.position + Vector3.up * introCamHeight;
        Vector3 camTargetPos = transform.position + flatDir * introCamDistance + Vector3.up * introCamHeight;
        Quaternion camTargetRot = Quaternion.LookRotation((lookTarget - camTargetPos).normalized, Vector3.up);

        yield return MoveCamera(cam.transform, camStartPos, camStartRot, camTargetPos, camTargetRot, camMoveTime);

        // Roaring 재생
        if (Anim != null)
            Anim.Play(roarStateName, 0, 0f);

        float roarLen = GetClipLength(roarStateName);
        if (roarLen <= 0f) roarLen = 3.0f;
        yield return new WaitForSeconds(roarLen);

        // 카메라 복귀
        yield return MoveCamera(cam.transform, cam.transform.position, cam.transform.rotation, camStartPos, camStartRot, camMoveTime);

        // 원복
        if (camFollow != null) camFollow.enabled = prevCamFollow;
        if (Anim != null)
        {
            Anim.applyRootMotion = prevRootMotion;
            Anim.SetFloat("Speed", prevSpeed);
        }
        if (agent != null && agent.enabled)
        {
            agent.updatePosition = prevUpdatePos;
            agent.updateRotation = prevUpdateRot;
            if (agent.isOnNavMesh) agent.isStopped = prevStopped;
        }

        //  패턴 시작 전에 Player 확보
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController pc = player.GetComponent<CharacterController>();
            if (pc != null)
            {
                pc.enabled = true;
                Player = pc;
            }
        }

        _introPlaying = false;
        StartPattern();
    }
    IEnumerator MoveCamera(Transform camTr, Vector3 fromPos, Quaternion fromRot, Vector3 toPos, Quaternion toRot, float moveTime)
    {
        if (moveTime <= 0f)
        {
            camTr.position = toPos;
            camTr.rotation = toRot;
            yield break;
        }

        float time = 0f;
        while (time < moveTime)
        {
            time += Time.deltaTime;
            float a = Mathf.Clamp01(time / moveTime);
            camTr.position = Vector3.Lerp(fromPos, toPos, a);
            camTr.rotation = Quaternion.Slerp(fromRot, toRot, a);
            yield return null;
        }

        camTr.position = toPos;
        camTr.rotation = toRot;
    }
    float GetClipLength(string clipName)
    {
        if (Anim == null || Anim.runtimeAnimatorController == null) return 0f;

        AnimationClip[] clips = Anim.runtimeAnimatorController.animationClips;
        if (clips == null) return 0f;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null && clips[i].name == clipName)
                return clips[i].length;
        }
        return 0f;
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