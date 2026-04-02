using Scripts.SkillSystem.Manage;
using Scripts.SkillSystem.Skills;
using Chipmunk.ComponentContainers;
using UnityEngine;

namespace Code.SHS.Entities.Enemies.FSM
{
    public class EnemySkillAimState : EnemyState
    {
        private ActiveSkillComponent _skillComponent;
        private EnemyAimProvider _aimProvider;
        private IAimSkill _aimSkill;
        private bool _isSuccess;
        private float aimTimer;
        private float aimDuration = 2.5f;

        public EnemySkillAimState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _skillComponent = container.Get<ActiveSkillComponent>();
            _aimProvider = container.Get<EnemyAimProvider>();
        }

        public override void Enter()
        {
            base.Enter();
            _isSuccess = false;
            _aimSkill = _skillComponent.CurrentSkill.GetComponent<IAimSkill>();
            _aimSkill.StartAiming();
            aimTimer = aimDuration;

            if (TargetEntity == null)
            {
                _aimSkill.CancelSkill();
                return;
            }
        }

        public override void Update()
        {
            base.Update();
            if (TargetEntity == null)
            {
                _aimSkill.CancelSkill();
                return;
            }

            aimTimer -= Time.deltaTime;
            if (aimTimer <= 0)
            {
                _isSuccess = true;
                _enemy.ChangeState(EnemyStateEnum.Skill);
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (_isSuccess)
                return;

            _aimSkill.CancelSkill();
        }
    }
}