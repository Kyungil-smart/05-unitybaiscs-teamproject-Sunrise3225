using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static Define;

public abstract class BossSkill
{
    public abstract BossSkillType Type { get; }
    public abstract IEnumerator Execute(BossMonster boss);
}

public class BossSkill_ApproachMelee : BossSkill
{
    public override BossSkillType Type => BossSkillType.ApproachMelee;

    public override IEnumerator Execute(BossMonster boss)
    {
        float time = 0f;

        boss.monsterState = MonsterState.Chase;
        boss.agent.isStopped = false;
        boss.agent.speed = boss.ChaseSpeed;

        while (time < boss.approachMaxTime && !boss.IsDead)
        {
            CharacterController player = boss.Player;
            if (player == null || player.IsDead) yield break;

            boss.agent.SetDestination(player.transform.position);

            float dir = Vector3.Distance(player.transform.position, boss.transform.position);
            if (dir <= boss.AttackDistance)
                break;

            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // 붙었으면 공격 한번
        if (boss.Player != null && !boss.Player.IsDead &&
            Vector3.Distance(boss.Player.transform.position, boss.transform.position) <= boss.AttackDistance)
        {
            boss.UpdateAttack();

            float wait = 0f;
            while (!boss.IsDead && wait < 2f && boss.monsterState != MonsterState.Chase)
            {
                wait += Time.deltaTime;
                yield return null;
            }
        }
    }
    public class BossSkill_SlamAttack : BossSkill
    {
        public override BossSkillType Type => BossSkillType.SlamAttack;

        public override IEnumerator Execute(BossMonster boss)
        {
            float time = 0f;
            CharacterController player = boss.Player;
            while (time < boss.approachMaxTime && !boss.IsDead)
            {
                if (player == null || player.IsDead) yield break;

                boss.agent.SetDestination(player.transform.position);

                float dir = Vector3.Distance(player.transform.position, boss.transform.position);
                if (dir <= boss.AttackDistance)
                    break;

                time += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            // 플레이어의 거리가 보스 사정거리가 아니면 바로 추적 전환
            if (player == null || player.IsDead) yield break;
            if (Vector3.Distance(player.transform.position, boss.transform.position) > boss.AttackDistance)
                yield break;

            boss.monsterState = MonsterState.AttackBegin;
            boss.agent.isStopped = true;
            boss.Anim.SetTrigger("Slam");

            yield return new WaitForSeconds(boss.slamWindup);
            Vector3 fxPos = boss.transform.position;
            Quaternion rot = Quaternion.identity;

            EffectManager.Instance.SpawnEffect(EffectManager.EffectType.BossSkill, fxPos, rot, null);
            if (boss.audioPlayer != null)
                boss.audioPlayer.PlayOneShot(boss.skillClip, volumeScale: .7f);

            boss.DoAoeDamage(boss.transform.position, boss.slamRadius, boss.Attack * boss.slamDamageMul);

            yield return new WaitForSeconds(0.1f);
            boss.monsterState = MonsterState.Chase;
            boss.agent.isStopped = false;
        }
    }

    public class BossSkill_RushAttack : BossSkill
    {
        public override BossSkillType Type => BossSkillType.RushAttack;

        public override IEnumerator Execute(BossMonster boss)
        {
            float time = 0f;

            // NavMesh로 추적
            boss.monsterState = MonsterState.Chase;
            boss.agent.isStopped = false;
            boss.agent.speed = boss.ChaseSpeed;

            while (time < boss.approachMaxTime && !boss.IsDead)
            {
                CharacterController player = boss.Player;
                if (player == null || player.IsDead) yield break;

                boss.agent.SetDestination(player.transform.position);

                float dist = Vector3.Distance(player.transform.position, boss.transform.position);
                if (dist <= boss.AttackDistance)
                    break;

                time += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            //  범위 밖이면 러시 발동 안 하고 종료
            CharacterController p = boss.Player;
            if (p == null || p.IsDead) yield break;
            if (Vector3.Distance(p.transform.position, boss.transform.position) > boss.AttackDistance)
                yield break;

            //  범위 안이면 BossRush 애니메이션 실행
            boss.monsterState = MonsterState.AttackBegin;
            boss.agent.isStopped = true;
            boss.agent.ResetPath();

            boss.Anim.ResetTrigger("BossRush");
            boss.Anim.SetTrigger("BossRush");

            yield return new WaitForSeconds(boss.rushWindup);

            boss.agent.isStopped = true;
            boss.agent.ResetPath();
            boss.agent.enabled = false; 

            Rigidbody rb = boss.GetComponent<Rigidbody>();
            Vector3 dir = (p.transform.position - boss.transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude < 0.0001f ? boss.transform.forward : dir.normalized;

            float traveled = 0f;
            while (traveled < boss.rushDistance && !boss.IsDead)
            {
                float step = boss.rushSpeed * Time.deltaTime;
                rb.MovePosition(rb.position + dir * step);
                traveled += step;
                yield return null;
            }

            // 러시 끝
            boss.agent.enabled = true;
            boss.agent.Warp(boss.transform.position);
            boss.agent.isStopped = false;
        }
    }
}