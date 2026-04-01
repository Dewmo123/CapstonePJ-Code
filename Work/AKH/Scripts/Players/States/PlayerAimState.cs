using Chipmunk.ComponentContainers;
using Scripts.Combat.Datas;

namespace Scripts.Players.States
{
    public class PlayerAimState : PlayerLocomotionCombatState
    {
        public PlayerAimState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _myMoveType = MoveType.Aim;
        }

        public override void Update()
        {
            base.Update();
            if (_player.PlayerInput.AttackKey
                && _weapon != null
                && _weapon.IsEquipped
                && _weapon is IAttackable attackable
                && attackable.CurrentAttackableState == AttackableState.CanAttack)
                _player.ChangeState(PlayerStateEnum.Attack);
            if (!_player.PlayerInput.AimKey)
                _player.ChangeState(PlayerStateEnum.Idle);
        }
    }
}
