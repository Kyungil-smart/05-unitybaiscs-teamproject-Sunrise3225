//using System.Collections.Generic;
//using UnityEngine;
//using static Define;

//public class BossMonster : MonsterController
//{
//    public bool randomPattern = false;
//    public List<BossSkillType> pattern = new List<BossSkillType>()
//    {
//        BossSkillType.ApproachMelee,
//        BossSkillType.JumpAttack,
//        BossSkillType.SlamAttack,
//    };
//    [Header("Skill Timing")]
//    public float skillGap = 0.35f;
//    // 1번 스킬
//    public float approachMaxTime = 2.0f; // 이 시간에 못 붙으면 다음 스킬로 넘어감
//    // 2번 스킬
//    public float jumpWindup = 0.2f;
//    public float jumpAttackRadius = 2.5f;
//    public float jumpAttackDamageMul = 1.2f;
//    // 3번 스킬
//    public float slamWindup = 0.35f;
//    public float slamRadius = 3.5f;
//    public float slamDamageMul = 1.5f;

//    Coroutine _coPattern;
//    int _patternIndex = 0;
//    bool _doingSkill = false;

//    protected override bool CanAutoAttack() => false;

//    //private void OnEnable()
//    //{
//    //    if (_coPattern != null) StopCoroutine(_coPattern);
//    //    _coPattern = StartCoroutine(CoPatternLoop());
//    //}
//    private void OnDisable()
//    {
//        if (_coPattern != null)
//        {
//            StopCoroutine(_coPattern);
//            _coPattern = null;
//        }
//    }
//    //IEnumerator CoPatternLoop()
//    //{
//    //    while (!IsDead)
//    //    {
//    //        if (_doingSkill)
//    //        {
//    //            yield return null;
//    //            continue;
//    //        }

//    //        if (Player == null || !Player.isActiveAndEnabled || Player.IsDead)
//    //        {
//    //            yield return null;
//    //            continue;
//    //        }
//    //        _doingSkill = true;

//    //        BossSkillType next = GetNextSkill();
//    //        yield return ExvuteSkill(next);


//    //    }
//    //}
//}
