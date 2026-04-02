using Chipmunk.ComponentContainers;
using Code.Players;
using Scripts.Combat.Datas;
using UnityEngine;
using Work.LKW.Code.Items;

namespace Code.SHS.Entities.Enemies.FSM.BehaviourState
{
    public class EnemyAttackState : EnemyState
    {
        private IAttackable _weaponItem;
        private EnemyEquipment _equipment;

        public EnemyAttackState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _equipment = container.Get<EnemyEquipment>();
        }

        public override void Enter()
        {
            base.Enter();
            _animatorTrigger.OnDamageCastTrigger += HandleDamageCast;

            if (_equipment.TryGetEquippedItem(EquipType.Hand, out EquipableItem item) &&
                item is IAttackable attackable)
                _weaponItem = attackable;
            _weaponItem?.EnterAttack();
            _behaviourManager.CurrentBehaviour?.SetCooldown();
            _movement.SetStop(false);
        }

        public override void Update()
        {
            base.Update();
            if (_isTriggerCall)
                _enemy.ChangeState(EnemyStateEnum.Aim);
            UpdateMovementAnimation();
        }

        private void HandleDamageCast()
        {
            if (_enemy.TargetProvider.Target != null)
            _movement.LookAtTarget(_enemy.TargetProvider.Target.transform.position);
            _weaponItem?.AttackTrigger();
        }

        public override void Exit()
        {
            base.Exit();
            _animatorTrigger.OnDamageCastTrigger -= HandleDamageCast;
        }
    }
}