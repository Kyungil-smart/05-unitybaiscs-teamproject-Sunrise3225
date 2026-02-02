
public class ATypeMonster : MonsterController
{
    // 가장 처음 만든 몬스터 (좌우 공격을 해서 부모 클래스에서 변경할 부분이 없음)
    protected override void InitStats()
    {
        PatrolSpeed = 4.2f;
        ChaseSpeed = 6.8f;
        Attack = 14f;
        Hp = 125f;
        MaxHp = 125f;
        AttackDistance = 1.8f;
    }
}
