using Chipmunk.ComponentContainers;
using Scripts.Enemies.EnemyBehaviours;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class MeleeEnemyChaseState : EnemyState
    {
        public MeleeEnemyChaseState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _movement.MoveType = NavMoveType.Sprint;
            _movement.SetLookAtTarget(null);
            _movement.SetStop(false);
        }

        public override void Update()
        {
            base.Update();

            if (TargetEntity == null)
                return;

            float distance = Vector3.Distance(TargetEntity.transform.position,_enemy.transform.position);
            if (distance <= _attackRange)
            {
                _enemy.ChangeState(EnemyStateEnum.Aim);
                return;
            }

            _movement.SetDestination(_targetProvider.LastTargetPosition);
            _behaviourManager.ExecuteOptimal();

            UpdateMovementAnimation();
        }
    }
}