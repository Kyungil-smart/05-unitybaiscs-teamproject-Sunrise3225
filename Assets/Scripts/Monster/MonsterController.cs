using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static Define;


public class MonsterController : MonoBehaviour, IDamageable
{
    #region Action
    public event Action OnBossDead;   // 보스가 있을 경우에
    public event Action<MonsterController> MonsterInfoUpdate;
    #endregion

    #region Creature State
    public ObjectType objectType;
    public MonsterState monsterState = MonsterState.Patrol;
    public bool IsDead;
    private bool _init = false;
    #endregion

    #region Target
    public CharacterController Player; // 플레이어 연결
    public LayerMask IsTarget;         // 타겟이 되는 레이어
    public LayerMask BlockMask;        // 시야를 막을수 있는 레이어
    #endregion

    #region Vision Detect
    public float fieldOfView = 50f;    // 시야 각도
    public float viewDistance = 10f;   // 시야 거리
    public Transform eyeTransform;     // 시야 위치
    RaycastHit[] hits = new RaycastHit[10];  // NonAlloc 버퍼
    #endregion

    #region Turn / Rotate
    [Range(0.01f, 2f)] public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    #endregion

    #region Attack
    public float attackRadius = 2f;  // 공격 범위
    private float attackDistance;    // 공격 시작되는 거리
    private Vector3 _attackForward;
    private bool _lockAttackLook = false;

    [Header("Attack Root")]
    public Transform attackRoot_R;
    public Transform attackRoot_L;
    protected Transform _attackRoot;
    protected bool NextRight = true;   // 왼손 오른손 번갈아가며 공격
    List<CharacterController> lastAttackTargets = new List<CharacterController>();
    #endregion

    [Header("Audio Player")]
    AudioSource audioPlayer;
    public AudioClip hitClip;     // 피격시 사운드
    public AudioClip deathClip;   // 사망시 사운드

    #region Coroutine
    Coroutine _coKnockback;
    Coroutine _deathMoveCoroutine;
    Coroutine _coDotDamage;
    Coroutine _coMove;
    #endregion

    [HideInInspector] public Animator Anim;
    [HideInInspector] public NavMeshAgent agent;   // 경로 계산 AI
    [HideInInspector] public MonsterSpawn monsterSpawn;
    private Rigidbody rigid;
    private Collider coll;
    private DropItem _dropItem;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

        if (attackRoot_R != null)
            Gizmos.DrawSphere(attackRoot_R.position, attackRadius);

        if (attackRoot_L != null)
            Gizmos.DrawSphere(attackRoot_L.position, attackRadius);

        if (eyeTransform == null) return;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Handles.color = new Color(1f, 1f, 1f, 0.2f);
        Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
    }

#endif

    protected virtual void InitStats()
    {
        patrolSpeed = 2f;
        chaseSpeed = 2.8f;
        attack = 20f;
        hp = 100f;
        attackDistance = 1.8f;
    }

    protected virtual string GetAttackTrigger(bool useRight)
    {
        return useRight ? "AttackR" : "AttackL";
    }
    #region Monster Stat
    private float hp;
    private float attack;
    private float chaseSpeed;
    private float patrolSpeed;
    public float Hp
    {
        get => hp;
        set => hp = value;
    }
    public float Attack
    {
        get => attack;
        set => attack = value;
    }
    public float ChaseSpeed
    {
        get => chaseSpeed;
        set => chaseSpeed = value;
    }
    public float PatrolSpeed
    {
        get => patrolSpeed;
        set => patrolSpeed = value;
    }
    public float AttackDistance
    {
        get => attackDistance;
        set => attackDistance = value;
    }
    #endregion
    private void OnEnable()
    {
        Player = null;

        agent.enabled = true;
        agent.isStopped = false;

        if (_coMove != null)
            StopCoroutine(_coMove);

        _coMove = StartCoroutine(UpdateMove());
    }
    private void OnDisable()
    {
        if (_coMove != null)
        {
            StopCoroutine(_coMove);
            _coMove = null;
        }

        if (_deathMoveCoroutine != null)
        {
            StopCoroutine(_deathMoveCoroutine);
            _deathMoveCoroutine = null;
        }
        agent.enabled = false;
    }
    private void Update()
    {
        if (IsDead) return;

        // 이동 방향 갱신
        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.0001f)
            _moveDir = velocity.normalized;

        if (CanAutoAttack())
            UpdateAttack();

        Anim.SetFloat("Speed", agent.desiredVelocity.magnitude);
    }
    private void Awake()
    {
        Init(monsterSpawn);
    }
    public virtual bool Init(MonsterSpawn ms)
    {
        if (_init) return false;
        _init = true;

        objectType = ObjectType.Monster;
        rigid = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        audioPlayer = GetComponent<AudioSource>();
        _dropItem = GetComponent<DropItem>(); // 드랍아이템 스크립트 선언
        if (coll != null) coll.enabled = true;
        Anim = GetComponentInChildren<Animator>();

        InitStats(); // 스텟 초기화
        
        agent.stoppingDistance = attackDistance - 0.2f;
        agent.speed = patrolSpeed;

        monsterState = MonsterState.Patrol;
        transform.localScale = new Vector3(1f, 1f, 1f);

        _attackRoot = (NextRight ? attackRoot_R : attackRoot_L);
        ms = monsterSpawn;
        return true;
    }

    Vector3 _moveDir;
    private void FixedUpdate()
    {
        if (IsDead || !this.gameObject.activeSelf) return;
        if (Player == null || !Player.isActiveAndEnabled) return;

        if (monsterState == MonsterState.Attack)
        {
            Vector3 dir = _lockAttackLook ? _attackForward : transform.forward;
            float deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            int size = Physics.SphereCastNonAlloc(_attackRoot.position, attackRadius, dir, hits, deltaDistance, IsTarget);

            for (int i = 0; i < size; i++)
            {
                CharacterController target = hits[i].collider.GetComponent<CharacterController>();
                if (target != null && !lastAttackTargets.Contains(target))
                {
                    target.PlayerTakeDamage(Attack);
                    lastAttackTargets.Add(target);
                    break;
                }
            }
        }
    }
    IEnumerator UpdateMove()
    {
        while (!IsDead)
        {
            if (Player != null && Player.isActiveAndEnabled && !Player.IsDead)
            {
                if (monsterState == MonsterState.Patrol)
                {
                    monsterState = MonsterState.Chase;
                    agent.speed = chaseSpeed;
                }
                if (!agent.enabled || !agent.isOnNavMesh)
                {
                    yield return null;
                    continue;
                }
                // 추적 대상이 존재하면 경로 갱신하고 이동을 진행
                agent.SetDestination(Player.transform.position);
            }
            else
            {
                if (Player != null)
                    Player = null;

                if (monsterState != MonsterState.Chase)
                {
                    monsterState = MonsterState.Patrol;
                    agent.speed = patrolSpeed;
                }

                if (agent.remainingDistance <= 3.0f)
                {
                    Vector3 patrolPosition = Utils.GetRandomPointOnNavMesh(transform.position, 8f, NavMesh.AllAreas);
                    agent.SetDestination(patrolPosition);
                }

                Collider[] colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, IsTarget);
                foreach (Collider collider in colliders)
                {
                    if (!IsTargetOnSight(collider.transform)) continue;

                    CharacterController target = collider.GetComponent<CharacterController>();
                    if (target.isActiveAndEnabled && !target.IsDead)
                    {
                        Player = target;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.2f); // 0.2초 마다 코루틴 갱신
        }
    }
    public virtual void UpdateAttack()
    {
        monsterState = MonsterState.AttackBegin;

        bool useRight = NextRight;
        _attackRoot = useRight ? attackRoot_R : attackRoot_L;
        NextRight = !NextRight;

        if (Player != null && Player.isActiveAndEnabled && !Player.IsDead)
        {
            Vector3 dir = Player.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            _attackForward = transform.forward;
            _lockAttackLook = true;
        }
        agent.isStopped = true;
        agent.updateRotation = false;  // 몬스터 회전 방지

        Anim.applyRootMotion = false;
        Anim.SetTrigger(GetAttackTrigger(useRight));
    }

    #region Animation Event
    public virtual void EnableAttack()  // 공격이 활성화 될 때
    {
        monsterState = MonsterState.Attack;
        lastAttackTargets.Clear();
    }
    public virtual void DisableAttack() // 공격이 끝났을 때
    {
        monsterState = MonsterState.Chase;
        agent.isStopped = false;
        _lockAttackLook = false;
        agent.updateRotation = true;
    }
    public void OnDieAnimEnd() // 죽는 애니메이션 이벤트
    {
        Destroy(gameObject);
    }
    #endregion


    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        hp -= Mathf.RoundToInt(damage);

        if (hp <= 0)
        {
            IsDead = true;
            OnDead();
            return;
        }

        // 공격 받으면 바로 플레이어에게 돌진
        if (Player != null && Player.isActiveAndEnabled && !Player.IsDead)
        {
            if (monsterState == MonsterState.Patrol)
                monsterState = MonsterState.Chase;
            agent.SetDestination(Player.transform.position);
        }

        InvokeMonsterData();
        // 공격중엔 넉백이 되지 않음
        if (objectType == ObjectType.Monster && monsterState != MonsterState.Attack)
        {
            if (_coKnockback == null)
                _coKnockback = StartCoroutine(CoKnockBack());
        }
        // TODO : 이펙트 추가해서 넣기

        if (hitClip != null)
            audioPlayer.PlayOneShot(hitClip, volumeScale: 0.5f);
    }

    public virtual void OnDead()
    {
        OnBossDead?.Invoke();
        InvokeMonsterData();
        // TODO : 몬스터 죽일때 상승하는 데이터 (몬스터 킬수, 스코어 등)

        if (objectType == ObjectType.Boss || objectType == ObjectType.EliteMonster)
            return;
        else
        {
            // TODO : 골드나 아이템 같은거 드랍
            Vector3 dropPos;
            if (Utils.RandomDropPointOnNavMesh(transform.position, 0.1f, 0.4f, out dropPos))
                _dropItem.MakeDropItem(dropPos);
            else
                _dropItem.MakeDropItem(transform.position);
        }

        StopAllCoroutines();
        _coKnockback = null;
        agent.enabled = false;
        if (deathClip != null) audioPlayer.PlayOneShot(deathClip, volumeScale: 0.1f); // 사망시 효과음
        Anim.applyRootMotion = true;
        Anim.SetTrigger("Die"); // 애니메이션 재생
    }
    
    IEnumerator CoKnockBack()
    {
        monsterState = MonsterState.OnDamage;

        // 넉백 동안 agent 멈추기
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        float elapsed = 0;
        while (true)
        {
            elapsed += Time.deltaTime;
            if (elapsed > KNOCKBACK_TIME) // 넉백 되는 시간
                break;

            Vector3 dir = _moveDir * -1f;
            Vector3 nextVec = dir.normalized * KNOCKBACK_SPEED * Time.deltaTime; // 넉백 되는 속도
            rigid.MovePosition(rigid.position + nextVec);

            yield return null;
        }
        agent.Warp(rigid.position);  // agent 위치를 현재 위치로 동기화

        monsterState = MonsterState.Chase;
        yield return new WaitForSeconds(KNOCKBACK_COOLTIME); // 넉백이 너무 자주 되지 않도록 설정

        _coKnockback = null;
        agent.isStopped = false;
        yield break;
    }
    public virtual void OnCollisionEnter(Collision collision)
    {
        CharacterController target = collision.collider.GetComponentInParent<CharacterController>();
        if (target == null) return;
        if (!target.isActiveAndEnabled) return;
        if (!isActiveAndEnabled) return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);

        _coDotDamage = StartCoroutine(CoStartDotDamage(target));
    }
    public virtual void OnCollisionExit(Collision collision)
    {
        CharacterController target = collision.collider.GetComponent<CharacterController>();
        if (target == null) return;
        if (!target.isActiveAndEnabled) return;
        if (!this.isActiveAndEnabled) return;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);

        _coDotDamage = null;
    }

    IEnumerator CoStartDotDamage(CharacterController target)
    {
        while (true)
        {
            target.PlayerTakeDamage(Attack / 2f);
            yield return new WaitForSeconds(0.4f);
        }
    }
    protected virtual bool CanAutoAttack()
    {
        return (Player != null && monsterState == MonsterState.Chase && Vector3.Distance(Player.transform.position, transform.position) <= attackDistance);
    }
    bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;
        Vector3 dir = target.position - eyeTransform.position;
        dir.y = 0f;

        if (Vector3.Angle(dir, eyeTransform.forward) > fieldOfView * 0.5f)
            return false;

        int mask = IsTarget.value | BlockMask.value;
        if (Physics.Raycast(eyeTransform.position, dir, out hit, viewDistance, mask))
        {
            return ((1 << hit.collider.gameObject.layer) & IsTarget.value) != 0;
        }

        return false;
    }

    public void InvokeMonsterData()
    {
        if (this.isActiveAndEnabled && gameObject.activeInHierarchy && objectType != ObjectType.Monster)
            MonsterInfoUpdate?.Invoke(this);
    }
}
