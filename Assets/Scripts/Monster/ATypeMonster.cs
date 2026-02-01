using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATypeMonster : MonsterController
{
    // 가장 처음 만든 몬스터 (좌우 공격을 해서 부모 클래스에서 변경할 부분이 없음)
    protected override void InitStats()
    {
        PatrolSpeed = 5.5f;
        ChaseSpeed = 5.2f;
        Attack = 14f;
        Hp = 100f;
        MaxHp = 100f;
        AttackDistance = 1.8f;
    }
}
