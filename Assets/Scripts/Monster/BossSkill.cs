using System.Collections;
using UnityEngine;
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
            if (boss.Player == null || boss.Player.IsDead) yield break;

            int rushCount = 3;
            float speed = boss.rushSpeed * 2.5f;
            float maxDistance = boss.rushDistance;

            for (int i = 0; i < rushCount; i++)
            {
                // 돌진 시작 전 플레이어 방향으로 몸 회전
                if (boss.Player != null && !boss.Player.IsDead)
                {
                    Vector3 lookDir = boss.Player.transform.position - boss.transform.position;
                    lookDir.y = 0;
                    if (lookDir != Vector3.zero)
                        boss.transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // 돌진 애니메이션 트리거
                boss.monsterState = MonsterState.AttackBegin;
                boss.agent.isStopped = true;
                boss.Anim.SetTrigger("Rush");

                Vector3 dir = boss.transform.forward; // 몸 방향 기준 돌진
                float traveled = 0f;
                while (traveled < maxDistance && !boss.IsDead)
                {
                    Vector3 move = dir * speed * Time.deltaTime;
                    boss.transform.position += move;
                    traveled += speed * Time.deltaTime;

                    boss.DoRushDamage(boss.transform.position, dir, 0.5f, boss.Attack * boss.rushDamageMul);
                    yield return null;
                }

                // 돌진 후 잠깐 멈춤
                boss.monsterState = MonsterState.Chase;
                boss.agent.isStopped = false;
                yield return new WaitForSeconds(1.0f); // 다음 돌진 전 짧은 멈춤
            }
        }
    }
}