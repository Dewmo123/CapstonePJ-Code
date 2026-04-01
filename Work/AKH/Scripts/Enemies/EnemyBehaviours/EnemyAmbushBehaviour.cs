using Chipmunk.ComponentContainers;
using Code.SHS.Entities.Enemies;
using Code.SHS.Entities.Enemies.FSM;
using Scripts.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.Enemies.EnemyBehaviours
{
    public class EnemyAmbushBehaviour : EnemyBehaviour
    {
        [SerializeField] private float ambushRange = 10f;
        private CharacterNavMovement _movement;
        private AttackCompo _attackCompo; // 공격 사거리를 확인하기 위함

        public override void Init(Enemy enemy)
        {
            base.Init(enemy);
            _movement = enemy.Get<CharacterNavMovement>();
            _attackCompo = enemy.Get<AttackCompo>();
        }


        public override void Execute()
        {
            // 플레이어에게 빠르게 돌진하여 공격 사거리 안으로 진입
            Vector3 targetPos = _enemy.TargetProvider.LastTargetPosition;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                _movement.SetDestinationForce(hit.position);
                _movement.MoveType = NavMoveType.Sprint; 
                _enemy.ChangeState(EnemyStateEnum.MoveTo);
            }
            else
            {
                // 이동 불가능 시 대기
                SetCooldown();
                _enemy.ChangeState(EnemyStateEnum.Aim);
            }
        }
    }
}
