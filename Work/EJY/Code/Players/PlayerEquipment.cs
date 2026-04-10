using AYellowpaper.SerializedCollections;
using Chipmunk.ComponentContainers;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.InventorySystems.Items;
using InGame.InventorySystem;
using Scripts.Combat.Datas;
using Scripts.Entities;
using System.Collections.Generic;
using System.Linq;
using Chipmunk.Modules.StatSystem;
using Scripts.Players;
using Scripts.Players.States;
using UnityEngine;
using Work.LKW.Code.Items;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.Players
{
    public struct TryEquipData
    {
        public bool IsSuccess;
        public EquipableItem EquippedItem;
        
        public TryEquipData(bool isSuccess, EquipableItem equippedItem)
        {
            IsSuccess = isSuccess;
            EquippedItem = equippedItem;
        }
    }
    
    public enum EquipType
    {
        None = -1,
        Hand = 0,
        Helmet = 1,
        Armor = 2,
        Count
    }

    public enum EquipSlotType
    {
        None = -1,
        Weapon1 = 0,
        Weapon2 = 1,
        Melee = 2,
        Helmet = 3,
        Armor = 4,
        Count
    }

    public enum HotbarSlotType
    {
        None = -1,
        Item1 = 0,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6,
        Item7,
        Count
    }

    public class PlayerEquipment : MonoBehaviour, IContainerComponent
    {
        [SerializeField] private SerializedDictionary<EquipType, Transform> equipTrms;
        [SerializeField] private EquipItemDataSO test;
        public ComponentContainer ComponentContainer { get; set; }

        private Player _player;
        private PlayerInventory _playerInventory;

        private StatOverrideBehavior _stat;

        // 현재 어떤 부위에 어떤 장비를 장착하고 있는지
        private Dictionary<EquipType, EquipableItem> _equips = new Dictionary<EquipType, EquipableItem>();

        // 플레리어의 슬롯
        private Dictionary<EquipSlotType, EquipSlot> _equipSlots = new Dictionary<EquipSlotType, EquipSlot>();
        private HotbarSlotType _handlingSlotType = HotbarSlotType.None;
        private HotbarSlotType _handledSlotType = HotbarSlotType.None;
        
        public HotbarSlotType HandlingSlotType => _handlingSlotType;
        public HotbarSlotType HandledSlotType => _handledSlotType;

        public void OnInitialize(ComponentContainer componentContainer)
        {
            for (int i = 0; i < (int)EquipType.Count; ++i)
            {
                _equips.Add((EquipType)i, null);
            }

            for (int i = 0; i < (int)EquipSlotType.Count; ++i)
            {
                EquipSlotType slotType = (EquipSlotType)i;
                var equipSlot = new EquipSlot(null, slotType);
                _equipSlots.Add(slotType, equipSlot);
            }

            _player = componentContainer.GetCompo<Player>(true);
            _playerInventory = componentContainer.GetComponent<PlayerInventory>();
            _stat = componentContainer.GetComponent<StatOverrideBehavior>();

            EventBus.Subscribe<ReplaceBulletEvent>(HandleReplaceBullet);
            EventBus.Subscribe<SwapEquipEvent>(HandleSwapEquip);
            EventBus.Subscribe<EquipByDragEvent>(HandleEquipByDrag);
            EventBus.Subscribe<UnEquipByDragEvent>(HandleUnEquipByDrag);
        }

        private void Start()
        {
            EventBus<UpdateEquipUIEvent>.Raise(new UpdateEquipUIEvent(_equipSlots.ToList()));
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ReplaceBulletEvent>(HandleReplaceBullet);
            EventBus.Unsubscribe<SwapEquipEvent>(HandleSwapEquip);
            EventBus.Unsubscribe<EquipByDragEvent>(HandleEquipByDrag);
            EventBus.Unsubscribe<UnEquipByDragEvent>(HandleUnEquipByDrag);
        }
        
        private void HandleEquipByDrag(EquipByDragEvent evt)
        {
            if (evt.Item is EquipableItem equipalbeItem && evt.Item.ItemData is EquipItemDataSO)
            {
                var tryData = Equip(equipalbeItem, evt.Type, true);
                if(tryData.IsSuccess && tryData.EquippedItem != null)
                    evt.StartSlot.SetData(tryData.EquippedItem, 1);
                    
            }
        }

        private void HandleUnEquipByDrag(UnEquipByDragEvent evt)
        {
            if (evt.Item is EquipableItem equipalbeItem && evt.Item.ItemData is EquipItemDataSO)
            {
                if(UnEquip(equipalbeItem, true, true))
                    evt.TargetSlot.SetData(equipalbeItem, 1);
            }
        }

        private void HandleSwapEquip(SwapEquipEvent evt)
        {
            var startSlotItem = evt.StartEquip.Item as EquipableItem;
            var targetSlotItem = evt.TargetEquip.Item as EquipableItem;
            
            if(evt.StartEquip == evt.TargetEquip) return;

            if (startSlotItem == null)
            {
                evt.TargetEquip.SetData(null);
                UpdateHotbarSlot(evt.TargetEquip.EquipType, null);
            }
            else
            {
                evt.TargetEquip.SetData(startSlotItem, 1);
                UpdateHotbarSlot(evt.TargetEquip.EquipType, startSlotItem);
            }

            if (targetSlotItem == null)
            {
                evt.StartEquip.SetData(null);
                UpdateHotbarSlot(evt.StartEquip.EquipType, null);
            }
            else
            {
                evt.StartEquip.SetData(targetSlotItem, 1);
                UpdateHotbarSlot(evt.StartEquip.EquipType, targetSlotItem);
            }

            if (evt.StartEquip.Item != null && evt.TargetEquip.Item != null)
            {
                if (evt.StartEquip.EquipType == (EquipSlotType)_handlingSlotType)
                {
                    RefreshHandItem(targetSlotItem);
                }
                else if (evt.TargetEquip.EquipType == (EquipSlotType)_handlingSlotType)
                {
                    RefreshHandItem(startSlotItem);
                }
            }
            else
            {
                var notEmptySlot = evt.StartEquip.Item != null ? evt.StartEquip : evt.TargetEquip;

                _handlingSlotType = (HotbarSlotType)notEmptySlot.EquipType;
            }

            EventBus<UpdateEquipUIEvent>.Raise(new UpdateEquipUIEvent(_equipSlots.ToList()));
        }

        private void UpdateHotbarSlot(EquipSlotType slotType, EquipableItem item)
        {
            if (!IsHotbarSlotType(slotType))
                return;

            if (item == null)
            {
                EventBus<UnEquipHotbarEvent>.Raise(new UnEquipHotbarEvent((int)slotType));
                return;
            }

            EventBus<EquipHotbarEvent>.Raise(new EquipHotbarEvent((int)slotType, item));
        }

        private static bool IsHotbarSlotType(EquipSlotType slotType)
            => slotType == EquipSlotType.Weapon1 || slotType == EquipSlotType.Weapon2 || slotType == EquipSlotType.Melee;
        private void RefreshHandItem(EquipableItem newItem)
        {
            if (_equips[EquipType.Hand] != null)
                (_equips[EquipType.Hand] as IEquipable).Unequip(_player);

            _equips[EquipType.Hand] = newItem;

            if (newItem != null)
            {
                newItem.Equip(_player, equipTrms[EquipType.Hand]);
            }
            EventBus.Raise(new ChangeHandlingEvent(newItem));
        }
        
        private void HandleReplaceBullet(ReplaceBulletEvent evt)
        {
            var currentHandleItem = GetEquippedItem(EquipType.Hand);

            if (currentHandleItem is GunItem gun && evt.Bullet.bulletDataSO != gun.currentBulletItem?.bulletDataSO)
            {
                gun.ChangeBullet(evt.Bullet);
                _player.ChangeState(PlayerStateEnum.Reload, true);
            }
        }

        public TryEquipData Equip(EquipableItem equipable, EquipSlotType type = EquipSlotType.None, bool byDrag = false)
        {
            EquipableItem equippedItem = null;

            EquipSlotType targetType = type;
            EquipSlotType itemType = equipable.EquipItemData.itemType.GetEquipSlotType();

            if (type == EquipSlotType.None)
                targetType = GetSuitableSlotType(itemType);

            if (!IsSuitable(targetType, itemType) || targetType == EquipSlotType.None) return new TryEquipData(false, null);

            var itemSlot = _equipSlots[targetType];

            // 이미 장착된게 있는지 확인, 없으면 추가 있으면 교체
            if (itemSlot.Item != null)
            {
                EquipableItem equipped = itemSlot.Item as EquipableItem;
                equippedItem = equipped;
                UnEquip(equipped, byDrag: byDrag, isExchange: true);
            }

            AddStatModify(equipable.EquipItemData);
            itemSlot.SetData(equipable, 1);

            EquipType equipType = targetType.GetEquipType();
            if (_equips.TryGetValue(equipType, out EquipableItem equippingItem) && equippingItem == null)
            {
                _equips[equipType] = equipable;
                if (equipType == EquipType.Hand)
                {
                    _handlingSlotType = (HotbarSlotType)targetType;
                    EventBus.Raise(new ChangeHandlingEvent(equipable));
                }
                
                equipable.Equip(_player, equipTrms[equipType]);
            }

            if (equipType == EquipType.Hand)
            {
                EventBus<EquipHotbarEvent>.Raise(new EquipHotbarEvent((int)targetType, equipable));
            }

            EventBus.Raise(new EquipItemEvent(itemSlot, targetType));

            EventBus.Raise(new UpdateEquipUIEvent(_equipSlots.ToList()));

            return new TryEquipData(true, equippedItem);
        }

        public bool UnEquip(EquipableItem equipped, bool isRaiseEvent = false, bool byDrag = false, bool isExchange = false)
        {
            EquipSlotType itemSlotType = GetEquippedSlotType(equipped);

            if (itemSlotType == EquipSlotType.None) return false;

            if (_equipSlots.TryGetValue(itemSlotType, out EquipSlot slot))
            {
                EquipType equipType = itemSlotType.GetEquipType();
                if (_equips.ContainsKey(equipType))
                {
                    if(!_playerInventory.InventoryHasBlankSlot()) return false; 
                    
                    if (equipType == EquipType.Hand)
                    {
                        EventBus<UnEquipHotbarEvent>.Raise(new UnEquipHotbarEvent((int)itemSlotType));
                    }

                    // 스탯 감소 처리
                    EventBus.Raise(new UnEquipItemEvent(slot, itemSlotType));
                    StatRemoveModify(equipped.EquipItemData);
                    slot.SetData(null);
                    UnequipItemAppearance(itemSlotType, isExchange);

                    if (!byDrag)
                        _playerInventory.TryAddItem(equipped);
                    
                    if (isRaiseEvent)
                        EventBus<UpdateEquipUIEvent>.Raise(new UpdateEquipUIEvent(_equipSlots.ToList()));

                    return true;
                }
            }

            return false;
        }

        public void ChangeHandlingItem(HotbarSlotType hotbarSlotType, EquipableItem targetItem)
        {
            if (hotbarSlotType == _handlingSlotType || targetItem == null)
                return;
            
            if (_equips.TryGetValue(EquipType.Hand, out EquipableItem currentEquipped))
            {
                _equips[EquipType.Hand] = null;
                (currentEquipped as IEquipable).Unequip(_player);
            }

            _equips[EquipType.Hand] = targetItem;
            _handledSlotType = _handlingSlotType;
            _handlingSlotType = hotbarSlotType;
    
            targetItem.Equip(_player, equipTrms[EquipType.Hand]);
            EventBus.Raise(new ChangeHandlingEvent(targetItem));
        }

        private void UnequipItemAppearance(EquipSlotType slotType, bool isExchange)
        {
            EquipType equipType = slotType.GetEquipType();

            if (_equips[equipType] == null) return;
            if (equipType == EquipType.Hand && (int)slotType == (int)_handlingSlotType)
            {

                HotbarSlotType spareSlotType = _handlingSlotType == HotbarSlotType.Item1
                    ? HotbarSlotType.Item2
                    : HotbarSlotType.Item1;

                _equips[equipType].Unequip(_player);
                _equips[equipType] = null;

                if (_equipSlots[(EquipSlotType)spareSlotType].Item is EquipableItem spareGun && !isExchange)
                {
                    _handlingSlotType = spareSlotType;
                    _equips[equipType] = spareGun;
                    _equips[equipType].Equip(_player, equipTrms[equipType]);
                    EventBus.Raise(new ChangeHandlingEvent(spareGun));
                }
                else
                {
                    _handlingSlotType = HotbarSlotType.None;
                    EventBus.Raise(new ChangeHandlingEvent(null));
                }
            }
            else if (equipType != EquipType.Hand)
            {
                _equips[equipType].Unequip(_player);
                _equips[equipType] = null;

            }
        }

        private void AddStatModify(EquipItemDataSO itemData)
        {
            foreach (var addStat in itemData.addStats)
            {
                _stat.AddModifier(addStat.targetStat, addStat, addStat.value);
            }
        }

        private void StatRemoveModify(EquipItemDataSO itemData)
        {
            foreach (var addStat in itemData.addStats)
            {
                _stat.RemoveModifier(addStat.targetStat, addStat);
            }
        }

        private EquipSlotType GetEquippedSlotType(EquipableItem equipable)
        {
            foreach (var kvp in _equipSlots)
            {
                if (kvp.Value.Item == equipable)
                    return kvp.Key;
            }

            return EquipSlotType.None;
        }

        public bool TryGetEquippedItem(EquipType type, out EquipableItem item)
        {
            item = _equips[type];
            if (item == null)
                return false;
            return true;
        }

        public EquipableItem GetEquippedItem(EquipType type) => _equips.GetValueOrDefault(type);

        public EquipableItem GetEquipSlotItem(EquipSlotType type)
        {
            if (_equipSlots.TryGetValue(type, out var slot))
            {
                return slot.Item as EquipableItem;
            }

            return null;
        }

        private EquipSlotType GetSuitableSlotType(EquipSlotType equipSlotType)
        {
            if (equipSlotType == EquipSlotType.Weapon1 || equipSlotType == EquipSlotType.Weapon2)
            {
                if (_equipSlots[EquipSlotType.Weapon1].Item == null) return EquipSlotType.Weapon1;
                if (_equipSlots[EquipSlotType.Weapon2].Item == null) return EquipSlotType.Weapon2;
                return EquipSlotType.Weapon2;
            }

            return equipSlotType;
        }

        private bool IsSuitable(EquipSlotType equipSlotType, EquipSlotType targetType)
        {
            if (equipSlotType == targetType ||
                (equipSlotType == EquipSlotType.Weapon1 && targetType == EquipSlotType.Weapon2) ||
                (equipSlotType == EquipSlotType.Weapon2 && targetType == EquipSlotType.Weapon1))
                return true;

            return false;
        }
    }
}
