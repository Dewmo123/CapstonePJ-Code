using Chipmunk.ComponentContainers;
using Scripts.Enemies.States;
using Scripts.Entities;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class EnemyChaseState : EnemyExecuteBehaviourState
    {
        public override float ExecuteTimer => 0.1f;

        public EnemyChaseState(ComponentContainer container, int animationHash) : base(container, animationHash)
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

            if (TargetEntity == null && _movement.IsArrived)
            {
                _enemy.ChangeState(EnemyStateEnum.Idle);
                return;
            }

            if (TargetEntity != null)
            {
                float distance = Vector3.Distance(_enemy.transform.position, _targetProvider.CurrentTarget.transform.position);
                if (distance <= _attackRange)
                {
                    _enemy.ChangeState(EnemyStateEnum.Aim);
                    return;
                }
            }

            Vector3 destination = TargetEntity != null ? TargetEntity.transform.position : _targetProvider.LastTargetPosition;
            _movement.SetDestination(destination);
            UpdateMovementAnimation();
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
            if (TargetEntity == null)
                _targetProvider.TargetLost(_targetProvider.LastTargetPosition);
        }
    }
}