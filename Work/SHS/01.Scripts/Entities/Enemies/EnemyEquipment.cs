using AYellowpaper.SerializedCollections;
using Chipmunk.ComponentContainers;
using Chipmunk.Library.Utility.GameEvents.Local;
using Code.InventorySystems;
using Code.InventorySystems.Items;
using Code.Players;
using Scripts.Combat.Datas;
using Scripts.Entities;
using System;
using System.Collections.Generic;
using Chipmunk.Modules.StatSystem;
using Code.EnemySpawn;
using Code.SHS.Entities.Enemies.Events.Local;
using UnityEngine;
using Work.LKW.Code.Items;
using Work.LKW.Code.Items.ItemInfo;

namespace Code.SHS.Entities.Enemies
{
    /// <summary>
    /// Enemy�� ��� ���� Ŭ����
    /// </summary>
    public class EnemyEquipSlot
    {
        public EquipableItem Item { get; private set; }
        public EquipType Type { get; private set; }

        public EnemyEquipSlot(EquipType type)
        {
            Type = type;
            Item = null;
        }

        public void SetItem(EquipableItem item)
        {
            Item = item;
        }

        public void Clear()
        {
            Item = null;
        }
    }

    public class EnemyEquipment : MonoBehaviour, IContainerComponent, ILocalEventSubscriber<EnemySpawnEvent>
    {
        [SerializeField] private SerializedDictionary<EquipType, Transform> equipTrms;

        public ComponentContainer ComponentContainer { get; set; }
        public bool IsInitialized => ComponentContainer != null;

        private Entity _entity;
        private StatOverrideBehavior _stat;
        private Inventory _enemyInventory;
        private Dictionary<EquipType, EnemyEquipSlot> _equips = new Dictionary<EquipType, EnemyEquipSlot>();

        public void OnInitialize(ComponentContainer componentContainer)
        {
            // store the container so IsInitialized becomes true and GetCompo calls use the correct container
            ComponentContainer = componentContainer;
            _enemyInventory = componentContainer.GetSubclassComponent<Inventory>();
            int equipSlotCnt = Enum.GetValues(typeof(EquipType)).Length;
            for (int i = 0; i < equipSlotCnt; i++)
            {
                EquipType type = (EquipType)i;
                var equipSlot = new EnemyEquipSlot(type);
                _equips.Add(type, equipSlot);
            }

            _entity = ComponentContainer.GetCompo<Entity>(true);
            _stat = ComponentContainer.GetCompo<StatOverrideBehavior>();

            // Ensure equipTrms exists and has a valid transform for each slot type.
            if (equipTrms == null)
                equipTrms = new SerializedDictionary<EquipType, Transform>();

            foreach (EquipType slotType in Enum.GetValues(typeof(EquipType)))
            {
                // If inspector didn't provide a transform for this slot, create a child GameObject and use it.
                if (!equipTrms.ContainsKey(slotType) || equipTrms[slotType] == null)
                {
                    Transform parent = _entity != null ? _entity.transform : this.transform;
                    var go = new GameObject($"Equip_TRM_{slotType}");
                    go.transform.SetParent(parent, false);
                    equipTrms[slotType] = go.transform;
                }
            }
        }

        public void OnLocalEvent(EnemySpawnEvent spawnEvent)
        {
            SetSpawnEquipments(spawnEvent.EnemyData.equipments);
        }

        public void SetSpawnEquipments(EnemyEquipData[] equipments)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("EnemyEquipment is not initialized yet!");
                return;
            }

            if (equipments == null) return;

            foreach (var equipData in equipments)
            {
                if (equipData.itemData == null) continue;

                EquipableItem item = equipData.itemData.CreateItem().Item as EquipableItem;

                if (item is ThrowableItem)
                {
                    _enemyInventory.TryAddItem(item, 1);
                }

                if (item != null)
                {
                    Equip(item, equipData.itemData, equipData.type);
                }
            }
        }

        public bool Equip(EquipableItem equipable, EquipItemDataSO itemData, EquipType type)
        {
            EquipType targetType = type;
            EquipType itemType = itemData.itemType.GetEquipSlotType().GetEquipType();

            if (itemType != targetType || targetType == EquipType.None)
                return false;

            var itemSlot = _equips[targetType];

            // �̹� �����Ȱ� �ִ��� Ȯ��, ������ ����
            if (itemSlot.Item != null && itemSlot.Item.ItemData is EquipItemDataSO equipItemData)
            {
                UnEquip(itemSlot.Item, equipItemData);
            }

            AddStatModify(itemData);
            itemSlot.SetItem(equipable);

            // use mapped transform for this slot if available, otherwise fallback to entity transform
            Transform parentTrm = _entity != null ? _entity.transform : this.transform;
            if (equipTrms != null)
            {
                if (equipTrms.TryGetValue(targetType, out Transform mapped))
                    parentTrm = mapped;
            }

            equipable.Equip(_entity, parentTrm);

            return true;
        }

        public bool UnEquip(EquipableItem equipped, EquipItemDataSO itemData)
        {
            EquipType itemType = GetEquippedSlotType(equipped);

            if (itemType == EquipType.None) return false;

            if (_equips.TryGetValue(itemType, out EnemyEquipSlot slot))
            {
                equipped.Unequip(_entity);
                StatRemoveModify(itemData);
                slot.Clear();
                return true;
            }

            return false;
        }

        private void AddStatModify(EquipItemDataSO itemData)
        {
            if (_stat == null) return;

            foreach (var addStat in itemData.addStats)
            {
                _stat.AddModifier(addStat.targetStat, addStat, addStat.value);
            }
        }

        private void StatRemoveModify(EquipItemDataSO itemData)
        {
            if (_stat == null) return;

            foreach (var addStat in itemData.addStats)
            {
                _stat.RemoveModifier(addStat.targetStat, addStat);
            }
        }

        private EquipType GetEquippedSlotType(EquipableItem equipable)
        {
            foreach (var kvp in _equips)
            {
                if (kvp.Value.Item == equipable)
                    return kvp.Key;
            }

            return EquipType.None;
        }

        public bool TryGetEquippedItem(EquipType type, out EquipableItem item)
        {
            EnemyEquipSlot slot = _equips[type];
            item = null;
            if (slot.Item == null)
                return false;
            item = slot.Item;
            return true;
        }

        public EnemyEquipSlot GetEquipSlot(EquipType type) => _equips.GetValueOrDefault(type);
    }
}
