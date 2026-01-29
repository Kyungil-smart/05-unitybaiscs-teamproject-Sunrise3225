using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ObjectType
    {
        Player,
        Monster,
        EliteMonster,
        Boss,
        Barrel,
        Box,
    }
    public enum MonsterState
    {
        Patrol,
        Chase,
        AttackBegin,
        Attack,
        OnDamage,
        Dead,
    }
    public enum BossSkillType
    {
        ApproachMelee,  // 접근하다가 근처면 공격, 아니면 스킵
        SlamAttack,     // 제자리 광역 내려찍기
    }

    #region 넉백 데이터
    public static float KNOCKBACK_TIME = 0.2f;        // 밀려나는 시간
    public static float KNOCKBACK_SPEED = 10;        // 밀려나는 속도
    public static float KNOCKBACK_COOLTIME = 0.2f;   // 넉백이 자주 되지 않게 시간 설정
    #endregion
}
