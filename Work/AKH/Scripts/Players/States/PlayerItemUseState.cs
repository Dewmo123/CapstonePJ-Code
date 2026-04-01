using Chipmunk.ComponentContainers;
using Code.InventorySystem;
using Code.Players;
using UnityEngine;
using Work.LKW.Code.Items;

namespace Scripts.Players.States
{
    public class PlayerItemUseState : PlayerMoveState
    {
        private UsableItem _item;
        private PlayerEquipment _equipment;
        private PlayerInventory _inventory;
        public PlayerItemUseState(ComponentContainer container, int animationHash) : base(container, animationHash)
        {
            _myMoveType = MoveType.Walk;
            _equipment = container.Get<PlayerEquipment>();
            _inventory = container.Get<PlayerInventory>();
        }
        public override void Enter()
        {
            base.Enter();

            if (_equipment.TryGetEquippedItem(EquipType.Hand, out EquipableItem item) && item is UsableItem usable)
                _item = usable;
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

            if (_item != null && _inventory.RemoveItem(_item, 1, false))
            {
                _item.Use(_player);
            }

            _equipment.ChangeHandlingItem(_equipment.HandledSlotType, _equipment.GetEquipSlotItem((EquipSlotType)_equipment.HandledSlotType));
        }
    }
}

