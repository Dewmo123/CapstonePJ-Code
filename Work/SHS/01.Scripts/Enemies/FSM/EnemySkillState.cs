using Chipmunk.ComponentContainers;
using Scripts.SkillSystem;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class EnemySkillState : EnemyState
    {
        private ActiveSkillComponent _skillCompo;
        private EnemyAimProvider _aimProvider;

        public EnemySkillState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _skillCompo = container.Get<ActiveSkillComponent>(true);
            _aimProvider = container.Get<EnemyAimProvider>();
        }

        public override void Enter()
        {
            base.Enter();
            _movement.SetStop(true);
            Debug.Assert(_skillCompo != null && _skillCompo.CurrentSkill != null,
                "CurrentSkill is null but you are in skill state");
            _animator.SetParam(_skillCompo.CurrentSkill.animHash, true);
            _skillCompo.CurrentSkill.UseSkill();
        }

        public override void Update()
        {
            base.Update();
            if (_isTriggerCall)
            {
                _enemy.ChangeState(EnemyStateEnum.Aim);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _skillCompo.CurrentSkill.EndSkill();
            _animator.SetParam(_skillCompo.CurrentSkill.animHash, false);
            _movement.SetStop(false);
        }
    }
}
