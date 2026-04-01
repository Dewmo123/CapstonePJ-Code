using Code.InventorySystems.Items;
using Code.Players;
using Scripts.Combat.ItemObjects;
using UnityEngine;
using Work.LKW.Code.Items;

namespace InGame.InventorySystem
{
    public class EquipSlot : ItemSlot
    {
        public EquipSlot(ItemBase item, EquipSlotType equipType) : base(item)
        {
            Debug.Assert(item == null || item is EquipableItem, "Invalid Item");
            EquipType = equipType;
        }
        public EquipableItem Equipable => Item as EquipableItem;
        public EquipSlotType EquipType { get; private set; }
        public ItemObject ItemObject => Equipable?.ItemObject;

        public bool CanEquip(ItemBase item)
        {
            if (item == null) return true;
            
            if (item is EquipableItem equipableItem)
            {
                return equipableItem.ItemData.itemType.IsAssignableTo(this.EquipType);
            }
    
            return false;
        }
    }
}