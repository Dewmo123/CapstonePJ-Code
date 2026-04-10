namespace Scripts.Enemies.EnemyBehaviourConditions
{
    public class IsTargetLostCondition : EnemyBehaviourCondition
    {
        public override bool Condition()
        {
            return _enemy.TargetProvider.Target == null;
        }
    }
}
