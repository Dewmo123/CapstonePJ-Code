using System.Collections.Generic;
using System.Linq;
using Chipmunk.GameEvents;
using Code.GameEvents;
using Code.Players;
using Code.UI.Inventory;
using UnityEngine;
using Work.LKW.Code.Items;

namespace InGame.InventorySystem
{
    public class EquipInventory : AbstractSlotUIsPanel
    {
        private Dictionary<EquipSlotType, EquipSlotUI> _equipSlotDict = new Dictionary<EquipSlotType, EquipSlotUI>();
        private List<KeyValuePair<EquipSlotType, EquipSlot>> _equipSlots;

        private string[] _itemText =
        {
            "주무기",
            "보조무기",
            "근접무기",
            "헬멧",
            "갑옷"
        };

        protected override void Awake()
        {
            base.Awake();

            _equipSlotDict = GetComponentsInChildren<EquipSlotUI>()
                .ToDictionary(slot => slot.ItemType);

            EventBus<EquipItemEvent>.OnEvent += HandleEquipItem;
            EventBus<UnEquipItemEvent>.OnEvent += HandleUnEquipItem;
            EventBus<UpdateEquipUIEvent>.OnEvent += HandleUpdateUI;
        }

        protected override void OnDestroy()
        {
            EventBus<EquipItemEvent>.OnEvent -= HandleEquipItem;
            EventBus<UnEquipItemEvent>.OnEvent -= HandleUnEquipItem;
            EventBus<UpdateEquipUIEvent>.OnEvent -= HandleUpdateUI;
            
            base.OnDestroy();
        }

        private void HandleUpdateUI(UpdateEquipUIEvent evt)
        {
            _equipSlots = evt.EquipSlots;
            UpdateSlotUI();
        }

        protected override void UpdateSlotUI()
        {
            List<EquipSlotUI> equipSlotUI = _equipSlotDict.Values.ToList();

            for (int i = 0; i < equipSlotUI.Count; i++)
            {
                equipSlotUI[i].Clear();
                var equipSlot = _equipSlots.FirstOrDefault(kvp => kvp.Key == equipSlotUI[i].ItemType);
                equipSlotUI[i].EnableFor(equipSlot.Value, _itemText[i]);
            }
        }

        private void HandleUnEquipItem(UnEquipItemEvent evt)
        {
            EquipableItem equipable = evt.ItemSlot.Item as EquipableItem;

            if (equipable != null && _equipSlotDict.TryGetValue(evt.Type,
                    out EquipSlotUI slot))
            {
                slot.SetIsEquipped(false);
            }
        }
        private void HandleEquipItem(EquipItemEvent evt)
        {
            EquipableItem equipable = evt.ItemSlot.Item as EquipableItem;

            if (equipable != null && _equipSlotDict.TryGetValue(evt.Type,
                    out EquipSlotUI slot))
            {
                slot.SetIsEquipped(true);
            }
        }
    }
}