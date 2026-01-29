using System.Collections;
using UnityEngine;
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

            boss.monsterState = MonsterState.AttackBegin;
            boss.agent.isStopped = true;

            boss.Anim.SetTrigger("Slam");

            yield return new WaitForSeconds(boss.slamWindup);

            boss.DoAoeDamage(boss.transform.position, boss.slamRadius, boss.Attack * boss.slamDamageMul);

            yield return new WaitForSeconds(0.1f);
            boss.monsterState = MonsterState.Chase;
            boss.agent.isStopped = false;
        }
    }
}
