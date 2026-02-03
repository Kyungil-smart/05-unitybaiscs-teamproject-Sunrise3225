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
    public float approachMaxTime = 5.0f; // �� �ð��� �� ������ ���� ��ų�� �Ѿ
    [Header("Slam Skill Setting")]
    public float slamWindup = 1.3f;
    public float slamRadius = 20f;
    public float slamDamageMul = 1.5f;
    [Header("RushAttack")]
    public float rushLaunchDelay = 2f;   // 애니 시작 후 던지는 타이밍
    public float rushLaunchSpeed = 7f;     // 던지는 속도(Force)
    public float rushTravelDistance = 6f;   // 이동 거리
    [Header("Boss Sound")]
    [SerializeField] public AudioClip skillClip;
    [SerializeField] public AudioClip attackClip;

    Coroutine _coPattern;
    int _patternIndex = 0;
    bool _doingSkill = false;
    bool _introPlaying = false;

    AnimationController animCtrl = null;

    #region for Ending
    [SerializeField] private GameObject endingUI;
    private bool isEnding = false;
    #endregion

    // ��ų ����
    Dictionary<BossSkillType, BossSkill> _skills;

    protected override bool CanAutoAttack() => false;

    protected override void InitStats()
    {
        PatrolSpeed = 2.5f;
        ChaseSpeed = 4.2f;
        Attack = 50f;
        Hp = 25000f;
        MaxHp = 25000f;
        AttackDistance = 3.0f;
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
    }
    private void Start()
    {
        InvokeMonsterData();
    }
    public override bool Init()
    {
        base.Init();
        objectType = ObjectType.Boss;
        transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        BuildSkills();
        return true;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Slam ����
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, slamRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.9f);
        Gizmos.DrawWireSphere(transform.position, slamRadius);

        // Windup ���� "���� ����" Ȯ�ο� �ؽ�Ʈ
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

    public override void OnDead()
    {
        base.OnDead();
        StartCoroutine(CoDeathSlowMotion());

        #region for Ending
        StartCoroutine(ShowEndingUIAfterDelay(3f));
        #endregion

        
    }
    IEnumerator CoDeathSlowMotion()
    {
        float prevScale = Time.timeScale;
        float prevFixed = Time.fixedDeltaTime;

        yield return new WaitForSecondsRealtime(1.0f);

        Time.timeScale = 0.2f; // 슬로우 모션
        Time.fixedDeltaTime = prevFixed * Time.timeScale;

        yield return new WaitForSecondsRealtime(3.5f);

        Time.timeScale = prevScale;
        Time.fixedDeltaTime = prevFixed;
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

        Camera cam = Camera.main; // 카메라 세팅
        if (cam == null)
        {
            _introPlaying = false;
            StartPattern();
            yield break;
        }

        GameObject playerGO = null;

        CharacterController player = null;
        if (playerTransform != null)
        {
            playerGO = playerTransform.gameObject;
            player = playerTransform.GetComponentInChildren<CharacterController>(true);
        }

        if (player == null)
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                player = playerGO.GetComponentInChildren<CharacterController>(true);
        }


        // 캠 고정 부분
        CameraController camFollow = cam.GetComponent<CameraController>();
        if (camFollow == null)
            camFollow = cam.GetComponentInParent<CameraController>(); // 카메라 정보 가져오기

        bool prevCamFollow = (camFollow != null) ? camFollow.enabled : false;
        if (camFollow != null)
            camFollow.enabled = false;

        // 캠의 현재 위치 정보 저장
        Transform camParent = cam.transform.parent;
        Vector3 camLocalPos = cam.transform.localPosition;
        Quaternion camLocalRot = cam.transform.localRotation;

        // 부모가 있으면 분리 (안하면 캐릭터를 따라감)
        if (camParent != null)
            cam.transform.SetParent(null, true);

        Vector3 camStartPos = cam.transform.position;
        Quaternion camStartRot = cam.transform.rotation;

        // ���� ����(�ٽ�: updatePosition = false)
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

            agent.updatePosition = false;   // �̰� �־�� Roaring �߿� �� �и�
            agent.updateRotation = false;
        }
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.Warp(transform.position);

        // ���� ī�޶� �ִ� ���� �������� ���� ������ �ٿ���, ������ �ٶ󺸰�
        Vector3 flatDir = camStartPos - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude < 0.0001f) 
            flatDir = -transform.forward;

        flatDir.Normalize();

        Vector3 lookTarget = transform.position + Vector3.up * introCamHeight;
        Vector3 camTargetPos = transform.position + flatDir * introCamDistance + Vector3.up * introCamHeight;
        Quaternion camTargetRot = Quaternion.LookRotation((lookTarget - camTargetPos).normalized, Vector3.up);

        yield return MoveCamera(cam.transform, camStartPos, camStartRot, camTargetPos, camTargetRot, camMoveTime);

        // Roaring ���
        if (Anim != null)
            Anim.CrossFade(roarStateName, 0.05f, 0);

        float roarLen = GetClipLength(roarStateName);
        if (roarLen <= 0f) roarLen = 3.0f;
        yield return new WaitForSeconds(roarLen);

        // ī�޶� ����
        yield return MoveCamera(cam.transform, cam.transform.position, cam.transform.rotation, camStartPos, camStartRot, camMoveTime);

        // ����
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

        if (camFollow != null) 
            camFollow.enabled = prevCamFollow; // 캠 위치를 기존의 캐릭터 위치로 복구

        if (camParent != null)
        {
            cam.transform.SetParent(camParent, true);
            cam.transform.localPosition = camLocalPos;
            cam.transform.localRotation = camLocalRot;
        }

        if (player != null) player.enabled = true; // 캐릭터 공격 활성화
        if (animCtrl != null) animCtrl.enabled = true;

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

    #region for Ending
    private IEnumerator ShowEndingUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (endingUI != null)
        {
            endingUI.transform.SetParent(null);
            endingUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isEnding = true;
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
}