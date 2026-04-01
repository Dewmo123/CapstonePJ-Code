using Chipmunk.ComponentContainers;
using Scripts.Entities;
using Scripts.SkillSystem;
using UnityEngine;

namespace Scripts.Players.States
{
    public class PlayerSkillState : PlayerState
    {
        private ActiveSkillComponent _skillCompo;
        public PlayerSkillState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _skillCompo = container.Get<ActiveSkillComponent>(true);
        }
        public override void Enter()
        {
            base.Enter();
            _movement.StopImmediately();
            Debug.Assert(_skillCompo != null && _skillCompo.CurrentSkill != null,
                "CurrentSkill is null but you are in skill state");
            _animator.SetParam(_skillCompo.CurrentSkill.animHash,true);
            _skillCompo.CurrentSkill.UseSkill();
        }
        public override void Update()
        {
            base.Update();
            if (_isTriggerCall)
                _player.ChangeState(PlayerStateEnum.Idle);
        }
        public override void Exit()
        {
            base.Exit();
            _skillCompo.CurrentSkill.EndSkill();
            _animator.SetParam(_skillCompo.CurrentSkill.animHash,false);
        }
    }
}
