using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATypeMonster : MonsterController
{
    // 가장 처음 만든 몬스터 (좌우 공격을 해서 부모 클래스에서 변경할 부분이 없음)
    protected override void InitStats()
    {
        base.InitStats();
    }
}
