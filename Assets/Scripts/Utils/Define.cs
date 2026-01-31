using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ItemType
    {
        HealItem,
        AmmoItem,
        FastItem,
        SlowItem,
        FreezeItem,
        GoldItem,
    }
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
        RushAttack      // 돌진공격 일단은 돌진공격 추후에 수정하자.
    }

    public enum StatType
    {
        Attack,
        MaxMagazine,
        MaxHp,
    }
    #region 넉백 데이터
    public static float KNOCKBACK_TIME = 0.2f;        // 밀려나는 시간
    public static float KNOCKBACK_SPEED = 10;        // 밀려나는 속도
    public static float KNOCKBACK_COOLTIME = 0.2f;   // 넉백이 자주 되지 않게 시간 설정
    #endregion

    #region 아이템 드랍 확률
    public static readonly float[] ITEM_DROP_PROB = new float[]
    {
        0.3f,  // Heal 아이템 드랍 확률
        0.4f,  // Ammo 아이템 드랍 확률
        0.15f, // Fast 아이템 드랍 확률
        0.14f, // Slow 아이템 드랍 확률
        0.01f, // Freeze 아이템 드랍
    };
    #endregion
}
