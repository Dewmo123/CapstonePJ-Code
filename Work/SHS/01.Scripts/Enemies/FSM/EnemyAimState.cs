using Chipmunk.ComponentContainers;
using Scripts.Combat;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class EnemyAimState : EnemyState
    {
        private float _aimTime;
        private float _minAimTime = 0f;
        private float _maxAimTime = 0f;
        private float _currentAimDuration;

        //private float _optimalRangeRatio = 0.7f;

        public EnemyAimState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _attackCompo = container.GetCompo<AttackCompo>();
        }

        public override void Enter()
        {
            base.Enter();
            _attackCompo.IsAim = true;
            _currentAimDuration = Random.Range(_minAimTime, _maxAimTime);
            _movement.MoveType = NavMoveType.Aim;
            _aimTime = 0f;
            Execute(); //행동에서 aimState 가면 유니티 터짐;;
        }

        public override void Update()
        {
            base.Update();

            if (TargetEntity == null)
            {
                _enemy.ChangeState(EnemyStateEnum.Chase);
                return;
            }

            float distance = Vector3.Distance(_enemy.transform.position, TargetEntity.transform.position);
            if (distance > _attackRange)
            {
                _enemy.ChangeState(EnemyStateEnum.Chase);
                return;
            }

            _movement.SetLookAtTarget(TargetEntity.transform);
            UpdateMovementAnimation();
            _aimTime += Time.deltaTime;
            Execute();
        }

        private void Execute()
        {
            if (_aimTime >= _currentAimDuration)
            {
                _behaviourManager.ExecuteOptimal();
            }
        }

        public override void Exit()
        {
            base.Exit();
            _attackCompo.IsAim = false;
        }
    }
}