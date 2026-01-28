using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTypeMonster : MonsterController
{
    protected override void InitStats()
    {
        PatrolSpeed = 2.5f;
        ChaseSpeed = 3.2f;
        Attack = 14f;
        Hp = 60f;
        AttackDistance = 1.8f;
    }
    protected override string GetAttackTrigger(bool useRight)
    {
        // Attack 애니메이션이 다른 경우에 추가하여 사용
        return base.GetAttackTrigger(useRight);
    }
}
