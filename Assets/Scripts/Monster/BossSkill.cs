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
            float rushTriggerDistance = boss.AttackDistance + 3.0f;

            while (!boss.IsDead)
            {
                CharacterController p = boss.Player;
                if (p == null || p.IsDead) yield break;

                if (Vector3.Distance(p.transform.position, boss.transform.position) <= rushTriggerDistance)
                    break;

                yield return new WaitForSeconds(0.1f);
            }

            boss.monsterState = MonsterState.AttackBegin;

            if (boss.agent != null && boss.agent.enabled)
            {
                boss.agent.isStopped = true;
                boss.agent.ResetPath();
            }

            // Trigger 대신 Bool로 러시 상태 유지
            if (boss.Anim != null)
            {
                boss.Anim.applyRootMotion = false;
                boss.Anim.SetBool("IsRushing", true);
            }

            // 애니 시작 후 던지는 타이밍
            float launchDelay = boss.rushLaunchDelay;
            if (launchDelay > 0f)
                yield return new WaitForSeconds(launchDelay);

            CharacterController player = boss.Player;
            if (player == null || player.IsDead || boss.IsDead)
            {
                if (boss.Anim != null) boss.Anim.SetBool("IsRushing", false);
                yield break;
            }

            Rigidbody rb = boss.GetComponent<Rigidbody>();
            if (rb == null)
            {
                if (boss.Anim != null) boss.Anim.SetBool("IsRushing", false);
                boss.monsterState = MonsterState.Chase;
                if (boss.agent != null && boss.agent.enabled) boss.agent.isStopped = false;
                yield break;
            }

            Vector3 dir = (player.transform.position - boss.transform.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude < 0.0001f ? boss.transform.forward : dir.normalized;

            // agent 끄기 전에 멈춤/리셋
            if (boss.agent != null && boss.agent.enabled)
            {
                boss.agent.isStopped = true;
                boss.agent.ResetPath();
                boss.agent.enabled = false;
            }

            rb.isKinematic = false;
            rb.velocity = Vector3.zero;

            // 속도 / 거리 분리
            float launchSpeed = boss.rushLaunchSpeed;
            float travelDistance = boss.rushTravelDistance;

            rb.AddForce(dir * launchSpeed, ForceMode.VelocityChange);

            Vector3 start = rb.position;
            float elapsed = 0f;
            float maxTime = 1.0f;

            while (!boss.IsDead)
            {
                elapsed += Time.deltaTime;

                if (Vector3.Distance(start, rb.position) >= travelDistance)
                    break;

                if (elapsed >= maxTime)
                    break;

                yield return null;
            }

            // 종료 처리
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            if (boss.agent != null)
            {
                boss.agent.enabled = true;
                boss.agent.Warp(rb.position);
                boss.agent.isStopped = false;
            }

            boss.monsterState = MonsterState.Chase;

            // 러시 애니 종료
            if (boss.Anim != null)
                boss.Anim.SetBool("IsRushing", false);
        }
    }
}