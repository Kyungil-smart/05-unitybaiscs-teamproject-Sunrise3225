
public class BTypeMonster : MonsterController
{
    protected override void InitStats()
    {
        PatrolSpeed = 4.5f;
        ChaseSpeed = 7.2f;
        Attack = 17f;
        Hp = 100f;
        MaxHp = 100f;
        AttackDistance = 1.8f;
    }
    protected override string GetAttackTrigger(bool useRight)
    {
        // Attack 애니메이션이 다른 경우에 추가하여 사용
        return base.GetAttackTrigger(useRight);
    }
}
